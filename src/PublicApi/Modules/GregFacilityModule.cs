using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // ⚡ Bolt Optimization: Use game-managed global collection for O(1) device counts
        // Replaces expensive O(N) FindObjectsOfType<Rack>() to eliminate GC allocations and main thread blocking.
        // Index 2 corresponds to Racks in the device counts array.
        var networkMap = global::Il2Cpp.NetworkMap.instance;
        if (networkMap != null)
        {
            var counts = networkMap.GetNumberOfDevices();
            if (counts != null && counts.Length > 2)
            {
                return counts[2];
            }
        }

        // Fallback for uninitialized game state
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
