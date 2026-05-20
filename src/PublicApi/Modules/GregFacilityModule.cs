using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        // Optimization: Replaced O(N) FindObjectsOfType with O(1) NetworkMap cached lookup
        // Expected Impact: Eliminates main thread hitches when querying rack counts on large maps
        var nm = global::Il2Cpp.NetworkMap.instance;
        if (nm == null || nm.Pointer == System.IntPtr.Zero) return 0;
        var counts = nm.GetNumberOfDevices();
        return counts != null && counts.Length > 2 ? counts[2] : 0;
    }
}
