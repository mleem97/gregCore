/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Prefix-Bypass für Rack.IsPositionAvailable (IL2CPP-Hohlmethode).
/// Maintainer:   Die originale Methode returniert immer false – dieser Patch stellt
///               eine Greg-seitige Registry bereit und dispatcht Events via HookBus.
///               Defensive: null-checks + Pointer validation.
/// </file-summary>

using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using gregCore.GameLayer.Hooks;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Patches.Hardware;

public static class RackPatch
{
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
            if (__instance == null || __instance.Pointer == IntPtr.Zero)
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

            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[RackPatch] IsPositionAvailable failed: {ex.Message}");
            __result = true;
            return false;
        }
    }

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

    public static void ClearRack(int rackHash)
    {
        lock (_lock)
        {
            _usedPositions.Remove(rackHash);
        }
    }

    public static int GetUsedCount(int rackHash)
    {
        lock (_lock)
        {
            return _usedPositions.TryGetValue(rackHash, out var used) ? used.Count : 0;
        }
    }
}
