using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        try
        {
            // [Bolt Optimization] Fast O(1) lookup using game's internal tracking
            // index 0: servers, index 1: switches, index 2: racks
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null)
            {
                var counts = nm.GetNumberOfDevices();
                if (counts != null && counts.Length > 2)
                    return counts[2];
            }
        }
        catch { }

        // Fallback for edge cases (e.g., main menu or uninitialized state)
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks.Length;
    }
}
