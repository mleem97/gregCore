using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // NetworkMap.GetNumberOfDevices() indices: 0 = Servers, 1 = Switches, 2 = Racks
    private const int DEVICE_INDEX_RACKS = 2;

    public int GetRackCount()
    {
        // Optimization: Use O(1) game-managed counter instead of O(N) FindObjectsOfType when available
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null)
            {
                var counts = nm.GetNumberOfDevices();
                if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
                {
                    return counts[DEVICE_INDEX_RACKS];
                }
            }
        }
        catch { /* Fallback on error */ }

        // Fallback for uninitialized game states (e.g. main menu)
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
