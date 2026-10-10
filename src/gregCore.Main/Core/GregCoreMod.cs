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
    public sealed partial class GregCoreMod : MelonMod
    {
        public static GregCoreMod Instance { get; private set; } = null!;
        public static IGregAPI? PublicAPI { get; private set; }
        public static new HarmonyLib.Harmony? HarmonyInstance { get; private set; }
        public static GregEventBus? EventBus { get; private set; }
        public static GregHookBus? HookBus { get; private set; }
        private bool _lateInitCompleted;
        private bool _shutdownRequested;

        public override void OnInitializeMelon()
        {
            Instance = this;
            MelonLogger.Msg("--- " + BuildInfo.ShortLabel + " ---");
            gregCore.Infrastructure.Logging.DevLog.Msg("Framework lifecycle started (Flavor " + BuildInfo.Flavor + ").");
            DisableIncompatibleMods();
            EnsureDirectories();
            WriteDoctorReport();
            InitSocial();
            RegisterIl2CppTypes();
            BuildServiceGraph();
            InitUi();
            InitHooks();
        }

        private void DisableIncompatibleMods()
        {
            try { gregCore.GameLayer.Patches.Hardware.IncompatibleModGuard.DisableIncompatibleIdMods(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        private static void EnsureDirectories()
        {
            try
            {
                string root = MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
                gregCore.Infrastructure.IO.GregDirectoryPolicy.EnsureLayout(root);
                var violations = gregCore.Infrastructure.IO.GregDirectoryPolicy.Audit(root);
                gregCore.Infrastructure.IO.GregDirectoryPolicy.LogReport(violations);
            }
            catch (Exception ex) { MelonLogger.Warning($"[gregCore] Directory policy failed: {ex.Message}"); }
        }

        private static void WriteDoctorReport()
        {
            try
            {
                var doctorPath = Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "gregCore", "doctor.json");
                var manifestPath = Path.Combine(MelonLoader.Utils.MelonEnvironment.GameRootDirectory, "Mods", "framework", "greg_hooks.json");
                var report = GregDoctor.Create(MelonLoader.Utils.MelonEnvironment.GameRootDirectory, manifestPath,
                    MelonLoader.Utils.MelonEnvironment.UserDataDirectory);
                GregDoctor.Write(doctorPath, report);
                MelonLogger.Msg($"[gregCore] Doctor: {report.Status}{(string.IsNullOrEmpty(report.ErrorCode) ? "" : $" ({report.ErrorCode})")}");
                gregCore.Infrastructure.Logging.DevLog.Msg("Doctor report: " + doctorPath);
                bool supported = string.IsNullOrEmpty(report.ErrorCode)
                    && string.Equals(report.Status, "SUPPORTED_GAME_BUILD", StringComparison.Ordinal);
                gregCore.Core.Diagnostics.GregGameCompat.MarkEvaluated(supported);
                MelonLogger.Msg("[gregCore][HwId] HWID " + (supported
                    ? "ACTIVE (gregID: Switch/PatchPanel/Server)."
                    : "PASSIVE on this build (IDs untouched, vanilla behaviour)."));
            }
            catch (Exception ex)
            {
                gregCore.Core.Diagnostics.GregGameCompat.MarkEvaluated(false);
                MelonLogger.Warning($"[gregCore] Doctor report failed: {ex.Message}");
            }
        }

        private static void InitSocial()
        {
            try
            {
                Infrastructure.Social.DiscordService.Initialize();
            }
            catch (Exception ex) { MelonLogger.Error($"[Discord] Init error: {ex.Message}"); }
        }

        private static void RegisterIl2CppTypes()
        {
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<GregHardwareID>();
                ClassInjector.RegisterTypeInIl2Cpp<GregSettingsHub>();
                ClassInjector.RegisterTypeInIl2Cpp<GregDevConsole>();
                MelonLogger.Msg("[gregCore] IL2CPP types registered.");
                gregCore.Infrastructure.Logging.DevLog.Msg("IL2CPP types registered (GregHardwareID, GregSettingsHub, GregDevConsole).");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] IL2CPP type registration failed: {ex.Message}");
            }
        }

        private void BuildServiceGraph()
        {
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
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Bootstrap failed: {ex.Message}");
                BuildServiceFallback();
            }
        }

        private void BuildServiceFallback()
        {
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

        private static void InitUi()
        {
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
        }

        private void InitHooks()
        {
            try
            {
                HarmonyInstance = new HarmonyLib.Harmony("gregCore.dynamic.hooks");
                InstallSaveGuard();
                InstallNativeHooks();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Dynamic hook initialization failed: {ex.Message}");
            }
        }

        private static void InstallSaveGuard()
        {
            try
            {
                gregCore.Infrastructure.Persistence.GregSaveGuard.Install(HarmonyInstance);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] SaveGuard failed: {ex.Message}");
            }
        }

        private void InstallNativeHooks()
        {
            try
            {
                if (EventBus == null || HookBus == null) return;
                var logger = new gregCore.Infrastructure.Logging.ConsoleLogger(LoggerInstance);
                gregCore.GameApi.GregGameModuleHost.Configure(HarmonyInstance, EventBus, logger);
                GregNativeEventHooks.Install(logger, HookBus, EventBus, HarmonyInstance);
                TryInstallModLoaderHooks(logger);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Native hooks failed: {ex.Message}");
            }
        }

        private void TryInstallModLoaderHooks(gregCore.Infrastructure.Logging.ConsoleLogger logger)
        {
            try
            {
                gregCore.GameLayer.Hooks.NativeModLoaderHooks.Install(logger, EventBus, HarmonyInstance);
            }
            catch (Exception hookEx)
            {
                MelonLogger.Error($"[gregCore] NativeModLoaderHooks failed: {hookEx.Message}");
            }
        }

        public override void OnUpdate()
        {
            if (_shutdownRequested) return;
            if (!_lateInitCompleted)
            {
                _lateInitCompleted = true;
                RunLateInit();
            }
            TickNotifications();
            TickKeybinds();
            TickServices();
            TickFonts();
            TickMenus();
            TickModHub();
        }

        private static void RunLateInit()
        {
            DiscoverHostsSafe();
            LoadPluginsSafe();
            LoadNativeSafe();
            CheckApiDriftSafe();
            ActivateLanguagesSafe();
            MelonLogger.Msg("[gregCore] Framework initialization complete.");
        }

        private static void DiscoverHostsSafe()
        {
            try { DiscoverGregExtHosts(); }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] gregExt discovery failed: {ex.Message}"); }
        }

        private static void LoadPluginsSafe()
        {
            try { GregServiceContainer.Get<IGregPluginRegistry>()?.LoadAll(); }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Mod registry activation failed: {ex.Message}"); }
        }

        private static void LoadNativeSafe()
        {
            try
            {
                var nativeService = GregServiceContainer.Get<IGregNativeModService>();
                nativeService?.RefreshGameSnapshot();
                nativeService?.LoadGregNativePlugins();
            }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Native mod activation failed: {ex.Message}"); }
        }

        private static void CheckApiDriftSafe()
        {
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
            catch (Exception ex) { MelonLogger.Warning($"[gregCore] GameApi drift check skipped: {ex.Message}"); }
        }

        private static void ActivateLanguagesSafe()
        {
            try
            {
                var modsDir = System.IO.Path.Combine(global::MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "Mods", "Scripts");
                GregLanguageRegistry.ScanAndActivate(modsDir);
            }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Language host activation failed: {ex.Message}"); }
        }

        private static void TickNotifications()
        {
            try { GregNotificationManager.Update(); }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Notification update failed: {ex.Message}"); }
        }

        private static void TickKeybinds()
        {
            try
            {
                GregServiceContainer.Get<gregCore.Infrastructure.Settings.Services.GregInputBindingService>()?.OnUpdate();
            }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Keybind update failed: {ex.Message}"); }
        }

        private static void TickServices()
        {
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
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Update callback failed: {ex.Message}"); }
        }

        private static void TickFonts()
        {
            try { GregFontLoader.Tick(); }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Font search tick failed: {ex.Message}"); }
        }

        private static void TickMenus()
        {
            try { gregCore.UI.GregMenuRegistry.Tick(Time.deltaTime); }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Menu registry tick failed: {ex.Message}"); }
        }

        private static void TickModHub()
        {
            try
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb != null && kb.f1Key != null && kb.f1Key.wasPressedThisFrame)
                    gregCore.UI.GregModHub.Toggle();
                gregCore.UI.GregModHub.Refresh();
            }
            catch (Exception ex) { MelonLogger.Error($"[gregCore] Mod-Hub tick failed: {ex.Message}"); }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                NotifySceneGuards();
                GregFontLoader.SearchFonts();
                UpdateScenePresence(sceneName);
                GregLanguageRegistry.OnSceneLoaded(sceneName);
                NotifyPluginSceneLoaded(sceneName);
                PublishSceneLoaded(buildIndex, sceneName);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Scene load callback failed: {ex.Message}");
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            try { gregCore.Infrastructure.Networking.GregNetSession.NotifySceneUnloading(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
