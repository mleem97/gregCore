using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private const int DEVICE_INDEX_RACKS = 2;
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    /// <summary>
    /// Gets the total number of racks placed in the facility.
    /// Optimized to use the game's internal NetworkMap device counts, falling back to FindObjectsOfType if unavailable.
    /// </summary>
    public int GetRackCount()
    {
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null)
            {
                var arr = nm.GetNumberOfDevices();
                if (arr != null && arr.Length > DEVICE_INDEX_RACKS)
                {
                    return arr[DEVICE_INDEX_RACKS];
                }
            }
        }
        catch { /* Ignore exception and fallback */ }

        // Fallback if NetworkMap is not ready (e.g., during Main Menu)
        var objects = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        if (objects == null) return 0;
        return objects.Length;
    }
}
