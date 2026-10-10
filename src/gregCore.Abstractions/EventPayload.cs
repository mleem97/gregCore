/// <file-summary>
/// Layer:       Core
/// Purpose:      Data model for events dispatched via the IGregEventBus.
/// Maintainer:   Blittable struct where possible. IsCancelled is the only mutable field.
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
