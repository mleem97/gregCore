using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;

namespace DataCenterModLoader;

public static partial class GameHooks
{
    private static int _eolSwitchDiagCounter = 0;

    public static uint GetEolServerCount()
    {
        try
        {
            var nm = NetworkMap.instance;
            if (nm == null) return 0;
            var dict = nm.servers;
            if (dict == null) return 0;

            // copy keys first to avoid Il2Cpp iteration issues
            var keys = new System.Collections.Generic.List<string>();
            foreach (var kvp in dict) keys.Add(kvp.Key);

            return CountEolServers(keys, key =>
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

    private static uint CountEolServers(List<string> keys, Func<string, Server> getter)
    {
        uint count = 0;
        foreach (var key in keys)
        {
            try
            {
                var server = getter(key);
                if (server == null) continue;
                if (IsServerEol(server)) count++;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return count;
    }

    private static bool IsServerEol(Server server)
    {
        if (server.isBroken) return false;
        // eolTime counts down; <= 0 means at/past EOL
        return server.eolTime <= 0;
    }

    public static uint GetEolSwitchCount()
    {
        try
        {
            var nm = NetworkMap.instance;
            if (nm == null) return 0;
            var dict = nm.switches;
            if (dict == null) return 0;

            var keys = new System.Collections.Generic.List<string>();
            foreach (var kvp in dict) keys.Add(kvp.Key);

            uint count = CountEolSwitches(keys, key =>
            {
                try { return dict[key]; }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            });

            TrackEolSwitchDiagnostics(count);
            return count;
        }
        catch { return 0; }
    }

    private static uint CountEolSwitches(List<string> keys, Func<string, NetworkSwitch> getter)
    {
        uint count = 0;
        foreach (var key in keys)
        {
            try
            {
                var sw = getter(key);
                if (sw == null) continue;
                if (sw.isBroken) continue;
                if (IsSwitchEol(sw)) count++;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return count;
    }

    private static bool IsSwitchEol(NetworkSwitch sw)
    {
        // Check both warning signs AND eolTime countdown (like servers)
        bool isEol = sw.existingWarningSigns > 0;
        if (!isEol)
        {
            try { isEol = sw.eolTime <= 0; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return isEol;
    }

    private static void TrackEolSwitchDiagnostics(uint count)
    {
        // Periodic diagnostic dump when EOL switches exist (every ~30s = 6 scans)
        if (count > 0)
        {
            _eolSwitchDiagCounter++;
            if (_eolSwitchDiagCounter >= 6)
            {
                _eolSwitchDiagCounter = 0;
                DumpSwitchDiagnostics();
            }
        }
        else
        {
            _eolSwitchDiagCounter = 0;
        }
    }

    public static uint GetFreeTechnicianCount()
    {
        try
        {
            var tm = TechnicianManager.instance;
            if (tm == null) return 0;

            var techs = tm.technicians;
            if (techs == null) return 0;
            int total = techs.Count;
            if (total == 0) return 0;

            // Primary: use GetActiveJobs() — counts all busy techs across all 6 slots
            try
            {
                return CountFreeByActiveJobs(tm, total);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            // Fallback: iterate isBusy per-technician (pre-update behaviour)
            return CountFreeByBusyFlag(tm, total);
        }
        catch { return 0; }
    }

    private static uint CountFreeByActiveJobs(TechnicianManager tm, int total)
    {
        var activeJobs = tm.GetActiveJobs();
        int activeCount = activeJobs != null ? activeJobs.Count : 0;
        return (uint)Math.Max(0, total - activeCount);
    }

    private static uint CountFreeByBusyFlag(TechnicianManager tm, int total)
    {
        uint count = 0;
        var techs = tm.technicians;
        for (int i = 0; i < total; i++)
        {
            try
            {
                var tech = techs[i];
                if (tech != null && !tech.isBusy) count++;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return count;
    }

    /// <summary>
    /// Logs detailed per-switch diagnostics to CrashLog so we can identify
    /// which switch is missing from EOL detection.
    /// </summary>
    public static void DumpSwitchDiagnostics()
    {
        try
        {
            var nm = NetworkMap.instance;
            var tm = TechnicianManager.instance;
            if (nm == null) { MelonLoader.MelonLogger.Msg("[SwitchDiag] NetworkMap is null"); return; }

            var dict = nm.switches;
            if (dict == null) { MelonLoader.MelonLogger.Msg("[SwitchDiag] switches dict is null"); return; }

            var keys = new System.Collections.Generic.List<string>();
            foreach (var kvp in dict) keys.Add(kvp.Key);

            MelonLoader.MelonLogger.Msg($"[SwitchDiag] --- {keys.Count} switch(es) in nm.switches ---");

            LogNormalSwitchEntries(keys, key =>
            {
                try { return dict[key]; }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            }, tm);
            LogBrokenSwitchEntries(nm);

            MelonLoader.MelonLogger.Msg($"[SwitchDiag] --- total: {keys.Count} normal + {CountBrokenSwitches(nm)} broken ---");
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Msg($"[SwitchDiag] exception: {ex.Message}");
        }
    }

    private static void LogNormalSwitchEntries(List<string> keys, Func<string, NetworkSwitch> getter, TechnicianManager tm)
    {
        foreach (var key in keys)
            LogSingleSwitchEntry(key, getter, tm);
    }

    private static void LogSingleSwitchEntry(string key, Func<string, NetworkSwitch> getter, TechnicianManager tm)
    {
        try
        {
            var sw = getter(key);
            if (sw == null) { MelonLoader.MelonLogger.Msg($"[SwitchDiag]   key={key} => null"); return; }

            bool broken = ReadSwitchBroken(sw);
            int warningSigns = ReadSwitchWarnings(sw);
            float eolTime = ReadSwitchEolTime(sw);

            bool assigned = false;
            try { if (tm != null) assigned = tm.IsDeviceAlreadyAssigned(sw, null); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            MelonLoader.MelonLogger.Msg(
                $"[SwitchDiag]   key={key} broken={broken} warningSigns={warningSigns} eolTime={eolTime:F1} assigned={assigned}"
            );
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Msg($"[SwitchDiag]   key={key} => exception: {ex.Message}");
        }
    }

    private static bool ReadSwitchBroken(NetworkSwitch sw)
    {
        try { return sw.isBroken; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    private static int ReadSwitchWarnings(NetworkSwitch sw)
    {
        try { return sw.existingWarningSigns; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return -999;
    }

    private static float ReadSwitchEolTime(NetworkSwitch sw)
    {
        try { return sw.eolTime; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return float.NaN;
    }

    private static int CountBrokenSwitches(NetworkMap nm)
    {
        try
        {
            var brokenDict = nm.brokenSwitches;
            if (brokenDict == null) return 0;
            var brokenKeys = new System.Collections.Generic.List<string>();
            foreach (var kvp in brokenDict) brokenKeys.Add(kvp.Key);
            return brokenKeys.Count;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return 0;
    }

    private static void LogBrokenSwitchEntries(NetworkMap nm)
    {
        // Also check brokenSwitches dict
        var brokenDict = nm.brokenSwitches;
        if (brokenDict == null) return;
        var brokenKeys = new System.Collections.Generic.List<string>();
        foreach (var kvp in brokenDict) brokenKeys.Add(kvp.Key);

        foreach (var key in brokenKeys)
            LogSingleBrokenSwitch(key, k =>
            {
                try { return brokenDict[k]; }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return null;
            });
    }

    private static void LogSingleBrokenSwitch(string key, Func<string, NetworkSwitch> getter)
    {
        try
        {
            var sw = getter(key);
            float eolTime = ReadSwitchEolTime(sw);
            int warningSigns = ReadSwitchWarnings(sw);

            MelonLoader.MelonLogger.Msg(
                $"[SwitchDiag]   BROKEN key={key} warningSigns={warningSigns} eolTime={eolTime:F1}"
            );
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
