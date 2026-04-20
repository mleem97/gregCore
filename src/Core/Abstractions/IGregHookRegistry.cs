using System.Collections.Generic;
using gregCore.Core.Models;

namespace gregCore.Core.Abstractions;

public interface IGregHookRegistry
{
    /// <summary>
    /// Ruft alle geladenen Hooks ab.
    /// </summary>
    IEnumerable<GregHookDef> GetAllHooks();

    /// <summary>
    /// Sucht einen Hook anhand seines Namens.
    /// </summary>
    bool TryGetHook(string name, out GregHookDef hookDef);

    /// <summary>
    /// Sucht eine Event-ID für einen Hooknamen (FFI Kompatibilität).
    /// </summary>
    bool TryGetEventId(string hookName, out int eventId);

    /// <summary>
    /// Sucht einen Hooknamen anhand der Event-ID (FFI Kompatibilität).
    /// </summary>
    bool TryGetHookName(int eventId, out string hookName);
}
