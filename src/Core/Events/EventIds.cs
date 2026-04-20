/// <file-summary>
/// Schicht:      Core
/// Zweck:        Definiert eindeutige IDs für Native FFI Events.
/// Maintainer:   Muss synchron mit C/Rust Headern bleiben.
/// </file-summary>

namespace gregCore.Core.Events;

public static class EventIds
{
    // [GREG_SYNC_INSERT_EVENTIDS]
    
    public const int PlayerCoinUpdated = 1001;
    public const int GameSaved = 2001;
    public const int ServerStatusChanged = 3001;
}
