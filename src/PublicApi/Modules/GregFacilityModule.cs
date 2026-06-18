using System;
using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private const int DEVICE_INDEX_RACKS = 2; // index 0: servers, index 1: switches, index 2: racks

    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        try {
            var nm = global::Il2Cpp.NetworkMap.instance;
            if (nm != null) {
                var counts = nm.GetNumberOfDevices();
                if (counts != null && counts.Length > DEVICE_INDEX_RACKS) {
                    return Math.Max(0, counts[DEVICE_INDEX_RACKS]);
                }
            }
            return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
        } catch {
            return 0;
        }
    }
}
