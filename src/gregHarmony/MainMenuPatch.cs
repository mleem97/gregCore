using System;
using System.Collections;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using greg.Core.UI;
using greg.Core.UI.Components;
using greg.Sdk.Services;

namespace greg.Harmony;

public static class MainMenuController
{
    private static GregMainMenuReplacement _menuInstance;

    private static T AddComponentSafe<T>(GameObject go) where T : Component
    {
        try
        {
            var method = typeof(GameObject).GetMethod("AddComponent", new Type[] { typeof(Type) });
            return method?.Invoke(go, new object[] { typeof(T) }) as T;
        }
        catch
        {
            return null;
        }
    }

    public static void Initialize()
    {
        if (_menuInstance != null) return;

        var go = new GameObject("GregMainMenu_Root");
        _menuInstance = AddComponentSafe<GregMainMenuReplacement>(go);

        _menuInstance?.Configure(
            onContinue: OnContinueClicked,
            onNewGame: OnNewGameClicked,
            onLoadGame: OnLoadGameClicked,
            onSettings: OnSettingsClicked,
            onMods: OnModsClicked,
            onQuit: OnQuitClicked,
            onReportBug: OnReportBugClicked,
            onDiscord: OnDiscordClicked,
            onWishlist: OnWishlistClicked,
            onTwitter: OnTwitterClicked,
            onStats: OnStatsClicked
        );
    }

    public static void Show() => _menuInstance?.Show();
    public static void Hide() => _menuInstance?.Hide();

    private static void OnContinueClicked()
    {
        MelonLogger.Msg("[MainMenu] Continue clicked");
        Hide();
        if (GameUIButtons.ClickContinueButton())
        {
            UIRouter.SetMode(UIMode.Playing);
        }
    }

    private static void OnNewGameClicked()
    {
        MelonLogger.Msg("[MainMenu] New Game clicked");
        Hide();
        if (GameUIButtons.ClickNewGameButton())
        {
            UIRouter.SetMode(UIMode.Playing);
        }
    }

    private static void OnLoadGameClicked()
    {
        MelonLogger.Msg("[MainMenu] Load Game clicked");
        if (GameUIButtons.ClickLoadButton())
        {
             Hide();
        }
    }

    private static void OnSettingsClicked()
    {
        MelonLogger.Msg("[MainMenu] Settings clicked");
        UIRouter.SetMode(UIMode.Settings);
        
        GameUIButtons.ClickButton("PauseMenuCanvas/Pause menu -  Settings Scripts");
    }

    private static void OnModsClicked()
    {
        MelonLogger.Msg("[MainMenu] Mods clicked");
        gregModConfigManager.Toggle(true);
    }

    private static void OnReportBugClicked()
    {
        MelonLogger.Msg("[MainMenu] Report Bug clicked");
        GameUIButtons.ClickReportBugButton();
    }

    private static void OnDiscordClicked()
    {
        MelonLogger.Msg("[MainMenu] Discord clicked");
        GameUIButtons.ClickDiscordButton();
    }

    private static void OnWishlistClicked()
    {
        MelonLogger.Msg("[MainMenu] Wishlist clicked");
        GameUIButtons.ClickWishlistButton();
    }

    private static void OnTwitterClicked()
    {
        MelonLogger.Msg("[MainMenu] Twitter clicked");
        GameUIButtons.ClickTwitterButton();
    }

    private static void OnStatsClicked()
    {
        MelonLogger.Msg("[MainMenu] Stats clicked");
        GameUIButtons.ClickStatsButton();
    }

    private static void OnQuitClicked()
    {
        MelonLogger.Msg("[MainMenu] Quit clicked");
        Application.Quit();
    }
}

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
public static class MainMenuPatch
{
    private static bool _isOriginalMenuDisabled = false;

    static void Postfix(MainMenu __instance)
    {
        MelonLoader.MelonCoroutines.Start(DisableOriginalMenuCoroutine(__instance));
    }

    private static IEnumerator DisableOriginalMenuCoroutine(MainMenu menu)
    {
        yield return new WaitForSeconds(0.3f);

        if (GregUxmlService.HasOverride("MainMenu"))
        {
            MelonLogger.Msg("[gregCore] UXML override found for MainMenu. Suppressing original UI...");
            
            var allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
            var gameObjects = new GameObject[allCanvases.Length];
            for (int i = 0; i < allCanvases.Length; i++)
            {
                gameObjects[i] = allCanvases[i].gameObject;
            }
            DisableGameCanvases(gameObjects);

            GregUxmlService.ShowOverride("MainMenu");
        }
        else
        {
            MelonLogger.Msg("[gregCore] No UXML override found for MainMenu. Using original game UI.");
        }
    }

    private static void DisableGameCanvases(GameObject[] allCanvases)
    {
        if (_isOriginalMenuDisabled) return;

        string[] keepCanvases = new string[]
        {
            "Canvas_OverAll",
            "CountersCanvas",
            "Canvas_Main"
        };

        foreach (var canvasGo in allCanvases)
        {
            if (canvasGo == null) continue;
            
            string canvasName = canvasGo.name;
            
            bool shouldKeep = false;
            foreach (var keep in keepCanvases)
            {
                if (canvasName.Contains(keep))
                {
                    shouldKeep = true;
                    break;
                }
            }

            if (!shouldKeep && !canvasGo.name.Contains("GregMainMenu"))
            {
                canvasGo.SetActive(false);
                MelonLogger.Msg($"[gregCore] Disabled original canvas: {canvasName}");
            }
        }

        _isOriginalMenuDisabled = true;
    }
}
