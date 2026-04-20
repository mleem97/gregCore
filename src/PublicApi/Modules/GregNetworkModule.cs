using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregNetworkModule
{
    private readonly GregApiContext _ctx;
    internal GregNetworkModule(GregApiContext ctx) => _ctx = ctx;

    public int GetSwitchCount() => UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.NetworkSwitch>().Length;
    public int GetBrokenSwitchCount() {
        int count = 0;
        foreach (var s in UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.NetworkSwitch>()) {
            if (s.isBroken) count++;
        }
        return count;
    }
}
