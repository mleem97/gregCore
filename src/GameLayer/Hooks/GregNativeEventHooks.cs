using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Loader-to-Harmony bridge. Game methods are resolved exclusively from
/// compatibility manifests so this assembly no longer needs compile-time
/// Harmony attributes for individual Assembly-CSharp types.
/// </summary>
[HarmonyPatch]
public sealed class GregNativeEventHooks : SafePatch
{
    private static bool _isInstalled;
    private static GregDynamicHookPatcher? _dynamicPatcher;

    public static GregDynamicHookPatcher? DynamicPatcher => _dynamicPatcher;

    public static void Install(
        IGregLogger logger,
        GregHookBus hookBus,
        GregEventBus eventBus,
        HarmonyLib.Harmony harmony,
        string? activeProfileId = null,
        bool safeMode = false)
    {
        if (_isInstalled) return;

        Setup(logger, hookBus);

        if (safeMode)
        {
            _logger?.Warning("Compatibility safe mode is active. Game Harmony hooks were not installed.");
            _isInstalled = true;
            return;
        }

        try
        {
            _dynamicPatcher = new GregDynamicHookPatcher(harmony, eventBus, logger);
            GregDynamicHookPatcher.SetGlobalBus(eventBus);
            GregDynamicHookPatcher.SetGlobalLogger(logger);

            IReadOnlyList<string> manifests = FindHookManifests();
            if (manifests.Count == 0)
                _logger?.Warning("No hook manifest found. The managed framework remains available without game hooks.");

            foreach (string manifest in manifests)
                _dynamicPatcher.InstallFromFile(manifest, activeProfileId);

            _logger?.Success(
                $"GregNativeEventHooks installed {_dynamicPatcher.InstalledCount} hooks; " +
                $"{_dynamicPatcher.FailedCount} definitions could not be resolved.");
        }
        catch (Exception ex)
        {
            _logger?.Error("Failed to initialize dynamic hook patcher", ex);
        }

        _isInstalled = true;
    }

    private static IReadOnlyList<string> FindHookManifests()
    {
        var result = new List<string>();
        string modsDirectory = global::MelonLoader.Utils.MelonEnvironment.ModsDirectory;
        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string? assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        AddFirstExisting(result,
            Path.Combine(modsDirectory, "game_hooks.v2.json"),
            assemblyDirectory == null ? null : Path.Combine(assemblyDirectory, "game_hooks.v2.json"),
            Path.Combine(gameRoot, "framework", "game_hooks.v2.json"),
            Path.Combine(gameRoot, "game_hooks.v2.json"));

        AddFirstExisting(result,
            Path.Combine(modsDirectory, "game_hooks.json"),
            assemblyDirectory == null ? null : Path.Combine(assemblyDirectory, "game_hooks.json"),
            Path.Combine(gameRoot, "game_hooks.json"));

        return result;
    }

    private static void AddFirstExisting(List<string> result, params string?[] candidates)
    {
        foreach (string? candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate) || !File.Exists(candidate))
                continue;

            if (!result.Contains(candidate, StringComparer.OrdinalIgnoreCase))
                result.Add(candidate);
            return;
        }
    }

    public const string WorldWallRegistered = "greg.WORLD.WallRegistered";
    public const string WorldWallRemoved = "greg.WORLD.WallRemoved";
    public const string WorldWallPlaced = "greg.WORLD.WallPlaced";
    public const string WorldWallDeviceMounted = "greg.WORLD.WallDeviceMounted";
    public const string WorldWallDeviceUnmounted = "greg.WORLD.WallDeviceUnmounted";
    public const string WorldWallDeviceSwapped = "greg.WORLD.WallDeviceSwapped";
    public const string WorldWallDeviceLabelSet = "greg.WORLD.WallDeviceLabelSet";
    public const string SystemButtonBuyWall = "greg.SYSTEM.ButtonBuyWall";
}
