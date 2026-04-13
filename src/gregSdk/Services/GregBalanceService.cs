using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Bridge service for the game's economic simulation and balance sheet.
/// </summary>
public static class GregBalanceService
{
    private static BalanceSheet GetInstance()
    {
        return BalanceSheet.instance;
    }

    public static float GetTotalMonthlySalary()
    {
        return GetInstance()?.totalMonthlySalary ?? 0f;
    }

    public static float GetCurrentSalaryExpense()
    {
        return GetInstance()?.currentSalaryExpense ?? 0f;
    }

    public static (float revenue, float penalties, float grandTotal) GetLatestSnapshot()
    {
        var snapshot = GetInstance()?.GetLatestSnapshot();
        if (snapshot == null) return (0f, 0f, 0f);

        return (snapshot.TotalRevenue, snapshot.TotalPenalties, snapshot.GrandTotal);
    }

    public static void RegisterManualSalary(int amount)
    {
        GetInstance()?.RegisterSalary(amount);
    }

    public static float GetMoney()
    {
        return PlayerManager.instance?.playerClass?.money ?? 0f;
    }

    public static void AddMoney(float amount)
    {
        var player = PlayerManager.instance?.playerClass;
        if (player != null)
        {
            player.money += amount;
            gregEventDispatcher.Emit(gregNativeEventHooks.PlayerCoinChanged, new { Amount = amount, NewTotal = player.money });
        }
    }

    public static float GetTotalRevenue()
    {
        return GetLatestSnapshot().revenue;
    }

    public static float GetTotalPenalties()
    {
        return GetLatestSnapshot().penalties;
    }
}

