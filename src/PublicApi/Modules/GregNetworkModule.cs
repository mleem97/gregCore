using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregNetworkModule
{
    private readonly GregApiContext _ctx;
    internal GregNetworkModule(GregApiContext ctx) => _ctx = ctx;

    public int GetSwitchCount() {
        try {
            var nm = global::Il2Cpp.NetworkMap.instance;
            return nm != null && nm.switches != null ? nm.switches.Count : 0;
        } catch { return 0; }
    }
    public int GetBrokenSwitchCount() {
        try {
            var nm = global::Il2Cpp.NetworkMap.instance;
            return nm != null && nm.brokenSwitches != null ? nm.brokenSwitches.Count : 0;
        } catch { return 0; }
    }
}
