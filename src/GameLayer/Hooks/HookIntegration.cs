using System;
using HarmonyLib;
using UnityEngine;
using MelonLoader;
using greg.Sdk;

namespace gregCore.GameLayer.Hooks
{
    public static class HookIntegration
    {
        public static void Install(object mod, bool auto) { }
        public static void LogPatchError(string mod, Exception ex) => MelonLogger.Error($"[{mod}] Patch Error: {ex.Message}");
        public static void Emit(string id, object? data = null) { }

        public static void Apply(HarmonyLib.Harmony harmony)
        {
            try 
            {
                var playerType = AccessTools.TypeByName("Player") ?? AccessTools.TypeByName("Il2Cpp.Player");
                if (playerType != null)
                {
                    harmony.Patch(AccessTools.Method(playerType, "UpdateCoin"), postfix: new HarmonyMethod(typeof(HookIntegration), nameof(Postfix_Generic)));
                }

                var saveManagerType = AccessTools.TypeByName("SaveManager") ?? AccessTools.TypeByName("Il2Cpp.SaveManager");
                if (saveManagerType != null)
                {
                    harmony.Patch(AccessTools.Method(saveManagerType, "SaveGame"), postfix: new HarmonyMethod(typeof(HookIntegration), nameof(Postfix_Generic)));
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gC-Hooks] Dynamic patch failed: {ex.Message}");
            }
        }

        public static void Postfix_Generic()
        {
            gregNativeEventHooks.OnCoinsChanged?.Invoke(null!);
            gregNativeEventHooks.GameLoaded?.Invoke(null!);
        }
    }
}
