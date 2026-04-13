using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace gregCoreSDK.Sdk.Services;

/// <summary>
/// Bridge service for interacting with the Computer Shop (Hardware Purchasing).
/// </summary>
public static class GregShopService
{
    private static ComputerShop GetInstance()
    {
        var instance = UnityEngine.Object.FindObjectOfType<ComputerShop>();
        if (instance == null)
            MelonLogger.Warning("[GregShopService] ComputerShop instance not found!");
        return instance;
    }

    public static void CloseShop()
    {
        GetInstance()?.CloseShop();
    }

    public static void DestroyAllSpawnedItems()
    {
        GetInstance()?.DestroyAllSpawnedItems();
    }

    public static void OpenColorPicker()
    {
        GetInstance()?.OpenColorPicker();
    }

    public static void UpdateCartTotal()
    {
        GetInstance()?.UpdateCartTotal();
    }

    public static void FreeUpSpawnPoint(int pointIndex)
    {
        GetInstance()?.FreeUpSpawnPoint(pointIndex);
    }
}
