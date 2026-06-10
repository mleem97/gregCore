using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private const int DEVICE_INDEX_RACKS = 2; // NetworkMap.instance.GetNumberOfDevices() index for racks
    private readonly GregApiContext _ctx;

    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
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
        catch { }

        // Graceful fallback to expensive lookup if singleton is null or array is invalid
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
