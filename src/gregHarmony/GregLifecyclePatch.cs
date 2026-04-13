using HarmonyLib;
using greg.Sdk;
using Il2Cpp;

namespace greg.Harmony;

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.OnLoad))]
public static class GregLifecyclePatch
{
    static void Postfix()
    {
        // Trigger greg.SYSTEM.GameLoaded
    }
}

[HarmonyPatch(typeof(TimeController), nameof(TimeController.OnDisable))]
public static class GregTimePatch
{
    static void Postfix()
    {
        // Trigger greg.SYSTEM.IncidentTriggered logic
    }
}
