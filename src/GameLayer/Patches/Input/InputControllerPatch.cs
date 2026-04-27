/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Prefix-Override für InputController-Properties (Move, Look, Interact).
/// Maintainer:   Die IL2CPP-Properties sind hohl und geben null/default zurück.
///               Dieser Patch leitet Input über UnityEngine.Input.GetAxis (Legacy) um.
/// </file-summary>

using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace gregCore.GameLayer.Patches.Input;

[HarmonyPatch]
internal static class InputControllerPatch
{
    /// <summary>
    /// Override für InputController.Move (Vector2 Property).
    /// Liest Horizontal/Vertical aus dem Legacy-Input-System.
    /// </summary>
    [HarmonyPatch(typeof(global::Il2Cpp.InputController), "Move", MethodType.Getter)]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool MovePrefix(global::Il2Cpp.InputController __instance, ref Vector2 __result)
    {
        try
        {
            if (__instance == null) return true;

            __result = new Vector2(
                UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical")
            );
            return false; // Skip hollow original
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[InputPatch] Move override failed: {ex.Message}");
            return true; // Fallback to original
        }
    }

    /// <summary>
    /// Override für InputController.Look (Vector2 Property).
    /// Liest Mouse X/Y aus dem Legacy-Input-System.
    /// </summary>
    [HarmonyPatch(typeof(global::Il2Cpp.InputController), "Look", MethodType.Getter)]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool LookPrefix(global::Il2Cpp.InputController __instance, ref Vector2 __result)
    {
        try
        {
            if (__instance == null) return true;

            __result = new Vector2(
                UnityEngine.Input.GetAxis("Mouse X"),
                UnityEngine.Input.GetAxis("Mouse Y")
            );
            return false; // Skip hollow original
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[InputPatch] Look override failed: {ex.Message}");
            return true;
        }
    }

    /// <summary>
    /// Override für InputController.Interact (bool Property).
    /// Nutzt linke Maustaste als Interact-Trigger.
    /// </summary>
    [HarmonyPatch(typeof(global::Il2Cpp.InputController), "Interact", MethodType.Getter)]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool InteractPrefix(global::Il2Cpp.InputController __instance, ref bool __result)
    {
        try
        {
            if (__instance == null) return true;

            __result = UnityEngine.Input.GetMouseButtonDown(0);
            return false; // Skip hollow original
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[InputPatch] Interact override failed: {ex.Message}");
            return true;
        }
    }
}
