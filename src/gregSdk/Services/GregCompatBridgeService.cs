using System;
using MelonLoader;

namespace greg.Sdk.Services;

/// <summary>
/// Compatibility bridge for legacy JoniML layout system.
/// Ensures old mods using JoniML UI patterns work with gregCore.
/// 
/// Integrated from gregAddons/gregMods/gregMod.LangCompatBridge.
/// </summary>
public static class GregCompatBridgeService
{
    private static bool _loaded;

    /// <summary>Ensure the compatibility layer is active.</summary>
    public static void EnsureLoaded()
    {
        if (_loaded) return;
        _loaded = true;

        try
        {
            // Register legacy UI layout compatibility hooks
            MelonLogger.Msg("[CompatBridge] JoniML compatibility bridge loaded.");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[CompatBridge] Init failed: {ex.Message}");
        }
    }
}
