using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    // NetworkMap.instance.GetNumberOfDevices() indices: 0 = servers, 1 = switches, 2 = racks
    private const int DEVICE_INDEX_RACKS = 2;

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // ⚡ Bolt Performance Optimization:
        // Use O(1) device count array instead of O(N) FindObjectsOfType native heap traversal when possible.
        // Expected impact: Eliminates native boundary crossing and full heap scan, turning an O(N) operation into O(1).
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }

        // Graceful fallback for main menu or uninitialized states
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
