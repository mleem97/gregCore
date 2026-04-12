using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using greg.Core.UI;
using TMPro;
using TMPro;
using System.Collections;

namespace greg.Harmony;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
public static class MainMenuPatch
{
    static void Postfix(MainMenu __instance)
    {
        MelonLoader.MelonCoroutines.Start(EnsureModsButtonCoroutine(__instance));
    }

    public static IEnumerator EnsureModsButtonCoroutine(MainMenu menu)
    {
        int retries = 0;
        GameObject settingsBtn = null;

        // Poll for the settings button up to 5 seconds
        while (retries < 20)
        {
            settingsBtn = GameObject.Find("Canvas/MiddleMenu/Buttons/Settings") 
                       ?? GameObject.Find("Settings") 
                       ?? (menu != null ? menu.settings : null);

            if (settingsBtn != null && settingsBtn.activeInHierarchy)
            {
                break;
            }

            retries++;
            yield return new WaitForSeconds(0.25f);
        }

        if (settingsBtn != null)
        {
            InjectModsButton(settingsBtn);
            StyleMenuButtons(settingsBtn.transform.parent);
            InjectVersionDisplay(settingsBtn.transform.root);
        }
        else
        {
            MelonLoader.MelonLogger.Warning("[gregCore] Failed to find Settings button in MainMenu for MODS injection.");
        }
    }

    public static void StyleMenuButtons(Transform buttonContainer)
    {
        if (buttonContainer == null) return;

        foreach (Transform child in buttonContainer)
        {
            var btn = child.GetComponent<Button>();
            if (btn == null && child.GetComponent<ButtonExtended>() == null) continue;

            var img = child.GetComponent<Image>();
            if (img != null) img.color = new Color(0.00f, 0.07f, 0.06f, 0.85f);

            var text = child.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.color = new Color(0.38f, 0.96f, 0.85f, 1f);
        }
    }

    public static void InjectVersionDisplay(Transform root)
    {
        try
        {
            if (GameObject.Find("greg_VersionDisplay") != null) return;

            var versionGo = new GameObject("greg_VersionDisplay");
            versionGo.transform.SetParent(root, false);

            var rect = versionGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(10, 10);
            rect.sizeDelta = new Vector2(300, 30);

            var text = versionGo.AddComponent<TextMeshProUGUI>();
            text.text = $"gregCore v{greg.Core.gregReleaseVersion.Current} <color=#1CEDE1>[teamGreg]</color>";
            text.fontSize = 14;
            text.color = new Color(0.38f, 0.96f, 0.85f, 0.7f);
            text.alignment = TextAlignmentOptions.BottomLeft;
        }
        catch (System.Exception ex)
        {
            greg.Core.CrashLog.LogException("MainMenuPatch.InjectVersionDisplay", ex);
        }
    }

    public static void InjectModsButton(GameObject settingsBtn)
    {
        try
        {
            if (GameObject.Find("greg_ModsButton") != null) return;

            // Fresh Clone
            GameObject modsButtonGo = Object.Instantiate(settingsBtn, settingsBtn.transform.parent);
            modsButtonGo.name = "greg_ModsButton";

            // PURGE all existing uGUI button logic from the clone
            var button = modsButtonGo.GetComponent<Button>();
            if (button != null)
            {
                button.onClick = new Button.ButtonClickedEvent(); 
                button.onClick.RemoveAllListeners();
                
                // Add OUR listener
                button.onClick.AddListener((System.Action)(() => {
                    MelonLoader.MelonLogger.Msg("[gregCore] Opening Mod Configuration...");
                    gregModConfigManager.Toggle(true);
                }));
            }

            // Styling: Luminescent Architect
            var img = modsButtonGo.GetComponent<Image>();
            if (img != null) img.color = new Color(0.00f, 0.07f, 0.06f, 0.85f);

            var textComp = modsButtonGo.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null) {
                textComp.text = "MODS";
                textComp.color = new Color(0.38f, 0.96f, 0.85f, 1f);
            }

            modsButtonGo.transform.SetSiblingIndex(settingsBtn.transform.GetSiblingIndex() + 1);
            MelonLoader.MelonLogger.Msg("[gregCore] v1.0.0.23: MODS button injected via resilient polling.");
        }
        catch (System.Exception ex) {
            greg.Core.CrashLog.LogException("MainMenuPatch.InjectModsButton", ex);
        }
    }
}
