using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2; // Index mapping for NetworkMap.GetNumberOfDevices()

    public int GetRackCount()
    {
        // O(1) lookup using game-managed network map
        var nm = global::Il2Cpp.NetworkMap.instance;
        if (nm != null)
        {
            var counts = nm.GetNumberOfDevices();
            if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
            {
                return counts[DEVICE_INDEX_RACKS];
            }
        }

        // Graceful fallback for uninitialized game states
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
