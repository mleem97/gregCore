using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2;

    /// <summary>
    /// Returns the number of racks in the facility.
    /// Uses the NetworkMap device cache (O(1)) instead of an expensive Unity Object search (O(N)).
    /// </summary>
    public int GetRackCount()
    {
        var instance = global::Il2Cpp.NetworkMap.instance;
        if (instance != null)
        {
            var arr = instance.GetNumberOfDevices();
            if (arr != null && arr.Length > DEVICE_INDEX_RACKS)
            {
                return arr[DEVICE_INDEX_RACKS];
            }
        }
        return 0;
    }
}
