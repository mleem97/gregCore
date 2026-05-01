using System;
using System.Collections.Generic;

namespace greg.Sdk
{
    public static class gregNativeEventHooks
    {
        // Change to Action<object> to support legacy API callbacks with null parameters
        public static Action<object>? OnCoinsChanged;
        public static Action<object>? OnXpChanged;
        public static Action<object>? OnReputationChanged;
        
        public static Action<object>? SystemGameLoaded;
        public static Action<object>? SystemGameSaved;
        
        public static Action<object>? GameLoaded;
        public static Action<object>? GameSaved;
        public static Action<object>? MoneyChanged;
        public static Action<object>? XpChanged;
        public static Action<object>? ReputationChanged;
        public static Action<object>? DayEnded;
        public static Action<object>? MonthEnded;

        public static Action<object>? GetByEventId(string eventId) => eventId switch
        {
            "OnCoinsChanged" => OnCoinsChanged,
            "OnXpChanged" => OnXpChanged,
            "OnReputationChanged" => OnReputationChanged,
            "system.GameLoaded" => SystemGameLoaded,
            "system.GameSaved" => SystemGameSaved,
            "GameLoaded" => GameLoaded,
            "GameSaved" => GameSaved,
            "MoneyChanged" => MoneyChanged,
            "XpChanged" => XpChanged,
            "ReputationChanged" => ReputationChanged,
            "DayEnded" => DayEnded,
            "MonthEnded" => MonthEnded,
            _ => null
        };
    }
}
