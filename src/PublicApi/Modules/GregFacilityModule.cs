using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // ⚡ Bolt: Optimize O(N) scene search to O(1) lookup via NetworkMap singleton
        var instance = global::Il2Cpp.NetworkMap.instance;
        if (instance != null)
        {
            var counts = instance.GetNumberOfDevices();
            if (counts != null && counts.Length > 2)
            {
                return counts[2];
            }
        }
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
