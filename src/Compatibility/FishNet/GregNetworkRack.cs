/// <file-summary>
/// Schicht:      Compatibility
/// Zweck:        FishNet NetworkBehaviour-Wrapper für Rack-Synchronisation.
/// Maintainer:   SyncVar für isPositionUsed-Array. Triggert GregEventBus bei Sync-Änderungen.
///               Wird nur geladen wenn FishNet als Assembly verfügbar ist.
/// </file-summary>

using System;
using System.Collections.Generic;
using MelonLoader;
using gregCore.Core.Events;
using gregCore.Core.Models;
using gregCore.GameLayer.Patches.Hardware;

namespace gregCore.Compatibility.FishNet;

/// <summary>
/// Netzwerk-Sync-Layer für Rack-Positionen.
/// 
/// Da FishNet als optionale Dependency geladen wird und wir in einer IL2CPP-Umgebung
/// ohne direkte NetworkBehaviour-Vererbung arbeiten, implementieren wir die Sync-Logik
/// als managed Wrapper, der FishNet-RPCs über Reflection aufruft.
/// </summary>
public sealed class GregNetworkRack : IDisposable
{
    private readonly GregEventBus _eventBus;
    private readonly IGregLogger _logger;
    
    /// <summary>
    /// Lokaler State-Cache: RackId → Set von belegten Positionen.
    /// Wird über RPCs zwischen Clients synchronisiert.
    /// </summary>
    private readonly Dictionary<int, HashSet<int>> _syncedPositions = new();
    private readonly object _lock = new();
    private bool _disposed;

    public GregNetworkRack(GregEventBus eventBus, IGregLogger logger)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _logger = logger?.ForContext("NetworkRack") ?? throw new ArgumentNullException(nameof(logger));
        
        // Subscribe to local rack events to broadcast
        _eventBus.Subscribe("greg.RACK.PositionMarkedUsed", OnLocalPositionChanged);
        _eventBus.Subscribe("greg.RACK.PositionMarkedFree", OnLocalPositionFreed);
        
        _logger.Success("GregNetworkRack initialized – listening for rack sync events.");
    }

    /// <summary>
    /// Wird aufgerufen wenn lokal eine Position belegt wird → Broadcast an andere Clients.
    /// </summary>
    private void OnLocalPositionChanged(EventPayload payload)
    {
        if (_disposed) return;
        try
        {
            if (payload.Data == null) return;
            
            var rackId = payload.Data.TryGetValue("RackId", out var r) ? Convert.ToInt32(r) : 0;
            var position = payload.Data.TryGetValue("Position", out var p) ? Convert.ToInt32(p) : -1;
            
            if (rackId == 0 || position < 0) return;
            
            // Update local sync cache
            lock (_lock)
            {
                if (!_syncedPositions.TryGetValue(rackId, out var positions))
                {
                    positions = new HashSet<int>();
                    _syncedPositions[rackId] = positions;
                }
                positions.Add(position);
            }
            
            // TODO: When FishNet is loaded, broadcast via ServerRpc
            // FishNetBridge.SendServerRpc("RackPositionUsed", rackId, position);
            
            _logger.Info($"Rack position synced: Rack={rackId}, Pos={position}, State=Used");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to sync rack position: {ex.Message}");
        }
    }

    /// <summary>
    /// Wird aufgerufen wenn lokal eine Position freigegeben wird.
    /// </summary>
    private void OnLocalPositionFreed(EventPayload payload)
    {
        if (_disposed) return;
        try
        {
            if (payload.Data == null) return;
            
            var rackId = payload.Data.TryGetValue("RackId", out var r) ? Convert.ToInt32(r) : 0;
            var position = payload.Data.TryGetValue("Position", out var p) ? Convert.ToInt32(p) : -1;
            
            if (rackId == 0 || position < 0) return;
            
            lock (_lock)
            {
                if (_syncedPositions.TryGetValue(rackId, out var positions))
                    positions.Remove(position);
            }
            
            // TODO: FishNetBridge.SendServerRpc("RackPositionFreed", rackId, position);
            
            _logger.Info($"Rack position synced: Rack={rackId}, Pos={position}, State=Free");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to sync rack position free: {ex.Message}");
        }
    }

    /// <summary>
    /// Verarbeitet eingehende Sync-Daten von anderen Clients.
    /// Aufgerufen über den FishNet ObserversRpc-Callback.
    /// </summary>
    public void ApplyRemotePositionUpdate(int rackId, int position, bool isUsed)
    {
        if (_disposed) return;
        try
        {
            if (isUsed)
            {
                RackPatch.MarkPositionUsed(rackId, position);
            }
            else
            {
                RackPatch.MarkPositionFree(rackId, position);
            }
            
            _logger.Info($"Remote rack update applied: Rack={rackId}, Pos={position}, Used={isUsed}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to apply remote rack update: {ex.Message}");
        }
    }

    /// <summary>
    /// Gibt den aktuellen Sync-State für ein Rack zurück (für Diagnostik).
    /// </summary>
    public HashSet<int> GetSyncedPositions(int rackId)
    {
        lock (_lock)
        {
            return _syncedPositions.TryGetValue(rackId, out var positions)
                ? new HashSet<int>(positions)
                : new HashSet<int>();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        _eventBus.Unsubscribe("greg.RACK.PositionMarkedUsed", OnLocalPositionChanged);
        _eventBus.Unsubscribe("greg.RACK.PositionMarkedFree", OnLocalPositionFreed);
        
        lock (_lock)
        {
            _syncedPositions.Clear();
        }
    }
}
