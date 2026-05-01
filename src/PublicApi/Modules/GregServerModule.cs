using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregServerModule
{
    private readonly GregApiContext _ctx;
    internal GregServerModule(GregApiContext ctx) => _ctx = ctx;

    public int GetCount() {
        try {
            var nm = global::Il2Cpp.NetworkMap.instance;
            return nm != null && nm.servers != null ? nm.servers.Count : 0;
        } catch { return 0; }
    }
    public int GetBrokenCount() {
        try {
            var nm = global::Il2Cpp.NetworkMap.instance;
            return nm != null && nm.brokenServers != null ? nm.brokenServers.Count : 0;
        } catch { return 0; }
    }
}
