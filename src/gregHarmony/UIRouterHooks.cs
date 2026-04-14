using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using greg.Core.UI;
using greg.Core.UI.Components;

namespace greg.Harmony;

public static class UIRouterHooks
{
    public static void Initialize()
    {
        UIRouter.OnModeChanged += OnUIModeChanged;
        MelonLogger.Msg("[UIRouterHooks] Initialized.");
    }

    private static void OnUIModeChanged(UIMode from, UIMode to)
    {
        MelonLogger.Msg($"[UIRouterHooks] UI Mode: {from} -> {to}");

        if (to == UIMode.MainMenu)
        {
            GregMainMenuReplacement.Instance?.Show();
        }
        else
        {
            GregMainMenuReplacement.Instance?.Hide();
        }

        if (to == UIMode.Paused)
        {
            GregPauseMenuReplacement.Instance?.Show();
        }
        else
        {
            GregPauseMenuReplacement.Instance?.Hide();
        }

        if (to == UIMode.ComputerShop)
        {
            GregShopReplacement.Instance?.Show();
        }
        else
        {
            GregShopReplacement.Instance?.Hide();
        }
    }
}

public static class UIRouterExtensions
{
    public static void OpenComputerShop() => UIRouter.SetMode(UIMode.ComputerShop);
    public static void OpenAssetManagement() => UIRouter.SetMode(UIMode.AssetManagement);
    public static void OpenBalanceSheet() => UIRouter.SetMode(UIMode.BalanceSheet);
    public static void OpenHire() => UIRouter.SetMode(UIMode.Hire);
    public static void OpenTutorial() => UIRouter.SetMode(UIMode.Tutorial);
    public static void ResumeGame() => UIRouter.SetMode(UIMode.Playing);
    public static void ReturnToMainMenu() => UIRouter.SetMode(UIMode.MainMenu);
}
