using System;
using System.Collections.Generic;
using greg.Sdk.Services;
using Il2Cpp;
using MelonLoader;

namespace greg.Mods.ResetSwitch.Core;

public static class MountedStateRepairer
{
    public enum RepairResult
    {
        Fixed,
        AlreadyOK,
        Skipped,
        Failed
    }

    public struct SwitchRepairReport
    {
        public string SwitchId;
        public string Label;
        public RepairResult Result;
        public string Reason;
    }

    public static List<SwitchRepairReport> Analyze()
    {
        var reports = new List<SwitchRepairReport>();
        var switches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>(true);
        MelonLogger.Msg($"[MountedRepairer] Starting analysis — {switches.Length} switches to check");

        var okCount = 0;
        var candidateCount = 0;
        var skippedCount = 0;

        foreach (var sw in switches)
        {
            var report = AnalyzeOne(sw);
            reports.Add(report);

            switch (report.Result)
            {
                case RepairResult.AlreadyOK:
                    okCount++;
                    break;
                case RepairResult.Fixed:
                    candidateCount++;
                    break;
                default:
                    skippedCount++;
                    break;
            }
        }

        MelonLogger.Msg($"[MountedRepairer] Analysis complete: {okCount} OK, {candidateCount} need repair, {skippedCount} skipped");
        return reports;
    }

    public static RepairResult RepairOne(NetworkSwitch sw)
    {
        var result = RepairOneInternal(sw, out _, out _);
        return result;
    }

    public static List<SwitchRepairReport> RepairAll()
    {
        var reports = new List<SwitchRepairReport>();
        var switches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>(true);

        var fixedCount = 0;
        var failedCount = 0;

        foreach (var sw in switches)
        {
            var result = RepairOneInternal(sw, out var reason, out var label);
            var switchId = sw == null ? string.Empty : sw.switchId;
            reports.Add(new SwitchRepairReport
            {
                SwitchId = switchId,
                Label = label,
                Result = result,
                Reason = reason
            });

            if (result == RepairResult.Fixed)
            {
                fixedCount++;
            }
            else if (result == RepairResult.Failed)
            {
                failedCount++;
            }
        }

        MelonLogger.Msg($"[MountedRepairer] === REPAIR ALL COMPLETE: {fixedCount} fixed, {failedCount} failed ===");
        return reports;
    }

    private static SwitchRepairReport AnalyzeOne(NetworkSwitch sw)
    {
        if (sw == null)
        {
            return new SwitchRepairReport { SwitchId = string.Empty, Label = string.Empty, Result = RepairResult.Skipped, Reason = "null reference" };
        }

        var label = string.IsNullOrWhiteSpace(sw.label) ? sw.name : sw.label;
        if (string.IsNullOrEmpty(sw.switchId))
        {
            return new SwitchRepairReport { SwitchId = string.Empty, Label = label, Result = RepairResult.Skipped, Reason = "no switchId (template)" };
        }

        var ports = CountPorts(sw);

        if (sw.isOn && ports > 0)
        {
            MelonLogger.Msg($"[MountedRepairer] {sw.switchId} ({label}): AlreadyOK — isOn=true, {ports} ports active");
            return new SwitchRepairReport { SwitchId = sw.switchId, Label = label, Result = RepairResult.AlreadyOK, Reason = "already mounted" };
        }

        if (sw.isBroken)
        {
            MelonLogger.Msg($"[MountedRepairer] {sw.switchId} ({label}): Skipped — isBroken=true");
            return new SwitchRepairReport { SwitchId = sw.switchId, Label = label, Result = RepairResult.Skipped, Reason = "hardware broken, reset first" };
        }

        if (!sw.isOn && !sw.isBroken)
        {
            MelonLogger.Msg($"[MountedRepairer] {sw.switchId} ({label}): CANDIDATE — isOn=false, isBroken=false → needs repair");
            return new SwitchRepairReport { SwitchId = sw.switchId, Label = label, Result = RepairResult.Fixed, Reason = "candidate" };
        }

        return new SwitchRepairReport { SwitchId = sw.switchId, Label = label, Result = RepairResult.Skipped, Reason = "no actionable state" };
    }

    private static RepairResult RepairOneInternal(NetworkSwitch sw, out string reason, out string label)
    {
        label = sw == null ? string.Empty : (string.IsNullOrWhiteSpace(sw.label) ? sw.name : sw.label);

        if (sw == null)
        {
            reason = "null reference";
            return RepairResult.Skipped;
        }

        if (string.IsNullOrEmpty(sw.switchId))
        {
            reason = "no switchId (template)";
            return RepairResult.Skipped;
        }

        if (sw.isBroken)
        {
            reason = "hardware broken, reset first";
            return RepairResult.Skipped;
        }

        var portCount = CountPorts(sw);
        MelonLogger.Msg($"[MountedRepairer] Checking {sw.switchId} | isOn={sw.isOn} | isBroken={sw.isBroken} | ports={portCount}");

        if (sw.isOn && portCount > 0)
        {
            reason = "already mounted and active";
            return RepairResult.AlreadyOK;
        }

        try
        {
            MelonLogger.Msg($"[MountedRepairer] === REPAIR START: {sw.switchId} ({label}) ===");

            if (greg.Sdk.Services.GregResetSwitchService.RepairMountedState(sw) && sw.isOn)
            {
                reason = "repaired";
                MelonLogger.Msg($"[MountedRepairer] === REPAIR RESULT: {sw.switchId} → FIXED ===");
                return RepairResult.Fixed;
            }

            reason = "TurnOn had no effect";
            MelonLogger.Error($"[MountedRepairer] === REPAIR RESULT: {sw.switchId} → FAILED ===");
            return RepairResult.Failed;
        }
        catch (Exception ex)
        {
            reason = ex.Message;
            MelonLogger.Error($"[MountedRepairer] ✗ Failed on {sw.switchId}: {ex.Message}");
            MelonLogger.Error($"[MountedRepairer] StackTrace: {ex.StackTrace}");
            return RepairResult.Failed;
        }
    }

    private static int CountPorts(NetworkSwitch sw)
    {
        if (sw == null || sw.cableLinkSwitchPorts == null)
        {
            return 0;
        }

        var count = 0;
        for (var i = 0; i < sw.cableLinkSwitchPorts.Count; i++)
        {
            if (sw.cableLinkSwitchPorts[i] != null)
            {
                count++;
            }
        }

        return count;
    }
}
