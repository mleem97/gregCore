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
using System.Text;
using MelonLoader;
using UnityEngine;

namespace gregCore.Infrastructure.Persistence;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static partial class GregEntityInventory
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
        public InventoryKind Kind { get; }
        public string NativeKey { get; }
        public string Uid { get; }
        public string Hint { get; }

        public Entry(InventoryKind kind, string nativeKey, string uid, string hint)
        {
            Kind = kind;
            NativeKey = nativeKey ?? "";
            Uid = uid ?? "";
            Hint = hint ?? "";
        }
    }

    public static string ServerPrefix { get; } = "gregID:Server:";
    public static string SwitchPrefix { get; } = "gregID:Switch:";
    public static string PatchPanelPrefix { get; } = "gregID:PatchPanel:";
    public static string UidPrefix { get; } = "gregUID:";

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
                    catch { /* ignored: best-effort inventory, game continues */ }
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
                    catch { /* ignored: best-effort inventory, game continues */ }
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
                    catch { /* ignored: best-effort inventory, game continues */ }
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
            try { MelonLogger.Warning("[gregCore][Save] " + report); } catch { /* ignored: best-effort inventory, game continues */ }
            return report;
        }
    }

    // ------------------------------------------------------------ rebuild

    /// <summary>
    /// Baut das Inventar aus den deserialisierten/geheilten Save-Daten.
    /// Aufruf im LoadNetworkState-Postfix (Healing-Prefix lief bereits).
    /// Respektiert EntityInventoryConfig (Enabled/Verbose/Dump).
    /// Sammeln/Codec liegen in den Partial-Dateien (Codeline-Limits).
    /// </summary>
    public static void RebuildFromNetworkData(global::Il2Cpp.NetworkSaveData networkData)
    {
        try
        {
            EntityInventoryConfig.Load();
            if (!EntityInventoryConfig.Enabled)
            {
                ClearDisabled();
                return;
            }
            if (networkData == null || networkData.Pointer == IntPtr.Zero) return;
            string dir, name, key;
            ResolveSaveKey(out dir, out name, out key);
            EnsureSidecarRegistered();
            EnsureMapLoaded(key, dir, name);
            var fresh = CollectAll(networkData);
            PublishFresh(fresh, key);
            try { Rebuilt?.Invoke(); } catch { /* ignored: best-effort inventory, game continues */ }
            MelonLogger.Msg("[gregCore][Save] Inventar: " + Summary());
            if (EntityInventoryConfig.VerboseLogging)
            {
                MelonLogger.Msg("[gregCore][Save] Inventar-Sidecar-Map: " + PersistedCount() + " Eintraege (saveKey=" + key + ").");
            }
            if (EntityInventoryConfig.DumpOnRebuild)
            {
                try { MelonLogger.Msg(Dump()); } catch { /* ignored: best-effort inventory, game continues */ }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning("[gregCore][Save] Inventar-Rebuild fehlgeschlagen: " + ex.GetBaseException().Message);
        }
    }

    private sealed class FreshData
    {
        internal readonly Dictionary<string, Entry> ByUid =
            new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        internal readonly Dictionary<string, Entry> ByKindKey =
            new Dictionary<string, Entry>(StringComparer.Ordinal);
        internal readonly HashSet<string> UsedUids =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        internal readonly Dictionary<string, string> Hints =
            new Dictionary<string, string>(StringComparer.Ordinal);
    }

    private static void ClearDisabled()
    {
        try
        {
            lock (_gate)
            {
                _byUid.Clear();
                _byKindKey.Clear();
                _saveKey = "";
                _ready = false;
            }
            MelonLogger.Msg("[gregCore][Save] EntityInventory deaktiviert (Pref) - kein Inventar.");
        }
        catch { /* ignored: best-effort inventory, game continues */ }
    }

    private static void ResolveSaveKey(out string dir, out string name, out string key)
    {
        dir = null;
        name = null;
        try { dir = global::Il2Cpp.SaveSystem.saveDirPath; } catch { dir = null; }
        try { name = global::Il2Cpp.SaveSystem.loadSaveName; } catch { name = null; }
        key = (dir ?? "") + "|" + (name ?? "");
    }

    private static void EnsureMapLoaded(string key, string dir, string name)
    {
        try
        {
            lock (_gate)
            {
                if (string.Equals(_mapLoadedForKey, key, StringComparison.Ordinal)) return;
                LoadPersistedMap(dir, name);
                _mapLoadedForKey = key;
            }
        }
        catch { /* ignored: best-effort inventory, game continues */ }
    }

    private static FreshData CollectAll(global::Il2Cpp.NetworkSaveData networkData)
    {
        var fresh = new FreshData();
        lock (_gate)
        {
            foreach (var kv in _persistedHint) fresh.Hints[kv.Key] = kv.Value;
        }
        CollectServers(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids);
        CollectSwitches(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids);
        CollectPatchPanels(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids);
        CollectCables(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids);
        CollectLacpGroups(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids);
        CollectRouters(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids, fresh.Hints);
        CollectFirewalls(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids, fresh.Hints);
        CollectSfpModules(networkData, fresh.ByUid, fresh.ByKindKey, fresh.UsedUids, fresh.Hints);
        return fresh;
    }

    private static void PublishFresh(FreshData fresh, string key)
    {
        try
        {
            lock (_gate)
            {
                _byUid.Clear();
                foreach (var kv in fresh.ByUid) _byUid[kv.Key] = kv.Value;
                _byKindKey.Clear();
                foreach (var kv in fresh.ByKindKey) _byKindKey[kv.Key] = kv.Value;
                _saveKey = key;
                _ready = true;
            }
        }
        catch { /* ignored: best-effort inventory, game continues */ }
    }
}
