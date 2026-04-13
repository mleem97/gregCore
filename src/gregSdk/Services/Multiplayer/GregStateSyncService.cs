using System;
using System.Collections.Generic;
using MelonLoader;

namespace greg.Sdk.Services.Multiplayer;

/// <summary>
/// Core SDK service for synchronizing mod state across connected players.
/// Handles: Rack blueprints, IPAM configs, Greg task assignments.
/// Extracted from FFM.Plugin.Multiplayer (gregMod.Multiplayer).
/// Currently stub — ready for network integration.
/// </summary>
public static class GregStateSyncService
{
    private static float _lastSyncTime;
    private const float SYNC_INTERVAL = 5.0f;

    private static readonly Dictionary<string, string> _syncedState = new();

    public static void Tick(float currentTime)
    {
        if (!GregMultiplayerService.IsConnected) return;

        if (currentTime - _lastSyncTime < SYNC_INTERVAL) return;
        _lastSyncTime = currentTime;

        // STUB: In production, scan for state changes and broadcast deltas via GregMultiplayerService.Broadcast()
    }

    /// <summary>Sync a rack blueprint to all connected players.</summary>
    public static void SyncRackBlueprint(string blueprintId, string blueprintJson)
    {
        _syncedState[$"rack.{blueprintId}"] = blueprintJson;
        MelonLogger.Msg($"[GregStateSyncService] Synced rack blueprint: {blueprintId}");
        GregMultiplayerService.Broadcast("SyncRack", blueprintJson);
    }

    /// <summary>Sync IPAM subnet config.</summary>
    public static void SyncSubnetConfig(string subnetId, string subnetJson)
    {
        _syncedState[$"ipam.{subnetId}"] = subnetJson;
        MelonLogger.Msg($"[GregStateSyncService] Synced IPAM subnet: {subnetId}");
        GregMultiplayerService.Broadcast("SyncSubnet", subnetJson);
    }

    /// <summary>Sync Greg task assignment.</summary>
    public static void SyncGregTask(string gregId, string taskJson)
    {
        _syncedState[$"greg.{gregId}"] = taskJson;
        GregMultiplayerService.Broadcast("SyncGregTask", taskJson);
    }

    public static IReadOnlyDictionary<string, string> GetSyncedState() => _syncedState;
}
