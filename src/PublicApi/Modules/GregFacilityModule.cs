using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private const int DEVICE_INDEX_RACKS = 2;

    private readonly GregApiContext _ctx;

    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;
    public int GetRackCount()
    {
        var instance = global::Il2Cpp.NetworkMap.instance;
        if (instance != null)
        {
            var devices = instance.GetNumberOfDevices();
            if (devices != null && devices.Length > DEVICE_INDEX_RACKS)
            {
                return devices[DEVICE_INDEX_RACKS];
            }
        }
        return 0; // Fallback if instance or array is missing
    }
}
