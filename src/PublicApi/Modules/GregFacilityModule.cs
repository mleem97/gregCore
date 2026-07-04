using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    // NetworkMap.instance.GetNumberOfDevices() returns an array where:
    // index 0: servers, index 1: switches, index 2: racks
    private const int DEVICE_INDEX_RACKS = 2;

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // Try to get count via the O(1) game-managed lookup first
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var counts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
            {
                return counts[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback to expensive FindObjectsOfType if singleton isn't ready
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
