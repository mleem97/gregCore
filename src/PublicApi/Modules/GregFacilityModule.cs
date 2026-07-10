using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // Index mapping for NetworkMap.instance.GetNumberOfDevices()
    // [0] = servers, [1] = switches, [2] = racks
    private const int DEVICE_INDEX_RACKS = 2;

    /// <summary>
    /// Gets the total number of racks in the facility.
    /// Optimized to use O(1) device counts array from NetworkMap when available.
    /// </summary>
    public int GetRackCount()
    {
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null)
            {
                var deviceCounts = nm.GetNumberOfDevices();
                if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
                {
                    return deviceCounts[DEVICE_INDEX_RACKS];
                }
            }
        }
        catch { }

        // Graceful fallback during uninitialized states (e.g., main menu)
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
