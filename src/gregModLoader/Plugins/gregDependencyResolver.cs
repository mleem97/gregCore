using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using MelonLoader.Utils;

namespace greg.Core.Plugins;

/// <summary>
/// Utility for recursive dependency resolution across mod directories.
/// </summary>
public static class gregDependencyResolver
{
    private static readonly HashSet<string> _foundAssemblies = new(StringComparer.OrdinalIgnoreCase);
    private static bool _scanComplete;
    private static readonly object _lock = new();

    /// <summary>
    /// Checks if the specified dependencies are present in the filesystem.
    /// Supports deep recursive searching in StreamingAssets and standard Mods folder.
    /// </summary>
    /// <param name="logger">Logger for reporting missing dependencies.</param>
    /// <param name="modName">Name of the mod requesting the check.</param>
    /// <param name="required">List of required DLL filenames (without extension).</param>
    /// <param name="optional">List of optional DLL filenames (without extension).</param>
    /// <returns>True if all required dependencies are found; otherwise, false.</returns>
    public static bool CheckDependencies(MelonLogger.Instance logger, string modName, string[] required, string[] optional = null)
    {
        EnsureScan(logger);

        bool allMet = true;
        var missingRequired = new List<string>();

        if (required != null)
        {
            foreach (var req in required)
            {
                if (!_foundAssemblies.Contains(req))
                {
                    allMet = false;
                    missingRequired.Add(req);
                }
            }
        }

        if (!allMet)
        {
            logger.Error($"[{modName}] CRITICAL: Missing required dependencies: {string.Join(", ", missingRequired)}");
            logger.Error($"[{modName}] This mod will be disabled to prevent crashes.");
        }

        if (optional != null)
        {
            foreach (var opt in optional)
            {
                if (!_foundAssemblies.Contains(opt))
                {
                    logger.Warning($"[{modName}] Optional dependency '{opt}' not found. Some features may be unavailable.");
                }
            }
        }

        return allMet;
    }

    private static void EnsureScan(MelonLogger.Instance logger)
    {
        lock (_lock)
        {
            if (_scanComplete) return;

            var roots = new List<string>
            {
                MelonEnvironment.ModsDirectory,
                Path.Combine(MelonEnvironment.GameRootDirectory, "Data Center_Data", "StreamingAssets", "mods")
            };

            foreach (var root in roots)
            {
                if (Directory.Exists(root))
                {
                    ScanDirectoryRecursive(root, 0, 12);
                }
            }

            _scanComplete = true;
            logger.Msg($"[gregCore] Dependency scan complete. Found {_foundAssemblies.Count} unique assemblies in mods directories.");
        }
    }

    private static void ScanDirectoryRecursive(string path, int currentDepth, int maxDepth)
    {
        if (currentDepth > maxDepth) return;

        try
        {
            // Scan files in current directory
            foreach (var file in Directory.GetFiles(path, "*.dll"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                _foundAssemblies.Add(name);
            }

            // Recurse into subdirectories
            foreach (var dir in Directory.GetDirectories(path))
            {
                ScanDirectoryRecursive(dir, currentDepth + 1, maxDepth);
            }
        }
        catch
        {
            // Ignore access errors
        }
    }
}
