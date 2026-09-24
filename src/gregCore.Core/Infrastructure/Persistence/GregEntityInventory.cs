/// <file-summary>
/// Schicht:      Infrastructure (Persistence)
/// Zweck:        Save-Inventar: beim Laden wird alles inventarisiert, was im
///               Save existiert (Server, Switches, Router, Firewalls,
///               PatchPanels, Kabel, SFP-Module, LACP-Gruppen). Jeder Eintrag
///               bekommt eine stabile, fuer den Spieler unsichtbare UID, ueber
///               die Mods Dinge direkt ansteuern koennen (TryFindLive).
///               Server/Switch/PatchPanel nutzen ihre gregID (HardwareId-
///               Persistence); Kabel/LACP deterministisch aus Vanilla-IDs;
///               Router/Firewall/SFP per Sidecar persistiert (Index + Hint,
///               reparaturfaehig bei Listen-Umbau). UIDs stehen nie in UI oder
///               Objektnamen (ausser den bestehenden gregIDs) - nur im Sidecar
///               (greg_inventory.&lt;save&gt;.tsv) und im Speicher.
/// Maintainer:   Rebuild laeuft im LoadNetworkState-Postfix (nach Healing).
///               Alle Public-Methoden try/catch + Safe-Defaults (GregAPI-Stil).
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;

namespace gregCore.Infrastructure.Persistence;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregEntityInventory
{
    public enum InventoryKind
    {
        Server,
        Switch,
        Router,
        Firewall,
        PatchPanel,
        Cable,
        SfpModule,
        LacpGroup,
    }

    public sealed class Entry
    {
        public readonly InventoryKind Kind;
        public readonly string NativeKey;
        public readonly string Uid;
        public readonly string Hint;

        public Entry(InventoryKind kind, string nativeKey, string uid, string hint)
        {
            Kind = kind;
            NativeKey = nativeKey ?? "";
            Uid = uid ?? "";
            Hint = hint ?? "";
        }
    }

    public const string ServerPrefix = "gregID:Server:";
    public const string SwitchPrefix = "gregID:Switch:";
    public const string PatchPanelPrefix = "gregID:PatchPanel:";
    public const string UidPrefix = "gregUID:";

    private const string SidecarModId = "inventory";

    public static event Action Rebuilt;

    private static readonly object _gate = new object();
    private static readonly Dictionary<string, Entry> _byUid =
        new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Entry> _byKindKey =
        new Dictionary<string, Entry>(StringComparer.Ordinal);
    // Persistierte UID-Map (Sidecar): kindKey -> uid / hint. Nur fuer Kinds
    // ohne stabilen Vanilla-Key (Router/Firewall/SFP) relevant.
    private static readonly Dictionary<string, string> _persistedUid =
        new Dictionary<string, string>(StringComparer.Ordinal);
    private static readonly Dictionary<string, string> _persistedHint =
        new Dictionary<string, string>(StringComparer.Ordinal);
    private static string _mapLoadedForKey;
    private static string _saveKey = "";
    private static bool _ready;
    private static bool _sidecarRegistered;

    public static bool IsReady
    {
        get { lock (_gate) { return _ready; } }
    }

    public static string SaveKey
    {
        get { lock (_gate) { return _saveKey; } }
    }

    public static int Count(InventoryKind kind)
    {
        try
        {
            int n = 0;
            lock (_gate)
            {
                foreach (var e in _byUid.Values)
                {
                    if (e != null && e.Kind == kind) n++;
                }
            }
            return n;
        }
        catch { return 0; }
    }

    public static IReadOnlyList<Entry> GetAll(InventoryKind kind)
    {
        try
        {
            var list = new List<Entry>();
            lock (_gate)
            {
                foreach (var e in _byUid.Values)
                {
                    if (e != null && e.Kind == kind) list.Add(e);
                }
            }
            return list;
        }
        catch { return Array.Empty<Entry>(); }
    }

    public static bool TryGetUid(InventoryKind kind, string nativeKey, out string uid)
    {
        uid = null;
        try
        {
            if (string.IsNullOrWhiteSpace(nativeKey)) return false;
            string kk = KindKey(kind, nativeKey.Trim());
            lock (_gate)
            {
                Entry e;
                if (_byKindKey.TryGetValue(kk, out e) && e != null && !string.IsNullOrEmpty(e.Uid))
                {
                    uid = e.Uid;
                    return true;
                }
            }
            return false;
        }
        catch { uid = null; return false; }
    }

    public static bool TryGetEntry(string uid, out Entry entry)
    {
        entry = null;
        try
        {
            if (string.IsNullOrWhiteSpace(uid)) return false;
            lock (_gate)
            {
                Entry e;
                if (_byUid.TryGetValue(uid.Trim(), out e) && e != null)
                {
                    entry = e;
                    return true;
                }
            }
            return false;
        }
        catch { entry = null; return false; }
    }

    /// <summary>
    /// Loest eine UID in das Live-GameObject auf. Funktioniert fuer Kinds mit
    /// lesbarer Live-ID (Server/Switch/PatchPanel). Kabel/Router/Firewall/SFP/
    /// LACP sind save-seitig inventarisiert (false + Doku).
    /// </summary>
    public static bool TryFindLive(string uid, out GameObject go)
    {
        go = null;
        try
        {
            Entry e;
            if (!TryGetEntry(uid, out e) || e == null) return false;
            switch (e.Kind)
            {
                case InventoryKind.Server:
                    try
                    {
                        var s = gregCore.Core.Networking.GregServers.FindById(e.NativeKey);
                        if (s != null && s.Pointer != IntPtr.Zero && s.gameObject != null)
                        {
                            go = s.gameObject;
                            return true;
                        }
                    }
                    catch { }
                    return false;
                case InventoryKind.Switch:
                    return TryFindSwitch(e.NativeKey, out go);
                case InventoryKind.PatchPanel:
                    return TryFindPatchPanel(e.NativeKey, out go);
                default:
                    return false;
            }
        }
        catch { go = null; return false; }
    }

    private static bool TryFindSwitch(string nativeKey, out GameObject go)
    {
        go = null;
        try
        {
            var all = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.NetworkSwitch>();
            if (all == null) return false;
            foreach (var s in all)
            {
                if (s == null || s.Pointer == IntPtr.Zero) continue;
                string id = null;
                try { id = s.switchId; } catch { continue; }
                if (string.Equals(id, nativeKey, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (s.gameObject != null) { go = s.gameObject; return true; }
                    }
                    catch { }
                    return false;
                }
            }
            return false;
        }
        catch { go = null; return false; }
    }

    private static bool TryFindPatchPanel(string nativeKey, out GameObject go)
    {
        go = null;
        try
        {
            var all = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.PatchPanel>();
            if (all == null) return false;
            foreach (var p in all)
            {
                if (p == null || p.Pointer == IntPtr.Zero) continue;
                string id = null;
                try { id = p.patchPanelId; } catch { continue; }
                if (string.Equals(id, nativeKey, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (p.gameObject != null) { go = p.gameObject; return true; }
                    }
                    catch { }
                    return false;
                }
            }
            return false;
        }
        catch { go = null; return false; }
    }

    public static string Summary()
    {
        try
        {
            var sb = new StringBuilder();
            bool first = true;
            foreach (InventoryKind k in Enum.GetValues(typeof(InventoryKind)))
            {
                if (!first) sb.Append(' ');
                first = false;
                sb.Append(k.ToString()).Append('=').Append(Count(k));
            }
            return sb.ToString();
        }
        catch { return ""; }
    }

    /// <summary>
    /// Vollstaendiges Inventar als Text (Kontrolle/Debugging). Pro Kind alle
    /// Eintraege mit UID, NativeKey, Hint und Live-Status (ID-Kinds).
    /// </summary>
    public static string Dump()
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("gregCore EntityInventory Dump (saveKey=" + SaveKey + ", ready=" + IsReady + ")");
            foreach (InventoryKind k in Enum.GetValues(typeof(InventoryKind)))
            {
                var all = GetAll(k);
                sb.AppendLine("[" + k.ToString() + "] count=" + all.Count);
                foreach (var e in all)
                {
                    if (e == null) continue;
                    string live = "-";
                    if (k == InventoryKind.Server || k == InventoryKind.Switch || k == InventoryKind.PatchPanel)
                    {
                        GameObject go;
                        live = TryFindLive(e.Uid, out go) && go != null ? "live" : "MISSING";
                    }
                    sb.AppendLine("  " + e.Uid + " <= " + e.NativeKey
                        + (string.IsNullOrEmpty(e.Hint) ? "" : " (" + e.Hint + ")")
                        + " [" + live + "]");
                }
            }
            return sb.ToString();
        }
        catch (Exception ex) { return "Dump failed: " + ex.GetBaseException().Message; }
    }

    /// <summary>
    /// Prueft das Inventar: Duplikat-UIDs, leere Keys, Live-Aufloesbarkeit
    /// (ID-Kinds). Gibt einen Report zurueck und loggt Warnungen.
    /// </summary>
    public static string Verify()
    {
        try
        {
            int dupes = 0, emptyKeys = 0, liveOk = 0, liveMissing = 0;
            var missingExamples = new List<string>();
            lock (_gate)
            {
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var e in _byUid.Values)
                {
                    if (e == null) continue;
                    if (!seen.Add(e.Uid)) dupes++;
                    if (string.IsNullOrWhiteSpace(e.NativeKey)) emptyKeys++;
                }
            }
            foreach (InventoryKind k in new[] { InventoryKind.Server, InventoryKind.Switch, InventoryKind.PatchPanel })
            {
                foreach (var e in GetAll(k))
                {
                    if (e == null) continue;
                    GameObject go;
                    if (TryFindLive(e.Uid, out go) && go != null) liveOk++;
                    else
                    {
                        liveMissing++;
                        if (missingExamples.Count < 5) missingExamples.Add(e.Uid);
                    }
                }
            }
            string report = "EntityInventory Verify: dupes=" + dupes + " emptyKeys=" + emptyKeys
                + " liveOk=" + liveOk + " liveMissing=" + liveMissing
                + (missingExamples.Count > 0 ? " z.B. " + string.Join(",", missingExamples.ToArray()) : "");
            if (dupes > 0 || emptyKeys > 0 || liveMissing > 0)
                MelonLogger.Warning("[gregCore][Save] " + report);
            else
                MelonLogger.Msg("[gregCore][Save] " + report);
            return report;
        }
        catch (Exception ex)
        {
            string report = "Verify failed: " + ex.GetBaseException().Message;
            try { MelonLogger.Warning("[gregCore][Save] " + report); } catch { }
            return report;
        }
    }

    // ------------------------------------------------------------ rebuild

    /// <summary>
    /// Baut das Inventar aus den deserialisierten/geheilten Save-Daten.
    /// Aufruf im LoadNetworkState-Postfix (Healing-Prefix lief bereits).
    /// Respektiert EntityInventoryConfig (Enabled/Verbose/Dump).
    /// </summary>
    public static void RebuildFromNetworkData(global::Il2Cpp.NetworkSaveData networkData)
    {
        try
        {
            EntityInventoryConfig.Load();
            if (!EntityInventoryConfig.Enabled)
            {
                lock (_gate)
                {
                    _byUid.Clear();
                    _byKindKey.Clear();
                    _saveKey = "";
                    _ready = false;
                }
                MelonLogger.Msg("[gregCore][Save] EntityInventory deaktiviert (Pref) - kein Inventar.");
                return;
            }
            if (networkData == null || networkData.Pointer == IntPtr.Zero) return;
            string dir = null, name = null;
            try { dir = global::Il2Cpp.SaveSystem.saveDirPath; } catch { dir = null; }
            try { name = global::Il2Cpp.SaveSystem.loadSaveName; } catch { name = null; }
            string key = (dir ?? "") + "|" + (name ?? "");
            EnsureSidecarRegistered();
            lock (_gate)
            {
                if (!string.Equals(_mapLoadedForKey, key, StringComparison.Ordinal))
                {
                    LoadPersistedMap(dir, name);
                    _mapLoadedForKey = key;
                }
            }

            var freshByUid = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
            var freshByKindKey = new Dictionary<string, Entry>(StringComparer.Ordinal);
            var usedUids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            // Persistierte Hints pro Rebuild frisch einlesen (Repair-Matching).
            Dictionary<string, string> hints;
            lock (_gate) { hints = new Dictionary<string, string>(_persistedHint, StringComparer.Ordinal); }

            CollectServers(networkData, freshByUid, freshByKindKey, usedUids);
            CollectSwitches(networkData, freshByUid, freshByKindKey, usedUids);
            CollectPatchPanels(networkData, freshByUid, freshByKindKey, usedUids);
            CollectCables(networkData, freshByUid, freshByKindKey, usedUids);
            CollectLacpGroups(networkData, freshByUid, freshByKindKey, usedUids);
            CollectRouters(networkData, freshByUid, freshByKindKey, usedUids, hints);
            CollectFirewalls(networkData, freshByUid, freshByKindKey, usedUids, hints);
            CollectSfpModules(networkData, freshByUid, freshByKindKey, usedUids, hints);

            lock (_gate)
            {
                _byUid.Clear();
                foreach (var kv in freshByUid) _byUid[kv.Key] = kv.Value;
                _byKindKey.Clear();
                foreach (var kv in freshByKindKey) _byKindKey[kv.Key] = kv.Value;
                _saveKey = key;
                _ready = true;
            }
            try { Rebuilt?.Invoke(); } catch { }
            MelonLogger.Msg("[gregCore][Save] Inventar: " + Summary());
            if (EntityInventoryConfig.VerboseLogging)
            {
                MelonLogger.Msg("[gregCore][Save] Inventar-Sidecar-Map: " + PersistedCount() + " Eintraege (saveKey=" + key + ").");
            }
            if (EntityInventoryConfig.DumpOnRebuild)
            {
                try { MelonLogger.Msg(Dump()); } catch { }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[gregCore][Save] Inventar-Rebuild fehlgeschlagen: " + ex.GetBaseException().Message);
        }
    }

    private static void AddEntry(Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey,
        HashSet<string> usedUids, InventoryKind kind, string nativeKey, string uid, string hint)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nativeKey) || string.IsNullOrWhiteSpace(uid)) return;
            if (!usedUids.Add(uid)) return; // UID-Kollision: ersten Eintrag behalten
            var e = new Entry(kind, nativeKey.Trim(), uid.Trim(), hint ?? "");
            byUid[e.Uid] = e;
            byKindKey[KindKey(kind, e.NativeKey)] = e;
        }
        catch { }
    }

    private static void CollectServers(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids)
    {
        try
        {
            var list = networkData.servers;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                string id = null;
                try
                {
                    var s = list[i];
                    if (s == null || s.Pointer == IntPtr.Zero) continue;
                    id = s.serverID;
                }
                catch { continue; }
                if (string.IsNullOrWhiteSpace(id)) continue;
                id = id.Trim();
                // Bereits geheilt (Normalfall im LoadNetworkState-Postfix) ->
                // UID ist die gregID selbst. Fallback (ungeheilt): determinist-
                // isch aus der VOLLEN ID (kein Strip - "X_0"/"X_1" duerfen nicht
                // kollidieren), stabil bis Healing + Save, danach gregID.
                string uid = IsGregDeviceUid(id, ServerPrefix)
                    ? id
                    : ServerPrefix + NewHex(id);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Server, id, uid, "");
            }
        }
        catch { }
    }

    private static void CollectSwitches(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids)
    {
        try
        {
            var list = networkData.switches;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                string id = null;
                try
                {
                    var s = list[i];
                    if (s == null || s.Pointer == IntPtr.Zero) continue;
                    id = s.switchID;
                }
                catch { continue; }
                if (string.IsNullOrWhiteSpace(id)) continue;
                id = id.Trim();
                // Siehe CollectServers (Healing-Normalfall + Voll-ID-Fallback).
                string uid = IsGregDeviceUid(id, SwitchPrefix)
                    ? id
                    : SwitchPrefix + NewHex(id);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Switch, id, uid, "");
            }
        }
        catch { }
    }

    private static void CollectPatchPanels(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids)
    {
        try
        {
            var list = networkData.patchPanels;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                string id = null;
                try
                {
                    var p = list[i];
                    if (p == null || p.Pointer == IntPtr.Zero) continue;
                    id = p.patchPanelID;
                }
                catch { continue; }
                if (string.IsNullOrWhiteSpace(id)) continue;
                id = id.Trim();
                // Siehe CollectServers (Healing-Normalfall + Voll-ID-Fallback).
                string uid = IsGregDeviceUid(id, PatchPanelPrefix)
                    ? id
                    : PatchPanelPrefix + NewHex(id);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.PatchPanel, id, uid, "");
            }
        }
        catch { }
    }

    private static void CollectCables(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids)
    {
        try
        {
            var list = networkData.cables;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                int cableId = -1;
                try
                {
                    var c = list[i];
                    if (c == null || c.Pointer == IntPtr.Zero) continue;
                    cableId = c.cableID;
                }
                catch { continue; }
                if (cableId < 0) continue;
                string nativeKey = "cable#" + cableId.ToString(CultureInfo.InvariantCulture);
                string uid = UidPrefix + "Cable:" + cableId.ToString(CultureInfo.InvariantCulture);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Cable, nativeKey, uid, "");
            }
        }
        catch { }
    }

    private static void CollectLacpGroups(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids)
    {
        try
        {
            var list = networkData.lacpGroups;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                int groupId = -1;
                try
                {
                    var g = list[i];
                    if (g == null || g.Pointer == IntPtr.Zero) continue;
                    groupId = g.groupId;
                }
                catch { continue; }
                if (groupId < 0) continue;
                string nativeKey = "lacp#" + groupId.ToString(CultureInfo.InvariantCulture);
                string uid = UidPrefix + "Lacp:" + groupId.ToString(CultureInfo.InvariantCulture);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.LacpGroup, nativeKey, uid, "");
            }
        }
        catch { }
    }

    private static void CollectRouters(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids,
        Dictionary<string, string> hints)
    {
        try
        {
            var list = networkData.routers;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                string hint = "";
                try
                {
                    var r = list[i];
                    if (r == null || r.Pointer == IntPtr.Zero) continue;
                    int routes = 0;
                    try { routes = r.routes != null ? r.routes.Count : 0; } catch { }
                    hint = "asn:" + r.asn.ToString(CultureInfo.InvariantCulture)
                        + "/routes:" + routes.ToString(CultureInfo.InvariantCulture);
                }
                catch { continue; }
                string nativeKey = "router#" + i.ToString(CultureInfo.InvariantCulture);
                string uid = ResolvePersistedUid(InventoryKind.Router, nativeKey, hint, hints, usedUids);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Router, nativeKey, uid, hint);
                RememberPersisted(InventoryKind.Router, nativeKey, uid, hint);
            }
        }
        catch { }
    }

    private static void CollectFirewalls(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids,
        Dictionary<string, string> hints)
    {
        try
        {
            var list = networkData.firewalls;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                string hint = "";
                try
                {
                    var f = list[i];
                    if (f == null || f.Pointer == IntPtr.Zero) continue;
                    int rules = 0;
                    try { rules = f.filterRules != null ? f.filterRules.Count : 0; } catch { }
                    hint = "cluster:" + (f.clusterIP ?? "") + "/rules:" + rules.ToString(CultureInfo.InvariantCulture);
                }
                catch { continue; }
                string nativeKey = "firewall#" + i.ToString(CultureInfo.InvariantCulture);
                string uid = ResolvePersistedUid(InventoryKind.Firewall, nativeKey, hint, hints, usedUids);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Firewall, nativeKey, uid, hint);
                RememberPersisted(InventoryKind.Firewall, nativeKey, uid, hint);
            }
        }
        catch { }
    }

    private static void CollectSfpModules(global::Il2Cpp.NetworkSaveData networkData,
        Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey, HashSet<string> usedUids,
        Dictionary<string, string> hints)
    {
        try
        {
            var list = networkData.sfpModules;
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                string hint = "";
                try
                {
                    var s = list[i];
                    if (s == null || s.Pointer == IntPtr.Zero) continue;
                    UnityEngine.Vector3 pos = s.position;
                    hint = "prefab:" + s.prefabID.ToString(CultureInfo.InvariantCulture)
                        + "/pos:" + pos.x.ToString("F2", CultureInfo.InvariantCulture)
                        + "," + pos.y.ToString("F2", CultureInfo.InvariantCulture)
                        + "," + pos.z.ToString("F2", CultureInfo.InvariantCulture);
                }
                catch { continue; }
                string nativeKey = "sfp#" + i.ToString(CultureInfo.InvariantCulture);
                string uid = ResolvePersistedUid(InventoryKind.SfpModule, nativeKey, hint, hints, usedUids);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.SfpModule, nativeKey, uid, hint);
                RememberPersisted(InventoryKind.SfpModule, nativeKey, uid, hint);
            }
        }
        catch { }
    }

    /// <summary>
    /// UID-Aufloesung fuer Kinds ohne stabilen Vanilla-Key: exakter Treffer
    /// (Key + Hint) gewinnt, sonst Hint-Reparatur ueber die ganze Liste,
    /// sonst neue UID. Verhindert UID-Churn bei Listen-Umbau.
    /// </summary>
    private static string ResolvePersistedUid(InventoryKind kind, string nativeKey, string hint,
        Dictionary<string, string> hints, HashSet<string> usedUids)
    {
        try
        {
            string kk = KindKey(kind, nativeKey);
            string knownUid = null, knownHint = null;
            lock (_gate)
            {
                _persistedUid.TryGetValue(kk, out knownUid);
                _persistedHint.TryGetValue(kk, out knownHint);
            }
            if (!string.IsNullOrEmpty(knownUid) && !usedUids.Contains(knownUid) &&
                string.Equals(knownHint ?? "", hint ?? "", StringComparison.Ordinal))
                return knownUid;
            // Repair: Hint woanders in der Liste wiederfinden (Index-Shift).
            if (!string.IsNullOrEmpty(hint))
            {
                lock (_gate)
                {
                    foreach (var kv in _persistedHint)
                    {
                        if (!kv.Key.StartsWith(kind.ToString() + "\n", StringComparison.Ordinal)) continue;
                        if (!string.Equals(kv.Value ?? "", hint, StringComparison.Ordinal)) continue;
                        string candidate;
                        if (_persistedUid.TryGetValue(kv.Key, out candidate) &&
                            !string.IsNullOrEmpty(candidate) && !usedUids.Contains(candidate))
                            return candidate;
                    }
                }
            }
            return NewUid(kind);
        }
        catch { return NewUid(kind); }
    }

    private static void RememberPersisted(InventoryKind kind, string nativeKey, string uid, string hint)
    {
        try
        {
            if (string.IsNullOrEmpty(uid)) return;
            lock (_gate)
            {
                _persistedUid[KindKey(kind, nativeKey)] = uid;
                _persistedHint[KindKey(kind, nativeKey)] = hint ?? "";
            }
        }
        catch { }
    }

    // ------------------------------------------------------------ config

    /// <summary>
    /// Control-Surface des Inventars (MelonPreferences, Kategorie
    /// gregCore.EntityInventory). Enabled=false schaltet Rebuilds ab.
    /// </summary>
    public static class EntityInventoryConfig
    {
        private static bool _loaded;
        private static readonly object _loadGate = new object();

        public static bool Enabled = true;
        public static bool VerboseLogging = false;
        public static bool DumpOnRebuild = false;

        public static void Load()
        {
            try
            {
                lock (_loadGate)
                {
                    if (_loaded) return;
                    _loaded = true;
                }
                var category = MelonPreferences.CreateCategory("gregCore.EntityInventory", "EntityInventory (Save-Inventar)");
                var enabled = category.CreateEntry("Enabled", true, "Inventar beim Laden aufbauen.");
                var verbose = category.CreateEntry("VerboseLogging", false, "Ausfuehrliche Inventar-Logs.");
                var dump = category.CreateEntry("DumpOnRebuild", false, "Vollstaendiges Inventar nach jedem Rebuild loggen.");
                try
                {
                    Enabled = enabled.Value;
                    VerboseLogging = verbose.Value;
                    DumpOnRebuild = dump.Value;
                }
                catch { }
                try { category.SaveToFile(false); } catch { }
            }
            catch { }
        }

        // Nur Tests/Tools: Laufzeit-Umschalter ohne Datei.
        public static void OverrideForTesting(bool? enabled, bool? verbose, bool? dump)
        {
            try
            {
                lock (_loadGate) { _loaded = true; }
                if (enabled.HasValue) Enabled = enabled.Value;
                if (verbose.HasValue) VerboseLogging = verbose.Value;
                if (dump.HasValue) DumpOnRebuild = dump.Value;
            }
            catch { }
        }
    }

    private static int PersistedCount()
    {
        try { lock (_gate) { return _persistedUid.Count; } }
        catch { return -1; }
    }

    // ------------------------------------------------------------ sidecar

    private static void EnsureSidecarRegistered()
    {
        try
        {
            lock (_gate)
            {
                if (_sidecarRegistered) return;
                _sidecarRegistered = true;
            }
            GregSaveGuard.RegisterSidecar(SidecarModId, SerializeMap, ParseMap);
        }
        catch { }
    }

    public static string SerializeMap()
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("# gregUID inventory: kind\tnativeKey\tuid\thint (unsichtbar, nur gregCore)");
            lock (_gate)
            {
                foreach (var kv in _persistedUid)
                {
                    int sep = kv.Key.IndexOf('\n');
                    if (sep <= 0) continue;
                    string kind = kv.Key.Substring(0, sep);
                    string nativeKey = kv.Key.Substring(sep + 1);
                    string hint = "";
                    _persistedHint.TryGetValue(kv.Key, out hint);
                    sb.Append(SanitizeField(kind)).Append('\t')
                      .Append(SanitizeField(nativeKey)).Append('\t')
                      .Append(SanitizeField(kv.Value)).Append('\t')
                      .Append(SanitizeField(hint)).AppendLine();
                }
            }
            return sb.ToString();
        }
        catch { return null; }
    }

    public static void ParseMap(string content)
    {
        try
        {
            if (string.IsNullOrEmpty(content)) return;
            var uids = ParseUidMap(content, out var hints);
            if (uids.Count == 0 && hints.Count == 0) return;
            lock (_gate)
            {
                foreach (var kv in uids) _persistedUid[kv.Key] = kv.Value;
                foreach (var kv in hints) _persistedHint[kv.Key] = kv.Value;
            }
        }
        catch { }
    }

    /// <summary>Reine TSV-Logik (kindKey -&gt; uid/hint), unit-testbar ohne Spiel.</summary>
    public static Dictionary<string, string> ParseUidMap(string content, out Dictionary<string, string> hints)
    {
        hints = new Dictionary<string, string>(StringComparer.Ordinal);
        var uids = new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            if (string.IsNullOrEmpty(content)) return uids;
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                var parts = line.Split('\t');
                if (parts.Length < 3) continue;
                string kind = parts[0].Trim();
                string nativeKey = parts[1].Trim();
                string uid = parts[2].Trim();
                string hint = parts.Length > 3 ? parts[3].Trim() : "";
                if (kind.Length == 0 || nativeKey.Length == 0 || uid.Length == 0) continue;
                if (!IsGregUid(uid)) continue;
                string kk = kind + "\n" + nativeKey;
                uids[kk] = uid;
                hints[kk] = hint;
            }
        }
        catch { }
        return uids;
    }

    private static void LoadPersistedMap(string dir, string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dir) || string.IsNullOrWhiteSpace(name)) return;
            string path = GregSaveGuard.SidecarPath(dir, name, SidecarModId);
            string content = null;
            try { if (File.Exists(path)) content = File.ReadAllText(path); } catch { return; }
            if (string.IsNullOrEmpty(content)) return;
            var uids = ParseUidMap(content, out var hints);
            lock (_gate)
            {
                _persistedUid.Clear();
                _persistedHint.Clear();
                foreach (var kv in uids) _persistedUid[kv.Key] = kv.Value;
                foreach (var kv in hints) _persistedHint[kv.Key] = kv.Value;
            }
            MelonLogger.Msg("[gregCore][Save] Inventar-Sidecar geladen: " + uids.Count + " UID(s).");
        }
        catch { }
    }

    // ------------------------------------------------------------ helpers (pure, testbar)

    private static readonly Regex GregIdTokenRegex =
        new Regex(@"gregID:[A-Za-z]+:[0-9A-Za-z_\-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex UnityDuplicateSuffixRegex =
        new Regex(@"^(.*)\s\(\d+\)$", RegexOptions.Compiled);

    /// <summary>
    /// Entfernt gregID-Token aus Anzeigetexten (Screens), ersetzt sie durch
    /// die Vanilla-Bezeichnung. No-Op wenn kein Token enthalten (billig).
    /// </summary>
    public static string ScrubGregIds(string text, string replacement)
    {
        try
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.IndexOf("gregID:", StringComparison.OrdinalIgnoreCase) < 0) return text;
            return GregIdTokenRegex.Replace(text, replacement ?? "");
        }
        catch { return text; }
    }

    /// <summary>
    /// Vanilla-Bezeichnung aus Objektnamen: "(Clone)" und Unity-Duplikat-
    /// Suffixe (" (1)") entfernen. gregID-Namen liefern "" (keine Vanilla-
    /// Form vorhanden - Aufrufer faellt zurueck).
    /// </summary>
    public static string CleanDisplayName(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            string n = name.Trim();
            if (n.StartsWith("gregID:", StringComparison.OrdinalIgnoreCase)) return "";
            int clone = n.IndexOf("(Clone)", StringComparison.OrdinalIgnoreCase);
            if (clone >= 0) n = n.Substring(0, clone).Trim();
            var m = UnityDuplicateSuffixRegex.Match(n);
            if (m.Success && m.Groups.Count > 1) n = m.Groups[1].Value.Trim();
            return n;
        }
        catch { return name ?? ""; }
    }

    public static string KindKey(InventoryKind kind, string nativeKey)
    {
        return kind.ToString() + "\n" + (nativeKey ?? "").Trim();
    }

    public static bool IsGregUid(string uid)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(uid)) return false;
            string u = uid.Trim();
            if (u.StartsWith(UidPrefix, StringComparison.OrdinalIgnoreCase)) return true;
            return IsGregDeviceUid(u, ServerPrefix)
                || IsGregDeviceUid(u, SwitchPrefix)
                || IsGregDeviceUid(u, PatchPanelPrefix);
        }
        catch { return false; }
    }

    public static bool IsGregDeviceUid(string uid, string prefix)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrEmpty(prefix)) return false;
            return uid.Trim().StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    public static string NewUid(InventoryKind kind)
    {
        try { return UidPrefix + kind.ToString() + ":" + Guid.NewGuid().ToString("N").ToUpperInvariant().Substring(0, 12); }
        catch { return UidPrefix + kind.ToString() + ":FFFFFFFFFFFF"; }
    }

    /// <summary>Deterministischer 12-Hex-Seed aus beliebigem Text (stabile Fallback-UIDs).</summary>
    public static string NewHex(string seed)
    {
        try
        {
            unchecked
            {
                uint h1 = 2166136261u, h2 = 16777619u;
                string s = seed ?? "";
                for (int i = 0; i < s.Length; i++)
                {
                    h1 ^= s[i];
                    h1 *= 16777619u;
                    h2 += s[i];
                    h2 *= 31u;
                }
                return h1.ToString("X8") + h2.ToString("X8").Substring(0, 4);
            }
        }
        catch { return "000000000000"; }
    }

    public static string SanitizeField(string value)
    {
        try
        {
            if (value == null) return "";
            return value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
        }
        catch { return ""; }
    }
}
