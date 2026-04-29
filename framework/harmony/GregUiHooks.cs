using System;
using HarmonyLib;
using greg.Core;
using greg.Sdk;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;

namespace gregFramework.Hooks;

/// <summary>
/// Harmony hooks for domain Ui (generated from Il2Cpp unpack).
/// </summary>
internal static class GregUiHooks
{
    private static void SafeEmit(string hookName, object? payload)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, hookName),
                payload);
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] SafeEmit '{hookName}' failed: {ex.Message}");
        }
    }

    // BalanceSheet.Awake
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.Awake))]
    [HarmonyPostfix]
    private static void OnBalanceSheetAwake(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetAwake failed: {ex.Message}");
        }
    }

    // BalanceSheet.Start
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.Start))]
    [HarmonyPostfix]
    private static void OnBalanceSheetStart(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetStart failed: {ex.Message}");
        }
    }

    // BalanceSheet.OnDestroy
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.OnDestroy))]
    [HarmonyPostfix]
    private static void OnBalanceSheetOnDestroy(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "OnDestroy",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetOnDestroy failed: {ex.Message}");
        }
    }

    // BalanceSheet.GetOrCreateRecord
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.GetOrCreateRecord))]
    [HarmonyPostfix]
    private static void OnBalanceSheetGetOrCreateRecord(BalanceSheet __instance, CustomerItem item)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "GetOrCreateRecord",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetGetOrCreateRecord failed: {ex.Message}");
        }
    }

    // BalanceSheet.RegisterSalary
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.RegisterSalary))]
    [HarmonyPostfix]
    private static void OnBalanceSheetRegisterSalary(BalanceSheet __instance, int monthlySalary)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "RegisterSalary",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetRegisterSalary failed: {ex.Message}");
        }
    }

    // BalanceSheet.CountFailingApps
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.CountFailingApps))]
    [HarmonyPostfix]
    private static void OnBalanceSheetCountFailingApps(BalanceSheet __instance, CustomerBase cb)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "CountFailingApps",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetCountFailingApps failed: {ex.Message}");
        }
    }

    // BalanceSheet.SaveSnapshot
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.SaveSnapshot))]
    [HarmonyPostfix]
    private static void OnBalanceSheetSaveSnapshot(BalanceSheet __instance, int month, Il2CppSystem.DateTime snapshotTime)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "SnapshotSaved",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetSaveSnapshot failed: {ex.Message}");
        }
    }

    // BalanceSheet.GetLatestSnapshot
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.GetLatestSnapshot))]
    [HarmonyPostfix]
    private static void OnBalanceSheetGetLatestSnapshot(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "GetLatestSnapshot",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetGetLatestSnapshot failed: {ex.Message}");
        }
    }

    // BalanceSheet.FillInBalanceSheet
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.FillInBalanceSheet))]
    [HarmonyPostfix]
    private static void OnBalanceSheetFillInBalanceSheet(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "FillInBalanceSheet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetFillInBalanceSheet failed: {ex.Message}");
        }
    }

    // BalanceSheet.AddSalaryRow
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.AddSalaryRow))]
    [HarmonyPostfix]
    private static void OnBalanceSheetAddSalaryRow(BalanceSheet __instance, float salaryExpense)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "SalaryRowAdded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetAddSalaryRow failed: {ex.Message}");
        }
    }

    // BalanceSheet.AddTotalRow
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.AddTotalRow))]
    [HarmonyPostfix]
    private static void OnBalanceSheetAddTotalRow(BalanceSheet __instance, float revenue, float penalties, float total)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "TotalRowAdded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetAddTotalRow failed: {ex.Message}");
        }
    }

    // BalanceSheet.AddHeaderRow
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.AddHeaderRow))]
    [HarmonyPostfix]
    private static void OnBalanceSheetAddHeaderRow(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "HeaderRowAdded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetAddHeaderRow failed: {ex.Message}");
        }
    }

    // BalanceSheet.AddSectionTitle
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.AddSectionTitle))]
    [HarmonyPostfix]
    private static void OnBalanceSheetAddSectionTitle(BalanceSheet __instance, string title)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "SectionTitleAdded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetAddSectionTitle failed: {ex.Message}");
        }
    }

    // BalanceSheet.InstantiateRow
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.InstantiateRow))]
    [HarmonyPostfix]
    private static void OnBalanceSheetInstantiateRow(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "InstantiateRow",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetInstantiateRow failed: {ex.Message}");
        }
    }

    // BalanceSheet.ClearRows
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.ClearRows))]
    [HarmonyPostfix]
    private static void OnBalanceSheetClearRows(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ClearRows",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetClearRows failed: {ex.Message}");
        }
    }

    // BalanceSheet.GetSaveData
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.GetSaveData))]
    [HarmonyPostfix]
    private static void OnBalanceSheetGetSaveData(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "GetSaveData",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetGetSaveData failed: {ex.Message}");
        }
    }

    // BalanceSheet.LoadFromSave
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.LoadFromSave))]
    [HarmonyPostfix]
    private static void OnBalanceSheetLoadFromSave(BalanceSheet __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "FromSaveLoaded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetLoadFromSave failed: {ex.Message}");
        }
    }

    // BalanceSheet.RestoreRecord
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.RestoreRecord))]
    [HarmonyPostfix]
    private static void OnBalanceSheetRestoreRecord(BalanceSheet __instance, CustomerRecordSaveData recData)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "RestoreRecord",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnBalanceSheetRestoreRecord failed: {ex.Message}");
        }
    }

    // MainMenu.Start
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
    [HarmonyPostfix]
    private static void OnMainMenuStart(MainMenu __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainMenuStart failed: {ex.Message}");
        }
    }

    // MainMenu.Continue
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Continue))]
    [HarmonyPostfix]
    private static void OnMainMenuContinue(MainMenu __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "Continue",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainMenuContinue failed: {ex.Message}");
        }
    }

    // MainMenu.NewGame
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.NewGame))]
    [HarmonyPostfix]
    private static void OnMainMenuNewGame(MainMenu __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "NewGame",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainMenuNewGame failed: {ex.Message}");
        }
    }

    // MainMenu.QuitGame
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.QuitGame))]
    [HarmonyPostfix]
    private static void OnMainMenuQuitGame(MainMenu __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "QuitGame",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainMenuQuitGame failed: {ex.Message}");
        }
    }

    // MainMenu.LoadGame
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.LoadGame))]
    [HarmonyPostfix]
    private static void OnMainMenuLoadGame(MainMenu __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "GameLoaded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainMenuLoadGame failed: {ex.Message}");
        }
    }

    // MainMenu.HideMiddleMenu
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.HideMiddleMenu))]
    [HarmonyPostfix]
    private static void OnMainMenuHideMiddleMenu(MainMenu __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "HideMiddleMenu",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainMenuHideMiddleMenu failed: {ex.Message}");
        }
    }

    // MainMenu.Settings
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Settings))]
    [HarmonyPostfix]
    private static void OnMainMenuSettings(MainMenu __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "tingsSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnMainMenuSettings failed: {ex.Message}");
        }
    }

    // SettingsControls.Start
    [HarmonyPatch(typeof(SettingsControls), nameof(SettingsControls.Start))]
    [HarmonyPostfix]
    private static void OnSettingsControlsStart(SettingsControls __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsControlsStart failed: {ex.Message}");
        }
    }

    // SettingsControls.LookSensitivity
    [HarmonyPatch(typeof(SettingsControls), nameof(SettingsControls.LookSensitivity))]
    [HarmonyPostfix]
    private static void OnSettingsControlsLookSensitivity(SettingsControls __instance, float fl)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "LookSensitivity",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsControlsLookSensitivity failed: {ex.Message}");
        }
    }

    // SettingsControls.InvertY
    [HarmonyPatch(typeof(SettingsControls), nameof(SettingsControls.InvertY))]
    [HarmonyPostfix]
    private static void OnSettingsControlsInvertY(SettingsControls __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "InvertY",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsControlsInvertY failed: {ex.Message}");
        }
    }

    // SettingsControls.LoadSettings
    [HarmonyPatch(typeof(SettingsControls), nameof(SettingsControls.LoadSettings))]
    [HarmonyPostfix]
    private static void OnSettingsControlsLoadSettings(SettingsControls __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "SettingsLoaded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsControlsLoadSettings failed: {ex.Message}");
        }
    }

    // SettingsGraphics.Start
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.Start))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsStart(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsStart failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetQuality
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetQuality))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetQuality(SettingsGraphics __instance, int qualityIndex)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "QualitySet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetQuality failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetFullScreen
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetFullScreen))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetFullScreen(SettingsGraphics __instance, bool isFullScreen)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "FullScreenSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetFullScreen failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetResDropDown
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetResDropDown))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetResDropDown(SettingsGraphics __instance, int resolutionIndex)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ResDropDownSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetResDropDown failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetResolution
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetResolution))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetResolution(SettingsGraphics __instance, int width, int height)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ResolutionSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetResolution failed: {ex.Message}");
        }
    }

    // SettingsGraphics.AvailableRefreshRate
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.AvailableRefreshRate))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsAvailableRefreshRate(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "AvailableRefreshRate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsAvailableRefreshRate failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetRefreshRate
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetRefreshRate))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetRefreshRate(SettingsGraphics __instance, int _refreshRate)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "RefreshRateSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetRefreshRate failed: {ex.Message}");
        }
    }

    // SettingsGraphics.LimitFrameRate
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.LimitFrameRate))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsLimitFrameRate(SettingsGraphics __instance, int _framerate)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "LimitFrameRate",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsLimitFrameRate failed: {ex.Message}");
        }
    }

    // SettingsGraphics.LoadSettings
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.LoadSettings))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsLoadSettings(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "SettingsLoaded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsLoadSettings failed: {ex.Message}");
        }
    }

    // SettingsGraphics.ChangeDepthOfField
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.ChangeDepthOfField))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsChangeDepthOfField(SettingsGraphics __instance, float startFarFocus, float endFarFocus)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ChangeDepthOfField",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsChangeDepthOfField failed: {ex.Message}");
        }
    }

    // SettingsGraphics.ResetDepthOfField
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.ResetDepthOfField))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsResetDepthOfField(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ResetDepthOfField",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsResetDepthOfField failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetFieldOfView
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetFieldOfView))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetFieldOfView(SettingsGraphics __instance, float fov)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "FieldOfViewSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetFieldOfView failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetShadowDistance
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetShadowDistance))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetShadowDistance(SettingsGraphics __instance, float distance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ShadowDistanceSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetShadowDistance failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetMotionBlur
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetMotionBlur))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetMotionBlur(SettingsGraphics __instance, float motion)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "MotionBlurSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetMotionBlur failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetExposure
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetExposure))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetExposure(SettingsGraphics __instance, float exposure)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ExposureSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetExposure failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetupAA
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetupAA))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetupAA(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "upAASet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetupAA failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetAntiAliasing
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetAntiAliasing))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetAntiAliasing(SettingsGraphics __instance, int index)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "AntiAliasingSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetAntiAliasing failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetAAQuality
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetAAQuality))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetAAQuality(SettingsGraphics __instance, int index)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "AAQualitySet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetAAQuality failed: {ex.Message}");
        }
    }

    // SettingsGraphics.IsDLSSSupported
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.IsDLSSSupported))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsIsDLSSSupported(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "IsDLSSSupported",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsIsDLSSSupported failed: {ex.Message}");
        }
    }

    // SettingsGraphics.PopulateMonitors
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.PopulateMonitors))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsPopulateMonitors(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "PopulateMonitors",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsPopulateMonitors failed: {ex.Message}");
        }
    }

    // SettingsGraphics.SetMonitor
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.SetMonitor))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsSetMonitor(SettingsGraphics __instance, int monitorIndex)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "MonitorSet",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsSetMonitor failed: {ex.Message}");
        }
    }

    // SettingsGraphics.RepopulateResolutions
    [HarmonyPatch(typeof(SettingsGraphics), nameof(SettingsGraphics.RepopulateResolutions))]
    [HarmonyPostfix]
    private static void OnSettingsGraphicsRepopulateResolutions(SettingsGraphics __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "RepopulateResolutions",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsGraphicsRepopulateResolutions failed: {ex.Message}");
        }
    }

    // SettingsVolume.Start
    [HarmonyPatch(typeof(SettingsVolume), nameof(SettingsVolume.Start))]
    [HarmonyPostfix]
    private static void OnSettingsVolumeStart(SettingsVolume __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "ComponentInitialized",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsVolumeStart failed: {ex.Message}");
        }
    }

    // SettingsVolume.MasterVolume
    [HarmonyPatch(typeof(SettingsVolume), nameof(SettingsVolume.MasterVolume))]
    [HarmonyPostfix]
    private static void OnSettingsVolumeMasterVolume(SettingsVolume __instance, float volume)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "MasterVolume",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsVolumeMasterVolume failed: {ex.Message}");
        }
    }

    // SettingsVolume.MusicVolume
    [HarmonyPatch(typeof(SettingsVolume), nameof(SettingsVolume.MusicVolume))]
    [HarmonyPostfix]
    private static void OnSettingsVolumeMusicVolume(SettingsVolume __instance, float volume)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "MusicVolume",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsVolumeMusicVolume failed: {ex.Message}");
        }
    }

    // SettingsVolume.EffectVolume
    [HarmonyPatch(typeof(SettingsVolume), nameof(SettingsVolume.EffectVolume))]
    [HarmonyPostfix]
    private static void OnSettingsVolumeEffectVolume(SettingsVolume __instance, float volume)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "EffectVolume",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsVolumeEffectVolume failed: {ex.Message}");
        }
    }

    // SettingsVolume.RacksVolume
    [HarmonyPatch(typeof(SettingsVolume), nameof(SettingsVolume.RacksVolume))]
    [HarmonyPostfix]
    private static void OnSettingsVolumeRacksVolume(SettingsVolume __instance, float volume)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "RacksVolume",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsVolumeRacksVolume failed: {ex.Message}");
        }
    }

    // SettingsVolume.LoadSettings
    [HarmonyPatch(typeof(SettingsVolume), nameof(SettingsVolume.LoadSettings))]
    [HarmonyPostfix]
    private static void OnSettingsVolumeLoadSettings(SettingsVolume __instance)
    {
        try
        {
            if (__instance == null || __instance.NativePointer == IntPtr.Zero) return;
            SafeEmit(
                "SettingsLoaded",
                new
                {
                    instance = __instance,
                });
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Hook OnSettingsVolumeLoadSettings failed: {ex.Message}");
        }
    }

}
