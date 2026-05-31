using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2;

    // ⚡ Bolt Optimization:
    // Replaced expensive O(N) FindObjectsOfType<Rack>() hierarchy traversal
    // with an O(1) indexed lookup from NetworkMap singleton array.
    // Impact: Prevents massive CPU spikes and GC allocations in late-game scenes when querying rack counts.
    public int GetRackCount()
    {
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }
        return 0;
    }
}
