using System;
using System.Collections.Generic;

namespace greg.Sdk.Services
{
    /// <summary>
    /// Service for optimizing route evaluation.
    /// Addresses P1 performance issue B-46.
    /// </summary>
    public static class GregRouteEvaluationService
    {
        private static readonly Dictionary<string, List<List<string>>> _routeCache = new();
        
        /// <summary>
        /// Attempts to get a cached route (Pointer-Swap optimization).
        /// </summary>
        public static bool TryGetCachedRoute(string baseName, string serverName, out List<List<string>> routes)
        {
            string key = $"{baseName}->{serverName}";
            return _routeCache.TryGetValue(key, out routes);
        }

        public static void CacheRoute(string baseName, string serverName, List<List<string>> routes)
        {
            string key = $"{baseName}->{serverName}";
            _routeCache[key] = routes;
            
            // Dispatch event for UI/Metrics
            // GregHookBus.Emit("greg.SERVER.RoutePointerSwapped", new { baseName, serverName });
        }

        public static void InvalidateCache(string deviceId = null)
        {
            if (deviceId == null)
            {
                _routeCache.Clear();
            }
            else
            {
                // Simple implementation: clear all if any device changes. 
                // A more complex version would map devices to routes.
                _routeCache.Clear();
            }
        }
    }
}
