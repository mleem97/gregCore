using System;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using greg.Sdk.Services;

namespace gregCoreSDK.Sdk.Internal;

/// <summary>
/// Deep-layer UI interceptor that swaps vanilla panels for gregCore replacements.
/// Fixes the duplicate element issue by aggressively suppressing vanilla graphics.
/// </summary>
[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.OnEnable))]
public static class GregUiHijacker
{
    private static GameObject _customPauseRoot;

    public static bool Prefix(PauseMenu __instance)
    {
        if (GregUiService.TryGetReplacement("PauseMenu", out var factory))
        {
            // Suppress Vanilla
            if (__instance.pauseMenuUI != null)
            {
                __instance.pauseMenuUI.SetActive(false);
                GregUiService.TakeoverVanillaUi(__instance.pauseMenuUI);
            }

            // Render Custom
            if (_customPauseRoot == null)
            {
                var canvas = GregUiService.CreateCanvas("greg.PauseMenu", 1000000);
                _customPauseRoot = new GameObject("Root");
                _customPauseRoot.transform.SetParent(canvas.transform, false);
                factory.Invoke(_customPauseRoot);
            }

            _customPauseRoot.SetActive(true);
            return false; // Skip vanilla OnEnable
        }

        return true;
    }

    [HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.Resume))]
    public static class PauseMenu_Resume_Patch
    {
        public static void Postfix()
        {
            if (_customPauseRoot != null) _customPauseRoot.SetActive(false);
        }
    }
}
