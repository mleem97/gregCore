using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using greg.Core;
using greg.Sdk.Services;
using Il2Cpp;
using greg.Mods.ResetSwitch.Config;
using greg.Mods.ResetSwitch.Integration;
using MelonLoader;
using UnityEngine;

namespace greg.Mods.ResetSwitch.Core;

public static class SwitchResetter
{
    public static int ResetMany(List<SwitchInfo> switches)
    {
        if (switches == null || switches.Count == 0)
        {
            return 0;
        }

        string backupPath = null;
        if (ModConfig.CreateBackup.Value)
        {
            backupPath = BackupManager.CreateBackup(switches);
            MelonLogger.Msg($"[SwitchResetter] Backup created: {backupPath}");
            GregCoreIntegration.FireBackupCreated(backupPath, switches.Count);
        }

        var resetCount = 0;
        foreach (var info in switches.Where(s => s?.Instance != null))
        {
            if (ResetInternal(info, backupPath))
            {
                resetCount++;
            }
        }

        return resetCount;
    }

    public static void Reset(NetworkSwitch sw)
    {
        if (sw == null) return;

        var info = new SwitchInfo
        {
            Instance = sw,
            Name = sw.label,
            RackId = "UNKNOWN"
        };

        ResetMany(new List<SwitchInfo> { info });
    }

    private static bool ResetInternal(SwitchInfo info, string backupPath)
    {
        var sw = info.Instance;
        if (sw == null)
        {
            return false;
        }

        if (!GregCoreIntegration.FireBeforeReset(sw.switchId, sw.label, info.RackId, "manual"))
        {
            MelonLogger.Warning($"[SwitchResetter] Reset canceled by hook for {sw.switchId}");
            return false;
        }

        try
        {
            MelonLogger.Msg($"[Reset] === FACTORY RESET: {sw.switchId} ===");
            MelonLogger.Msg($"[Reset] Pre-state: isOn={sw.isOn} isBroken={sw.isBroken} ports={GetPortCount(sw)}");

            if (TryGregCoreFactoryReset(sw))
            {
                MelonLogger.Msg($"[Reset] FactoryReset returned from gregCore service");
                NetworkRebuildHelper.ForceRebuild($"reset:{sw.switchId}");
                MelonLogger.Msg($"[Reset] Post-state: isOn={sw.isOn} isBroken={sw.isBroken} ports={GetPortCount(sw)}");
                GregCoreIntegration.FireSwitchReset(sw.switchId, sw.label, info.RackId, backupPath);
                MelonLogger.Msg($"[Reset] ✓ Done: {sw.switchId}");
                return true;
            }

            MelonLogger.Warning("[Reset] gregCore FactoryReset unavailable, using local fallback sequence.");
            MelonLogger.Msg($"[SwitchResetter] >>> INITIATING FACTORY RESET: ID={sw.switchId}, Label={sw.label} <<<");

            MelonLogger.Msg("[SwitchResetter] Step 1: Powering OFF...");
            sw.isOn = false;
            TryInvokeBool(sw, "PowerButton", false);

            MelonLogger.Msg("[SwitchResetter] Step 2: Disconnecting cables...");
            DisconnectAllCables(sw);

            MelonLogger.Msg("[SwitchResetter] Step 3: Clearing LACP groups...");
            TryInvokeFirst(sw, "ClearLacpGroups", "ClearAllLacpGroups", "RemoveAllLacpGroups", "LacpGroupRemoved");

            MelonLogger.Msg("[SwitchResetter] Step 4: Clearing map/routing...");
            TryInvokeFirst(sw, "ClearMap", "ResetRouting", "RebuildRoutingMap");
            if (NetworkMap.instance != null)
            {
                TryInvokeFirst(NetworkMap.instance, "ClearMap", "RebuildMap", "ValidateTopology");
            }

            MelonLogger.Msg("[SwitchResetter] Step 5: Resetting switch config state...");
            TryInvokeFirst(sw, "SwitchConfigClosed", "CloseSwitchConfig", "ResetConfig");

            MelonLogger.Msg("[SwitchResetter] Step 6: Purging VLAN port configurations...");
            int vlanCleared = gregGameHooks.ClearDisallowedVlansPerPort(sw);
            if (vlanCleared > 0)
            {
                MelonLogger.Msg($"[SwitchResetter] -> Cleared {vlanCleared} disallowed VLAN entries.");
            }
            else
            {
                MelonLogger.Msg("[SwitchResetter] -> No VLAN configuration found to clear.");
            }

            MelonLogger.Msg("[SwitchResetter] Step 7: Repairing hardware status (isBroken = false)...");
            sw.isBroken = false;

            MelonLogger.Msg("[SwitchResetter] Step 8: Re-initializing common game functions...");
            sw.TurnOffCommonFunctions();
            sw.TurnOnCommonFunction();
            TryInvokeFirst(sw, "TurnOffCommonFunctions");
            TryInvokeFirst(sw, "TurnOnCommonFunction", "TurnOnCommonFunctions");

            MelonLogger.Msg("[SwitchResetter] Step 9: Powering ON...");
            sw.isOn = true;
            TryInvokeBool(sw, "PowerButton", true);

            MelonLogger.Msg("[SwitchResetter] Step 10: Verifying topology and updating UI...");
            sw.UpdateScreenUI();
            var verified = FlowSimulator.HasPathToService(sw.switchId);
            var powerOkay = sw.isOn;
            var brokenOkay = !sw.isBroken;
            var health = FlowAnalyzer.Analyze(sw);
            MelonLogger.Msg($"[SwitchResetter] Verify: topology={verified}, power={powerOkay}, broken={brokenOkay}, flow={health}");

            if (!powerOkay || !brokenOkay)
            {
                MelonLogger.Warning($"[SwitchResetter] Post-check failed for {sw.switchId}. Reset considered unsuccessful.");
                return false;
            }

            NetworkRebuildHelper.ForceRebuild($"reset:{sw.switchId}");

            GregCoreIntegration.FireSwitchReset(sw.switchId, sw.label, info.RackId, backupPath);

            MelonLogger.Msg($"[SwitchResetter] >>> FACTORY RESET COMPLETED SUCCESSFULLY for {sw.switchId} <<<");
            MelonLogger.Msg($"[Reset] Post-state: isOn={sw.isOn} isBroken={sw.isBroken} ports={GetPortCount(sw)}");
            MelonLogger.Msg($"[Reset] ✓ Done: {sw.switchId}");
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[Reset] ✗ Exception on {sw.switchId}: {ex.Message}");
            MelonLogger.Error($"[Reset] StackTrace: {ex.StackTrace}");
            MelonLogger.Error($"[SwitchResetter] !!! CRITICAL FAILURE DURING RESET of {sw.switchId} !!!");
            MelonLogger.Error($"[SwitchResetter] Error: {ex.Message}");
            MelonLogger.Error($"[SwitchResetter] StackTrace: {ex.StackTrace}");
            return false;
        }
    }

    private static bool TryGregCoreFactoryReset(NetworkSwitch sw)
    {
        try
        {
            MelonLogger.Msg($"[Reset] Calling greg.Sdk.Services.GregResetSwitchService.FactoryResetSwitch() on {sw.switchId}");
            greg.Sdk.Services.GregResetSwitchService.FactoryResetSwitch(sw);
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Reset] gregCore FactoryReset call failed: {ex.Message}");
        }

        return false;
    }

    private static int GetPortCount(NetworkSwitch sw)
    {
        if (sw?.cableLinkSwitchPorts == null)
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

    private static void DisconnectAllCables(NetworkSwitch sw)
    {
        TryInvokeFirst(sw, "ClearAllCables", "DisconnectAllCables");

        if (sw.cableLinkSwitchPorts == null)
        {
            return;
        }

        var disconnected = 0;
        for (var i = 0; i < sw.cableLinkSwitchPorts.Count; i++)
        {
            var cable = sw.cableLinkSwitchPorts[i];
            if (cable == null)
            {
                continue;
            }

            if (TryInvokeFirst(cable, "Disconnect", "BreakConnection", "ClearConnection", "Unplug"))
            {
                disconnected++;
            }

            TrySetNumber(cable, "connectionSpeed", 0f);
        }

        var before = sw.cableLinkSwitchPorts.Count;
        for (var i = 0; i < sw.cableLinkSwitchPorts.Count; i++)
        {
            sw.cableLinkSwitchPorts[i] = null;
        }

        MelonLogger.Msg($"[SwitchResetter] Cable cleanup: method-disconnects={disconnected}, slots cleared={before}");
    }

    private static bool TryInvokeFirst(object target, params string[] methodNames)
    {
        foreach (var methodName in methodNames)
        {
            if (TryInvokeZeroArg(target, methodName))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryInvokeZeroArg(object target, string methodName)
    {
        if (target == null || string.IsNullOrWhiteSpace(methodName))
        {
            return false;
        }

        try
        {
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    method.Invoke(target, null);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SwitchResetter] Optional invoke {methodName} failed: {ex.Message}");
        }

        return false;
    }

    private static void TryInvokeBool(object target, string methodName, bool arg)
    {
        if (target == null || string.IsNullOrWhiteSpace(methodName))
        {
            return;
        }

        try
        {
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(bool))
                {
                    method.Invoke(target, new object[] { arg });
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SwitchResetter] Optional invoke {methodName} failed: {ex.Message}");
        }
    }

    private static void TrySetNumber(object target, string memberName, float value)
    {
        if (target == null)
        {
            return;
        }

        try
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            var field = target.GetType().GetField(memberName, flags);
            if (field != null && field.FieldType == typeof(float))
            {
                field.SetValue(target, value);
                return;
            }

            var property = target.GetType().GetProperty(memberName, flags);
            if (property != null && property.PropertyType == typeof(float) && property.CanWrite)
            {
                property.SetValue(target, value);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SwitchResetter] Failed to set {memberName}: {ex.Message}");
        }
    }
}
