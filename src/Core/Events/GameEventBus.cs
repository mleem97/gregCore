using System;

namespace gregCore.Core.Events
{
    /// <summary>
    /// Managed Event Bus for UI Data Binding.
    /// Prevents IL2CPP trampoline issues by keeping data flow in managed code.
    /// </summary>
    public static class GameEventBus
    {
        public static event Action<float>? OnCoinsChanged;
        public static event Action<float>? OnReputationChanged;
        public static event Action<float>? OnXpChanged;

        public static void PublishCoins(float amount) => OnCoinsChanged?.Invoke(amount);
        public static void PublishReputation(float amount) => OnReputationChanged?.Invoke(amount);
        public static void PublishXp(float amount) => OnXpChanged?.Invoke(amount);
    }
}
