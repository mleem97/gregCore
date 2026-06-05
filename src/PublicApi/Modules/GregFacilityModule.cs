using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2; // index 0 = servers, 1 = switches, 2 = racks

    public int GetRackCount()
    {
        if (global::Il2Cpp.NetworkMap.instance == null) return 0;

        var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
        if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
        {
            return deviceCounts[DEVICE_INDEX_RACKS];
        }
        return 0;
    }
}
