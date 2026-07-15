using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2; // 0 = Servers, 1 = Switches, 2 = Racks

    /// <summary>
    /// Gets the total number of racks.
    /// Optimized to use NetworkMap device counts (O(1)) instead of FindObjectsOfType (O(N)).
    /// </summary>
    public int GetRackCount()
    {
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var counts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
            {
                return counts[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback for uninitialized states (e.g. main menu)
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
