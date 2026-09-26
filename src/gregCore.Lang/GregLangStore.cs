/// <file-summary>
/// Layer:       Core (gregCore.Lang)
/// Purpose:     File-backed translation tables with fallback chain:
///              Mods/Data/&lt;modId&gt;/&lt;lang&gt;.json -> en.json -> caller default.
///              Pure logic (filesystem + System.Text.Json only), no
///              Unity/MelonLoader dependencies, so it is unit-testable.
/// Maintainer:  Keep all lookups exception-free; a broken table must never
///              break a mod.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace gregCore.Lang;

/// <summary>
/// Loads and caches per-mod translation tables from a data root
/// (<c>&lt;game&gt;/Mods/Data</c>). Thread-safe. Every public method is
/// exception-free: on any failure the caller default is returned.
/// </summary>
public sealed class GregLangStore
{
    private readonly string _dataRoot;
    private readonly object _gate = new();
    private readonly Dictionary<string, Dictionary<string, string>> _cache =
        new(StringComparer.Ordinal);

    /// <param name="dataRoot">Directory containing one subfolder per mod id
    /// (e.g. <c>&lt;game&gt;/Mods/Data</c>).</param>
    public GregLangStore(string dataRoot)
    {
        _dataRoot = dataRoot ?? "";
    }

    /// <summary>
    /// Looks up <paramref name="key"/> for <paramref name="modId"/>.
    /// Order: requested language file, <c>en.json</c>, then
    /// <paramref name="defaultText"/>. Optional <paramref name="args"/> are
    /// applied with <c>string.Format</c> (format errors keep the raw text).
    /// </summary>
    public string Translate(string modId, string key, string defaultText, string language, params object[] args)
    {
        string? text = Lookup(modId, key, language);
        if (text == null)
            text = defaultText ?? key ?? "";

        if (args != null && args.Length > 0)
        {
            try { text = string.Format(text, args); }
            catch { /* keep raw text on bad format strings */ }
        }
        return text;
    }

    /// <summary>Languages with a translation file for <paramref name="modId"/> (e.g. "en", "de").</summary>
    public IReadOnlyList<string> AvailableLanguages(string modId)
    {
        var result = new List<string>();
        try
        {
            string? dir = ModDir(modId);
            if (dir == null || !Directory.Exists(dir))
                return result;
            foreach (var file in Directory.GetFiles(dir, "*.json"))
            {
                try
                {
                    string code = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
                    if (code.Length == 2 && result.IndexOf(code) < 0)
                        result.Add(code);
                }
                catch { /* skip unreadable names */ }
            }
        }
        catch { /* best-effort */ }
        result.Sort(StringComparer.Ordinal);
        return result;
    }

    /// <summary>Clears all cached tables (used after a language switch).</summary>
    public void Reload()
    {
        try
        {
            lock (_gate) { _cache.Clear(); }
        }
        catch { /* best-effort */ }
    }

    private string? Lookup(string modId, string key, string language)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            string lang = LangCodes.Normalize(language);
            if (lang == LangCodes.Auto)
                lang = LangCodes.Fallback;

            var table = GetTable(modId, lang);
            if (table != null && table.TryGetValue(key, out var hit) && !string.IsNullOrEmpty(hit))
                return hit;

            if (!lang.Equals(LangCodes.Fallback, StringComparison.Ordinal))
            {
                var fallback = GetTable(modId, LangCodes.Fallback);
                if (fallback != null && fallback.TryGetValue(key, out var fb) && !string.IsNullOrEmpty(fb))
                    return fb;
            }
        }
        catch { /* fall through to caller default */ }
        return null;
    }

    private Dictionary<string, string> GetTable(string modId, string lang)
    {
        string cacheKey = (modId ?? "") + "|" + (lang ?? "");
        lock (_gate)
        {
            if (_cache.TryGetValue(cacheKey, out var cached) && cached != null)
                return cached;
        }

        var loaded = LoadTable(modId ?? "", lang ?? LangCodes.Fallback);

        lock (_gate) { _cache[cacheKey] = loaded; }
        return loaded;
    }

    private Dictionary<string, string> LoadTable(string modId, string lang)
    {
        var empty = new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            string? dir = ModDir(modId);
            if (dir == null)
                return empty;
            string file = Path.Combine(dir, lang + ".json");
            if (!File.Exists(file))
                return empty;

            string json = File.ReadAllText(file);
            if (string.IsNullOrWhiteSpace(json))
                return empty;

            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (parsed == null)
                return empty;

            var table = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var kv in parsed)
            {
                if (!string.IsNullOrEmpty(kv.Key) && kv.Value != null)
                    table[kv.Key] = kv.Value;
            }
            return table;
        }
        catch
        {
            // Malformed JSON or IO errors degrade to the fallback chain.
            return empty;
        }
    }

    /// <summary>Validated mod directory, or null when the id is unsafe.</summary>
    private string? ModDir(string modId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(modId) || string.IsNullOrWhiteSpace(_dataRoot))
                return null;
            foreach (char c in modId)
            {
                bool ok = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')
                    || (c >= '0' && c <= '9') || c == '.' || c == '_' || c == '-';
                if (!ok)
                    return null;
            }
            if (modId.IndexOf("..", StringComparison.Ordinal) >= 0)
                return null;
            return Path.Combine(_dataRoot, modId);
        }
        catch { return null; }
    }
}
