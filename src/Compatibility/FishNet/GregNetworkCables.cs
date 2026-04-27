/// <file-summary>
/// Schicht:      Compatibility
/// Zweck:        FishNet NetworkBehaviour-Wrapper für Kabel-Synchronisation.
/// Maintainer:   SyncList für Kabel-Bézier-Punkte. Nutzt CablePositionsPatch.SetBaseId
///               beim Sync, um ID-Kollisionen zu vermeiden.
/// </file-summary>

using System;
using System.Collections.Generic;
using MelonLoader;
using gregCore.Core.Events;
using gregCore.Core.Models;
using gregCore.GameLayer.Patches.Networking;

namespace gregCore.Compatibility.FishNet;

/// <summary>
/// Repräsentiert ein synchronisiertes Kabel mit Start/End-Punkten und Bézier-Kontrollpunkten.
/// </summary>
public record SyncedCableData
{
    public int CableId { get; init; }
    public string StartDeviceId { get; init; } = string.Empty;
    public string EndDeviceId { get; init; } = string.Empty;
    public int StartPort { get; init; }
    public int EndPort { get; init; }
    
    /// <summary>
    /// Bézier-Kontrollpunkte als flat float array [x1,y1,z1, x2,y2,z2, ...].
    /// </summary>
    public float[] BezierPoints { get; init; } = Array.Empty<float>();
}

/// <summary>
/// Netzwerk-Sync-Layer für Kabel-Verbindungen.
/// Hält eine synchronisierte Liste aller aktiven Kabel.
/// </summary>
public sealed class GregNetworkCables : IDisposable
{
    private readonly GregEventBus _eventBus;
    private readonly IGregLogger _logger;
    
    /// <summary>
    /// Synchronized cable list – repliziert den SyncList-Ansatz von FishNet.
    /// </summary>
    private readonly Dictionary<int, SyncedCableData> _cables = new();
    private readonly object _lock = new();
    private bool _disposed;

    public GregNetworkCables(GregEventBus eventBus, IGregLogger logger)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _logger = logger?.ForContext("NetworkCables") ?? throw new ArgumentNullException(nameof(logger));
        
        // Subscribe to cable creation events from CablePositionsPatch
        _eventBus.Subscribe("greg.CABLE.Created", OnCableCreated);
        
        _logger.Success("GregNetworkCables initialized – listening for cable sync events.");
    }

    private void OnCableCreated(EventPayload payload)
    {
        if (_disposed) return;
        try
        {
            if (payload.Data == null) return;
            
            var cableId = payload.Data.TryGetValue("CableId", out var id) ? Convert.ToInt32(id) : 0;
            if (cableId == 0) return;
            
            var cableData = new SyncedCableData { CableId = cableId };
            
            lock (_lock)
            {
                _cables[cableId] = cableData;
            }
            
            // TODO: Broadcast via FishNet ServerRpc
            // FishNetBridge.SendServerRpc("CableCreated", cableData);
            
            _logger.Info($"Cable created and queued for sync: ID={cableId}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to sync cable creation: {ex.Message}");
        }
    }

    /// <summary>
    /// Registriert ein vollständiges Kabel mit Routing-Daten.
    /// </summary>
    public void RegisterCable(SyncedCableData cableData)
    {
        if (_disposed || cableData == null) return;
        
        try
        {
            lock (_lock)
            {
                _cables[cableData.CableId] = cableData;
            }
            
            _eventBus.Publish("greg.NET.CableRegistered", new EventPayload
            {
                HookName = "greg.NET.CableRegistered",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>
                {
                    { "CableId", cableData.CableId },
                    { "StartDevice", cableData.StartDeviceId },
                    { "EndDevice", cableData.EndDeviceId },
                    { "BezierPointCount", cableData.BezierPoints.Length / 3 }
                },
                IsCancelable = false
            });
            
            _logger.Info($"Cable registered: ID={cableData.CableId}, " +
                        $"{cableData.StartDeviceId}:{cableData.StartPort} → " +
                        $"{cableData.EndDeviceId}:{cableData.EndPort}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to register cable: {ex.Message}");
        }
    }

    /// <summary>
    /// Entfernt ein Kabel aus der Sync-Liste.
    /// </summary>
    public void RemoveCable(int cableId)
    {
        if (_disposed) return;
        
        try
        {
            lock (_lock)
            {
                _cables.Remove(cableId);
            }
            
            // TODO: FishNetBridge.SendServerRpc("CableRemoved", cableId);
            
            _logger.Info($"Cable removed from sync: ID={cableId}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to remove cable: {ex.Message}");
        }
    }

    /// <summary>
    /// Verarbeitet eingehende Sync-Daten von anderen Clients.
    /// Aktualisiert auch den CablePositionsPatch ID-Counter.
    /// </summary>
    public void ApplyRemoteCableSync(SyncedCableData cableData)
    {
        if (_disposed || cableData == null) return;
        
        try
        {
            lock (_lock)
            {
                _cables[cableData.CableId] = cableData;
            }
            
            // Stelle sicher, dass der lokale ID-Counter nicht kollidiert
            CablePositionsPatch.SetBaseId(cableData.CableId);
            
            _logger.Info($"Remote cable sync applied: ID={cableData.CableId}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to apply remote cable sync: {ex.Message}");
        }
    }

    /// <summary>
    /// Gibt alle synchronisierten Kabel zurück (für Diagnostik/UI).
    /// </summary>
    public IReadOnlyCollection<SyncedCableData> GetAllCables()
    {
        lock (_lock)
        {
            return new List<SyncedCableData>(_cables.Values);
        }
    }

    public int CableCount
    {
        get
        {
            lock (_lock)
            {
                return _cables.Count;
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        _eventBus.Unsubscribe("greg.CABLE.Created", OnCableCreated);
        
        lock (_lock)
        {
            _cables.Clear();
        }
    }
}
