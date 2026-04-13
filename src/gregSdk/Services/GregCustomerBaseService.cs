using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Bridge service for interacting with Customer Bases and App requirements.
/// </summary>
public static class GregCustomerBaseService
{
    private static CustomerBase GetInstance(string objectName)
    {
        var go = GameObject.Find(objectName);
        if (go == null) return null;
        return go.GetComponent<CustomerBase>();
    }

    public static void UpdateCustomerServerCountAndSpeed(string customerBaseName, int serverCount, float speed)
    {
        var instance = GetInstance(customerBaseName);
        instance?.UpdateCustomerServerCountAndSpeed(serverCount, speed);
    }

    public static void AddAppPerformance(string customerBaseName, int appId, float performance)
    {
        var instance = GetInstance(customerBaseName);
        instance?.AddAppPerformance(appId, performance);
    }

    public static void ResetAllAppSpeeds(string customerBaseName)
    {
        var instance = GetInstance(customerBaseName);
        instance?.ResetAllAppSpeeds();
    }

    public static bool IsIPPresent(string customerBaseName, string ipAddress)
    {
        var instance = GetInstance(customerBaseName);
        return instance != null && instance.IsIPPresent(ipAddress);
    }

    public static int GetAppIDForIP(string customerBaseName, string ipAddress)
    {
        var instance = GetInstance(customerBaseName);
        if (instance != null)
            return instance.GetAppIDForIP(ipAddress);
        return -1;
    }
}

