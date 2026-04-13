using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using greg.Core.Scripting;

namespace gregCoreSDK.Core.UI.Patches;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
public class MainMenuStartPatch
{
    public static void Postfix(MainMenu __instance)
    {
        try
        {
            if (__instance == null) return;
            
            GregHookBus.Fire("greg.UI.MainMenu.Opened", new {
                instanceId = __instance.GetInstanceID()
            });
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[MainMenuStartPatch] {ex.Message}");
        }
    }
}
