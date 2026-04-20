/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Blockiert die Pause-Logik des Spiels wenn die Console offen ist.
/// Maintainer:   Harmony Patch. Zielt auf die InputSystem-Aktion 'Pause'.
/// </file-summary>

using HarmonyLib;
using gregCore.Infrastructure.UI;

namespace gregCore.GameLayer.Patches.UI;

[HarmonyPatch(typeof(global::Il2Cpp.InputController.IUIActions), nameof(global::Il2Cpp.InputController.IUIActions.OnPause))]
internal static class InputPausePatch
{
    [HarmonyPrefix]
    internal static bool Prefix()
    {
        // Wenn die DevConsole offen ist, darf das Pause-Menü nicht aufgehen
        if (GregDevConsole.Instance.IsOpen)
        {
            return false; // Verhindert das Auslösen der Original-Methode
        }
        return true;
    }
}
