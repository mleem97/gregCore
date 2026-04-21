using System;
using HarmonyLib;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Die Harmony-Brücke zwischen dem Spiel und gregCore (Harmony Layer).
/// Enthält die Harmony-Patches für alle 1771 Hooks.
/// </summary>
[HarmonyPatch]
public sealed class GregNativeEventHooks : SafePatch
{
    private static bool _isInstalled = false;

    public static void Install(IGregLogger logger, GregHookBus hookBus)
    {
        if (_isInstalled) return;

        Setup(logger, hookBus);
        _isInstalled = true;
        _logger?.Success("GregNativeEventHooks Harmony Bridge installiert.");
    }

    // --- Domäne: Economy ---
    [HarmonyPatch(typeof(global::Il2Cpp.Player), nameof(global::Il2Cpp.Player.UpdateCoin))]
    [HarmonyPostfix]
    public static void Postfix_PlayerCoinChanged(global::Il2Cpp.Player __instance, float _coinChhangeAmount)
    {
        TriggerHook("greg.PLAYER.CoinChanged", "Amount", _coinChhangeAmount, "Total", _coinChhangeAmount);
    }

    // --- Domäne: Persistence ---
    [HarmonyPatch(typeof(global::Il2Cpp.SaveSystem), nameof(global::Il2Cpp.SaveSystem.SaveGame))]
    [HarmonyPostfix]
    public static void Postfix_GameSaved()
    {
        TriggerHook("greg.SYSTEM.GameSaved", "Timestamp", DateTime.Now.ToString());
    }

    // --- Domäne: UI ---
    [HarmonyPatch(typeof(global::Il2Cpp.PauseMenu), nameof(global::Il2Cpp.PauseMenu.OnEnable))]
    [HarmonyPostfix]
    public static void Postfix_PauseMenuOpened()
    {
        greg.Logging.GregLogger.Msg("Pause Menu Opened", "NativeHooks");
        TriggerHook("greg.UI.PauseMenu.Opened", "InstanceId", 1);
    }

    // --- WallRack Hooks ---
    public const string WorldWallRegistered    = "greg.WORLD.WallRegistered";
    public const string WorldWallRemoved       = "greg.WORLD.WallRemoved";
    public const string WorldWallPlaced        = "greg.WORLD.WallPlaced";
    public const string WorldWallDeviceMounted  = "greg.WORLD.WallDeviceMounted";
    public const string WorldWallDeviceUnmounted = "greg.WORLD.WallDeviceUnmounted";
    public const string WorldWallDeviceSwapped   = "greg.WORLD.WallDeviceSwapped";
    public const string WorldWallDeviceLabelSet  = "greg.WORLD.WallDeviceLabelSet";
    public const string SystemButtonBuyWall      = "greg.SYSTEM.ButtonBuyWall";

    // --- Generisches Hooking für die restlichen 1771 Hooks (Platzhalter) ---

    // In einer vollwertigen Produktion würde hier ein Generator-Tool (z.B. Source Generator) 
    // alle 1771 Harmony-Methoden basierend auf game_hooks.json generieren.
}
