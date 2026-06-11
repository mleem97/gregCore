using UnityEngine;
using System;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;

    // Il2Cpp.NetworkMap.instance.GetNumberOfDevices() returns an array where:
    // Index 0: servers, Index 1: switches, Index 2: racks
    private const int DEVICE_INDEX_RACKS = 2;

    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    public int GetRackCount()
    {
        try
        {
            // O(1) retrieval if game is initialized
            if (global::Il2Cpp.NetworkMap.instance != null)
            {
                var deviceCounts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
                if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
                {
                    return deviceCounts[DEVICE_INDEX_RACKS];
                }
            }
        }
        catch (Exception)
        {
            // Ignore exception, fallback to O(N) lookup
        }

        // Graceful fallback to O(N) lookup (e.g. main menu or before initialization)
        var racks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return racks != null ? racks.Length : 0;
    }
}
