using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace gregSdk.Services;

/// <summary>
/// Bridge service for interacting with in-game Server objects.
/// </summary>
public static class GregServerInteractionService
{
    private static Server GetServerInstance(string serverName)
    {
        var go = GameObject.Find(serverName);
        if (go == null) return null;
        return go.GetComponent<Server>();
    }

    public static void PressPowerButton(string serverName, bool forceState = false)
    {
        GetServerInstance(serverName)?.PowerButton(forceState);
    }

    public static bool IsAnyCableConnected(string serverName)
    {
        return GetServerInstance(serverName)?.IsAnyCableConnected() ?? false;
    }

    public static void ClearWarningSign(string serverName, bool isPreserved = false)
    {
        GetServerInstance(serverName)?.ClearWarningSign(isPreserved);
    }

    public static void ClearErrorSign(string serverName)
    {
        GetServerInstance(serverName)?.ClearErrorSign();
    }

    public static void RepairDevice(string serverName)
    {
        GetServerInstance(serverName)?.RepairDevice();
    }

    public static void SetIP(string serverName, string ipAddress)
    {
        GetServerInstance(serverName)?.SetIP(ipAddress);
    }

    public static void UpdateCustomer(string serverName, int customerId)
    {
        GetServerInstance(serverName)?.UpdateCustomer(customerId);
    }

    public static void UpdateAppID(string serverName, int appId)
    {
        GetServerInstance(serverName)?.UpdateAppID(appId);
    }
}
