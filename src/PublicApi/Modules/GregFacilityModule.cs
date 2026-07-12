using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // Index 2 represents racks in GetNumberOfDevices array (0: servers, 1: switches)
    private const int DEVICE_INDEX_RACKS = 2;

    public int GetRackCount()
    {
        // Optimization: Use O(1) device counts from NetworkMap instead of O(N) FindObjectsOfType
        var networkMap = global::Il2Cpp.NetworkMap.instance;
        if (networkMap != null)
        {
            var counts = networkMap.GetNumberOfDevices();
            if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
            {
                return counts[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback during uninitialized game states
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
