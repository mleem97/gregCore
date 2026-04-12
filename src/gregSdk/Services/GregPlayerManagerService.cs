using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Bridge service for controlling player state and UI cursor interactions.
/// </summary>
public static class GregPlayerManagerService
{
    private static PlayerManager GetInstance()
    {
        var instance = UnityEngine.Object.FindObjectOfType<PlayerManager>();
        if (instance == null)
            MelonLogger.Warning("[GregPlayerManagerService] PlayerManager instance not found!");
        return instance;
    }

    public static void ConfineCursorForUI()
    {
        GetInstance()?.ConfinedCursorforUI();
    }

    public static void LockCursorForPlayerMovement()
    {
        GetInstance()?.LockedCursorForPlayerMovement();
    }

    public static void StopPlayerMovement()
    {
        GetInstance()?.PlayerStopMovement();
    }

    public static void TriggerGainIOPSEffect()
    {
        GetInstance()?.GainIOPSEffect();
    }
}
