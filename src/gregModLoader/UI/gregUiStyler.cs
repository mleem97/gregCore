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

                // Check if it's a structural element (Panel/Background)
                if (name.Contains("background") || name.Contains("panel") || name.Contains("border"))
                {
                    img.color = Abyss;
                    
                    // Remove 1px solid borders if they exist (via Outline component)
                    var outline = img.GetComponent<Outline>();
                    if (outline != null)
                    {
                        UnityEngine.Object.Destroy(outline);
                    }
                }
                // Check if it's an interactive element (Button)
                else if (name.Contains("button") || img.GetComponent<Button>() != null)
                {
                    img.color = SurfaceContainer;
                    
                    var btn = img.GetComponent<Button>();
                    if (btn != null)
                    {
                        var colors = btn.colors;
                        colors.normalColor = SurfaceContainer;
                        colors.highlightedColor = PrimaryTeal;
                        colors.pressedColor = new Color(0.03f, 0.75f, 0.65f, 1f);
                        btn.colors = colors;
                    }
                }
            }

            // Restyle Typography (Editorial Edge)
            var texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (var txt in texts)
            {
                if (txt == null) continue;
                
                // Swap default white/black to OnSurface for reduced eye strain
                if (txt.color.r > 0.8f && txt.color.g > 0.8f && txt.color.b > 0.8f) // Nearly white
                {
                    txt.color = OnSurface;
                }
                else if (txt.color.r < 0.2f && txt.color.g < 0.2f && txt.color.b < 0.2f) // Nearly black
                {
                    txt.color = PrimaryTeal;
                }
            }

            MelonLoader.MelonLogger.Msg("[gregCore] Luminescent Architect style applied to vanilla UI.");
        }
        catch (Exception ex)
        {
            CrashLog.LogException("gregUiStyler.ApplyLuminescentStyle", ex);
        }
    }
}
