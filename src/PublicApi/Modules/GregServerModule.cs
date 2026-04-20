using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregServerModule
{
    private readonly GregApiContext _ctx;
    internal GregServerModule(GregApiContext ctx) => _ctx = ctx;

    public int GetCount() => UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Server>().Length;
    public int GetBrokenCount() {
        int count = 0;
        foreach (var s in UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Server>()) {
            if (s.isBroken) count++;
        }
        return count;
    }
}
