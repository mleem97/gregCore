using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using MelonLoader;

namespace gregFramework.Core;

/// <summary>
/// Resolves deprecated hook name spellings to canonical greg.* names.
/// Mappings are loaded from greg_hooks.json (hook.legacy → hook.name).
/// </summary>
public static class GregCompatBridge
{
    private static readonly object Sync = new();
    private static Dictionary<string, string> _legacyToCanonical = new(StringComparer.Ordinal);
    private static bool _loaded;

    public static void EnsureLoaded(string jsonPath = null)
    {
        lock (Sync)
        {
            if (_loaded)
                return;

            var path = jsonPath ?? ResolveDefaultJsonPath();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                MelonLogger.Warning("[gregCore] greg_hooks.json not found; legacy hook resolution disabled.");
                _loaded = true;
                return;
            }

            try
            {
                var text = File.ReadAllText(path);
                using var doc = JsonDocument.Parse(text);
                if (!doc.RootElement.TryGetProperty("hooks", out var hooks) || hooks.ValueKind != JsonValueKind.Array)
                {
                    _loaded = true;
                    return;
                }

                foreach (var el in hooks.EnumerateArray())
                {
                    if (!el.TryGetProperty("name", out var nameProp))
                        continue;
                    var canonical = nameProp.GetString();
                    if (string.IsNullOrWhiteSpace(canonical))
                        continue;
                    if (!el.TryGetProperty("legacy", out var legacyProp))
                        continue;
                    var legacy = legacyProp.GetString();
                    if (string.IsNullOrWhiteSpace(legacy))
                        continue;
                    _legacyToCanonical[legacy] = canonical;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[gregCore] Failed to parse greg_hooks.json: {ex.Message}");
            }

            _loaded = true;
        }
    }

    private static string ResolveDefaultJsonPath()
    {
        try
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(dir))
                return null;
            return Path.Combine(dir, "greg_hooks.json");
        }
        catch
        {
            return null;
        }
    }

    public static bool TryGetCanonical(string hookName, out string gregName)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(hookName))
        {
            gregName = null;
            return false;
        }

        if (hookName.StartsWith("greg.", StringComparison.Ordinal))
        {
            gregName = hookName;
            return true;
        }

        lock (Sync)
        {
            if (_legacyToCanonical.TryGetValue(hookName, out var mapped))
            {
                gregName = mapped;
                return true;
            }
        }

        gregName = null;
        return false;
    }

    public static string Resolve(string hookName)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(hookName))
            return hookName;

        lock (Sync)
        {
            if (_legacyToCanonical.TryGetValue(hookName, out var modern))
            {
                MelonLogger.Warning($"[gregCore] Deprecated hook id redirected to canonical '{modern}'.");
                return modern;
            }
        }

        return hookName;
    }
}
