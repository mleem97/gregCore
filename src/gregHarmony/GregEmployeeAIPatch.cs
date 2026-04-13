using HarmonyLib;
using greg.Sdk;
using Il2Cpp;

namespace gregCoreSDK.Harmony;

[HarmonyPatch(typeof(AICharacterControl), nameof(AICharacterControl.OnCreated))]
public static class GregEmployeeAIPatch
{
    static void Postfix(AICharacterControl __instance)
    {
        greg.Sdk.Services.GregEmployeeAIBridge.ForceStateTransition(__instance.name, "Initialize");
    }
}
