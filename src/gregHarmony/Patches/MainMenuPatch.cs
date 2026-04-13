using System;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;
using MelonLoader;

namespace greg.Harmony.Patches;

/// <summary>
/// Patch for Task 1.4: Adds a "MODS" button to the Main Menu.
/// </summary>
[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
public static class MainMenuPatch
{
    public static Action OnModsMenuOpened;

    public static void Postfix(MainMenu __instance)
    {
        try
        {
            MelonLogger.Msg("[gregCore] Patching MainMenu to add MODS button...");

            // Find an existing button to clone (e.g., settings)
            GameObject settingsBtn = __instance.settings;
            if (settingsBtn == null) return;

            // Create MODS button
            GameObject modsBtn = GameObject.Instantiate(settingsBtn, settingsBtn.transform.parent);
            modsBtn.name = "greg_ModsButton";
            
            // Adjust position (offset from original)
            modsBtn.transform.localPosition += new Vector3(0, -60, 0);

            // Update Text
            var textComp = modsBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = "MODS";
            }

            // Setup Click Listener
            var btnComp = modsBtn.GetComponent<Button>();
            if (btnComp != null)
            {
                btnComp.onClick = new Button.ButtonClickedEvent();
                btnComp.onClick.AddListener(new Action(() => {
                    MelonLogger.Msg("[gregCore] MODS button clicked!");
                    OnModsMenuOpened?.Invoke();
                }));
            }

            MelonLogger.Msg("[gregCore] MODS button successfully added.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Failed to patch MainMenu: {ex.Message}");
        }
    }
}
