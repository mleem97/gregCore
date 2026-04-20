/// <file-summary>
/// Schicht:      Core
/// Zweck:        Repräsentiert einen eindeutigen Hook-Namen.
/// Maintainer:   Format "greg.{Domain}.{Event}".
/// </file-summary>

namespace gregCore.Core.Models;

public readonly record struct HookName
{
    public string Domain { get; init; }
    public string Event { get; init; }

    private readonly string? _full;
    public string Full => _full ?? $"greg.{Domain}.{Event}";

    public static HookName Parse(string full)
    {
        ArgumentNullException.ThrowIfNull(full);
        var parts = full.Split('.');
        if (parts.Length >= 3 && parts[0] == "greg")
        {
            return new HookName { Domain = parts[1], Event = parts[2], _full = full };
        }
        throw new ArgumentException($"Invalid HookName format: {full}");
    }

    public static HookName Create(string domain, string eventName)
    {
        ArgumentNullException.ThrowIfNull(domain);
        ArgumentNullException.ThrowIfNull(eventName);
        return new HookName { Domain = domain, Event = eventName, _full = $"greg.{domain}.{eventName}" };
    }

    public override string ToString() => Full;
}
