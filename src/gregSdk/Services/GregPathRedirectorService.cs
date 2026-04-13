using System;
using System.IO;
using System.Collections.Generic;
using MelonLoader;
using MelonLoader.Utils;

namespace greg.Sdk.Services;

/// <summary>
/// Mod path redirector service. Resolves mod paths from multiple sources:
/// standard Mods/, Steam Workshop, and custom user paths.
/// 
/// Integrated from gregAddons/gregMods/gregMod.PathRedirector.
/// </summary>
public static class GregPathRedirectorService
{
    private static readonly List<string> _additionalPaths = new();

    /// <summary>All registered mod directories (Mods/ + Workshop + custom).</summary>
    public static IReadOnlyList<string> ModPaths => _cachedPaths;
    private static List<string> _cachedPaths = new();

    /// <summary>Initialize on framework startup. Scans for additional mod paths.</summary>
    public static void Initialize()
    {
        _cachedPaths.Clear();

        // Primary: MelonLoader Mods directory
        string modsDir = MelonEnvironment.ModsDirectory;
        if (Directory.Exists(modsDir))
            _cachedPaths.Add(modsDir);

        // Secondary: Steam Workshop
        string workshopPath = ResolveSteamWorkshopPath();
        if (!string.IsNullOrEmpty(workshopPath) && Directory.Exists(workshopPath))
            _cachedPaths.Add(workshopPath);

        // Tertiary: User-registered paths
        foreach (var p in _additionalPaths)
        {
            if (Directory.Exists(p))
                _cachedPaths.Add(p);
        }

        MelonLogger.Msg($"[PathRedirector] Initialized with {_cachedPaths.Count} mod paths.");
    }

    /// <summary>Register an additional mod lookup path.</summary>
    public static void RegisterPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || _additionalPaths.Contains(path)) return;
        _additionalPaths.Add(path);
        Initialize(); // Refresh cache
    }

    /// <summary>Find a file across all registered mod paths.</summary>
    public static string FindFile(string relativePath)
    {
        foreach (var basePath in _cachedPaths)
        {
            string fullPath = Path.Combine(basePath, relativePath);
            if (File.Exists(fullPath))
                return fullPath;
        }
        return null;
    }

    /// <summary>Find a directory across all registered mod paths.</summary>
    public static string FindDirectory(string relativePath)
    {
        foreach (var basePath in _cachedPaths)
        {
            string fullPath = Path.Combine(basePath, relativePath);
            if (Directory.Exists(fullPath))
                return fullPath;
        }
        return null;
    }

    private static string ResolveSteamWorkshopPath()
    {
        try
        {
            // Steam workshop content for Data Center (AppID from game)
            string gameRoot = MelonEnvironment.GameRootDirectory;
            string workshopContent = Path.Combine(
                Directory.GetParent(gameRoot)?.Parent?.Parent?.FullName ?? "",
                "workshop", "content");

            if (Directory.Exists(workshopContent))
            {
                // Find subdirectories for this game
                foreach (var dir in Directory.GetDirectories(workshopContent))
                {
                    if (Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories).Length > 0)
                        return dir;
                }
            }
        }
        catch { }

        return null;
    }
}
