using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace gregSdk.Services;

/// <summary>
/// Bridge service for the game's internal Localisation and translation system.
/// </summary>
public static class GregLocalisationService
{
    private static Localisation GetInstance()
    {
        return Localisation.instance;
    }

    public static string GetTextByID(int id)
    {
        return GetInstance()?.ReturnTextByID(id) ?? $"MISSING_LOC_{id}";
    }

    public static void ChangeLanguage(int languageIndex)
    {
        GetInstance()?.ChangeLocalisation(languageIndex);
    }

    public static int GetCurrentlySelectedLanguage()
    {
        var instance = GetInstance();
        return instance != null ? (int)instance.currentlySelectedLanguage : 0;
    }
}
