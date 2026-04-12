using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using greg.Core.Scripting;

namespace greg.Core.UI.Patches;

[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.OnEnable))]
public class PauseMenuOpenPatch
{
    public static void Postfix(PauseMenu __instance)
    {
        try
        {
            if (__instance == null) return;
            
            GregHookBus.Fire("greg.UI.PauseMenu.Opened", new {
                instanceId = __instance.GetInstanceID()
            });
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[PauseMenuOpenPatch] {ex.Message}");
        }
    }
}

[HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.OnDisable))]
public class PauseMenuClosePatch
{
    public static void Postfix(PauseMenu __instance)
    {
        try
        {
            if (__instance == null) return;
            
            GregHookBus.Fire("greg.UI.PauseMenu.Closed", new {
                instanceId = __instance.GetInstanceID()
            });
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[PauseMenuClosePatch] {ex.Message}");
        }
    }
}
