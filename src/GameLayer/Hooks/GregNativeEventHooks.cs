using System;
using HarmonyLib;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Die Harmony-Brücke zwischen dem Spiel und gregCore.
/// Delegiert an GregDynamicHookPatcher für alle 1771+ Hooks.
/// </summary>
[HarmonyPatch]
public sealed class GregNativeEventHooks : SafePatch
{
    private static bool _isInstalled = false;
    private static GregDynamicHookPatcher? _dynamicPatcher;

    public static void Install(IGregLogger logger, GregHookBus hookBus, GregEventBus eventBus, HarmonyLib.Harmony harmony)
    {
        if (_isInstalled) return;

        Setup(logger, hookBus);

        try
        {
            // Initialize dynamic patcher for all 1771+ hooks from game_hooks.json
            _dynamicPatcher = new GregDynamicHookPatcher(harmony, eventBus, logger);
            GregDynamicHookPatcher.SetGlobalBus(eventBus);
            GregDynamicHookPatcher.SetGlobalLogger(logger);

            string hooksFile = System.IO.Path.Combine(
                global::MelonLoader.Utils.MelonEnvironment.ModsDirectory,
                "game_hooks.json");

            if (!System.IO.File.Exists(hooksFile))
            {
                // Fallback: look in assembly directory
                var asmDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!string.IsNullOrEmpty(asmDir))
                {
                    hooksFile = System.IO.Path.Combine(asmDir, "game_hooks.json");
                }
            }

            if (!System.IO.File.Exists(hooksFile))
            {
                // Final fallback: project root
                hooksFile = System.IO.Path.Combine(
                    global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory,
                    "game_hooks.json");
            }

            _dynamicPatcher.InstallFromFile(hooksFile);

            _logger?.Success($"GregNativeEventHooks Harmony Bridge installiert. Patched {_dynamicPatcher.InstalledCount} methods.");
        }
        catch (Exception ex)
        {
            _logger?.Error("Failed to initialize dynamic hook patcher", ex);
        }

        _isInstalled = true;
    }

    // --- Domäne: Economy ---
    [HarmonyPatch(typeof(global::Il2Cpp.Player), nameof(global::Il2Cpp.Player.UpdateCoin))]
    [HarmonyPostfix]
    public static void Postfix_PlayerCoinChanged(global::Il2Cpp.Player __instance, float _coinChhangeAmount)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            TriggerHook("greg.PLAYER.CoinChanged", "Amount", _coinChhangeAmount, "Total", _coinChhangeAmount);
        }
        catch (Exception ex)
        {
            _logger?.Error("Hook PlayerCoinChanged failed", ex);
        }
    }

    // --- Domäne: Persistence ---
    [HarmonyPatch(typeof(global::Il2Cpp.SaveSystem), nameof(global::Il2Cpp.SaveSystem.SaveGame))]
    [HarmonyPostfix]
    public static void Postfix_GameSaved()
    {
        try
        {
            TriggerHook("greg.SYSTEM.GameSaved", "Timestamp", DateTime.Now.ToString());
        }
        catch (Exception ex)
        {
            _logger?.Error("Hook GameSaved failed", ex);
        }
    }

    // --- Domäne: UI ---
    [HarmonyPatch(typeof(global::Il2Cpp.PauseMenu), nameof(global::Il2Cpp.PauseMenu.OnEnable))]
    [HarmonyPostfix]
    public static void Postfix_PauseMenuOpened()
    {
        try
        {
            greg.Logging.GregLogger.Msg("Pause Menu Opened", "NativeHooks");
            TriggerHook("greg.UI.PauseMenu.Opened", "InstanceId", 1);
        }
        catch (Exception ex)
        {
            _logger?.Error("Hook PauseMenuOpened failed", ex);
        }
    }

    // --- WallRack Hook Constants ---
    public const string WorldWallRegistered    = "greg.WORLD.WallRegistered";
    public const string WorldWallRemoved       = "greg.WORLD.WallRemoved";
    public const string WorldWallPlaced        = "greg.WORLD.WallPlaced";
    public const string WorldWallDeviceMounted  = "greg.WORLD.WallDeviceMounted";
    public const string WorldWallDeviceUnmounted = "greg.WORLD.WallDeviceUnmounted";
    public const string WorldWallDeviceSwapped   = "greg.WORLD.WallDeviceSwapped";
    public const string WorldWallDeviceLabelSet  = "greg.WORLD.WallDeviceLabelSet";
    public const string SystemButtonBuyWall      = "greg.SYSTEM.ButtonBuyWall";
}
