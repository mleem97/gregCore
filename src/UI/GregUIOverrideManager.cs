using UnityEngine;
using MelonLoader;
using System.Linq;

namespace gregCore.UI
{
    /// <summary>
    /// Manages the "Total Conversion" of the game's UI.
    /// Hides vanilla HUD elements and redirects calls to GregUI.
    /// </summary>
    public static class GregUIOverrideManager
    {
        public static void HideVanillaUI()
        {
            // Find common vanilla canvases in "Data Center"
            // We disable the Canvas component instead of SetActive(false) to prevent native logic crashes.
            var canvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases)
            {
                // Skip our own UI
                if (canvas.gameObject.name.StartsWith("gregCore")) continue;
                
                // Hide persistent HUD elements
                if (canvas.gameObject.name.Contains("HUD") || 
                    canvas.gameObject.name.Contains("PlayerStatus") ||
                    canvas.gameObject.name.Contains("MiniMap"))
                {
                    canvas.enabled = false;
                    MelonLogger.Msg($"[gC-UI] Overrode vanilla UI component: {canvas.gameObject.name}");
                }
            }
        }

        public static void ShowVanillaUI()
        {
            var canvases = UnityEngine.Object.FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases)
            {
                if (canvas.gameObject.name.StartsWith("gregCore")) continue;
                canvas.enabled = true;
            }
        }
    }
}
