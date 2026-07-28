using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using gregCore.UI;
using greg.UI.Settings;
using gregCore.Infrastructure.UI;
using gregCore.Core.Compatibility;
using gregCore.Core.Events;
using gregCore.Core.Persistence;
using gregCore.Sdk;
using gregCore.Sdk.Language;
using gregCore.GameLayer.Hooks;
using gregCore.GameLayer.Interop;
using gregCore.Core.Abstractions;

[assembly: MelonInfo(typeof(gregCore.Core.GregCoreMod), "gregCore", "1.2.1", "TeamGreg")]
[assembly: MelonColor(255, 0, 191, 165)]
[assembly: MelonPriority(-1000)]

namespace gregCore.Core;

/// <summary>
/// MelonLoader host for gregCore. Loader-specific lifecycle work remains here;
/// managed framework services are initialized before IL2CPP-dependent adapters.
/// </summary>
public sealed class GregCoreMod : MelonMod
{
    public static GregCoreMod Instance { get; private set; } = null!;
    public static IGregAPI? PublicAPI { get; private set; }
    public static new HarmonyLib.Harmony? HarmonyInstance { get; private set; }
    public static GregEventBus? EventBus { get; private set; }
    public static GregHookBus? HookBus { get; private set; }
    public static CompatibilityProfile? ActiveCompatibilityProfile { get; private set; }
    public static CompatibilityReport? CompatibilityReport { get; private set; }
    public static bool SafeMode => CompatibilityReport?.SafeMode ?? true;

    private static bool _lateInitCompleted;
    private static bool _shutdownRequested;
    private Il2CppTypeRegistry? _typeRegistry;

    public override void OnInitializeMelon()
    {
        Instance = this;
        MelonLogger.Msg("--- gregCore Framework Boot v1.2.1 ---");

        var logger = new gregCore.Infrastructure.Logging.ConsoleLogger(LoggerInstance);

        InitializeManagedCore(logger);
        VerifyCompatibility(logger);
        InitializeIl2CppAdapters(logger);
        InitializeOptionalServices();
    }

    private static void InitializeManagedCore(IGregLogger logger)
    {
        try
        {
            EventBus = new GregEventBus(logger);
            HookBus = new GregHookBus(logger);
            API.GregAPI.Initialize(logger);
            MelonLogger.Msg("[gregCore] Managed event buses and public API initialized.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Managed core initialization failed: {ex.Message}");
        }
    }

    private static void VerifyCompatibility(IGregLogger logger)
    {
        try
        {
            string? currentPath = FindCompatibilityCurrentFile();
            if (currentPath == null)
            {
                CompatibilityReport = new CompatibilityReport
                {
                    ProfileId = "unresolved",
                    Level = CompatibilityLevel.Unknown,
                    SafeMode = true,
                    Issues = new[]
                    {
                        new CompatibilityIssue(
                            "PROFILE_MISSING",
                            "compat/current.json was not found. Game adapters are disabled.",
                            IsFatal: true)
                    }
                };
                logger.Warning(CompatibilityReport.ToDiagnosticText());
                return;
            }

            var pointer = JsonConvert.DeserializeObject<CurrentCompatibilityPointer>(File.ReadAllText(currentPath))
                ?? throw new InvalidDataException($"Invalid compatibility pointer: {currentPath}");
            string profilePath = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(currentPath)!, pointer.Profile));

            ActiveCompatibilityProfile = CompatibilityProfileLoader.Load(profilePath);
            CompatibilityReport = CompatibilityVerifier.Verify(
                ActiveCompatibilityProfile,
                ResolveReferenceFile,
                detectedUnityVersion: Application.unityVersion,
                detectedArchitecture: RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
                detectedPlatform: DetectPlatform());

            if (CompatibilityReport.SafeMode)
                logger.Warning(CompatibilityReport.ToDiagnosticText());
            else
                logger.Info(CompatibilityReport.ToDiagnosticText());
        }
        catch (Exception ex)
        {
            CompatibilityReport = new CompatibilityReport
            {
                ProfileId = "invalid",
                Level = CompatibilityLevel.Incompatible,
                SafeMode = true,
                Issues = new[]
                {
                    new CompatibilityIssue("PROFILE_ERROR", ex.Message, IsFatal: true)
                }
            };
            logger.Error("Compatibility verification failed; safe mode enabled.", ex);
        }
    }

    private void InitializeIl2CppAdapters(IGregLogger logger)
    {
        bool allowsInjection = !SafeMode &&
            (ActiveCompatibilityProfile?.Supports("classInjection") ?? false);

        _typeRegistry = new Il2CppTypeRegistry(logger);
        _typeRegistry.Register<GregHardwareID>(required: true, profileAllowsInjection: allowsInjection);
        _typeRegistry.Register<GregSettingsHub>(required: false, profileAllowsInjection: allowsInjection);
        _typeRegistry.Register<GregDevConsole>(required: false, profileAllowsInjection: allowsInjection);

        bool adaptersReady = !SafeMode && _typeRegistry.RequiredRegistrationsSucceeded();
        if (!adaptersReady)
        {
            MelonLogger.Warning("[gregCore] Running managed-only safe mode; IL2CPP UI and Harmony adapters are disabled.");
            return;
        }

        try
        {
            GregUIManager.Initialize();
            GregDevConsole.Initialize();
            GregSettingsHub.Initialize();
            MelonLogger.Msg("[gregCore] UI Toolkit adapters initialized.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] UI adapter initialization failed: {ex.Message}");
        }

        try
        {
            HarmonyInstance = new HarmonyLib.Harmony("gregCore.dynamic.hooks");
            if (EventBus != null && HookBus != null)
            {
                GregNativeEventHooks.Install(
                    logger,
                    HookBus,
                    EventBus,
                    HarmonyInstance,
                    ActiveCompatibilityProfile?.ProfileId,
                    safeMode: SafeMode);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Dynamic hook initialization failed: {ex.Message}");
        }
    }

    private static void InitializeOptionalServices()
    {
        try
        {
            Infrastructure.Social.DiscordService.Initialize();
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Discord] Optional service initialization failed: {ex.Message}");
        }
    }

    public override void OnUpdate()
    {
        if (_shutdownRequested) return;

        if (!_lateInitCompleted)
        {
            _lateInitCompleted = true;
            try
            {
                DiscoverGregExtHosts();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] gregExt discovery failed: {ex.Message}");
            }

            try
            {
                string modsDir = Path.Combine(
                    global::MelonLoader.Utils.MelonEnvironment.UserDataDirectory,
                    "Mods", "Scripts");
                GregLanguageRegistry.ScanAndActivate(modsDir);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Language host activation failed: {ex.Message}");
            }

            MelonLogger.Msg($"[gregCore] Framework initialization complete. SafeMode={SafeMode}.");
        }

        if (!SafeMode)
        {
            try { GregNotificationManager.Update(); }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Notification update failed: {ex.Message}"); }

            try { GregFontLoader.Tick(); }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Font search tick failed: {ex.Message}"); }
        }

        try { GregLanguageRegistry.OnUpdate(Time.deltaTime); }
        catch (Exception ex) { MelonLogger.Error($"[gregCore] Update callback failed: {ex.Message}"); }

        try { EventBus?.FlushDeferredEvents(); }
        catch (Exception ex) { MelonLogger.Error($"[gregCore] Event flush failed: {ex.Message}"); }
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        try
        {
            if (!SafeMode)
            {
                GregFontLoader.SearchFonts();
                if (!sceneName.Equals("MainMenu", StringComparison.Ordinal))
                    GregUIOverrideManager.HideVanillaUI();
            }

            Infrastructure.Social.DiscordService.UpdatePresence(
                sceneName.Equals("MainMenu", StringComparison.Ordinal)
                    ? "Planning Next Build"
                    : "Managing Infrastructure",
                sceneName.Equals("MainMenu", StringComparison.Ordinal)
                    ? "Main Menu"
                    : $"Scene: {sceneName}");

            GregLanguageRegistry.OnSceneLoaded(sceneName);
            HookBus?.Dispatch("OnSceneLoaded", new gregCore.Core.Models.EventPayload
            {
                HookName = "OnSceneLoaded",
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>
                {
                    ["BuildIndex"] = buildIndex,
                    ["SceneName"] = sceneName,
                    ["SafeMode"] = SafeMode
                }
            });
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Scene load callback failed: {ex.Message}");
        }
    }

    public override void OnApplicationQuit()
    {
        if (_shutdownRequested) return;
        _shutdownRequested = true;

        try
        {
            GregLanguageRegistry.Shutdown();
            if (!SafeMode) GregUIManager.Shutdown();
            Infrastructure.Social.DiscordService.Shutdown();
            EventBus?.Dispose();
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Shutdown failed: {ex.Message}");
        }

        base.OnApplicationQuit();
    }

    private static string? FindCompatibilityCurrentFile()
    {
        string? assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string modsDirectory = global::MelonLoader.Utils.MelonEnvironment.ModsDirectory;

        string?[] candidates =
        {
            assemblyDirectory == null ? null : Path.Combine(assemblyDirectory, "compat", "current.json"),
            Path.Combine(modsDirectory, "gregCore", "compat", "current.json"),
            Path.Combine(modsDirectory, "compat", "current.json"),
            Path.Combine(gameRoot, "compat", "current.json")
        };

        return candidates.FirstOrDefault(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path));
    }

    private static string? ResolveReferenceFile(ReferenceCompatibility reference)
    {
        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string? assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string fileName = reference.Path.Replace('/', Path.DirectorySeparatorChar);

        string?[] candidates =
        {
            Path.IsPathRooted(fileName) ? fileName : null,
            Path.Combine(gameRoot, "MelonLoader", "Il2CppAssemblies", fileName),
            Path.Combine(gameRoot, "MelonLoader", "net6", fileName),
            Path.Combine(gameRoot, "BepInEx", "interop", fileName),
            assemblyDirectory == null ? null : Path.Combine(assemblyDirectory, fileName),
            assemblyDirectory == null ? null : Path.Combine(assemblyDirectory, "references", fileName)
        };

        return candidates.FirstOrDefault(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path));
    }

    private static string DetectPlatform()
    {
        if (OperatingSystem.IsWindows()) return "windows";
        if (OperatingSystem.IsLinux()) return "linux";
        if (OperatingSystem.IsMacOS()) return "macos";
        return "unknown";
    }

    private static void DiscoverGregExtHosts()
    {
        IEnumerable<Assembly> extAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.GetName().Name?.StartsWith("gregExt.", StringComparison.Ordinal) == true);

        foreach (Assembly assembly in extAssemblies)
        {
            try
            {
                IEnumerable<Type> hostTypes = assembly.GetTypes()
                    .Where(type => typeof(IGregLanguageHost).IsAssignableFrom(type) &&
                                   !type.IsInterface && !type.IsAbstract);

                foreach (Type hostType in hostTypes)
                {
                    if (Activator.CreateInstance(hostType) is not IGregLanguageHost instance)
                        continue;

                    GregLanguageRegistry.RegisterHost(instance.HostId, instance);
                    MelonLogger.Msg(
                        $"[gregCore] gregExt host registered: {instance.HostId} ({instance.HostName})");
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                MelonLogger.Warning(
                    $"[gregCore] Could not load types from {assembly.GetName().Name}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error(
                    $"[gregCore] gregExt discovery error in {assembly.GetName().Name}: {ex.Message}");
            }
        }
    }

    private sealed class CurrentCompatibilityPointer
    {
        [JsonProperty("profile")]
        public string Profile { get; set; } = string.Empty;
    }
}

/// <summary>
/// Narrow legacy assembly redirect. It never aliases arbitrary gregCore assembly
/// versions and therefore cannot hide binary compatibility errors.
/// </summary>
public sealed class DataCenterModLoaderMod : MelonMod
{
    private static readonly HashSet<string> LegacyAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "DataCenterModLoader",
        "DataCenterModLoader.Core"
    };

    static DataCenterModLoaderMod()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
        {
            string? simpleName = new AssemblyName(args.Name).Name;
            return simpleName != null && LegacyAssemblyNames.Contains(simpleName)
                ? typeof(DataCenterModLoaderMod).Assembly
                : null;
        };
    }
}
