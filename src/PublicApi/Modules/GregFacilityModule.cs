using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2; // Index 0: servers, 1: switches, 2: racks

    public int GetRackCount()
    {
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null)
            {
                var arr = nm.GetNumberOfDevices();
                if (arr != null && arr.Length > DEVICE_INDEX_RACKS)
                {
                    return arr[DEVICE_INDEX_RACKS];
                }
            }
        }
        catch { }

        // Fallback
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
