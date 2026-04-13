using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Bridge service for controlling the MainGameManager and general game states.
/// </summary>
public static class GregGameManagerService
{
    private static MainGameManager GetInstance()
    {
        var instance = UnityEngine.Object.FindObjectOfType<MainGameManager>();
        if (instance == null)
            MelonLogger.Warning("[GregGameManagerService] MainGameManager instance not found!");
        return instance;
    }

    public static void CloseAnyCanvas(bool isCustomerChoice = false)
    {
        GetInstance()?.CloseAnyCanvas(isCustomerChoice);
    }

    public static void OpenAnyCanvas()
    {
        GetInstance()?.OpenAnyCanvas();
    }

    public static void SetAutoSaveEnabled(bool enabled)
    {
        GetInstance()?.SetAutoSaveEnabled(enabled);
    }

    public static void SetAutoSaveInterval(float minutes)
    {
        GetInstance()?.SetAutoSaveInterval(minutes);
    }

    public static void RestartAutoSave()
    {
        GetInstance()?.RestartAutoSave();
    }

    public static void ShuffleAvailableCustomers()
    {
        GetInstance()?.ShuffleAvailableCustomers();
    }

    public static void ShuffleAvailableSubnets()
    {
        GetInstance()?.ShuffleAvailableSubnets();
    }
}
