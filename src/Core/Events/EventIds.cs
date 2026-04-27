/// <file-summary>
/// Schicht:      Core
/// Zweck:        Definiert eindeutige IDs für Native FFI Events.
/// Maintainer:   Muss synchron mit C/Rust Headern bleiben.
/// </file-summary>

namespace gregCore.Core.Events;

public static class EventIds
{
    // [GREG_SYNC_INSERT_EVENTIDS]
    
    // Economy
    public const int PlayerCoinUpdated = 1001;
    public const int PlayerXpUpdated = 1002;
    public const int PlayerReputationUpdated = 1003;
    
    // System
    public const int GameSaved = 2001;
    
    // Hardware
    public const int ServerStatusChanged = 3001;
    public const int RackPositionQueried = 3002;
    public const int RackPositionUsed = 3003;
    public const int RackPositionFreed = 3004;
    
    // Networking / Cables
    public const int CableCreated = 4001;
    
    // Input
    public const int InputMoveOverridden = 5001;
    public const int InputLookOverridden = 5002;
    public const int InputInteractOverridden = 5003;
}
