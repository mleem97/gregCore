using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    // NetworkMap.instance.GetNumberOfDevices() returns an array where index 0 is servers, index 1 is switches, index 2 is racks
    private const int DEVICE_INDEX_RACKS = 2;

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // Optimization: Use O(1) game-managed lookup if available
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }

        // Graceful fallback to slow lookup during uninitialized states (e.g., main menu)
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
