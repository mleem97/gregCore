using System;
using System.Collections.Generic;
using UnityEngine;

namespace greg.Sdk.Services;

/// <summary>
/// SDK Service for managing the JADE-style HUD overlay.
/// Allows any mod (C#, Lua, TS, Rust) to display information onto a unified telemetry box.
/// </summary>
public static class GregHudService
{
    public static void Initialize()
    {
        // Stub implementation
    }

    public static void UpdateJadeBox(string title, string subHeader, List<GregMetadataEntry> entries)
    {
        // Stub implementation
    }

    public static void HideJadeBox()
    {
        // Stub implementation
    }
}
