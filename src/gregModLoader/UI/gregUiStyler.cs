using System;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;

namespace greg.Core.UI;

/// <summary>
/// Replaces the vanilla game UI with the "Luminescent Architect" design system.
/// </summary>
public static class gregUiStyler
{
    private static readonly Color Abyss = new Color(0.00f, 0.07f, 0.06f, 0.85f);
    private static readonly Color SurfaceContainer = new Color(0.00f, 0.12f, 0.11f, 0.90f);
    public static readonly Color PrimaryTeal = new Color(0.38f, 0.96f, 0.85f, 1f);
    private static readonly Color OnSurface = new Color(0.75f, 0.99f, 0.96f, 1f);

    public static void ApplyLuminescentStyle()
    {
        try
        {
            // Restyle Panels and Backgrounds
            var images = UnityEngine.Object.FindObjectsOfType<Image>(true);
            foreach (var img in images)
            {
                if (img == null || img.gameObject == null) continue;

                string name = img.gameObject.name.ToLower();

                // ONLY target specific UI elements to prevent "everything transparent" bug
                if (name.Contains("settings") || name.Contains("menu") || name.Contains("panel") || name.Contains("popup"))
                {
                    // Slightly more opaque Abyss for readability
                    img.color = new Color(0.00f, 0.07f, 0.06f, 0.95f);
                    
                    var outline = img.GetComponent<Outline>();
                    if (outline != null) outline.enabled = false; // Disable instead of Destroy
                }
                else if (img.GetComponent<Button>() != null)
                {
                    img.color = SurfaceContainer;
                }
            }

            // Restyle Text
            var texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (var txt in texts)
            {
                if (txt == null) continue;
                if (txt.color.r > 0.7f && txt.color.g > 0.7f) txt.color = OnSurface;
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("gregUiStyler.ApplyLuminescentStyle", ex);
        }
    }
}
