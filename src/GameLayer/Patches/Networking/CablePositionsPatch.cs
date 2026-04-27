/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Prefix-Bypass für CablePositions.CreateNewCable (IL2CPP-Hohlmethode).
/// Maintainer:   Die originale Methode returniert immer 0, was zu ID-Kollisionen führt.
///               Dieser Patch generiert thread-safe unique IDs via Atomaren Counter.
/// </file-summary>

using System;
using System.Threading;
using HarmonyLib;
using MelonLoader;
using gregCore.GameLayer.Hooks;

namespace gregCore.GameLayer.Patches.Networking;

[HarmonyPatch]
public static class CablePositionsPatch
{
    /// <summary>
    /// Atomarer ID-Counter für thread-sichere Cable-ID-Generierung.
    /// Startet bei 1 (0 ist der IL2CPP-Default und signalisiert "ungültig").
    /// </summary>
    private static int _nextCableId = 1;

    [HarmonyPatch(typeof(global::Il2Cpp.CablePositions), nameof(global::Il2Cpp.CablePositions.CreateNewCable))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool CreateNewCablePrefix(
        global::Il2Cpp.CablePositions __instance,
        ref int __result)
    {
        try
        {
            if (__instance == null)
            {
                __result = 0;
                return false;
            }

            // Thread-safe ID-Generierung via Interlocked
            __result = Interlocked.Increment(ref _nextCableId);

            HookIntegration.Emit("greg.CABLE.Created",
                new gregCore.Core.Models.EventPayload
                {
                    HookName = "greg.CABLE.Created",
                    OccurredAtUtc = DateTime.UtcNow,
                    Data = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "CableId", __result },
                        { "Source", "PrefixBypass" }
                    }
                });

            return false; // Skip hollow original (always returns 0)
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CablePatch] CreateNewCable failed: {ex.Message}");
            // Fallback: Generiere ID basierend auf Timestamp
            __result = Environment.TickCount & 0x7FFFFFFF;
            return false;
        }
    }

    /// <summary>
    /// Setzt den ID-Counter auf den höchsten bekannten Wert.
    /// Aufzurufen beim Laden eines Spielstands, um Kollisionen zu vermeiden.
    /// </summary>
    public static void SetBaseId(int baseId)
    {
        int current;
        do
        {
            current = _nextCableId;
            if (baseId < current) return; // Bereits höher
        }
        while (Interlocked.CompareExchange(ref _nextCableId, baseId + 1, current) != current);

        MelonLogger.Msg($"[CablePatch] Cable ID counter set to {baseId + 1}");
    }

    /// <summary>
    /// Gibt die nächste ID zurück ohne sie zu verbrauchen (für Diagnostik).
    /// </summary>
    public static int PeekNextId() => _nextCableId;
}
