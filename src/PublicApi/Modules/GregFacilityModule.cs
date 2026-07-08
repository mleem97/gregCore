using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    // Index mapping for Il2Cpp.NetworkMap.instance.GetNumberOfDevices()
    // 0 = Servers, 1 = Switches, 2 = Racks
    private const int DEVICE_INDEX_RACKS = 2;

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // ⚡ Bolt: Optimize expensive O(N) FindObjectsOfType lookup with O(1) network map lookup.
        // Impact: Reduces CPU time for rack count queries from ~O(N) to O(1) preventing hitching.
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }

        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
