/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Extrahiert Daten aus dem IL2CPP Player-Objekt + Prefix-Bypasses.
/// Maintainer:   Prefix-Patches überschreiben hohle IL2CPP-Methoden (UpdateCoin/UpdateXP
///               returnieren immer false). Postfix-Callbacks bleiben für Backward-Compat.
/// </file-summary>

using System;
using HarmonyLib;
using MelonLoader;
using gregCore.GameLayer.Hooks;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Patches.Economy;

// [GREG_SYNC_INSERT_PATCHES]

[HarmonyPatch]
internal static class PlayerPatch
{
    // ─── PREFIX BYPASSES (IL2CPP-Hohlmethoden) ───────────────────────────

    /// <summary>
    /// Prefix für Player.UpdateCoin – die IL2CPP-Methode returniert immer false.
    /// Setzt __result auf true, damit der Coin-Update als erfolgreich gilt.
    /// </summary>
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
            if (__instance == null) return true;

            // Allow the coin update by setting result to true
            __result = true;

            // Dispatch event via HookIntegration
            var payload = EventPayloadBuilder.ForValueChange("money", 0f, _coinChhangeAmount);
            HookIntegration.Emit(HookName.Create("economy", "PlayerCoinUpdated").ToString(), payload);

            // Also trigger the legacy callback
            OnCoinUpdated(__instance, _coinChhangeAmount);

            return false; // Skip hollow original
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[PlayerPatch] UpdateCoin prefix failed: {ex.Message}");
            return true; // Fallback: try original
        }
    }

    /// <summary>
    /// Prefix für Player.UpdateXP – die IL2CPP-Methode returniert immer false.
    /// </summary>
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
            if (__instance == null) return true;

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

    // ─── POSTFIX CALLBACKS (Backward-Compat, weiterhin aufrufbar) ────────

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
