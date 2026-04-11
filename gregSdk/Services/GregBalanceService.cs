using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace gregSdk.Services;

/// <summary>
/// Bridge service for the game's internal Balance Sheet and financial simulation.
/// </summary>
public static class GregBalanceService
{
    private static BalanceSheet GetInstance()
    {
        // Internal classes often use a static 'instance' field
        return BalanceSheet.instance;
    }

    public static void RegisterSalary(int monthlySalary)
    {
        GetInstance()?.RegisterSalary(monthlySalary);
    }

    public static int GetTotalMonthlySalary()
    {
        var instance = GetInstance();
        return instance != null ? (int)instance.totalMonthlySalary : 0;
    }

    public static float GetCurrentSalaryExpense()
    {
        var instance = GetInstance();
        return instance != null ? instance.currentSalaryExpense : 0f;
    }
}
