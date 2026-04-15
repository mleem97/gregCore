using System.Collections.Generic;
using UnityEngine;

namespace greg.Sdk.Services
{
    public enum OrbRenderDensity
    {
        All = 0,
        SeventyFive = 1,
        Fifty = 2,
        TwentyFive = 3,
        Off = 4
    }

    /// <summary>
    /// Service to manage data orb rendering density and per-customer toggles.
    /// Addresses P0 performance issue B-49.
    /// </summary>
    public static class GregOrbRenderService
    {
        public static OrbRenderDensity Density { get; set; } = OrbRenderDensity.All;
        private static readonly Dictionary<int, bool> _customerToggles = new();
        private static int _spawnCounter = 0;

        public static void SetCustomerToggle(int customerId, bool enabled)
        {
            _customerToggles[customerId] = enabled;
        }

        public static bool IsCustomerEnabled(int customerId)
        {
            if (_customerToggles.TryGetValue(customerId, out bool enabled))
                return enabled;
            return true; // Default to enabled
        }

        public static bool ShouldSpawn(int customerId)
        {
            if (Density == OrbRenderDensity.Off) return false;
            
            if (!IsCustomerEnabled(customerId))
                return false;

            _spawnCounter++;
            return Density switch
            {
                OrbRenderDensity.SeventyFive => _spawnCounter % 4 != 0,
                OrbRenderDensity.Fifty => _spawnCounter % 2 == 0,
                OrbRenderDensity.TwentyFive => _spawnCounter % 4 == 0,
                _ => true
            };
        }
    }
}
