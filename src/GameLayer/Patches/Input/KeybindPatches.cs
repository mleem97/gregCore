using HarmonyLib;
using UnityEngine;

namespace gregCore.GameLayer.Patches.Input;

/// <summary>
/// Patches fÃ¼r Input-Handling. 
/// Vormals genutzte Console-Blocking-Logik wurde entfernt, 
/// da der Fokus nun auf dem MelonLoader-Terminal liegt.
/// </summary>
[HarmonyPatch]
internal static class KeybindPatches
{
    // Must specify ArgumentTypes to disambiguate GetKeyDown(KeyCode) from GetKeyDown(string)
    [HarmonyPatch(typeof(UnityEngine.Input), nameof(UnityEngine.Input.GetKeyDown),
        argumentTypes: new[] { typeof(UnityEngine.KeyCode) })]
    [HarmonyPrefix]
    private static bool BlockPWhenConsoleOpen(ref bool __result, KeyCode key)
    {
        if (key == KeyCode.P && global::gregCore.Infrastructure.UI.GregDevConsole.Instance.IsOpen)
        {
            __result = false;
            return false;
        }
        return true;
    }
}
