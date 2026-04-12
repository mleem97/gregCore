using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using greg.Core.UI;
using Il2CppTMPro;

namespace greg.Harmony;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
public static class MainMenuPatch
{
    static void Postfix(MainMenu __instance)
    {
        MelonLoader.MelonCoroutines.Start(DelayedInjection(__instance));
    }

    private static System.Collections.IEnumerator DelayedInjection(MainMenu menu)
    {
        yield return new WaitForSeconds(0.8f); // Slightly more time for uGUI to settle
        InjectModsButton(menu);
    }

    public static void InjectModsButton(MainMenu menu)
    {
        try
        {
            if (GameObject.Find("greg_ModsButton") != null) return;

            GameObject settingsBtn = GameObject.Find("Canvas/MiddleMenu/Buttons/Settings") 
                                  ?? GameObject.Find("Settings") 
                                  ?? (menu != null ? menu.settings : null);

            if (settingsBtn == null) return;

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
            MelonLoader.MelonLogger.Msg("[gregCore] v1.0.0.15: MODS button injected with isolated listeners.");
        }
        catch (System.Exception ex) {
            greg.Core.CrashLog.LogException("MainMenuPatch.InjectModsButton", ex);
        }
    }
}
