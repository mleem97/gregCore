using UnityEngine;

namespace gregCore.PublicApi.Modules;

public sealed class GregFacilityModule
{
    private readonly GregApiContext _ctx;
    private const int DEVICE_INDEX_RACKS = 2;

    internal GregFacilityModule(GregApiContext ctx) => _ctx = ctx;

    /// <summary>
    /// Gets the number of racks in the facility.
    /// O(1) performance via NetworkMap array when available, falling back to O(N) scene search.
    /// </summary>
    public int GetRackCount()
    {
        var nm = global::Il2Cpp.NetworkMap.instance;
        if (nm != null)
        {
            var devices = nm.GetNumberOfDevices();
            if (devices != null && devices.Length > DEVICE_INDEX_RACKS)
            {
                return devices[DEVICE_INDEX_RACKS];
            }
        }

        return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>().Length;
    }
}
