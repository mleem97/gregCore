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
            if (global::Il2Cpp.NetworkMap.instance != null)
            {
                var counts = global::Il2Cpp.NetworkMap.instance.GetNumberOfDevices();
                if (counts != null && counts.Length > 2)
                {
                    return counts[2]; // O(1) retrieval instead of O(N) scene search
                }
            }
        }
        catch (System.Exception)
        {
            // Graceful fallback to scene search
        }

        var fallbackRacks = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Rack>();
        return fallbackRacks != null ? fallbackRacks.Length : 0;
    }
}
