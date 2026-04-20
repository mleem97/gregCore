using System;

namespace gregCore.Infrastructure.Settings.Models;

public abstract class BaseSettingEntry
{
    public string ModId { get; set; }
    public string SettingId { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    
    public string TypeName { get; set; }

    public string GetFullId() => $"{ModId}.{SettingId}";
}

public class SettingEntry<T> : BaseSettingEntry
{
    public T Value { get; set; }
    public T DefaultValue { get; set; }

    // Ignored in JSON
    [Newtonsoft.Json.JsonIgnore]
    public Action<T> OnValueChanged { get; set; }

    public SettingEntry()
    {
        TypeName = typeof(T).Name;
    }
}
