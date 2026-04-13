using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Bridge service for interacting with the game's internal time and date simulation.
/// </summary>
public static class GregTimeService
{
    private static TimeController GetInstance()
    {
        var instance = UnityEngine.Object.FindObjectOfType<TimeController>();
        if (instance == null)
            MelonLogger.Warning("[GregTimeService] TimeController instance not found!");
        return instance;
    }

    public static float GetCurrentTimeInHours()
    {
        return GetInstance()?.CurrentTimeInHours() ?? 0f;
    }

    public static bool IsTimeBetween(float startHour, float endHour)
    {
        return GetInstance()?.TimeIsBetween(startHour, endHour) ?? false;
    }

    public static int GetHoursFromDate(float time, int day)
    {
        return GetInstance()?.HoursFromDate(time, day) ?? 0;
    }

    public static int GetCurrentDay()
    {
        return GetInstance()?.day ?? 0;
    }
}
