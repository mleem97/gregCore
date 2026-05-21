using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace greg.Multiplayer.Patches
{
    /// <summary>
    /// Patches the ESC / Pause menu to inject a "Multiplayer" tab button.
    /// Confirmed class/method from ILSpy: global::Il2Cpp.PauseMenu.OnEnable()
    /// </summary>
    [HarmonyPatch]
    internal static class EscMenuPatch
    {
        [HarmonyPatch(typeof(global::Il2Cpp.PauseMenu), nameof(global::Il2Cpp.PauseMenu.OnEnable))]
        [HarmonyPostfix]
        private static void OnEnable_Postfix()
        {
            // Find or spawn HUD and toggle
            var hud = UnityEngine.Object.FindObjectOfType<MultiplayerHud>();
            hud?.Toggle();
        }
    }
}
