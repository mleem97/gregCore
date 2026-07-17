using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2;

    public int GetRackCount()
    {
        // PERFORMANCE: O(1) fast path.
        // NetworkMap tracks active devices; fetching from it is significantly faster than
        // the O(N) scene-wide search of FindObjectsOfType<Rack>().
        var nm = global::Il2Cpp.NetworkMap.instance;
        if (nm != null)
        {
            var counts = nm.GetNumberOfDevices();
            // Verify array bounds before access to prevent exceptions
            if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
            {
                return counts[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback in case NetworkMap is uninitialized or null
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
