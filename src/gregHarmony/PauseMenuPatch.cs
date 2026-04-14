using System.Collections;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using greg.Core.UI;
using greg.Core.UI.Components;

namespace greg.Harmony;

[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.OnEnable))]
public static class PauseMenuPatch
{
    static void Postfix(PauseMenu __instance)
    {
        UIRouter.SetMode(UIMode.Paused);
        InitializeReplacement();
        
        // Hide original UI elements safely
        if (__instance.gameObject.activeSelf)
        {
             // We can't disable the object itself because it might break game logic
             // but we can disable the visual child elements.
             foreach(Transform child in __instance.transform)
             {
                 child.gameObject.SetActive(false);
             }
        }
        
        GregPauseMenuReplacement.Instance?.Show();
    }

    private static void InitializeReplacement()
    {
        if (GregPauseMenuReplacement.Instance != null) return;
        
        var go = new GameObject("GregPauseMenu_Root");
        var comp = go.AddComponent<GregPauseMenuReplacement>();
        comp.Configure(
            onResume: OnResumeClicked,
            onSettings: OnSettingsClicked,
            onSave: OnSaveClicked,
            onLoad: OnLoadClicked,
            onMods: OnModsClicked,
            onQuitToMenu: OnQuitToMenuClicked,
            onQuitToDesktop: OnQuitToDesktopClicked
        );
    }

    private static void OnResumeClicked()
    {
        GregPauseMenuReplacement.Instance?.Hide();
        UIRouter.SetMode(UIMode.Playing);
        // Find the PauseMenu original instance and call Resume
        var menu = UnityEngine.Object.FindObjectOfType<PauseMenu>();
        menu?.Resume();
    }

    private static void OnSettingsClicked()
    {
        MelonLogger.Msg("[PauseMenu] Settings clicked");
    }

    private static void OnSaveClicked()
    {
        MelonLogger.Msg("[PauseMenu] Save clicked");
        GameUIButtons.ClickSaveButton();
    }

    private static void OnLoadClicked()
    {
        MelonLogger.Msg("[PauseMenu] Load clicked");
        GameUIButtons.ClickLoadButton();
    }

    private static void OnModsClicked()
    {
        gregModConfigManager.Toggle(true);
    }

    private static void OnQuitToMenuClicked()
    {
        MelonLogger.Msg("[PauseMenu] Quit to Menu clicked");
        GameUIButtons.ClickButtonByName("QuitToMenu"); 
    }

    private static void OnQuitToDesktopClicked()
    {
        Application.Quit();
    }
}

[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.Resume))]
public static class PauseMenuResumePatch
{
    static void Postfix()
    {
        GregPauseMenuReplacement.Instance?.Hide();
        UIRouter.SetMode(UIMode.Playing);
    }
}

