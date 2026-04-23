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
    }
}
