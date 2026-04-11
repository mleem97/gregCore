using HarmonyLib;
using gregSdk;
using Il2Cpp;

namespace gregHarmony;

[HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.OnCreated))]
public static class GregEmployeeAIPatch
{
    static void Postfix(AICharacterControl __instance)
    {
        gregSdk.Services.GregEmployeeAIBridge.ForceStateTransition(__instance.name, "Initialize");
    }
}
