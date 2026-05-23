using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;

    // Il2Cpp.NetworkMap.instance.GetNumberOfDevices() returns an array mapping:
    // 0 = Servers, 1 = Switches, 2 = Racks
    private const int DEVICE_INDEX_RACKS = 2;

    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // ⚡ Bolt: Replace O(N) heap traversal with O(1) lookup via NetworkMap cache
        var nm = global::Il2Cpp.NetworkMap.instance;
        if (nm == null) return 0;

        var devices = nm.GetNumberOfDevices();
        return (devices != null && devices.Length > DEVICE_INDEX_RACKS) ? devices[DEVICE_INDEX_RACKS] : 0;
    }
}
