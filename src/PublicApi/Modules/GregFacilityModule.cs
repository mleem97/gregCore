using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private const int DEVICE_INDEX_RACKS = 2;
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        var nm = global::Il2Cpp.NetworkMap.instance;
        return nm != null ? nm.GetNumberOfDevices()[DEVICE_INDEX_RACKS] : 0;
    }
}
