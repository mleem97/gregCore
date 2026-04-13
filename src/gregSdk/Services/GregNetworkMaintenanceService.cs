using System;
using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using greg.Core;

namespace gregCoreSDK.Sdk.Services;

public static class GregNetworkMaintenanceService
{
    public struct SwitchStatus
    {
        public string Id;
        public string Label;
        public bool IsOn;
        public bool IsBroken;
        public bool HasPhysicalFlow;
        public NetworkSwitch NativeInstance;
    }

    public static System.Collections.Generic.List<SwitchStatus> GetAllInstalledSwitches()
    {
        var result = new System.Collections.Generic.List<SwitchStatus>();
        var switches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>(true);

        foreach (var sw in switches)
        {
            // Check if it's in a rack
            bool inRack = sw.GetComponentInParent<RackPosition>() != null;
            if (!inRack) continue;

            bool hasFlow = false;
            if (NetworkMap.instance != null && NetworkMap.instance.adjacencyList != null)
            {
                if (NetworkMap.instance.adjacencyList.ContainsKey(sw.switchId))
                {
                    var connections = NetworkMap.instance.adjacencyList[sw.switchId];
                    if (connections != null && connections.Count > 0)
                    {
                        hasFlow = true;
                    }
                }
            }

            result.Add(new SwitchStatus
            {
                Id = sw.switchId,
                Label = sw.label,
                IsOn = sw.isOn,
                IsBroken = sw.isBroken,
                HasPhysicalFlow = hasFlow,
                NativeInstance = sw
            });
        }

        return result;
    }

    public static void FactoryReset(NetworkSwitch sw)
    {
        if (sw == null) return;

        try
        {
            MelonLogger.Msg($"[GregNetworkMaintenance] Resetting Switch: {sw.switchId} ({sw.label})");

            // 1. Repair and Power On
            sw.isBroken = false;
            sw.isOn = true;

            // 2. Clear VLAN config (Factory Default = All Allowed)
            int vlanCleared = gregGameHooks.ClearDisallowedVlansPerPort(sw);
            if (vlanCleared > 0)
            {
                MelonLogger.Msg($"[GregNetworkMaintenance] -> Cleared {vlanCleared} disallowed VLAN entries.");
            }

            // 3. Update Visuals
            sw.UpdateScreenUI();
            
            // 4. Force Network Map Update if possible
            // NetworkMap doesn't have a direct "Rebuild" but connecting/disconnecting triggers it.
            // We'll just rely on the game's next tick.
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregNetworkMaintenance] Failed to reset switch {sw.switchId}: {ex.Message}");
        }
    }

    public static void ResetAllBrokenSwitches()
    {
        var switches = GetAllInstalledSwitches();
        foreach (var status in switches)
        {
            if (status.IsBroken || !status.HasPhysicalFlow)
            {
                FactoryReset(status.NativeInstance);
            }
        }
    }
}
