using UnityEngine;

namespace greg.Sdk.Services
{
    /// <summary>
    /// Service for throttling expensive update loops and profiling.
    /// Addresses P1 performance issue B-52.
    /// </summary>
    public static class GregPerformanceProfilerService
    {
        /// <summary>
        /// Returns true if the current frame is an update interval for the given key.
        /// </summary>
        public static bool ShouldUpdate(int interval)
        {
            if (interval <= 1) return true;
            return Time.frameCount % interval == 0;
        }

        /// <summary>
        /// Returns true if the current frame is an update interval for a specific key/tag.
        /// This allows staggered updates across different systems.
        /// </summary>
        public static bool ShouldUpdateStaggered(string key, int interval)
        {
            if (interval <= 1) return true;
            int hash = key.GetHashCode();
            return (Time.frameCount + (hash % interval)) % interval == 0;
        }
    }
}
