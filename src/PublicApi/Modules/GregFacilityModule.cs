using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private const int DEVICE_INDEX_RACKS = 2; // Index 0: servers, Index 1: switches, Index 2: racks

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

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

        var objects = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return objects != null ? objects.Length : 0;
    }
}
