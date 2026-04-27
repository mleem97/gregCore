using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace greg.Multiplayer.Patches
{
    /// <summary>
    /// Patches the ESC / Pause menu to inject a "Multiplayer" tab button.
    /// TODO: Confirm class/method from ILSpy – likely UIMenuManager.Show() or PauseMenu.Open().
    /// </summary>
    [HarmonyPatch]
    internal static class EscMenuPatch
    {
        [HarmonyPatch("PauseMenuUI", "Open")]
        [HarmonyPostfix]
        private static void Open_Postfix()
        {
            // Find or spawn HUD and toggle
            var hud = UnityEngine.Object.FindObjectOfType<MultiplayerHud>();
            hud?.Toggle();
        }
    }
}
