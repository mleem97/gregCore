using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;

namespace greg.Harmony;

[HarmonyPatch(typeof(GetCurrentVersion), nameof(GetCurrentVersion.Start))]
public static class GregVersionPatch
{
    static void Postfix(GetCurrentVersion __instance)
    {
        var tmp = __instance.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            if (!tmp.text.EndsWith("#greg"))
                tmp.text += " #greg";
            return;
        }

        var txt = __instance.GetComponent<Text>();
        if (txt != null)
        {
            if (!txt.text.EndsWith("#greg"))
                txt.text += " #greg";
        }
    }
}
