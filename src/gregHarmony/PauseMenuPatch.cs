using System.Collections;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using greg.Core.UI;

namespace greg.Harmony;

[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.OnEnable))]
public static class PauseMenuPatch
{
    static void Postfix(PauseMenu __instance)
    {
        UIRouter.SetMode(UIMode.Paused);
        MelonLoader.MelonCoroutines.Start(DelayedInjection(__instance));
    }

    private static IEnumerator DelayedInjection(PauseMenu menu)
    {
        yield return new WaitForSeconds(0.5f);
        InjectModsButton(menu);
    }

    public static void InjectModsButton(PauseMenu menu)
    {
        try
        {
            if (GameObject.Find("greg_PauseModsButton") != null) return;

            GameObject resumeBtn = menu.resumeButton;
            if (resumeBtn == null) return;

            GameObject modsButtonGo = Object.Instantiate(resumeBtn, resumeBtn.transform.parent);
            modsButtonGo.name = "greg_PauseModsButton";

            var button = modsButtonGo.GetComponent<Button>();
            if (button != null)
            {
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener((System.Action)(() => {
                    gregModConfigManager.Toggle(true);
                }));
            }

            var img = modsButtonGo.GetComponent<Image>();
            if (img != null) img.color = new Color(0.00f, 0.07f, 0.06f, 0.85f);

            var textComp = modsButtonGo.GetComponentInChildren<TextMeshProUGUI>();
            if (textComp != null) {
                textComp.text = "MODS";
                textComp.color = new Color(0.38f, 0.96f, 0.85f, 1f);
            }

            modsButtonGo.transform.SetSiblingIndex(resumeBtn.transform.parent.childCount - 1);
            
            MelonLoader.MelonLogger.Msg("[gregCore] MODS button injected into Pause Menu.");
        }
        catch (System.Exception ex) {
            greg.Core.CrashLog.LogException("PauseMenuPatch.InjectModsButton", ex);
        }
    }
}

[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.Resume))]
public static class PauseMenuResumePatch
{
    static void Postfix()
    {
        UIRouter.SetMode(UIMode.Playing);
    }
}

