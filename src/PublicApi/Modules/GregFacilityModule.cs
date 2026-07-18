using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        try
        {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null)
            {
                var arr = nm.GetNumberOfDevices();
                if (arr != null && arr.Length > 2)
                {
                    return arr[2]; // index 2 represents racks
                }
            }
        }
        catch { }

        // Fallback for uninitialized game states
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
