using System;
using HarmonyLib;
using greg.Core;
using greg.Sdk;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace gregFramework.Hooks;

/// <summary>
/// Harmony hooks for domain Player (generated from Il2Cpp unpack).
/// All patches are defensive: null-checks, try-catch, and NativePointer validation.
/// </summary>
internal static class GregPlayerHooks
{
    private static void SafeEmit(string hookName, object? payload)
    {
        try { gregEventDispatcher.Emit(hookName, payload); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Event dispatch failed for {hookName}: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    [HarmonyPostfix]
    private static void OnPlayerStart(Player __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "ComponentInitialized"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerStart failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.CheckFallsThroughMap))]
    [HarmonyPostfix]
    private static void OnPlayerCheckFallsThroughMap(Player __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "CheckFallsThroughMap"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerCheckFallsThroughMap failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.LoadPlayer))]
    [HarmonyPostfix]
    private static void OnPlayerLoadPlayer(Player __instance, PlayerData data)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "Loaded"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerLoadPlayer failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdateCoin))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool PrefixPlayerUpdateCoin(Player __instance, float _coinChangeAmount, bool withoutSound, ref bool __result)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero)
            {
                __result = false;
                return false;
            }
            __instance.money += _coinChangeAmount;
            __result = true;
            SafeEmit(gregHookName.Create(GregDomain.Player, "MoneyChanged"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp, changeAmount = _coinChangeAmount });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook PrefixPlayerUpdateCoin failed: {ex.Message}");
            __result = false;
        }
        return false;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.DropAllItems))]
    [HarmonyPostfix]
    private static void OnPlayerDropAllItems(Player __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "DroppedAllItems"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerDropAllItems failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.WarpPlayer))]
    [HarmonyPostfix]
    private static void OnPlayerWarpPlayer(Player __instance, Vector3 _position, Quaternion _rotation)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "Warped"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerWarpPlayer failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdateReputation))]
    [HarmonyPostfix]
    private static void OnPlayerUpdateReputation(Player __instance, float amount)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "ReputationChanged"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerUpdateReputation failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdateXP))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool PrefixPlayerUpdateXP(Player __instance, float amount, ref bool __result)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero)
            {
                __result = false;
                return false;
            }
            __instance.xp += amount;
            __result = true;
            SafeEmit(gregHookName.Create(GregDomain.Player, "XpChanged"), new { money = __instance.money, reputation = __instance.reputation, xp = __instance.xp, changeAmount = amount });
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook PrefixPlayerUpdateXP failed: {ex.Message}");
            __result = false;
        }
        return false;
    }

    [HarmonyPatch(typeof(PlayerHit), nameof(PlayerHit.OnEnable))]
    [HarmonyPostfix]
    private static void OnPlayerHitOnEnable(PlayerHit __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "ComponentInitialized"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerHitOnEnable failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.Awake))]
    [HarmonyPostfix]
    private static void OnPlayerManagerAwake(PlayerManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "ComponentInitialized"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerManagerAwake failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.Start))]
    [HarmonyPostfix]
    private static void OnPlayerManagerStart(PlayerManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "ComponentInitialized"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerManagerStart failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.ConfinedCursorforUI))]
    [HarmonyPostfix]
    private static void OnPlayerManagerConfinedCursorforUI(PlayerManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "ConfinedCursorforUI"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerManagerConfinedCursorforUI failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.PlayerStopMovement))]
    [HarmonyPostfix]
    private static void OnPlayerManagerPlayerStopMovement(PlayerManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "PlayerStopMovement"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerManagerPlayerStopMovement failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.LockedCursorForPlayerMovement))]
    [HarmonyPostfix]
    private static void OnPlayerManagerLockedCursorForPlayerMovement(PlayerManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "LockedCursorForPlayerMovement"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerManagerLockedCursorForPlayerMovement failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.DefaultActionEffect))]
    [HarmonyPostfix]
    private static void OnPlayerManagerDefaultActionEffect(PlayerManager __instance, Vector3 _position, float _time)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "DefaultActionEffect"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerManagerDefaultActionEffect failed: {ex.Message}"); }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.GainIOPSEffect))]
    [HarmonyPostfix]
    private static void OnPlayerManagerGainIOPSEffect(PlayerManager __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(gregHookName.Create(GregDomain.Player, "GainIOPSEffect"), new { instance = __instance });
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore] Hook OnPlayerManagerGainIOPSEffect failed: {ex.Message}"); }
    }
}
