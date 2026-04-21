/// <file-summary>
/// Schicht:      Core
/// Zweck:        Datenmodell für Events, die über den IGregEventBus verschickt werden.
/// Maintainer:   Blittable struct wo möglich. IsCancelled ist das einzige mutable Feld.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace gregCore.Core.Models;

// [GREG_SYNC_INSERT_DTOS]

[StructLayout(LayoutKind.Sequential)]
public record EventPayload
{
    public string HookName { get; init; } = null!;
    public DateTime OccurredAtUtc { get; init; }
    public IReadOnlyDictionary<string, object> Data { get; init; } = null!;
    public bool IsCancelable { get; init; }
    public bool IsCancelled { get; set; }
}
