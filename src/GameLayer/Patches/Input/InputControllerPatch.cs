/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Prefix-Override für InputController-Properties (Move, Look, Interact).
/// STATUS:       DEACTIVATED — Targets do not exist in current Il2CppInterop dummy-assembly.
/// Maintainer:   Requires rewrite against actual InputAction map structure.
/// </file-summary>

using HarmonyLib;

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
    // Placeholder only. Do not add Legacy Input fallbacks here; Data Center uses
    // Unity's Input System package, and the dummy DLL must be re-inspected before
    // patching any generated InputAction accessors.
}
