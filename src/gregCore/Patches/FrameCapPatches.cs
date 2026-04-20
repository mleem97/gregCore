using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using greg.Diagnostic;
using gregCore.Services;

namespace gregCore.Patches;

[HarmonyPatch(typeof(UnityEngine.Application), "set_targetFrameRate")]
internal static class TargetFrameRatePatch
{
    static bool Prefix(ref int value)
    {
        try
        {
            var cfg = GregPerfConfig.Instance;
            if (value < 0 || value > cfg.MaxAllowedFps)
            {
                MelonLogger.Msg($"[PerfCore] Blocked targetFrameRate={value} → capped to {cfg.CurrentTarget}");
                value = cfg.CurrentTarget;
            }
        }
        catch (Exception ex) { MelonLogger.Error($"{nameof(TargetFrameRatePatch)}: {ex.Message}"); }
        return true;
    }
}

[HarmonyPatch(typeof(UnityEngine.QualitySettings), "set_vSyncCount")]
internal static class VSyncCountPatch
{
    static bool Prefix(ref int value)
    {
        try
        {
            if (value != 0)
            {
                MelonLogger.Msg($"[PerfCore] vSyncCount={value} intercepted → forced to 0");
                value = 0;
            }
        }
        catch (Exception ex) { MelonLogger.Error($"{nameof(VSyncCountPatch)}: {ex.Message}"); }
        return true;
    }
}

[HarmonyPatch(typeof(UnityEngine.SceneManagement.SceneManager), "Internal_SceneLoaded")]
internal static class SceneLoadFrameCapPatch
{
    static void Postfix(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        try
        {
            GregFrameCapService.Instance?.OnSceneLoaded(scene.name);
        }
        catch (Exception ex) { MelonLogger.Error($"{nameof(SceneLoadFrameCapPatch)}: {ex.Message}"); }
    }
}