using System;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace gregModLoader;

/// <summary>
/// UI-related Harmony patches for gregCore.
/// These patches are responsible for triggering UI modernization or web-replacement.
/// MOVED TO SEPARATE FILE TO ALLOW EXCLUSION FROM CORE BUILD.
/// </summary>
[HarmonyPatch(typeof(HRSystem), "OnEnable")]
internal static class Patch_HRSystem_OnEnable
{
    internal static void Postfix(HRSystem __instance)
    {
        try
        {
            // Even without integrated UI, we keep the employee injection as it's gameplay-core.
            CustomEmployeeManager.InjectIntoHRSystem(__instance);
            HireRosterService.ExportAvailableHiresSnapshot();
            
            // Bridge calls: these only do something if a plugin registered handlers.
            bool handledByWeb = gregUiExtensionBridge.TryApplyOrReplace(__instance.gameObject, "HRSystem");
            if (!handledByWeb)
                gregUiExtensionBridge.TryModernize(__instance.gameObject, "HRSystem.OnEnable");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("HRSystem.OnEnable custom employee injection", ex);
        }
    }
}

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
internal static class Patch_MainMenu_Start
{
    internal static void Postfix(MainMenu __instance)
    {
        try
        {
            bool handledByWeb = gregUiExtensionBridge.TryApplyOrReplace(__instance.gameObject, "MainMenu");
            if (!handledByWeb)
                gregUiExtensionBridge.TryModernize(__instance.gameObject, "MainMenu.Start");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("MainMenu.Start ui modernize", ex);
        }
    }
}

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Settings))]
internal static class Patch_MainMenu_Settings
{
    internal static void Postfix(MainMenu __instance)
    {
        try
        {
            gregUiExtensionBridge.OnSettingsOpened(__instance);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("MainMenu.Settings mod menu", ex);
        }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.InteractOnClick))]
internal static class Patch_ComputerShop_InteractOnClick
{
    internal static void Postfix(ComputerShop __instance)
    {
        try
        {
            bool handledByWeb = gregUiExtensionBridge.TryApplyOrReplace(__instance.gameObject, "ComputerShop");
            if (!handledByWeb)
                gregUiExtensionBridge.TryModernize(__instance.gameObject, "ComputerShop.InteractOnClick");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ComputerShop.InteractOnClick ui modernize", ex);
        }
    }
}
