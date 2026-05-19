using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount() {
        try {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm == null) return 0;
            var counts = nm.GetNumberOfDevices();
            return counts != null && counts.Length > 2 ? System.Math.Max(0, counts[2]) : 0;
        } catch { return 0; }
    }
}
