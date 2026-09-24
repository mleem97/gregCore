using System.Collections.Generic;

namespace gregCore.Sdk.Models;

/// <summary>
/// Unified payload model for all framework events and hooks (SDK layer).
/// </summary>
public sealed class GregPayload
{
    public string HookName { get; init; } = string.Empty;
    public string Trigger { get; init; } = string.Empty;
    public Dictionary<string, object> Data { get; init; } = new();
    public string Version { get; init; } = "1.0.0";

    public GregPayload() { }

    public GregPayload(string hookName, string trigger)
    {
        HookName = hookName;
        Trigger = trigger;
    }

    public T? GetValue<T>(string key)
    {
        if (Data.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;
        return default;
    }

    public static T Get<T>(object payload, string fieldName, T fallback)
    {
        if (payload is GregPayload p)
        {
            return p.GetValue<T>(fieldName) ?? fallback;
        }
        
        if (payload is gregCore.Core.Models.EventPayload ep && ep.Data.TryGetValue(fieldName, out var val))
        {
            try { return (T)System.Convert.ChangeType(val, typeof(T)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        return fallback;
    }
}
