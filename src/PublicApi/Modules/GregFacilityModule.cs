using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private const int DEVICE_INDEX_RACKS = 2; // index 0: servers, index 1: switches, index 2: racks
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
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
        // Graceful fallback during uninitialized states
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
