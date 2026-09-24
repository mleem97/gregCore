/// <file-summary>
/// Layer:       Core (gregCore.Lang)
/// Purpose:     Language-code normalization for the gregCore translation
///              service. Pure logic, no Unity/MelonLoader dependencies, so it
///              is unit-testable without the game.
/// Maintainer:  Extend SupportedCodes when a new language ships.
/// </file-summary>

using System;
using System.Collections.Generic;

namespace gregCore.Lang;

/// <summary>
/// Normalizes user, preference, and system language names to two-letter
/// codes used as translation file names (<c>Mods/Data/&lt;modId&gt;/de.json</c>).
/// Unknown input always maps to <see cref="Fallback"/> ("en") so callers
/// never need their own fallback.
/// </summary>
public static class LangCodes
{
    /// <summary>Fallback language. Every mod must ship <c>en.json</c>.</summary>
    public const string Fallback = "en";

    /// <summary>Preference value that selects the system language.</summary>
    public const string Auto = "auto";

    private static readonly Dictionary<string, string> SystemNameToCode =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "english", "en" },
            { "german", "de" },
            { "french", "fr" },
            { "spanish", "es" },
            { "italian", "it" },
            { "portuguese", "pt" },
            { "brazilian portuguese", "pt" },
            { "russian", "ru" },
            { "polish", "pl" },
            { "dutch", "nl" },
            { "turkish", "tr" },
            { "chinese", "zh" },
            { "chinese simplified", "zh" },
            { "chinese traditional", "zh" },
            { "japanese", "ja" },
            { "korean", "ko" },
            { "czech", "cs" },
            { "hungarian", "hu" },
            { "swedish", "sv" },
            { "norwegian", "no" },
            { "danish", "da" },
            { "finnish", "fi" },
            { "greek", "el" },
            { "ukrainian", "uk" },
            { "arabic", "ar" },
            { "thai", "th" },
            { "vietnamese", "vi" },
            { "indonesian", "id" },
        };

    /// <summary>
    /// Normalizes a language code (<c>"de"</c>, <c>"de-DE"</c>), a Unity
    /// <c>SystemLanguage</c> name (<c>"German"</c>), or <c>"auto"</c> to a
    /// two-letter code. Unknown or empty input returns <see cref="Fallback"/>.
    /// </summary>
    public static string Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Fallback;

        var s = input.Trim();

        if (s.Equals(Auto, StringComparison.OrdinalIgnoreCase))
            return Auto;

        // Already a code: "de", "de-DE", "DE".
        if (s.Length >= 2 && IsAsciiLetter(s[0]) && IsAsciiLetter(s[1]))
        {
            bool restOk = true;
            for (int i = 2; i < s.Length; i++)
            {
                char c = s[i];
                if (c != '-' && c != '_' && !IsAsciiLetter(c)) { restOk = false; break; }
            }
            if (restOk && (s.Length == 2 || s.Length == 5))
                return s.Substring(0, 2).ToLowerInvariant();
        }

        if (SystemNameToCode.TryGetValue(s, out var code))
            return code;

        return Fallback;
    }

    private static bool IsAsciiLetter(char c) =>
        (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
}
