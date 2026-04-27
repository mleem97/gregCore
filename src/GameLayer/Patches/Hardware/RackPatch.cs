/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Prefix-Bypass für Rack.IsPositionAvailable (IL2CPP-Hohlmethode).
/// Maintainer:   Die originale Methode returniert immer false – dieser Patch stellt
///               eine Greg-seitige Registry bereit und dispatcht Events via HookBus.
/// </file-summary>

using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using gregCore.GameLayer.Hooks;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Patches.Hardware;

[HarmonyPatch]
public static class RackPatch
{
    /// <summary>
    /// Greg-seitige Registry für belegte Rack-Positionen.
    /// Key = Rack-Instance-HashCode, Value = Set der belegten Positionen.
    /// </summary>
    private static readonly Dictionary<int, HashSet<int>> _usedPositions = new();
    private static readonly object _lock = new();

    [HarmonyPatch(typeof(global::Il2Cpp.Rack), nameof(global::Il2Cpp.Rack.IsPositionAvailable))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool IsPositionAvailablePrefix(
        global::Il2Cpp.Rack __instance,
        int positionIndex,
        ref bool __result)
    {
        try
        {
            if (__instance == null)
            {
                __result = false;
                return false;
            }

            int rackId = __instance.GetHashCode();

            lock (_lock)
            {
                if (!_usedPositions.TryGetValue(rackId, out var used))
                {
                    used = new HashSet<int>();
                    _usedPositions[rackId] = used;
                }

                __result = !used.Contains(positionIndex);
            }

            HookIntegration.Emit("greg.RACK.PositionQueried",
                new EventPayload
                {
                    HookName = "greg.RACK.PositionQueried",
                    OccurredAtUtc = DateTime.UtcNow,
                    Data = new Dictionary<string, object>
                    {
                        { "RackId", rackId },
                        { "Position", positionIndex },
                        { "Available", __result }
                    }
                });

            return false; // Skip original (it's hollow – always returns false)
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[RackPatch] IsPositionAvailable failed: {ex.Message}");
            __result = true; // Fail-open: allow placement on error
            return false;
        }
    }

    /// <summary>
    /// Registriert eine Position als belegt.
    /// Wird von Placement-Logik und Multiplayer-Sync aufgerufen.
    /// </summary>
    public static void MarkPositionUsed(int rackHash, int position)
    {
        lock (_lock)
        {
            if (!_usedPositions.TryGetValue(rackHash, out var used))
            {
                used = new HashSet<int>();
                _usedPositions[rackHash] = used;
            }
            used.Add(position);
        }

        HookIntegration.Emit("greg.RACK.PositionMarkedUsed",
            new EventPayload
            {
                HookName = "greg.RACK.PositionMarkedUsed",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>
                {
                    { "RackId", rackHash },
                    { "Position", position }
                }
            });
    }

    /// <summary>
    /// Gibt eine Position wieder frei.
    /// </summary>
    public static void MarkPositionFree(int rackHash, int position)
    {
        lock (_lock)
        {
            if (_usedPositions.TryGetValue(rackHash, out var used))
                used.Remove(position);
        }

        HookIntegration.Emit("greg.RACK.PositionMarkedFree",
            new EventPayload
            {
                HookName = "greg.RACK.PositionMarkedFree",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>
                {
                    { "RackId", rackHash },
                    { "Position", position }
                }
            });
    }

    /// <summary>
    /// Entfernt alle Position-Daten für ein Rack (z.B. beim Entfernen).
    /// </summary>
    public static void ClearRack(int rackHash)
    {
        lock (_lock)
        {
            _usedPositions.Remove(rackHash);
        }
    }

    /// <summary>
    /// Gibt die Anzahl der belegten Positionen für ein Rack zurück.
    /// </summary>
    public static int GetUsedCount(int rackHash)
    {
        lock (_lock)
        {
            return _usedPositions.TryGetValue(rackHash, out var used) ? used.Count : 0;
        }
    }
}
