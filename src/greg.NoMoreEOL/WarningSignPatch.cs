using HarmonyLib;
using Il2Cpp;
using System;
using gregCore.GameLayer.Hooks;

namespace greg.NoMoreEOL
{
    [HarmonyPatch]
    public static class WarningSignPatch
    {
        [HarmonyPatch(typeof(global::Il2Cpp.StaticUIElements), "InstantiateErrorWarningSign")]
        [HarmonyPrefix]
        public static bool SkipInstantiate(ref int __result)
        {
            try
            {
                if (!Main.WarningsVisible)
                {
                    __result = -1;
                    return false; // Skip the original method
                }
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(WarningSignPatch), ex);
            }
            return true;
        }

        [HarmonyPatch(typeof(global::Il2Cpp.PositionIndicator), "Awake")]
        [HarmonyPostfix]
        public static void OnPositionIndicatorAwake(global::Il2Cpp.PositionIndicator __instance)
        {
            try
            {
                if (__instance != null)
                {
                    Main.Indicators.Add(__instance);

                    // Apply current visibility setting to new indicators
                    if (__instance.gameObject != null)
                    {
                        __instance.gameObject.SetActive(Main.WarningsVisible);
                    }
                }
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(WarningSignPatch), ex);
            }
        }

        [HarmonyPatch(typeof(global::Il2Cpp.PositionIndicator), "OnDestroy")]
        [HarmonyPostfix]
        public static void OnPositionIndicatorOnDestroy(global::Il2Cpp.PositionIndicator __instance)
        {
            try
            {
                if (__instance != null)
                {
                    Main.Indicators.Remove(__instance);
                }
            }
            catch (Exception ex)
            {
                HookIntegration.LogPatchError(nameof(WarningSignPatch), ex);
            }
        }
    }
}
