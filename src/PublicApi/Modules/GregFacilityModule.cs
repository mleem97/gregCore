using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // Index mapping for Il2Cpp.NetworkMap.instance.GetNumberOfDevices()
    private const int DEVICE_INDEX_RACKS = 2;

    /// <summary>
    /// Gets the number of racks in the facility.
    /// Optimized to use the game-managed NetworkMap to avoid expensive FindObjectsOfType calls in O(1) time.
    /// </summary>
    public int GetRackCount()
    {
        // O(1) fast path using game-managed singleton
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }

        // Graceful fallback during uninitialized game states
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
