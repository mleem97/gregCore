using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2;
    public int GetRackCount()
    {
        var nm = global::Il2Cpp.NetworkMap.instance;
        if (nm == null) return 0;
        var arr = nm.GetNumberOfDevices();
        return arr != null && arr.Length > DEVICE_INDEX_RACKS ? System.Math.Max(0, arr[DEVICE_INDEX_RACKS]) : 0;
    }
}
