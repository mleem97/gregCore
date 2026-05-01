using HarmonyLib;
using UnityEngine;

namespace gregCore.GameLayer.Patches.Input;

/// <summary>
/// DEACTIVATED — Patching UnityEngine.Input.GetKeyDown is not viable under IL2CPP.
/// The Legacy Input Manager is disabled in this game (Input System package active).
/// All input polling has been migrated to UnityEngine.InputSystem.
/// </summary>
[HarmonyPatch]
internal static class KeybindPatches
{
    /*
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
    */
}
