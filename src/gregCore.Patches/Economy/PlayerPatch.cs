/// <file-summary>
/// Layer:       GameLayer
/// Purpose:     Extracts data from the IL2CPP Player object + prefix bypasses.
/// Maintainer:  Prefix patches override hollow IL2CPP methods (UpdateCoin/UpdateXP
///               always return false). Postfix callbacks remain for backward compat.
///               Defensive: null-checks + pointer validation.
/// </file-summary>

using System;
using HarmonyLib;
using MelonLoader;
using gregCore.GameLayer.Hooks;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Patches.Economy;

internal static class PlayerPatch
{
    [HarmonyPatch(typeof(global::Il2Cpp.Player), nameof(global::Il2Cpp.Player.UpdateCoin))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static void UpdateCoinPrefix(
        global::Il2Cpp.Player __instance,
        float _coinChhangeAmount)
    {
        // Observe-only: vanilla original ALWAYS runs (no __result, no skip).
        // Swallowing it here froze money/XP silently — events must never
        // replace game logic they don't reimplement.
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;

            OnCoinUpdated(__instance, _coinChhangeAmount);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[PlayerPatch] UpdateCoin prefix failed: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.Player), nameof(global::Il2Cpp.Player.UpdateXP))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static void UpdateXPPrefix(
        global::Il2Cpp.Player __instance,
        float amount)
    {
        // Observe-only: see UpdateCoinPrefix.
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;

            OnXpUpdated(__instance, amount);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[PlayerPatch] UpdateXP prefix failed: {ex.Message}");
        }
    }

    internal static void OnCoinUpdated(object __instance, float _coinChhangeAmount)
    {
        try
        {
            var payload = EventPayloadBuilder.ForValueChange("money", 0f, _coinChhangeAmount);
            HookIntegration.Emit(HookName.Create("economy", "PlayerCoinUpdated").ToString(), payload);
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnCoinUpdated), ex);
        }
    }

    internal static void OnXpUpdated(object __instance, float amount)
    {
        try
        {
            var payload = EventPayloadBuilder.ForValueChange("xp", 0f, amount);
            HookIntegration.Emit(HookName.Create("economy", "PlayerXpUpdated").ToString(), payload);
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnXpUpdated), ex);
        }
    }

    internal static void OnReputationUpdated(object __instance, float amount)
    {
        try
        {
            var payload = EventPayloadBuilder.ForValueChange("reputation", 0f, amount);
            HookIntegration.Emit(HookName.Create("economy", "PlayerReputationUpdated").ToString(), payload);
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnReputationUpdated), ex);
        }
    }
}
