using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;

namespace DataCenterModLoader;

public static partial class GameHooks
{
    /// <summary>
    /// The new game update added <c>CommandCenterOperator</c> NPCs that must be hired before
    /// <c>ProcessDispatchQueue</c> will move jobs from <c>pendingDispatches</c> to actual
    /// technicians.  If no operator is hired the queue grows forever while techs stand idle.
    ///
    /// This method bypasses that requirement: it drains <c>pendingDispatches</c> directly and
    /// calls <c>Technician.AssignJob</c> on every free technician we can find.  It is called
    /// immediately after every <c>SendTechnician</c> so the SysAdmin mod keeps working even
    /// without a hired Command-Center Operator.
    /// </summary>
    public static void ForceProcessPendingQueue(TechnicianManager tm)
    {
        try
        {
            var pending = tm.pendingDispatches;
            if (pending == null || pending.Count == 0) return;

            var techs = tm.technicians;
            if (techs == null || techs.Count == 0) return;

            // Build active-technician set via GetActiveJobs() for accuracy
            var activeTechIds = BuildActiveTechIdSet(tm);

            int assigned = AssignPendingToFreeTechs(tm, activeTechIds);

            if (assigned > 0)
                CrashLog.Log($"ForceProcessPendingQueue: force-assigned {assigned} job(s) (bypassed CommandCenterOperator check)");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ForceProcessPendingQueue", ex);
        }
    }

    private static HashSet<int> BuildActiveTechIdSet(TechnicianManager tm)
    {
        var activeTechIds = new System.Collections.Generic.HashSet<int>();
        try
        {
            var activeJobs = tm.GetActiveJobs();
            if (activeJobs != null)
            {
                foreach (var aj in activeJobs)
                    CollectActiveTechId(aj, activeTechIds);
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return activeTechIds;
    }

    private static void CollectActiveTechId(object jobObj, HashSet<int> activeTechIds)
    {
        try
        {
            dynamic aj = jobObj;
            if (aj.assignedTechnician != null)
                activeTechIds.Add((int)aj.assignedTechnician.technicianID);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static int AssignPendingToFreeTechs(TechnicianManager tm, HashSet<int> activeTechIds)
    {
        int assigned = 0;
        var pending = tm.pendingDispatches;
        var techs = tm.technicians;
        if (pending == null || techs == null) return 0;
        for (int i = 0; i < techs.Count && pending.Count > 0; i++)
        {
            try
            {
                var tech = techs[i];
                if (tech == null) continue;

                // Skip techs that are already working
                if (IsTechBusy(tech, activeTechIds)) continue;

                // Dequeue next pending job and assign directly
                var job = pending.Dequeue();
                tech.AssignJob(job);
                assigned++;

                LogForceAssignedJob(job, tech);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return assigned;
    }

    private static bool IsTechBusy(Technician tech, HashSet<int> activeTechIds)
    {
        bool busy = activeTechIds.Contains(tech.technicianID);
        if (!busy)
        {
            try { busy = tech.isBusy; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return busy;
    }

    private static void LogForceAssignedJob(object jobObj, Technician tech)
    {
        try
        {
            string deviceName = ReadJobDeviceName(jobObj);
            CrashLog.Log($"ForceProcessPendingQueue: assigned '{deviceName}' → tech #{tech.technicianID} ({tech.technicianName})");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static string ReadJobDeviceName(object jobObj)
    {
        try
        {
            dynamic job = jobObj;
            return (string)job.DeviceName;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "?";
    }

    private static void LogDispatchSkipped(string op, int skipped, int total)
    {
        if (skipped > 0)
            CrashLog.Log($"{op}: no target — {skipped}/{total} device(s) already assigned in queue");
    }

    private static bool TryGetDispatchTargets(out NetworkMap nm, out TechnicianManager tm)
    {
        nm = NetworkMap.instance;
        tm = TechnicianManager.instance;
        return nm != null && tm != null;
    }

    private static bool HasNoFreeTechnician()
    {
        return GetFreeTechnicianCount() == 0;
    }

    // Returns: 1 = dispatched, 0 = no target, -1 = no free technician
    public static int DispatchRepairServer()
    {
        try
        {
            if (!TryGetDispatchTargets(out var nm, out var tm)) return 0;

            if (HasNoFreeTechnician()) return -1;

            var dict = nm.brokenServers;
            if (dict == null || dict.Count == 0) return 0;

            // copy keys to avoid iteration issues
            var keys = new System.Collections.Generic.List<string>();
            foreach (var kvp in dict) keys.Add(kvp.Key);

            return TryDispatchRepairServer(tm, keys, key =>
            {
                try { return dict[key]; }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            });
        }
        catch { return 0; }
    }

    private static int TryDispatchRepairServer(TechnicianManager tm, List<string> keys, Func<string, Server> getter)
    {
        int skipped = 0;
        foreach (var key in keys)
        {
            try
            {
                Server server = getter(key);
                if (server == null) continue;

                if (tm.IsDeviceAlreadyAssigned(null, server)) { skipped++; continue; }

                tm.SendTechnician(null, server);
                ForceProcessPendingQueue(tm);
                return 1;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        LogDispatchSkipped("DispatchRepairServer", skipped, keys.Count);
        return 0;
    }

    public static int DispatchRepairSwitch()
    {
        try
        {
            if (!TryGetDispatchTargets(out var nm, out var tm)) return 0;

            if (HasNoFreeTechnician()) return -1;

            var dict = nm.brokenSwitches;
            if (dict == null || dict.Count == 0) return 0;

            var keys = new System.Collections.Generic.List<string>();
            foreach (var kvp in dict) keys.Add(kvp.Key);

            return TryDispatchRepairSwitch(tm, keys, key =>
            {
                try { return dict[key]; }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            });
        }
        catch { return 0; }
    }

    private static int TryDispatchRepairSwitch(TechnicianManager tm, List<string> keys, Func<string, NetworkSwitch> getter)
    {
        int skipped = 0;
        foreach (var key in keys)
        {
            try
            {
                NetworkSwitch sw = getter(key);
                if (sw == null) continue;

                if (tm.IsDeviceAlreadyAssigned(sw, null)) { skipped++; continue; }

                tm.SendTechnician(sw, null);
                ForceProcessPendingQueue(tm);
                return 1;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        LogDispatchSkipped("DispatchRepairSwitch", skipped, keys.Count);
        return 0;
    }

    public static int DispatchReplaceServer()
    {
        try
        {
            if (!TryGetDispatchTargets(out var nm, out var tm)) return 0;

            if (HasNoFreeTechnician()) return -1;

            var dict = nm.servers;
            if (dict == null || dict.Count == 0) return 0;

            var keys = new System.Collections.Generic.List<string>();
            foreach (var kvp in dict) keys.Add(kvp.Key);

            return TryDispatchReplaceServer(tm, keys, key =>
            {
                try
                {
                    var endpoint = dict[key];
                    if (endpoint == null) return null;
                    return endpoint.TryCast<Server>();
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            });
        }
        catch { return 0; }
    }

    private static bool IsServerReplaceCandidate(Server server)
    {
        if (server == null) return false;
        if (server.isBroken) return false;
        if (server.eolTime > 0) return false;
        return true;
    }

    private static int TryDispatchReplaceServer(TechnicianManager tm, List<string> keys, Func<string, Server> getter)
    {
        int skipped = 0;
        foreach (var key in keys)
        {
            try
            {
                Server server = getter(key);
                if (!IsServerReplaceCandidate(server)) continue;

                if (tm.IsDeviceAlreadyAssigned(null, server)) { skipped++; continue; }

                tm.SendTechnician(null, server);
                ForceProcessPendingQueue(tm);
                return 1;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        LogDispatchSkipped("DispatchReplaceServer", skipped, keys.Count);
        return 0;
    }

    public static int DispatchReplaceSwitch()
    {
        try
        {
            if (!TryGetDispatchTargets(out var nm, out var tm)) return 0;

            if (HasNoFreeTechnician()) return -1;

            var dict = nm.switches;
            if (dict == null || dict.Count == 0) return 0;

            var keys = new System.Collections.Generic.List<string>();
            foreach (var kvp in dict) keys.Add(kvp.Key);

            return TryDispatchReplaceSwitch(tm, keys, key =>
            {
                try { return dict[key]; }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            });
        }
        catch { return 0; }
    }

    private static bool IsSwitchReplaceCandidate(NetworkSwitch sw)
    {
        if (sw == null) return false;
        if (sw.isBroken) return false;
        // Check both warning signs AND eolTime countdown (like servers)
        bool isEol = sw.existingWarningSigns > 0;
        if (!isEol)
        {
            try { isEol = sw.eolTime <= 0; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return isEol;
    }

    private static int TryDispatchReplaceSwitch(TechnicianManager tm, List<string> keys, Func<string, NetworkSwitch> getter)
    {
        int skipped = 0;
        foreach (var key in keys)
        {
            try
            {
                NetworkSwitch sw = getter(key);
                if (!IsSwitchReplaceCandidate(sw)) continue;

                if (tm.IsDeviceAlreadyAssigned(sw, null)) { skipped++; continue; }

                tm.SendTechnician(sw, null);
                ForceProcessPendingQueue(tm);
                return 1;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        LogDispatchSkipped("DispatchReplaceSwitch", skipped, keys.Count);
        return 0;
    }
}
