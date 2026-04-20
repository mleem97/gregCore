using System;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk;

public static class gregNativeEventHooks
{
    // Legacy support for older mods expecting static actions - MUST be nullable fields to match older binaries exactly
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

    public static class ByEventId
    {
        public static void MoneyChanged(float newAmount) => PlayerCoinChanged?.Invoke(newAmount);
        public static void XpChanged(float newXp) => PlayerXpChanged?.Invoke(newXp);
        public static void ReputationChanged(float newRep) => PlayerReputationChanged?.Invoke(newRep);

        public static void ServerPowered(object server) { }
        public static void ServerBroken(object server) => gregNativeEventHooks.ServerBroken?.Invoke(server);
        public static void ServerRepaired(object server) => gregNativeEventHooks.ServerRepaired?.Invoke(server);
        public static void ServerInstalled(object server) => gregNativeEventHooks.ServerInstalled?.Invoke(server);
        public static void CableConnected(object cable) { }
        public static void CableDisconnected(object cable) { }
        public static void ServerCustomerChanged(object server, int customers) { }
        public static void ServerAppChanged(object server, int appId) { }
        public static void DayEnded(int day) => gregNativeEventHooks.DayEnded?.Invoke(day);
        public static void MonthEnded(int month) => gregNativeEventHooks.MonthEnded?.Invoke(month);
        public static void CustomerAccepted(object customer) => gregNativeEventHooks.CustomerAccepted?.Invoke(customer);
        public static void CustomerSatisfied(object customer) { }
        public static void CustomerUnsatisfied(object customer) { }
        public static void ShopCheckout(float total) => gregNativeEventHooks.ShopCheckout?.Invoke(total);
        public static void ShopItemAdded(int itemId) { }
        public static void ShopCartCleared() { }
        public static void EmployeeHired(object tech) { }
        public static void EmployeeFired(object tech) { }
        public static void GameSaved() => gregNativeEventHooks.SystemGameSaved?.Invoke();
        public static void GameLoaded() => gregNativeEventHooks.SystemGameLoaded?.Invoke();
        public static void GameAutoSaved() { }
    }

    public static class ByName
    {
        public static float GetPlayerMoney() => 0f;
        public static float GetPlayerXp() => 0f;
        public static float GetPlayerReputation() => 0f;
        public static int GetTimeOfDay() => (int)(Il2Cpp.TimeController.instance?.currentTimeOfDay ?? 0f);
        public static int GetDay() => 1;
        public static Transform? GetPlayerCamera() => Camera.main?.transform;
    }
}