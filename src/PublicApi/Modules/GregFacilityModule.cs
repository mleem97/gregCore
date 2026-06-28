using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    // Index 0 = Servers, 1 = Switches, 2 = Racks
    private const int DEVICE_INDEX_RACKS = 2;
    private readonly GregApiContext _ctx;

    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm == null)
            {
                return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
            }

            var counts = nm.GetNumberOfDevices();
            if (counts != null && counts.Length > DEVICE_INDEX_RACKS)
            {
                return counts[DEVICE_INDEX_RACKS];
            }
            return 0;
        }
        catch
        {
            return 0;
        }
    }
}
