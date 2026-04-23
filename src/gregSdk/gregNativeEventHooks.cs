using System;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk
{
    /// <summary>
    /// Legacy-Schnittstelle für ältere Mods (z.B. CableThrottle).
    /// Diese Klasse MUSS exakt diese statischen Felder bereitstellen, um MissingFieldExceptions zu vermeiden.
    /// </summary>
    public static class gregNativeEventHooks
    {
        // Legacy support for older mods expecting static actions.
        public static Action? SystemGameLoaded;
        public static Action? SystemGameSaved;
        public static Action<float>? PlayerCoinChanged;
        public static Action<float>? PlayerReputationChanged;
        public static Action<float>? PlayerXpChanged;
        public static Action<int>? DayEnded;
        public static Action<int>? MonthEnded;
        public static Action<object>? CustomerAccepted;
        public static Action<object>? ServerInstalled;
        public static Action<object>? ServerBroken;
        public static Action<object>? ServerRepaired;
        public static Action<float>? ShopCheckout;

        static gregNativeEventHooks()
        {
            SystemGameLoaded = delegate { };
            SystemGameSaved = delegate { };
            PlayerCoinChanged = delegate { };
            PlayerReputationChanged = delegate { };
            PlayerXpChanged = delegate { };
            DayEnded = delegate { };
            MonthEnded = delegate { };
            CustomerAccepted = delegate { };
            ServerInstalled = delegate { };
            ServerBroken = delegate { };
            ServerRepaired = delegate { };
            ShopCheckout = delegate { };
        }

        public static class ByEventId
        {
            public static void MoneyChanged(float newAmount) => gregNativeEventHooks.PlayerCoinChanged?.Invoke(newAmount);
            public static void XpChanged(float newXp) => gregNativeEventHooks.PlayerXpChanged?.Invoke(newXp);
            public static void ReputationChanged(float newRep) => gregNativeEventHooks.PlayerReputationChanged?.Invoke(newRep);
            public static void GameSaved() => gregNativeEventHooks.SystemGameSaved?.Invoke();
            public static void GameLoaded() => gregNativeEventHooks.SystemGameLoaded?.Invoke();
            public static void ShopCheckoutTriggered(float amount) => gregNativeEventHooks.ShopCheckout?.Invoke(amount);
            public static void DayEnded(int day) => gregNativeEventHooks.DayEnded?.Invoke(day);
            public static void MonthEnded(int month) => gregNativeEventHooks.MonthEnded?.Invoke(month);
        }

        // --- Hilfsmethoden für Legacy-Mods ---
        public static float GetPlayerMoney() => 0f;
        public static int GetTimeOfDay() => (int)(Il2Cpp.TimeController.instance?.currentTimeOfDay ?? 0f);
        public static int GetDay() => 1;
        public static Transform? GetPlayerCamera() => Camera.main?.transform;
    }
}
