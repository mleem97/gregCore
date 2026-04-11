using HarmonyLib;
using gregSdk;
using Il2Cpp;

namespace gregHarmony;

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
