/// <file-summary>
/// Layer:       Core (gregCore.Lang)
/// Purpose:     Static facade for the central translation service. Owns the
///              "gregCore/Language" MelonPreferences entry (default "auto" =
///              system language), resolves Mods/Data and dispatches lookups
///              to GregLangStore. Lazily self-initializes so mods can call T()
///              regardless of load order.
/// Maintainer:  Keep this class reflection-friendly: mods reach it via the
///              type name "gregCore.Lang.GregLang, gregCore" (no hard refs).
///              Never throw out of T().
/// </file-summary>

using System;
using System.IO;

namespace gregCore.Lang;

/// <summary>
/// Central translation service for all Greg Data Center mods.
/// Translations live in <c>&lt;game&gt;/Mods/Data/&lt;modId&gt;/&lt;lang&gt;.json</c>
/// (flat key/value JSON, e.g. <c>{ "panel.title": "Trainer" }</c>).
/// Fallback chain: active language file -&gt; <c>en.json</c> -&gt; caller default.
/// </summary>
public static class GregLang
{
    /// <summary>Mod id of gregCore's own table (<c>Mods/Data/gregCore/</c>).</summary>
    public const string CoreModId = "gregCore";

    private static readonly object Gate = new();
    private static GregLangStore _store;
    private static bool _initialized;
    private static string _current = LangCodes.Fallback;
    private static string _dataRoot = "";

    /// <summary>Active two-letter language code ("en", "de", ...).</summary>
    public static string Current
    {
        get { lock (Gate) { return _current; } }
    }

    /// <summary>Fired on the calling thread after <see cref="SetLanguage"/> reloaded all tables.</summary>
    public static event Action LanguageChanged;

    /// <summary>
    /// Translates <paramref name="key"/> for <paramref name="modId"/>.
    /// Never throws; returns the fallback chain result (see class docs).
    /// </summary>
    public static string T(string modId, string key, string defaultText)
    {
        try
        {
            EnsureInitialized();
            var store = _store;
            if (store == null)
                return defaultText ?? key ?? "";
            string lang;
            lock (Gate) { lang = _current; }
            return store.Translate(modId, key, defaultText, lang);
        }
        catch { return defaultText ?? key ?? ""; }
    }

    /// <summary>Translated text with <c>string.Format</c> arguments applied.</summary>
    public static string T(string modId, string key, string defaultText, params object[] args)
    {
        try
        {
            EnsureInitialized();
            var store = _store;
            if (store == null)
            {
                if (args != null && args.Length > 0)
                {
                    try { return string.Format(defaultText ?? key ?? "", args); }
                    catch { /* keep raw */ }
                }
                return defaultText ?? key ?? "";
            }
            string lang;
            lock (Gate) { lang = _current; }
            return store.Translate(modId, key, defaultText, lang, args);
        }
        catch { return defaultText ?? key ?? ""; }
    }

    /// <summary>Idempotent explicit init (also runs lazily from <see cref="T"/>).</summary>
    public static void Initialize()
    {
        try { EnsureInitialized(); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    /// <summary>
    /// Switches the language (<c>"de"</c>, <c>"en"</c>, or <c>"auto"</c>),
    /// persists the preference, reloads all tables, and fires
    /// <see cref="LanguageChanged"/>. Unknown codes fall back to "en".
    /// </summary>
    public static void SetLanguage(string code)
    {
        try
        {
            EnsureInitialized();
            string normalized = LangCodes.Normalize(code);
            string resolved = normalized == LangCodes.Auto ? DetectSystemLanguage() : normalized;

            lock (Gate)
            {
                _current = resolved;
                try { _store?.Reload(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }

            try { SavePreference(normalized); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            try { global::MelonLoader.MelonLogger.Msg($"[gregCore.Lang] Language: {resolved} (pref: {normalized})."); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            Action handler;
            lock (Gate) { handler = LanguageChanged; }
            try { handler?.Invoke(); } catch { /* subscriber best-effort */ }
        }
        catch { /* never break the caller */ }
    }

    private static void EnsureInitialized()
    {
        lock (Gate)
        {
            if (_initialized)
                return;
            _initialized = true;
        }

        try
        {
            string modsDir = "";
            try { modsDir = global::MelonLoader.Utils.MelonEnvironment.ModsDirectory ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (string.IsNullOrWhiteSpace(modsDir))
            {
                string gameRoot = "";
                try { gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                if (string.IsNullOrWhiteSpace(gameRoot))
                {
                    try { gameRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                try { modsDir = Path.Combine(gameRoot, "Mods"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }

            string root = "";
            try { root = Path.Combine(modsDir, "Data"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            string preference = LoadPreference();

            string resolved = preference == LangCodes.Auto ? DetectSystemLanguage() : LangCodes.Normalize(preference);

            lock (Gate)
            {
                _dataRoot = root;
                try { _store = new GregLangStore(root); } catch { _store = null; }
                _current = resolved;
            }
        }
        catch
        {
            lock (Gate)
            {
                try { _store = new GregLangStore(""); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                _current = LangCodes.Fallback;
            }
        }
    }

    private static string LoadPreference()
    {
        try
        {
            var cat = global::MelonLoader.MelonPreferences.CreateCategory("gregCore", "gregCore");
            var existingLang = cat.GetEntry<string>("Language");
            if (existingLang == null)
            {
                cat.CreateEntry("Language", LangCodes.Auto, "Language",
                    "UI language for Greg mods (two-letter code, or 'auto' = system language). Translations: Mods/Data/<modId>/<lang>.json.");
                try { cat.SaveToFile(false); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                return LangCodes.Auto;
            }
            return string.IsNullOrWhiteSpace(existingLang.Value) ? LangCodes.Auto : existingLang.Value;
        }
        catch { return LangCodes.Auto; }
    }

    private static void SavePreference(string normalized)
    {
        try
        {
            var cat = global::MelonLoader.MelonPreferences.CreateCategory("gregCore", "gregCore");
            var existing = cat.GetEntry<string>("Language");
            if (existing != null)
                existing.Value = normalized;
            else
                cat.CreateEntry("Language", normalized, "Language",
                    "UI language for Greg mods (two-letter code, or 'auto' = system language). Translations: Mods/Data/<modId>/<lang>.json.");
            try { global::MelonLoader.MelonPreferences.Save(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static string DetectSystemLanguage()
    {
        try
        {
            string name = "";
            try { name = UnityEngine.Application.systemLanguage.ToString(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            string code = LangCodes.Normalize(name);
            return code == LangCodes.Auto ? LangCodes.Fallback : code;
        }
        catch { return LangCodes.Fallback; }
    }
}
