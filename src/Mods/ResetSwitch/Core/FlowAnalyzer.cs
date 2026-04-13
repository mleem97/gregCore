using Il2Cpp;
using UnityEngine;

namespace greg.Mods.ResetSwitch.Core;

public static class FlowAnalyzer
{
    public static FlowStatus Analyze(NetworkSwitch sw)
    {
        var deep = FlowSimulator.Evaluate(sw);
        return deep switch
        {
            DeepFlowStatus.Active => FlowStatus.OK,
            DeepFlowStatus.Idle => FlowStatus.Degraded,
            _ => FlowStatus.Dead
        };
    }
}
