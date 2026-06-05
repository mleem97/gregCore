using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2; // index 0 = servers, 1 = switches, 2 = racks

    public int GetRackCount()
    {
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm == null) return 0;
            var arr = nm.GetNumberOfDevices();
            if (arr != null && arr.Length > DEVICE_INDEX_RACKS)
            {
                return System.Math.Max(0, arr[DEVICE_INDEX_RACKS]);
            }
            return 0;
        }
        catch { return 0; }
    }
}
