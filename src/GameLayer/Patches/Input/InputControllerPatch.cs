/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Prefix-Override für InputController-Properties (Move, Look, Interact).
/// STATUS:       DEACTIVATED — Targets do not exist in current Il2CppInterop dummy-assembly.
/// Maintainer:   Requires rewrite against actual InputAction map structure.
/// </file-summary>

using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace gregCore.GameLayer.Patches.Input;

[HarmonyPatch]
internal static class InputControllerPatch
{
    // DEACTIVATED: InputController does NOT expose Move/Look/Interact properties
    // in the Il2CppInterop dummy-assembly for this game version.
    // Inspected members (Mono.Cecil):
    //   - Type: InputController extends Il2CppSystem.Object
    //   - Properties present: m_Player_Move, m_Player_Look, m_Player_Interact (backing fields)
    //   - No public getters for Move/Look/Interact exist.
    //
    // Consequently, Harmony cannot resolve the patch targets and throws:
    //   "Could not find property for type Il2Cpp.InputController and name Move"
    //
    // Repair strategy:
    //   1. Inspect the generated InputController dummy-DLL to locate the actual
    //      InputAction properties (likely under InputController.Player.Move etc.).
    //   2. Patch the ReadValue<Vector2>() / ReadValue<float>() / WasPressedThisFrame()
    //      methods of the specific InputAction instances, OR
    //   3. Implement a non-Harmony input-polling layer that intercepts at the
    //      application level without patching non-existent properties.
    //
    // Until then, this file remains a placeholder to prevent HarmonyInit exceptions.

    /*
    [HarmonyPatch(typeof(global::Il2Cpp.InputController), "Move", MethodType.Getter)]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool MovePrefix(global::Il2Cpp.InputController __instance, ref Vector2 __result)
    {
        try
        {
            if (__instance == null) return true;
            __result = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));
            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[InputPatch] Move override failed: {ex.Message}");
            return true;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(global::Il2Cpp.InputController), "Look", MethodType.Getter)]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool LookPrefix(global::Il2Cpp.InputController __instance, ref Vector2 __result)
    {
        try
        {
            if (__instance == null) return true;
            __result = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[InputPatch] Look override failed: {ex.Message}");
            return true;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(global::Il2Cpp.InputController), "Interact", MethodType.Getter)]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool InteractPrefix(global::Il2Cpp.InputController __instance, ref bool __result)
    {
        try
        {
            if (__instance == null) return true;
            __result = UnityEngine.Input.GetMouseButtonDown(0);
            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[InputPatch] Interact override failed: {ex.Message}");
            return true;
        }
    }
    */
}
