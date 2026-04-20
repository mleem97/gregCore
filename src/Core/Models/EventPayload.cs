/// <file-summary>
/// Schicht:      Core
/// Zweck:        Datenmodell für Events, die über den IGregEventBus verschickt werden.
/// Maintainer:   Blittable struct wo möglich. IsCancelled ist das einzige mutable Feld.
/// </file-summary>

namespace gregCore.Core.Models;

// [GREG_SYNC_INSERT_DTOS]

[StructLayout(LayoutKind.Sequential)]
public record EventPayload
{
    public string HookName { get; init; }
    public DateTime OccurredAtUtc { get; init; }
    public IReadOnlyDictionary<string, object> Data { get; init; }
    public bool IsCancelable { get; init; }
    public bool IsCancelled { get; set; }
}
