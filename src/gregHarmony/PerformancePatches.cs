using System;
using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using greg.Sdk.Services;
using Il2Cpp;

namespace greg.Harmony
{
    /// <summary>
    /// Harmony patches for performance optimizations.
    /// Implements SafePatch pattern as per GregCore standards.
    /// </summary>
    
    [HarmonyPatch(typeof(PacketSpawnerSystem), "SpawnPacket")]
    public static class Patch_PacketSpawnerSystem_SpawnPacket
    {
        static bool Prefix(PacketSpawnerSystem __instance)
        {
            try
            {
                // P0: Data Orb density control (B-49)
                // For now, use a default customer ID or fetch from context if available.
                // In a real implementation, we'd map spawnerEntity to a customer ID.
                const int defaultCustomerId = 0;
                
                if (!GregOrbRenderService.ShouldSpawn(defaultCustomerId))
                {
                    return false; // Skip original SpawnPacket
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[SafePatch] PacketSpawnerSystem.SpawnPacket: {ex.Message}");
                return true; // Always return true to avoid breaking core game logic
            }
        }
    }

    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.FindAllRoutes), new Type[] { typeof(string), typeof(string) })]
    public static class Patch_NetworkMap_FindAllRoutes
    {
        static bool Prefix(NetworkMap __instance, string baseName, string serverName, ref Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<string>> __result)
        {
            try
            {
                // P1: Route evaluation optimization (B-46) - Pointer-Swap Strategy
                if (GregRouteEvaluationService.TryGetCachedRoute(baseName, serverName, out var cachedRoutes))
                {
                    __result = cachedRoutes;
                    return false; // Skip original expensive FindAllRoutes
                }

                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[SafePatch] NetworkMap.FindAllRoutes Prefix: {ex.Message}");
                return true;
            }
        }

        static void Postfix(string baseName, string serverName, Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<string>> __result)
        {
            try
            {
                if (__result != null)
                {
                    GregRouteEvaluationService.CacheRoute(baseName, serverName, __result);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[SafePatch] NetworkMap.FindAllRoutes Postfix: {ex.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveDevice))]
    public static class Patch_NetworkMap_InvalidateCache
    {
        static void Postfix(string name)
        {
            try
            {
                // Invalidate route cache when network topology changes
                GregRouteEvaluationService.InvalidateCache(name);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[SafePatch] NetworkMap.RemoveDevice: {ex.Message}");
            }
        }
    }
}
