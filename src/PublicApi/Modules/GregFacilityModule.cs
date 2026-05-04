using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    // ⚡ Bolt: Performance Optimization - Replaced O(N) FindObjectsOfType with O(1) cache
    public int GetRackCount() => (int)gregCore.GameLayer.Patches.Hardware.RackPatch.GetRackCount();
}
