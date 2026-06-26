using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    // NetworkMap.GetNumberOfDevices() index for racks
    private const int DEVICE_INDEX_RACKS = 2;

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback for when NetworkMap is not ready
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
