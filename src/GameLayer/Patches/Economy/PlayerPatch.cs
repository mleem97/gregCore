/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Extrahiert Daten aus dem IL2CPP Player-Objekt + Prefix-Bypasses.
/// Maintainer:   Prefix-Patches überschreiben hohle IL2CPP-Methoden (UpdateCoin/UpdateXP
///               returnieren immer false). Postfix-Callbacks bleiben für Backward-Compat.
///               Defensive: null-checks + NativePointer validation.
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
    private static bool UpdateCoinPrefix(
        global::Il2Cpp.Player __instance,
        float _coinChhangeAmount,
        ref bool __result)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return true;

            __result = true;

            var payload = EventPayloadBuilder.ForValueChange("money", 0f, _coinChhangeAmount);
            HookIntegration.Emit(HookName.Create("economy", "PlayerCoinUpdated").ToString(), payload);
            OnCoinUpdated(__instance, _coinChhangeAmount);

            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[PlayerPatch] UpdateCoin prefix failed: {ex.Message}");
            return true;
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.Player), nameof(global::Il2Cpp.Player.UpdateXP))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool UpdateXPPrefix(
        global::Il2Cpp.Player __instance,
        float amount,
        ref bool __result)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return true;

            __result = true;

            var payload = EventPayloadBuilder.ForValueChange("xp", 0f, amount);
            HookIntegration.Emit(HookName.Create("economy", "PlayerXpUpdated").ToString(), payload);
            OnXpUpdated(__instance, amount);

            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[PlayerPatch] UpdateXP prefix failed: {ex.Message}");
            return true;
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
