/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Prefix-Bypass für CablePositions.CreateNewCable (IL2CPP-Hohlmethode).
/// Maintainer:   Die originale Methode returniert immer 0, was zu ID-Kollisionen führt.
///               Dieser Patch generiert thread-safe unique IDs via Atomaren Counter.
///               Defensive: null-checks + Pointer validation.
/// </file-summary>

using System;
using System.Threading;
using HarmonyLib;
using MelonLoader;
using gregCore.GameLayer.Hooks;

namespace gregCore.GameLayer.Patches.Networking;

public static class CablePositionsPatch
{
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
            if (__instance == null || __instance.Pointer == IntPtr.Zero)
            {
                __result = 0;
                return false;
            }

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

            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CablePatch] CreateNewCable failed: {ex.Message}");
            __result = Environment.TickCount & 0x7FFFFFFF;
            return false;
        }
    }

    public static void SetBaseId(int baseId)
    {
        int current;
        do
        {
            current = _nextCableId;
            if (baseId < current) return;
        }
        while (Interlocked.CompareExchange(ref _nextCableId, baseId + 1, current) != current);

        MelonLogger.Msg($"[CablePatch] Cable ID counter set to {baseId + 1}");
    }

    public static int PeekNextId() => _nextCableId;
}
