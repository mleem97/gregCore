using HarmonyLib;
using gregSdk;
using Il2Cpp;

namespace gregHarmony;

[HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.OnCableRemoved))]
public static class GregNetworkTopologyPatch
{
    static void Postfix()
    {
        // Trigger framework event for topology change
    }
}
