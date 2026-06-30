using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;

    // In Il2Cpp.NetworkMap.instance.GetNumberOfDevices(), index 2 represents racks.
    private const int DEVICE_INDEX_RACKS = 2;

    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // Optimization: Use O(1) game-managed array if singleton is initialized
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback for uninitialized states (e.g. main menu)
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
