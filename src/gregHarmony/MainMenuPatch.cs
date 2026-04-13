using System;
using System.Collections;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using greg.Core.UI;
using greg.Core.UI.Components;

namespace greg.Harmony;

public static class MainMenuController
{
    private static GregMainMenuReplacement _menuInstance;

    public static void Initialize()
    {
        if (_menuInstance != null) return;

        var go = new GameObject("GregMainMenu_Root");
        _menuInstance = go.AddComponent<GregMainMenuReplacement>();

        _menuInstance.Configure(
            onPlay: OnPlayClicked,
            onSettings: OnSettingsClicked,
            onMods: OnModsClicked,
            onQuit: OnQuitClicked
        );
    }

    public static void Show() => _menuInstance?.Show();
    public static void Hide() => _menuInstance?.Hide();

    private static void OnPlayClicked()
    {
        MelonLogger.Msg("[MainMenu] Play clicked");
        Hide();
        
        if (GameUIButtons.ClickLoadButton())
        {
            MelonLogger.Msg("[MainMenu] Load button invoked via reflection");
        }
        else
        {
            GameMethodInvoker.Invoke(GameUIElements.Types.MainGameManager, "LoadGame");
        }
        
        UIRouter.SetMode(UIMode.Playing);
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
        int retries = 0;
        GameObject[] canvasesToDisable = null;

        while (retries < 20)
        {
            var allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
            if (allCanvases.Length > 0)
            {
                canvasesToDisable = new GameObject[allCanvases.Length];
                for (int i = 0; i < allCanvases.Length; i++)
                {
                    canvasesToDisable[i] = allCanvases[i].gameObject;
                }
                break;
            }

            retries++;
            yield return new WaitForSeconds(0.25f);
        }

        if (canvasesToDisable != null)
        {
            DisableGameCanvases(canvasesToDisable);
        }
        else
        {
            MelonLogger.Warning("[gregCore] No canvases found.");
        }

        yield return new WaitForSeconds(0.3f);

        InitializeGregMenu();
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
                MelonLogger.Msg($"[gregCore] Disabled canvas: {canvasName}");
            }
        }

        _isOriginalMenuDisabled = true;
        MelonLogger.Msg("[gregCore] Original game canvases disabled.");
    }

    private static void InitializeGregMenu()
    {
        try
        {
            UIRouter.Initialize();
            MainMenuController.Initialize();
            MainMenuController.Show();
            MelonLogger.Msg("[gregCore] GregMainMenuReplacement initialized successfully.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Failed to initialize GregMenu: {ex.Message}");
            greg.Core.CrashLog.LogException("MainMenuPatch.InitializeGregMenu", ex);
        }
    }
}
