using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using greg.Core.Scripting;

namespace greg.Core.UI.Patches;

[HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.Awake))]
public class StaticUIElementsAwakePatch
{
    public static void Postfix(StaticUIElements __instance)
    {
        try
        {
            if (__instance == null) return;
            
            GregHookBus.Fire("greg.UI.HUD.Ready", new {
                instanceId = __instance.GetInstanceID(),
                canvasName = __instance.canvasStatic?.name ?? "unknown"
            });
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[StaticUIElementsAwakePatch] {ex.Message}");
        }
    }
}
