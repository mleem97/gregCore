using System;
using UnityEngine;

namespace gregCore.Infrastructure.Settings.Models;

public class KeybindEntry
{
    public string ModId { get; set; }
    public string ActionId { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public KeyCode CurrentKey { get; set; }
    public KeyCode DefaultKey { get; set; }
    public string Category { get; set; }
    public bool HasConflict { get; set; }

    // Ignored in JSON, used at runtime
    [Newtonsoft.Json.JsonIgnore]
    public Action OnPress { get; set; }

    public string GetFullId() => $"{ModId}.{ActionId}";
}
