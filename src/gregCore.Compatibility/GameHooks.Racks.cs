using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;

namespace DataCenterModLoader;

public static partial class GameHooks
{
    public static int EnsureAllRackPositionUIDs()
    {
        try
        {
            var mgr = MainGameManager.instance;
            if (mgr == null)
            {
                CrashLog.Log("[WorldSync] EnsureAllRackPositionUIDs: MainGameManager is null");
                return 0;
            }

            var sorted = CollectSortedRackPositions();
            if (sorted == null || sorted.Count == 0)
            {
                CrashLog.Log("[WorldSync] EnsureAllRackPositionUIDs: no RackPositions found");
                return 0;
            }

            int assigned = AssignRackPositionUIDs(mgr, sorted);

            RefreshServerRackRefs();
            RefreshSwitchRackRefs();
            RefreshPatchPanelRackRefs();

            CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs: assigned {assigned}/{sorted.Count} positions (counter now {mgr.lastUsedRackPositionGlobalUID})");
            return assigned;
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs failed: {ex.Message}");
            return 0;
        }
    }

    private static List<RackPosition> CollectSortedRackPositions()
    {
        var allPositions = UnityEngine.Object.FindObjectsOfType<RackPosition>();
        if (allPositions == null || allPositions.Count == 0)
            return new List<RackPosition>();

        var sorted = new List<RackPosition>();
        foreach (var rp in allPositions)
        {
            if (rp != null) sorted.Add(rp);
        }

        sorted.Sort(CompareRackPositions);
        return sorted;
    }

    private static int CompareRackPositions(RackPosition a, RackPosition b)
    {
        var pa = a.transform.position;
        var pb = b.transform.position;
        int cmp = pa.x.CompareTo(pb.x);
        if (cmp != 0) return cmp;
        cmp = pa.z.CompareTo(pb.z);
        if (cmp != 0) return cmp;
        cmp = pa.y.CompareTo(pb.y);
        if (cmp != 0) return cmp;
        cmp = a.positionIndex.CompareTo(b.positionIndex);
        if (cmp != 0) return cmp;

        string nameA = "", nameB = "";
        try { nameA = a.rack?.gameObject?.name ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try { nameB = b.rack?.gameObject?.name ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return string.Compare(nameA, nameB, StringComparison.Ordinal);
    }

    private static int AssignRackPositionUIDs(MainGameManager mgr, List<RackPosition> sorted)
    {
        const int SYNC_UID_BASE = 10000;
        mgr.lastUsedRackPositionGlobalUID = SYNC_UID_BASE;

        int assigned = 0;
        foreach (var rp in sorted)
        {
            try
            {
                mgr.lastUsedRackPositionGlobalUID++;
                rp.rackPosGlobalUID = mgr.lastUsedRackPositionGlobalUID;
                assigned++;
            }
            catch { /* field access can fail during teardown */ }
        }
        return assigned;
    }

    private static void RefreshServerRackRefs()
    {
        try
        {
            // Fast path: NetworkMap registry first (Bolt perf-opt), slow
            // FindObjectsOfType fallback when the map is unavailable.
            var netMap = global::Il2Cpp.NetworkMap.instance;
            int updated = 0;
            if (netMap != null && netMap.servers != null)
                updated = RefreshServerRefsFromMap(netMap);
            else
                updated = RefreshServerRefsFromScene();

            if (updated > 0)
                CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs: updated {updated} server rackPositionUID references");
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs: server ref update failed: {ex.Message}");
        }
    }

    private static int RefreshServerRefsFromMap(global::Il2Cpp.NetworkMap netMap)
    {
        int updated = 0;
        foreach (var kvp in netMap.servers)
        {
            Il2Cpp.Server srv = null;
            try { srv = kvp.Value?.TryCast<Il2Cpp.Server>(); } catch { continue; }
            try
            {
                if (srv != null && srv.currentRackPosition != null)
                {
                    int oldUid = srv.rackPositionUID;
                    int newUid = srv.currentRackPosition.rackPosGlobalUID;
                    if (oldUid != newUid)
                    {
                        srv.rackPositionUID = newUid;
                        updated++;
                    }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return updated;
    }

    private static int RefreshServerRefsFromScene()
    {
        int updated = 0;
        var servers = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Server>();
        foreach (var srv in servers)
        {
            try
            {
                if (srv.currentRackPosition != null)
                {
                    int oldUid = srv.rackPositionUID;
                    int newUid = srv.currentRackPosition.rackPosGlobalUID;
                    if (oldUid != newUid)
                    {
                        srv.rackPositionUID = newUid;
                        updated++;
                    }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return updated;
    }

    private static void RefreshSwitchRackRefs()
    {
        try
        {
            var netMapSw = global::Il2Cpp.NetworkMap.instance;
            int swUpdated = 0;
            if (netMapSw != null && netMapSw.switches != null)
                swUpdated = RefreshSwitchRefsFromMap(netMapSw);
            else
                swUpdated = RefreshSwitchRefsFromScene();

            if (swUpdated > 0)
                CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs: updated {swUpdated} switch rackPositionUID references");
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs: switch ref update failed: {ex.Message}");
        }
    }

    private static int RefreshSwitchRefsFromMap(global::Il2Cpp.NetworkMap netMapSw)
    {
        int swUpdated = 0;
        foreach (var kvp in netMapSw.switches)
        {
            Il2Cpp.NetworkSwitch sw = null;
            try { sw = kvp.Value?.TryCast<Il2Cpp.NetworkSwitch>(); } catch { continue; }
            try
            {
                if (sw != null && sw.currentRackPosition != null)
                {
                    int oldUid = sw.rackPositionUID;
                    int newUid = sw.currentRackPosition.rackPosGlobalUID;
                    if (oldUid != newUid)
                    {
                        sw.rackPositionUID = newUid;
                        swUpdated++;
                    }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return swUpdated;
    }

    private static int RefreshSwitchRefsFromScene()
    {
        int swUpdated = 0;
        var switches = UnityEngine.Object.FindObjectsOfType<Il2Cpp.NetworkSwitch>();
        foreach (var sw in switches)
        {
            try
            {
                if (sw.currentRackPosition != null)
                {
                    int oldUid = sw.rackPositionUID;
                    int newUid = sw.currentRackPosition.rackPosGlobalUID;
                    if (oldUid != newUid)
                    {
                        sw.rackPositionUID = newUid;
                        swUpdated++;
                    }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return swUpdated;
    }

    private static void RefreshPatchPanelRackRefs()
    {
        try
        {
            // No NetworkMap fast path for patch panels: the game type has
            // no patchPanels registry (verified against game assemblies —
            // only servers/switches exist). Scene scan only.
            int ppUpdated = RefreshPatchPanelRefsFromScene();

            if (ppUpdated > 0)
                CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs: updated {ppUpdated} patchpanel rackPositionUID references");
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] EnsureAllRackPositionUIDs: patchpanel ref update failed: {ex.Message}");
        }
    }


    private static int RefreshPatchPanelRefsFromScene()
    {
        int ppUpdated = 0;
        var panels = UnityEngine.Object.FindObjectsOfType<Il2Cpp.PatchPanel>();
        foreach (var pp in panels)
        {
            try
            {
                if (pp != null && pp.currentRackPosition != null)
                {
                    int oldUid = pp.rackPositionUID;
                    int newUid = pp.currentRackPosition.rackPosGlobalUID;
                    if (oldUid != newUid)
                    {
                        pp.rackPositionUID = newUid;
                        ppUpdated++;
                    }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return ppUpdated;
    }
}
