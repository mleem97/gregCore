using System;
using UnityEngine;

namespace gregCore.Infrastructure.Settings.Models;

public class KeybindEntry
{
    public string ModId { get; set; } = null!;
    public string ActionId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public KeyCode CurrentKey { get; set; }
    public KeyCode DefaultKey { get; set; }
    public string Category { get; set; } = null!;
    public bool HasConflict { get; set; }

    /// <summary>
    /// True when the registry moved this entry off its requested key
    /// (collision or game-reserved) onto a free fallback key. Survives
    /// JSON persistence like CurrentKey.
    /// </summary>
    public bool AutoResolved { get; set; }

    // Ignored in JSON, used at runtime
    [Newtonsoft.Json.JsonIgnore]
    public Action OnPress { get; set; } = null!;

    public string GetFullId() => $"{ModId}.{ActionId}";
}
