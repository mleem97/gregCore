using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    // Index mapping: 0 = Servers, 1 = Switches, 2 = Racks
    private const int DEVICE_INDEX_RACKS = 2;

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        var nm = global::Il2Cpp.NetworkMap.instance;
        if (nm != null)
        {
            var counts = nm.GetNumberOfDevices();
            if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
            {
                return System.Math.Max(0, counts[DEVICE_INDEX_RACKS]);
            }
        }
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
