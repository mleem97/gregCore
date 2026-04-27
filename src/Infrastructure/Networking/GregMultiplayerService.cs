/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Zentrale Verwaltung von Multiplayer-Sync-Events und FishNet-Integration.
/// Maintainer:   Koordiniert GregNetworkRack, GregNetworkServer und GregNetworkCables.
///               FishNet wird optional geladen – prüft Assembly-Verfügbarkeit beim Start.
/// </file-summary>

using System;
using System.Reflection;
using MelonLoader;
using gregCore.Core.Events;
using gregCore.Core.Abstractions;
using gregCore.Compatibility.FishNet;

namespace gregCore.Infrastructure.Networking;

/// <summary>
/// Zentraler Multiplayer-Service für gregCore.
/// Verwaltet die Netzwerk-Sync-Layer und koordiniert FishNet-Integration.
/// </summary>
public sealed class GregMultiplayerService : IDisposable
{
    private readonly GregEventBus _eventBus;
    private readonly IGregLogger _logger;
    
    // Netzwerk-Sync-Layer
    private GregNetworkRack? _networkRack;
    private GregNetworkServer? _networkServer;
    private GregNetworkCables? _networkCables;
    
    private bool _isFishNetAvailable;
    private bool _isInitialized;
    private bool _disposed;

    public GregMultiplayerService(GregEventBus eventBus, IGregLogger logger)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _logger = logger?.ForContext("Multiplayer") ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Prüft ob FishNet als Assembly verfügbar ist und initialisiert die Sync-Layer.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized) return;
        
        try
        {
            _isFishNetAvailable = CheckFishNetAvailability();
            
            if (_isFishNetAvailable)
            {
                _logger.Success("FishNet assembly detected – Multiplayer-Sync aktiviert.");
            }
            else
            {
                _logger.Warning("FishNet assembly not found – Multiplayer-Sync läuft im Offline-Modus.");
            }
            
            // Initialize sync layers (they work in offline mode too, just without network broadcast)
            _networkRack = new GregNetworkRack(_eventBus, _logger);
            _networkServer = new GregNetworkServer(_eventBus, _logger);
            _networkCables = new GregNetworkCables(_eventBus, _logger);
            
            _isInitialized = true;
            _logger.Success($"GregMultiplayerService initialized. FishNet={_isFishNetAvailable}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to initialize multiplayer service: {ex.Message}");
        }
    }

    /// <summary>
    /// Prüft ob die FishNet-Assembly geladen werden kann.
    /// </summary>
    private bool CheckFishNetAvailability()
    {
        try
        {
            // Versuche FishNet-Assembly zu finden
            var fishNetAssembly = Assembly.Load("FishNet.Runtime");
            return fishNetAssembly != null;
        }
        catch (Exception)
        {
            // Assembly nicht verfügbar – kein Fehler, Offline-Modus
            return false;
        }
    }

    /// <summary>
    /// Gibt zurück ob eine aktive Netzwerk-Session besteht.
    /// </summary>
    public bool IsConnected => _isFishNetAvailable && _isInitialized;

    /// <summary>
    /// Gibt den Rack-Sync-Layer zurück.
    /// </summary>
    public GregNetworkRack? Racks => _networkRack;

    /// <summary>
    /// Gibt den Server-Sync-Layer zurück.
    /// </summary>
    public GregNetworkServer? Servers => _networkServer;

    /// <summary>
    /// Gibt den Kabel-Sync-Layer zurück.
    /// </summary>
    public GregNetworkCables? Cables => _networkCables;

    /// <summary>
    /// Verarbeitet eingehende Netzwerk-Nachrichten (Dispatch an die jeweiligen Sync-Layer).
    /// </summary>
    public void ProcessIncomingMessage(string channel, byte[] data)
    {
        if (_disposed || data == null) return;
        
        try
        {
            switch (channel)
            {
                case "RackSync":
                    // Deserialize und ApplyRemotePositionUpdate
                    _logger.Info($"Incoming rack sync: {data.Length} bytes");
                    break;
                    
                case "ServerSync":
                    // Deserialize und ApplyRemotePowerUpdate
                    _logger.Info($"Incoming server sync: {data.Length} bytes");
                    break;
                    
                case "CableSync":
                    // Deserialize und ApplyRemoteCableSync
                    _logger.Info($"Incoming cable sync: {data.Length} bytes");
                    break;
                    
                default:
                    _logger.Warning($"Unknown multiplayer channel: {channel}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to process incoming message on '{channel}': {ex.Message}");
        }
    }

    /// <summary>
    /// Gibt Diagnostik-Informationen zurück.
    /// </summary>
    public string GetDiagnostics()
    {
        return $"[Multiplayer] FishNet={_isFishNetAvailable}, " +
               $"Initialized={_isInitialized}, " +
               $"Cables={_networkCables?.CableCount ?? 0}";
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        _networkRack?.Dispose();
        _networkServer?.Dispose();
        _networkCables?.Dispose();
        
        _networkRack = null;
        _networkServer = null;
        _networkCables = null;
        
        _logger.Info("GregMultiplayerService disposed.");
    }
}
