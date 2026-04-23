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
[HarmonyPatch]
internal static class GregUiHooks
{
    // BalanceSheet.Awake
    [HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.Awake))]
    [HarmonyPostfix]
    private static void OnBalanceSheetAwake(BalanceSheet __instance)
    {
        try
        {
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ComponentInitialized"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ComponentInitialized"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "OnDestroy"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "GetOrCreateRecord"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "RegisterSalary"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "CountFailingApps"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "SnapshotSaved"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "GetLatestSnapshot"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "FillInBalanceSheet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "SalaryRowAdded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "TotalRowAdded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "HeaderRowAdded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "SectionTitleAdded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "InstantiateRow"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ClearRows"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "GetSaveData"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "FromSaveLoaded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "RestoreRecord"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ComponentInitialized"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "Continue"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "NewGame"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "QuitGame"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "GameLoaded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "HideMiddleMenu"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "tingsSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ComponentInitialized"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "LookSensitivity"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "InvertY"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "SettingsLoaded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ComponentInitialized"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "QualitySet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "FullScreenSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ResDropDownSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ResolutionSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "AvailableRefreshRate"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "RefreshRateSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "LimitFrameRate"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "SettingsLoaded"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ChangeDepthOfField"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ResetDepthOfField"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "FieldOfViewSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ShadowDistanceSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "MotionBlurSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ExposureSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "upAASet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "AntiAliasingSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "AAQualitySet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "IsDLSSSupported"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "PopulateMonitors"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "MonitorSet"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "RepopulateResolutions"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "ComponentInitialized"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "MasterVolume"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "MusicVolume"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "EffectVolume"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "RacksVolume"),
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
            gregEventDispatcher.Emit(
                gregHookName.Create(GregDomain.Ui, "SettingsLoaded"),
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
