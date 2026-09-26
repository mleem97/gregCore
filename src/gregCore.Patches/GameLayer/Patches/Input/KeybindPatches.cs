using HarmonyLib;

namespace gregCore.GameLayer.Patches.Input;

/// <summary>
/// DEACTIVATED — Patching UnityEngine.Input.GetKeyDown is not viable under IL2CPP.
/// The Legacy Input Manager is disabled in this game (Input System package active).
/// All input polling has been migrated to UnityEngine.InputSystem.
/// </summary>
[HarmonyPatch]
internal static class KeybindPatches
{
    // Placeholder only. Use GregInputBindingService / UnityEngine.InputSystem
    // for keybind logic; do not patch Legacy Input methods in IL2CPP.
}
