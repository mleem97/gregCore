using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    private const int DEVICE_INDEX_RACKS = 2; // 0 = Servers, 1 = Switches, 2 = Racks

    /// <summary>
    /// Gets the total number of racks currently in the facility.
    /// Uses an O(1) lookup via NetworkMap with a fallback to O(N) FindObjectsOfType during uninitialized states.
    /// </summary>
    public int GetRackCount()
    {
        var networkMap = global::Il2Cpp.NetworkMap.instance;
        if (networkMap != null)
        {
            var deviceCounts = networkMap.GetNumberOfDevices();
            if (deviceCounts != null && deviceCounts.Length > DEVICE_INDEX_RACKS)
            {
                return deviceCounts[DEVICE_INDEX_RACKS];
            }
        }

        // Fallback to expensive lookup during uninitialized states
        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
