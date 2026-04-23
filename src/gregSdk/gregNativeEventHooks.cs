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
        public static Action SystemGameLoaded = delegate { };
        public static Action SystemGameSaved = delegate { };
        public static Action<float> PlayerCoinChanged = _ => { };
        public static Action<float> PlayerReputationChanged = _ => { };
        public static Action<float> PlayerXpChanged = _ => { };
        public static Action<int> DayEnded = _ => { };
        public static Action<int> MonthEnded = _ => { };
        public static Action<object> CustomerAccepted = _ => { };
        public static Action<object> ServerInstalled = _ => { };
        public static Action<object> ServerBroken = _ => { };
        public static Action<object> ServerRepaired = _ => { };
        public static Action<float> ShopCheckout = _ => { };

        static gregNativeEventHooks()
        {
            // Initialized above for field-level safety
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
        public static float GetPlayerMoney() => (float)(Il2Cpp.SaveData.instance?.playerData?.coins ?? 0f);
        public static int GetTimeOfDay() => (int)(Il2Cpp.TimeController.instance?.currentTimeOfDay ?? 0f);
        public static int GetDay() => 1; // Todo: Find real day field if needed
        public static Transform? GetPlayerCamera() => Camera.main?.transform;
    }
}
