using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

        private const int DEVICE_INDEX_RACKS = 2;

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

        // Fallback for uninitialized game states (e.g. main menu)
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
