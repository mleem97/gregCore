using System;
using System.Collections.Generic;

namespace frameworkSdk
{
    public static class GregFeatureGuard
    {
        private static readonly HashSet<string> _disabledFeatures = new();
        public static bool IsVanillaSave { get; private set; }

        public static event Action<string>? OnFeatureStateChanged;

        public static void SetVanillaSaveStatus(bool isVanilla)
        {
            if (IsVanillaSave != isVanilla)
            {
                IsVanillaSave = isVanilla;
                if (isVanilla)
                {
                    DisableFeature("GridPlacement");
                    DisableFeature("SaveEngine.Write");
                }
            }
        }

        public static void DisableFeature(string featureKey)
        {
            if (_disabledFeatures.Add(featureKey))
            {
                OnFeatureStateChanged?.Invoke(featureKey);
            }
        }

        public static void EnableFeature(string featureKey)
        {
            if (_disabledFeatures.Remove(featureKey))
            {
                OnFeatureStateChanged?.Invoke(featureKey);
            }
        }

        public static bool IsEnabled(string featureKey)
        {
            if (IsVanillaSave && (featureKey == "GridPlacement" || featureKey == "SaveEngine.Write"))
            {
                return false;
            }
            return !_disabledFeatures.Contains(featureKey);
        }
    }
}
