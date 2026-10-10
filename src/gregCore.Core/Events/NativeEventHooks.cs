/// <file-summary>
/// Layer:       Core
/// Purpose:     Maps hook names to native event IDs for FFI.
/// Maintainer:  Allows native mods to receive events by ID.
/// </file-summary>

namespace gregCore.Core.Events;

public static class NativeEventHooks
{
    private static readonly Dictionary<string, int> _hookToId = new()
    {
        // [GREG_SYNC_INSERT_MAPPINGS]
        { HookName.Create("economy", "PlayerCoinUpdated").Full, EventIds.PlayerCoinUpdated },
        { HookName.Create("persistence", "GameSaved").Full, EventIds.GameSaved },
        { HookName.Create("hardware", "ServerStatusChanged").Full, EventIds.ServerStatusChanged }
    };

    public static bool TryGetEventId(string hookName, out int eventId) =>
        _hookToId.TryGetValue(hookName, out eventId);
}
