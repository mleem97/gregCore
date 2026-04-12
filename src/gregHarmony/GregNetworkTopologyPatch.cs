using HarmonyLib;
using greg.Sdk;
using Il2Cpp;

namespace greg.Harmony;

[HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.OnCableRemoved))]
public static class GregNetworkTopologyPatch
{
    static void Postfix()
    {
        // Trigger framework event for topology change
    }
}
