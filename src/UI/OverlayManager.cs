#nullable enable
using System;
using System.Collections.Generic;

namespace gregCore.UI;

/// <summary>
/// Centralized coordinator for framework-wide overlays. 
/// Tracks visibility of independent panels to manage raycast blocking and input intercepts.
/// </summary>
public static class OverlayManager
{
    private static readonly HashSet<string> _visibleOverlays = new();
    
    /// <summary>
    /// Returns true if any framework overlay is currently visible.
    /// </summary>
    public static bool IsAnyOverlayVisible => _visibleOverlays.Count > 0;

    /// <summary>
    /// Notifies the manager that an overlay has changed visibility.
    /// </summary>
    public static void SetVisible(string overlayId, bool visible)
    {
        if (visible)
        {
            _visibleOverlays.Add(overlayId);
        }
        else
        {
            _visibleOverlays.Remove(overlayId);
        }

        // Apply raycast blocker state based on global visibility
        // UiRaycastBlocker.SetBlocking(IsAnyOverlayVisible);
    }

    /// <summary>
    /// Forces all overlays to be hidden.
    /// </summary>
    public static void HideAll()
    {
        _visibleOverlays.Clear();
        // UiRaycastBlocker.SetBlocking(false);
    }
}
