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
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null) {
                var devices = nm.GetNumberOfDevices();
                if (devices != null && devices.Length > 2) return devices[2];
            }
            return 0;
        }
        catch { return 0; }
    }
}
