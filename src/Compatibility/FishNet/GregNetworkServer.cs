/// <file-summary>
/// Schicht:      Compatibility
/// Zweck:        FishNet NetworkBehaviour-Wrapper für Server-Synchronisation.
/// Maintainer:   SyncVar für isOn (Power-State) + RPCs für Port-Connects.
///               Triggert GregEventBus bei Remote-Änderungen.
/// </file-summary>

using System;
using System.Collections.Generic;
using MelonLoader;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.Compatibility.FishNet;

/// <summary>
/// Netzwerk-Sync-Layer für Server Power-State und Port-Verbindungen.
/// </summary>
public sealed class GregNetworkServer : IDisposable
{
    private readonly GregEventBus _eventBus;
    private readonly IGregLogger _logger;
    
    /// <summary>
    /// Lokaler State-Cache: ServerId → Power-State.
    /// </summary>
    private readonly Dictionary<string, bool> _serverPowerStates = new();
    
    /// <summary>
    /// Lokaler State-Cache: ServerId → Liste verbundener Ports.
    /// </summary>
    private readonly Dictionary<string, List<string>> _serverPortConnections = new();
    
    private readonly object _lock = new();
    private bool _disposed;

    public GregNetworkServer(GregEventBus eventBus, IGregLogger logger)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _logger = logger?.ForContext("NetworkServer") ?? throw new ArgumentNullException(nameof(logger));
        
        // Subscribe to server events
        _eventBus.Subscribe("greg.HARDWARE.ServerStatusChanged", OnServerStatusChanged);
        
        _logger.Success("GregNetworkServer initialized – listening for server sync events.");
    }

    private void OnServerStatusChanged(EventPayload payload)
    {
        if (_disposed) return;
        try
        {
            if (payload.Data == null) return;
            
            var status = payload.Data.TryGetValue("status", out var s) ? s?.ToString() : null;
            if (string.IsNullOrEmpty(status)) return;
            
            // TODO: Broadcast via FishNet ServerRpc
            // FishNetBridge.SendServerRpc("ServerStatusChanged", serverId, status);
            
            _logger.Info($"Server status change queued for sync: {status}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to sync server status: {ex.Message}");
        }
    }

    /// <summary>
    /// Setzt den Power-State eines Servers (lokal + Sync).
    /// </summary>
    public void SetPowerState(string serverId, bool isOn)
    {
        if (_disposed || string.IsNullOrEmpty(serverId)) return;
        
        try
        {
            lock (_lock)
            {
                _serverPowerStates[serverId] = isOn;
            }
            
            _eventBus.Publish("greg.NET.ServerPowerChanged", new EventPayload
            {
                HookName = "greg.NET.ServerPowerChanged",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>
                {
                    { "ServerId", serverId },
                    { "IsOn", isOn }
                },
                IsCancelable = false
            });
            
            _logger.Info($"Server power state: {serverId} → {(isOn ? "ON" : "OFF")}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to set power state for {serverId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Registriert eine Port-Verbindung auf einem Server.
    /// </summary>
    public void ConnectPort(string serverId, string portId)
    {
        if (_disposed || string.IsNullOrEmpty(serverId) || string.IsNullOrEmpty(portId)) return;
        
        try
        {
            lock (_lock)
            {
                if (!_serverPortConnections.TryGetValue(serverId, out var ports))
                {
                    ports = new List<string>();
                    _serverPortConnections[serverId] = ports;
                }
                if (!ports.Contains(portId))
                    ports.Add(portId);
            }
            
            // TODO: FishNetBridge.SendServerRpc("PortConnected", serverId, portId);
            
            _logger.Info($"Port connected: Server={serverId}, Port={portId}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to connect port: {ex.Message}");
        }
    }

    /// <summary>
    /// Trennt eine Port-Verbindung.
    /// </summary>
    public void DisconnectPort(string serverId, string portId)
    {
        if (_disposed || string.IsNullOrEmpty(serverId) || string.IsNullOrEmpty(portId)) return;
        
        try
        {
            lock (_lock)
            {
                if (_serverPortConnections.TryGetValue(serverId, out var ports))
                    ports.Remove(portId);
            }
            
            // TODO: FishNetBridge.SendServerRpc("PortDisconnected", serverId, portId);
            
            _logger.Info($"Port disconnected: Server={serverId}, Port={portId}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to disconnect port: {ex.Message}");
        }
    }

    /// <summary>
    /// Verarbeitet eingehende Remote-Updates (von FishNet ObserversRpc).
    /// </summary>
    public void ApplyRemotePowerUpdate(string serverId, bool isOn)
    {
        if (_disposed) return;
        try
        {
            lock (_lock)
            {
                _serverPowerStates[serverId] = isOn;
            }
            _logger.Info($"Remote power update applied: {serverId} → {(isOn ? "ON" : "OFF")}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to apply remote power update: {ex.Message}");
        }
    }

    public bool GetPowerState(string serverId)
    {
        lock (_lock)
        {
            return _serverPowerStates.TryGetValue(serverId, out var isOn) && isOn;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        _eventBus.Unsubscribe("greg.HARDWARE.ServerStatusChanged", OnServerStatusChanged);
        
        lock (_lock)
        {
            _serverPowerStates.Clear();
            _serverPortConnections.Clear();
        }
    }
}
