using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // Index mapping for GetNumberOfDevices: 0: Servers, 1: Switches, 2: Racks
    private const int DEVICE_INDEX_RACKS = 2;

    public int GetRackCount()
    {
        // Fast O(1) path
        if (global::Il2Cpp.NetworkMap.instance != null)
        {
            var arr = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
            if (arr != null && arr.Length > DEVICE_INDEX_RACKS)
            {
                return arr[DEVICE_INDEX_RACKS];
            }
        }
        // Slow fallback
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
