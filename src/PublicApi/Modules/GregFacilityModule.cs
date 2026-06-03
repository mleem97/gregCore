using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // Device array index: 0 = servers, 1 = switches, 2 = racks
    private const int DEVICE_INDEX_RACKS = 2;

    public int GetRackCount()
    {
        // NetworkMap is a singleton and may be null during main menu, init, or destruction
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var devices = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (devices != null && devices.Length > DEVICE_INDEX_RACKS)
            {
                return devices[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback or early return 0 if map is unavailable
        return 0;
    }
}
