using System;
using UnityEngine;

namespace gregCoreSDK.Sdk.Definitions;

/// <summary>
/// Manifest for a UI replacement or override.
/// </summary>
[Serializable]
public class GregUiReplacementManifest
{
    /// <summary>The full path to the UI element (e.g., "Canvas/Panel/Button").</summary>
    public string UiPath { get; set; } = string.Empty;

    /// <summary>Whether to activate or deactivate the element.</summary>
    public bool? Active { get; set; }

    /// <summary>New position (local).</summary>
    public Vector3? LocalPosition { get; set; }

    /// <summary>New rotation (local Euler angles).</summary>
    public Vector3? LocalEulerAngles { get; set; }

    /// <summary>New scale (local).</summary>
    public Vector3? LocalScale { get; set; }

    /// <summary>New color (if the element has an Image or Text component).</summary>
    public Color? Color { get; set; }

    /// <summary>New text (if the element has a TextMeshProUGUI or Text component).</summary>
    public string Text { get; set; }

    /// <summary>Author of the override.</summary>
    public string Author { get; set; } = "Unknown";

    /// <summary>Priority of the override (higher overrides lower).</summary>
    public int Priority { get; set; } = 0;
}
