using HarmonyLib;
using greg.Sdk.Services;
using Il2Cpp;

namespace greg.Harmony;

[HarmonyPatch(typeof(PacketSpawnerSystem), nameof(PacketSpawnerSystem.OnUpdate))]
public static class GregPacketSpawnerPatch
{
    static void Postfix()
    {
        // Link to GregPacketService if needed
    }
}

[HarmonyPatch(typeof(WaypointInitializationSystem), nameof(WaypointInitializationSystem.OnUpdate))]
public static class GregWaypointSystemPatch
{
    static void Postfix()
    {
        // Link to GregWaypointService if needed
    }
}

[HarmonyPatch(typeof(ReusableFunctions), nameof(ReusableFunctions.HexToColor))]
public static class GregReusableFunctionsPatch
{
    static void Postfix(string hex, ref UnityEngine.Color __result)
    {
        // Hook into core color conversion if needed
    }
}
