using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace greg.Sdk.Services;

public enum GregDeepFlowStatus
{
    Active,
    Idle,
    Isolated,
    Broken,
    PoweredOff
}

public sealed class GregResetSwitchScanItem
{
    public NetworkSwitch Instance;
    public string SwitchId;
    public string Label;
    public GregDeepFlowStatus DeepStatus;
    public bool IsOn;
    public bool IsBroken;
}

public static class GregResetSwitchService
{
    public static List<GregResetSwitchScanItem> ScanAllSwitches()
    {
        var result = new List<GregResetSwitchScanItem>();
        var switches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>(true);

        foreach (var sw in switches)
        {
            if (sw == null || string.IsNullOrWhiteSpace(sw.switchId))
            {
                continue;
            }

            result.Add(new GregResetSwitchScanItem
            {
                Instance = sw,
                SwitchId = sw.switchId,
                Label = string.IsNullOrWhiteSpace(sw.label) ? sw.name : sw.label,
                DeepStatus = EvaluateDeepStatus(sw),
                IsOn = sw.isOn,
                IsBroken = sw.isBroken
            });
        }

        return result;
    }

    public static GregDeepFlowStatus EvaluateDeepStatus(NetworkSwitch sw)
    {
        if (sw == null)
        {
            return GregDeepFlowStatus.Broken;
        }

        if (sw.isBroken)
        {
            return GregDeepFlowStatus.Broken;
        }

        if (sw.cableLinkSwitchPorts != null)
        {
            float totalSpeed = 0f;
            var connectedPorts = 0;

            for (var i = 0; i < sw.cableLinkSwitchPorts.Count; i++)
            {
                var port = sw.cableLinkSwitchPorts[i];
                if (port == null)
                {
                    continue;
                }

                connectedPorts++;
                totalSpeed += port.connectionSpeed;
            }

            if (totalSpeed > 0.01f)
            {
                return GregDeepFlowStatus.Active;
            }

            if (connectedPorts > 0)
            {
                return sw.isOn ? GregDeepFlowStatus.Idle : GregDeepFlowStatus.Isolated;
            }
        }

        if (!sw.isOn)
        {
            return GregDeepFlowStatus.PoweredOff;
        }

        if (NetworkMap.instance?.adjacencyList != null && NetworkMap.instance.adjacencyList.ContainsKey(sw.switchId))
        {
            var neighbors = NetworkMap.instance.adjacencyList[sw.switchId];
            if (neighbors != null && neighbors.Count > 0)
            {
                return GregDeepFlowStatus.Idle;
            }
        }

        return GregDeepFlowStatus.Isolated;
    }

    public static bool HasPathToService(string startSwitchId)
    {
        if (NetworkMap.instance == null || NetworkMap.instance.adjacencyList == null)
        {
            return false;
        }

        var visited = new HashSet<string>();
        var queue = new Queue<string>();

        queue.Enqueue(startSwitchId);
        visited.Add(startSwitchId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            if (IsNodeAService(currentId))
            {
                return true;
            }

            if (!NetworkMap.instance.adjacencyList.ContainsKey(currentId))
            {
                continue;
            }

            var neighbors = NetworkMap.instance.adjacencyList[currentId];
            if (neighbors == null)
            {
                continue;
            }

            for (var i = 0; i < neighbors.Count; i++)
            {
                var edge = neighbors[i];
                var nextId = Traverse.Create(edge).Field("Item1").GetValue<string>();
                if (string.IsNullOrWhiteSpace(nextId) || visited.Contains(nextId))
                {
                    continue;
                }

                visited.Add(nextId);
                queue.Enqueue(nextId);
            }
        }

        return false;
    }

    public static bool RepairMountedState(NetworkSwitch sw)
    {
        if (sw == null || string.IsNullOrWhiteSpace(sw.switchId) || sw.isBroken)
        {
            return false;
        }

        try
        {
            sw.TurnOffCommonFunctions();
            sw.isBroken = false;
            sw.isOn = true;
            sw.TurnOnCommonFunction();
            sw.UpdateScreenUI();
            ForceNetworkRebuild($"repair:{sw.switchId}");
            return sw.isOn;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[GregResetSwitchService] Repair failed for {sw.switchId}: {ex.Message}");
            return false;
        }
    }

    public static void FactoryResetSwitch(NetworkSwitch sw)
    {
        if (sw == null)
        {
            return;
        }

        GregNetworkMaintenanceService.FactoryReset(sw);
        ForceNetworkRebuild($"reset:{sw.switchId}");
    }

    public static void ForceNetworkRebuild(string reason)
    {
        try
        {
            if (NetworkMap.instance != null)
            {
                TryInvokeFirst(NetworkMap.instance,
                    "ClearMap",
                    "RebuildMap",
                    "ValidateTopology",
                    "RecalculateAllPaths",
                    "RefreshMap");
            }

            var switches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>(true);
            for (var i = 0; i < switches.Length; i++)
            {
                var sw = switches[i];
                if (sw == null)
                {
                    continue;
                }

                if (sw.isOn && !sw.isBroken)
                {
                    TryInvokeFirst(sw,
                        "TurnOnCommonFunction",
                        "TurnOnCommonFunctions",
                        "UpdateConnectionStatus",
                        "RefreshPorts",
                        "RecalculateFlow");
                }

                sw.UpdateScreenUI();
            }

            var cables = UnityEngine.Object.FindObjectsOfType<CableLink>(true);
            for (var i = 0; i < cables.Length; i++)
            {
                var cable = cables[i];
                if (cable == null)
                {
                    continue;
                }

                TryInvokeFirst(cable,
                    "ValidateConnection",
                    "RefreshConnection",
                    "RecalculateConnection",
                    "UpdateConnection");
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[GregResetSwitchService] ForceNetworkRebuild failed ({reason}): {ex.Message}");
        }
    }

    private static bool IsNodeAService(string nodeId)
    {
        var servers = UnityEngine.Object.FindObjectsOfType<Server>(true);
        foreach (var server in servers)
        {
            if (server.ServerID != nodeId)
            {
                continue;
            }

            var customerId = Traverse.Create(server).Field("customerId").GetValue<int>();
            if (customerId != 0)
            {
                return true;
            }
        }

        var cables = UnityEngine.Object.FindObjectsOfType<CableLink>(true);
        foreach (var cable in cables)
        {
            if (cable.switchID == nodeId && cable.CustomerID != 0)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryInvokeFirst(object target, params string[] methodNames)
    {
        if (target == null || methodNames == null)
        {
            return false;
        }

        for (var i = 0; i < methodNames.Length; i++)
        {
            if (TryInvokeZeroArg(target, methodNames[i]))
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
            var methods = target.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                if (!string.Equals(method.Name, methodName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (method.GetParameters().Length == 0)
                {
                    method.Invoke(target, null);
                    return true;
                }
            }
        }
        catch
        {
        }

        return false;
    }
}
