/// <file-summary>
/// Layer:       Infrastructure (Persistence)
/// Purpose:     Second part of GregEntityInventory (partial): collecting
///               save entries per kind + UID resolution for kinds without
///               a stable vanilla key. Split due to codeline limits.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace gregCore.Infrastructure.Persistence;

public static partial class GregEntityInventory
{
    private static void AddEntry(Dictionary<string, Entry> byUid, Dictionary<string, Entry> byKindKey,
        HashSet<string> usedUids, InventoryKind kind, string nativeKey, string uid, string hint)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nativeKey) || string.IsNullOrWhiteSpace(uid)) return;
            if (!usedUids.Add(uid)) return; // UID collision: keep first entry
            var e = new Entry(kind, nativeKey.Trim(), uid.Trim(), hint ?? "");
            byUid[e.Uid] = e;
            byKindKey[KindKey(kind, e.NativeKey)] = e;
        }
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
                // Already healed (normal case in the LoadNetworkState postfix) ->
                // UID is the gregID itself. Fallback (unhealed): deterministic
                // from the FULL ID (no stripping - "X_0"/"X_1" must not
                // collide), stable until healing + save, then gregID.
                string uid = IsGregDeviceUid(id, ServerPrefix)
                    ? id
                    : ServerPrefix + NewHex(id);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Server, id, uid, "");
            }
        }
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
                // See CollectServers (healing normal case + full-ID fallback).
                string uid = IsGregDeviceUid(id, SwitchPrefix)
                    ? id
                    : SwitchPrefix + NewHex(id);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Switch, id, uid, "");
            }
        }
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
                // See CollectServers (healing normal case + full-ID fallback).
                string uid = IsGregDeviceUid(id, PatchPanelPrefix)
                    ? id
                    : PatchPanelPrefix + NewHex(id);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.PatchPanel, id, uid, "");
            }
        }
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
                    try { routes = r.routes != null ? r.routes.Count : 0; } catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
                    try { rules = f.filterRules != null ? f.filterRules.Count : 0; } catch { /* ignored: best-effort save scan, faulty entry skipped */ }
                    hint = "cluster:" + (f.clusterIP ?? "") + "/rules:" + rules.ToString(CultureInfo.InvariantCulture);
                }
                catch { continue; }
                string nativeKey = "firewall#" + i.ToString(CultureInfo.InvariantCulture);
                string uid = ResolvePersistedUid(InventoryKind.Firewall, nativeKey, hint, hints, usedUids);
                AddEntry(byUid, byKindKey, usedUids, InventoryKind.Firewall, nativeKey, uid, hint);
                RememberPersisted(InventoryKind.Firewall, nativeKey, uid, hint);
            }
        }
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
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
                    Vector3 pos = s.position;
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
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
    }

    /// <summary>
    /// UID resolution for kinds without a stable vanilla key: exact hit
    /// (key + hint) wins, otherwise hint repair across the whole list,
    /// otherwise a new UID. Prevents UID churn on list rebuild.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1172:Unused method parameters should be removed", Justification = "Reserved for the hint-repair pass (persisted UID hints across list rebuilds, see FreshData.Hints). Intentionally unwired until GregEntityInventoryCollect enables it.")]
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
            // Repair: find hint elsewhere in the list (index shift).
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
        catch { /* ignored: best-effort save scan, faulty entry skipped */ }
    }
}
