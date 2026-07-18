using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // Device index for racks in NetworkMap.GetNumberOfDevices() array
    private const int DEVICE_INDEX_RACKS = 2;

    public int GetRackCount()
    {
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null)
            {
                var devices = nm.GetNumberOfDevices();
                if (devices != null && devices.Length > DEVICE_INDEX_RACKS)
                {
                    return devices[DEVICE_INDEX_RACKS];
                }
            }
        }
        catch { }

        // Graceful fallback to FindObjectsOfType if NetworkMap is not available
        var fallbackRacks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return fallbackRacks != null ? fallbackRacks.Length : 0;
    }
}
