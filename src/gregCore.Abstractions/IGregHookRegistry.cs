using System.Collections.Generic;
using gregCore.Core.Models;

namespace gregCore.Core.Abstractions;

public interface IGregHookRegistry
{
    /// <summary>
    /// Retrieves all loaded hooks.
    /// </summary>
    IEnumerable<GregHookDef> GetAllHooks();

    /// <summary>
    /// Looks up a hook by its name.
    /// </summary>
    bool TryGetHook(string name, out GregHookDef hookDef);

    /// <summary>
    /// Looks up an event ID for a hook name (FFI compatibility).
    /// </summary>
    bool TryGetEventId(string hookName, out int eventId);

    /// <summary>
    /// Looks up a hook name by event ID (FFI compatibility).
    /// </summary>
    bool TryGetHookName(int eventId, out string hookName);
}
