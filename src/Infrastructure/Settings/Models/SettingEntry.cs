using System;

namespace gregCore.Infrastructure.Settings.Models;

public abstract class BaseSettingEntry
{
    public string ModId { get; set; } = null!;
    public string SettingId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    
    public string TypeName { get; set; } = null!;

    public string GetFullId() => $"{ModId}.{SettingId}";
}

public class SettingEntry<T> : BaseSettingEntry
{
    public T Value { get; set; } = default!;
    public T DefaultValue { get; set; } = default!;

    // Ignored in JSON
    [Newtonsoft.Json.JsonIgnore]
    public Action<T> OnValueChanged { get; set; } = null!;

    public SettingEntry()
    {
        TypeName = typeof(T).Name;
    }
}
