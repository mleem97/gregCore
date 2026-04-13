using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace gregCoreSDK.Sdk.Services;

/// <summary>
/// Bridge service for the game's internal Localisation and translation system.
/// Implements support for custom mod translations via the 'greg' namespace.
/// </summary>
public static class GregLocalisationService
{
    private static readonly Dictionary<string, int> _customKeyToIdMap = new();
    private static readonly Dictionary<int, Dictionary<int, string>> _customTranslations = new();
    private const int MOD_ID_START = 1000000;
    private static int _nextId = MOD_ID_START;

    private static Localisation GetInstance()
    {
        return Localisation.instance;
    }

    /// <summary>
    /// Returns the localized text for a given term key.
    /// If it's a mod-registered key, it uses the greg translation map.
    /// </summary>
    public static string GetTerm(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        // Check if it's a custom mod key
        if (_customKeyToIdMap.TryGetValue(key, out int customId))
        {
            int currentLang = GetCurrentlySelectedLanguage();
            if (_customTranslations.TryGetValue(currentLang, out var langDict) && langDict.TryGetValue(customId, out string text))
            {
                return text;
            }
            // Fallback to English if current lang is not available for this key
            if (_customTranslations.TryGetValue((int)Localisation.Languages.English, out var engDict) && engDict.TryGetValue(customId, out string engText))
            {
                return engText;
            }
        }

        // Standard game lookup or fallback
        return GetInstance()?.ReturnTextByID(customId > 0 ? customId : 0) ?? key;
    }

    /// <summary>
    /// Registers a new translation term for mods.
    /// </summary>
    public static void RegisterTerm(string key, string englishText, Dictionary<int, string> otherLanguages = null)
    {
        if (_customKeyToIdMap.ContainsKey(key)) return;

        int id = _nextId++;
        _customKeyToIdMap[key] = id;

        // Register English
        AddTranslation((int)Localisation.Languages.English, id, englishText);

        // Register others
        if (otherLanguages != null)
        {
            foreach (var kv in otherLanguages)
            {
                AddTranslation(kv.Key, id, kv.Value);
            }
        }
    }

    private static void AddTranslation(int langId, int termId, string text)
    {
        if (!_customTranslations.TryGetValue(langId, out var dict))
        {
            dict = new Dictionary<int, string>();
            _customTranslations[langId] = dict;
        }
        dict[termId] = text;
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
        return instance != null ? (int)instance.currentlySelectedLanguage : (int)Localisation.Languages.English;
    }
}
