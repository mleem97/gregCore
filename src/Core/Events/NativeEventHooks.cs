/// <file-summary>
/// Schicht:      Core
/// Zweck:        Mappt Hook-Namen auf Native Event-IDs für FFI.
/// Maintainer:   Erlaubt es nativen Mods, Events via ID zu empfangen.
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
