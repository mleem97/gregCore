using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        var netMap = global::Il2Cpp.NetworkMap.instance;
        if (netMap != null)
        {
            var counts = netMap.GetNumberOfDevices();
            if (counts != null && counts.Length > 2)
            {
                return counts[2]; // Index 0: servers, 1: switches, 2: racks
            }
        }

        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
