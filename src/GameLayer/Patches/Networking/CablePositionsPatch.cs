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

            try { EmitCreate(__result); } catch { }

            return false;
        }
        catch (Exception ex)
        {
            try { LogError(ex); } catch { }
            __result = Environment.TickCount & 0x7FFFFFFF;
            return false;
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void EmitCreate(int id)
    {
        HookIntegration.Emit("greg.CABLE.Created",
            new gregCore.Core.Models.EventPayload
            {
                HookName = "greg.CABLE.Created",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "CableId", id },
                    { "Source", "PrefixBypass" }
                }
            });
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void LogError(Exception ex)
    {
        MelonLogger.Error($"[CablePatch] CreateNewCable failed: {ex.Message}");
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

        try { LogBaseIdSync(baseId); } catch { }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void LogBaseIdSync(int baseId)
    {
        MelonLogger.Msg($"[CablePatch] Cable ID counter set to {baseId + 1}");
    }

    public static int PeekNextId() => _nextCableId;


}
