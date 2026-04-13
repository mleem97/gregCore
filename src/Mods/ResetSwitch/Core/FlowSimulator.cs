using System;
using System.Collections.Generic;
using greg.Sdk.Services;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace greg.Mods.ResetSwitch.Core;

public enum DeepFlowStatus { Active, Idle, Isolated, Broken, PoweredOff }

public static class FlowSimulator
{
    /// <summary>
    /// Checks if a switch has a logical path to a "Service" (a node with a CustomerID).
    /// </summary>
    public static bool HasPathToService(string startSwitchId)
    {
        return greg.Mods.ResetSwitchService.HasPathToService(startSwitchId);
    }

    public static DeepFlowStatus Evaluate(NetworkSwitch sw)
    {
        return greg.Mods.ResetSwitchService.EvaluateDeepStatus(sw) switch
        {
            GregDeepFlowStatus.Active => DeepFlowStatus.Active,
            GregDeepFlowStatus.Idle => DeepFlowStatus.Idle,
            GregDeepFlowStatus.PoweredOff => DeepFlowStatus.PoweredOff,
            GregDeepFlowStatus.Broken => DeepFlowStatus.Broken,
            _ => DeepFlowStatus.Isolated
        };
    }
}
