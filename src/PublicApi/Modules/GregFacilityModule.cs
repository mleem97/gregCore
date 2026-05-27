using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2;

    public int GetRackCount() => global::Il2Cpp.NetworkMap.instance != null ? global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices()[DEVICE_INDEX_RACKS] : 0;
}
