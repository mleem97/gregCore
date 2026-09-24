using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using gregCore.UI;
using greg.UI.Settings;
using gregCore.Infrastructure.UI;
using gregCore.Core.Events;
using gregCore.Core.Persistence;
using gregCore.Sdk;
using gregCore.Sdk.Language;
using gregCore.GameLayer.Hooks;
using gregCore.Core.Abstractions;
using gregCore.Core.Exceptions;
using gregCore.Infrastructure.Plugins;
using gregCore.Infrastructure.Settings;
using gregCore.Core.Diagnostics;
using gregCore.GameLayer.Bootstrap;
using Il2CppInterop.Runtime.Injection;

[assembly: MelonInfo(typeof(gregCore.Core.GregCoreMod),
    gregCore.Core.BuildInfo.Name, gregCore.Core.BuildInfo.Version, gregCore.Core.BuildInfo.Author)]
[assembly: MelonColor(255, 0, 191, 165)] // Teal
[assembly: MelonPriority(-1000)] // Load first!

namespace gregCore.Core
{
    /// <summary>
    /// Central MelonMod entry point for gregCore.
    /// Provides the modding framework backbone, IL2CPP type registration,
    /// UI Toolkit initialization, dynamic hook patching, and gregExt discovery.
    /// </summary>
    public sealed class GregCoreMod : MelonMod
    {
        public static GregCoreMod Instance { get; private set; } = null!;
        public static IGregAPI? PublicAPI { get; private set; }
        public static new HarmonyLib.Harmony? HarmonyInstance { get; private set; }
        public static GregEventBus? EventBus { get; private set; }
        public static GregHookBus? HookBus { get; private set; }
        private static bool _lateInitCompleted;
        private static bool _shutdownRequested;

        public override void OnInitializeMelon()
        {
            Instance = this;
            MelonLogger.Msg("--- " + BuildInfo.ShortLabel + " ---");
            gregCore.Infrastructure.Logging.DevLog.Msg("Framework-Lebenszyklus gestartet (Flavor " + BuildInfo.Flavor + ").");

            // Kompatibilitaetswache: alten 404-ID-Mod entpatchen, falls vorhanden
            // (erneut beim Szenen-Laden fuer spaet geladene Mods).
            try { gregCore.GameLayer.Patches.Hardware.IncompatibleModGuard.DisableIncompatibleIdMods(); } catch { }
            MelonLogger.Msg("[gregCore][HwId] Eigenes Hardware-ID-System aktiv (gregID-Schema: Switch/PatchPanel/Server).");

            // Verzeichnis-Policy: Layout sicherstellen + Verstoesse melden.
            try
            {
                string root = MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
                gregCore.Infrastructure.IO.GregDirectoryPolicy.EnsureLayout(root);
                var violations = gregCore.Infrastructure.IO.GregDirectoryPolicy.Audit(root);
                gregCore.Infrastructure.IO.GregDirectoryPolicy.LogReport(violations);
            }
            catch (Exception ex) { MelonLogger.Warning($"[gregCore] Directory policy failed: {ex.Message}"); }

            try
            {
                var doctorPath = Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "gregCore", "doctor.json");
                var manifestPath = Path.Combine(MelonLoader.Utils.MelonEnvironment.GameRootDirectory, "Mods", "framework", "greg_hooks.json");
                var report = GregDoctor.Create(MelonLoader.Utils.MelonEnvironment.GameRootDirectory, manifestPath,
                    MelonLoader.Utils.MelonEnvironment.UserDataDirectory);
                GregDoctor.Write(doctorPath, report);
                MelonLogger.Msg($"[gregCore] Doctor: {report.Status}{(string.IsNullOrEmpty(report.ErrorCode) ? "" : $" ({report.ErrorCode})")}");
                gregCore.Infrastructure.Logging.DevLog.Msg("Doctor-Report: " + doctorPath);
            }
            catch (Exception ex) { MelonLogger.Warning($"[gregCore] Doctor report failed: {ex.Message}"); }

            // Initialize Social Services
            try
            {
                Infrastructure.Social.DiscordService.Initialize();
            }
            catch (Exception ex) { MelonLogger.Error($"[Discord] Init error: {ex.Message}"); }

            // Register persistent IL2CPP components
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<GregHardwareID>();
                ClassInjector.RegisterTypeInIl2Cpp<GregSettingsHub>();
                ClassInjector.RegisterTypeInIl2Cpp<GregDevConsole>();
                MelonLogger.Msg("[gregCore] IL2CPP types registered.");
                gregCore.Infrastructure.Logging.DevLog.Msg("IL2CPP-Typen registriert (GregHardwareID, GregSettingsHub, GregDevConsole).");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] IL2CPP type registration failed: {ex.Message}");
            }

            // Build the shared service graph exactly once. This wires the public API,
            // performance governor, settings, plugin registry and canonical hook bus.
            try
            {
                var logger = new gregCore.Infrastructure.Logging.ConsoleLogger(LoggerInstance);
                GregBootstrapper.Build(LoggerInstance);
                EventBus = GregServiceContainer.Get<GregEventBus>();
                HookBus = GregServiceContainer.Get<GregHookBus>();
                API.GregAPI.LogSink = (m, t) => GregDevConsole.Instance?.AddLog(m, t);
                API.GregAPI.Initialize(logger, EventBus, HookBus,
                    GregServiceContainer.Get<IGregPersistenceService>()
                    ?? new Infrastructure.Config.GregPersistenceService(logger));
                if (EventBus == null || HookBus == null)
                    throw new GregInitException("Bootstrap did not register the shared event and hook buses.");
                MelonLogger.Msg("[gregCore] Shared service graph initialized.");
                gregCore.Infrastructure.Logging.DevLog.Msg("Service-Graph verdrahtet: EventBus=" + (EventBus != null)
                    + ", HookBus=" + (HookBus != null) + ", API=" + (API.GregAPI.LogSink != null) + ".");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Bootstrap failed: {ex.Message}");
                // Keep lifecycle diagnostics alive if an optional game dependency is
                // absent. The fallback is deliberately not registered as a service.
                try
                {
                    var logger = new gregCore.Infrastructure.Logging.ConsoleLogger(LoggerInstance);
                    EventBus ??= new GregEventBus(logger);
                    HookBus ??= new GregHookBus(logger);
                    API.GregAPI.LogSink = (m, t) => GregDevConsole.Instance?.AddLog(m, t);
                    API.GregAPI.Initialize(logger, EventBus, HookBus,
                        GregServiceContainer.Get<IGregPersistenceService>()
                        ?? new Infrastructure.Config.GregPersistenceService(logger));
                }
                catch (Exception fallbackEx)
                {
                    MelonLogger.Error($"[gregCore] Event bus fallback failed: {fallbackEx.Message}");
                }
            }

            // Initialize UI Toolkit root
            try
            {
                GregUIManager.Initialize();
                GregDevConsole.Initialize();
                GregSettingsHub.Initialize();
                gregCore.Infrastructure.Scripting.Lua.Modules.LuaUiModule.RegisterTabHandler =
                    (tabId, label, registerTab) => GregSettingsHub.RegisterTab(tabId, label, registerTab);
                MelonLogger.Msg("[gregCore] UI Toolkit root initialized.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] UI initialization failed: {ex.Message}");
            }

            // Initialize Harmony and dynamic hook patcher
            try
            {
                HarmonyInstance = new HarmonyLib.Harmony("gregCore.dynamic.hooks");
                try
                {
                    gregCore.Infrastructure.Persistence.GregSaveGuard.Install(HarmonyInstance);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] SaveGuard failed: {ex.Message}");
                }
                if (EventBus != null && HookBus != null)
                {
                    var logger = new gregCore.Infrastructure.Logging.ConsoleLogger(LoggerInstance);
                    gregCore.GameApi.GregGameModuleHost.Configure(HarmonyInstance, EventBus, logger);
                    GregNativeEventHooks.Install(logger, HookBus, EventBus, HarmonyInstance);
                    try
                    {
                        gregCore.GameLayer.Hooks.NativeModLoaderHooks.Install(logger, EventBus, HarmonyInstance);
                    }
                    catch (Exception hookEx)
                    {
                        MelonLogger.Error($"[gregCore] NativeModLoaderHooks failed: {hookEx.Message}");
                    }
                    gregCore.Infrastructure.Logging.DevLog.Msg("Harmony-Hooks installiert: GregSaveGuard, GregNativeEventHooks, NativeModLoaderHooks.");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Dynamic hook initialization failed: {ex.Message}");
            }
        }

        public override void OnUpdate()
        {
            if (_shutdownRequested) return;

            // Deferred late initialization (Load-Order Safety)
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
                    GregServiceContainer.Get<IGregPluginRegistry>()?.LoadAll();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] Mod registry activation failed: {ex.Message}");
                }
                try
                {
                    var nativeService = GregServiceContainer.Get<IGregNativeModService>();
                    nativeService?.RefreshGameSnapshot();
                    nativeService?.LoadGregNativePlugins();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] Native mod activation failed: {ex.Message}");
                }
                try
                {
                    var fingerprint = gregCore.Core.Diagnostics.GameFingerprint.Capture(
                        global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory);
                    if (!string.IsNullOrWhiteSpace(fingerprint.AssemblyCSharpSha256) &&
                        !string.Equals(fingerprint.AssemblyCSharpSha256,
                            gregCore.GameApi.GregGameApiRegistry.SourceSha256,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        MelonLogger.Warning("[gregCore] GameApi drift: Assembly-CSharp differs from generation source. " +
                            "Re-run tools/GameApiGenerator/regenerate.sh after game updates.");
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[gregCore] GameApi drift check skipped: {ex.Message}");
                }
                try
                {
                    var modsDir = System.IO.Path.Combine(global::MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "Mods", "Scripts");
                    GregLanguageRegistry.ScanAndActivate(modsDir);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] Language host activation failed: {ex.Message}");
                }

                MelonLogger.Msg("[gregCore] Framework initialization complete.");
            }

            // Update UI systems
            try
            {
                GregNotificationManager.Update();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Notification update failed: {ex.Message}");
            }

            try
            {
                if (GregServiceContainer.Get<gregCore.Infrastructure.Performance.GregPerformanceGovernor>() is { } governor)
                    governor.OnUpdate();
                GregServiceContainer.Get<GregModSettingsService>()?.FlushPendingSave();
                GregLanguageRegistry.OnUpdate(Time.deltaTime);
                global::gregCore.PublicApi.greg._context?.MainThread.Drain();
                if (GregServiceContainer.Get<IGregPluginRegistry>() is GregPluginRegistry registry)
                    registry.Update(Time.deltaTime);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Update callback failed: {ex.Message}");
            }

            // Font search: retries periodically until game fonts are found
            try
            {
                GregFontLoader.Tick();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Font search tick failed: {ex.Message}");
            }

            // Mod-Menues: Input-Lock + Panel-Animationen/Drag (UI Baukasten)
            try
            {
                gregCore.UI.GregMenuRegistry.Tick(Time.deltaTime);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Menu registry tick failed: {ex.Message}");
            }

            // F1: zentrales Mod-Hub
            try
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb != null && kb.f1Key != null && kb.f1Key.wasPressedThisFrame)
                    gregCore.UI.GregModHub.Toggle();
                gregCore.UI.GregModHub.Refresh();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Mod-Hub tick failed: {ex.Message}");
            }

            // FlushDeferredEvents existiert nicht (mehr): GetStats() meldet
            // ggf. aufgestaute Events, ein Flush ist nicht vorgesehen.
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                // Kompatibilitaetswache (zweiter Lauf: faengt spaet geladene
                // inkompatible ID-Mods; bereits behandelte werden uebersprungen).
                try { gregCore.GameLayer.Patches.Hardware.IncompatibleModGuard.DisableIncompatibleIdMods(); } catch { }
                // Netz-Session: Szene ist stabil, Coop-Lookups ab jetzt sicher.
                try { gregCore.Infrastructure.Networking.GregNetSession.NotifySceneLoaded(); } catch { }
                // Save-Sidecars der Mods fuer diesen Spielstand laden.
                try { gregCore.Infrastructure.Persistence.GregSaveGuard.LoadSidecarsForCurrentSave(); } catch { }
                // Lazy font search — fonts are only available after scene load
                GregFontLoader.SearchFonts();

                if (sceneName != "MainMenu")
                {
                    GregUIOverrideManager.HideVanillaUI();
                    Infrastructure.Social.DiscordService.UpdatePresence("Managing Infrastructure", $"Scene: {sceneName}");
                }
                else
                {
                    Infrastructure.Social.DiscordService.UpdatePresence("Planning Next Build", "Main Menu");
                }
                GregLanguageRegistry.OnSceneLoaded(sceneName);
                (GregServiceContainer.Get<IGregPluginRegistry>() as GregPluginRegistry)?.SceneLoaded(sceneName);

                // Notify mods about scene change
                var scenePayload = new gregCore.Core.Models.EventPayload
                {
                    HookName = "gregMod.lifecycle.sceneLoaded",
                    OccurredAtUtc = DateTime.UtcNow,
                    Data = new Dictionary<string, object>
                    {
                        { "buildIndex", buildIndex },
                        { "sceneName", sceneName }
                    }
                };
                HookBus?.Dispatch("gregMod.lifecycle.sceneLoaded", scenePayload);
                EventBus?.Publish("gregMod.lifecycle.sceneLoaded", scenePayload);
                // Deprecated compatibility bridge.
                HookBus?.Dispatch("greg.lifecycle.SceneLoaded", scenePayload);
                EventBus?.Publish("greg.lifecycle.SceneLoaded", scenePayload);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Scene load callback failed: {ex.Message}");
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            try { gregCore.Infrastructure.Networking.GregNetSession.NotifySceneUnloading(); } catch { }
        }

        public override void OnApplicationQuit()        {
            if (_shutdownRequested) return;
            _shutdownRequested = true;

            try
            {
                GregLanguageRegistry.Shutdown();
                (GregServiceContainer.Get<IGregPluginRegistry>() as GregPluginRegistry)?.Shutdown();
                (GregServiceContainer.Get<IGregNativeModService>() as gregCore.Infrastructure.Plugins.Native.GregNativeModService)?.Shutdown();
                GregUIManager.Shutdown();
                Infrastructure.Social.DiscordService.Shutdown();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Shutdown failed: {ex.Message}");
            }
            base.OnApplicationQuit();
        }

        /// <summary>
        /// Discovers IGregLanguageHost implementations in assemblies named gregExt.*
        /// or marked with [GregExtension] and registers them dynamically.
        /// </summary>
        private static void DiscoverGregExtHosts()
        {
            var extAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name?.StartsWith("gregExt.") == true);

            foreach (var asm in extAssemblies)
            {
                try
                {
                    var hostTypes = asm.GetTypes()
                        .Where(t => typeof(IGregLanguageHost).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var hostType in hostTypes)
                    {
                        var instance = (IGregLanguageHost?)Activator.CreateInstance(hostType);
                        if (instance != null)
                        {
                            GregLanguageRegistry.RegisterHost(instance.HostId, instance);
                            MelonLogger.Msg($"[gregCore] gregExt host registered: {instance.HostId} ({instance.HostName})");
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    MelonLogger.Warning($"[gregCore] Could not load types from {asm.GetName().Name}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore] gregExt discovery error in {asm.GetName().Name}: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Assembly resolution shim to redirect legacy mod loader references to gregCore.
    /// </summary>
    public sealed class DataCenterModLoaderMod : MelonMod
    {
        static DataCenterModLoaderMod()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.StartsWith("DataCenterModLoader") || args.Name.StartsWith("gregCore"))
                {
                    return typeof(DataCenterModLoaderMod).Assembly;
                }
                return null;
            };
        }
    }
}
