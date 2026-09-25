/// <file-summary>
/// Layer:       GameLayer
/// Purpose:     Creates and configures the GregServiceContainer.
/// Maintainer:  Single place where implementations are bound to interfaces. Validates startup.
/// </file-summary>

using System;
using System.IO;
using System.Reflection;
using gregCore.Infrastructure.Logging;
using gregCore.Infrastructure.Config;
using gregCore.Infrastructure.Ffi;
using gregCore.Infrastructure.Plugins;
using gregCore.Infrastructure.Scripting.Lua;
using gregCore.Infrastructure.Scripting.Js;
using gregCore.GameLayer.Hooks;
using gregCore.Core.Abstractions;
using gregCore.Infrastructure.Settings;
using gregCore.Infrastructure.Settings.Services;

namespace gregCore.GameLayer.Bootstrap;

    internal static class GregBootstrapper
    {
        private static string[] _manifestLibraryFolders = Array.Empty<string>();

        public static IReadOnlyList<string> ManifestLibraryFolders => _manifestLibraryFolders;

        public static GregServiceContainer Build(global::MelonLoader.MelonLogger.Instance melonLogger)
        {
            InstallAssemblyResolver();
            LoadManifestFolders(melonLogger);
            var container = new GregServiceContainer();
            var logger = new ConsoleLogger(melonLogger);
            container.Register<IGregLogger>(logger);
            greg.Logging.GregLogger.Initialize(melonLogger);
            greg.Logging.GregLogger.Msg("Bootstrapper starting...", "BOOT");
            var core = CreateCore(logger);
            RegisterCore(container, logger, core);
            var settings = CreateSettings(logger, core.Bus);
            RegisterSettings(container, settings);
            RegisterNative(container, logger, core.Bus);
            RegisterUi(container, logger, settings, core);
            var apiContext = CreateApiContext(container, logger, core);
            settings.PluginRegistry.Configure(apiContext);
            RegisterRuntime(container, logger, core, apiContext);
            ValidateStartup(container);
            logger.Info("All services registered");
            return container;
        }

        private static void InstallAssemblyResolver()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => ResolveDependency(args);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        private static Assembly ResolveDependency(ResolveEventArgs args)
        {
            try
            {
                var name = new AssemblyName(args.Name).Name;
                var depDir = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.ModsDirectory, "gregDependencies");
                var targetPath = Path.Combine(depDir, name + ".dll");
                if (File.Exists(targetPath)) return Assembly.LoadFrom(targetPath);
                foreach (var libDir in _manifestLibraryFolders)
                {
                    try
                    {
                        var candidate = Path.Combine(libDir, name + ".dll");
                        if (global::gregCore.Infrastructure.IO.GregDeactivatedGuard.IsDeactivatedPath(candidate))
                            continue;
                        if (File.Exists(candidate))
                            return Assembly.LoadFrom(candidate);
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  /* per-folder best-effort */ }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return null;
        }

        private static void LoadManifestFolders(global::MelonLoader.MelonLogger.Instance melonLogger)
        {
            try
            {
                var validation = gregCore.Core.Mods.GregModpackManifest.TryLoad(
                    global::MelonLoader.Utils.MelonEnvironment.ModsDirectory);
                _manifestLibraryFolders = validation.LibraryFolders.ToArray();
                foreach (var warn in validation.Warnings.Take(10))
                {
                    try { melonLogger.Warning($"[gregCore][Manifest] {warn}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                }
                if (validation.Found)
                {
                    try { melonLogger.Msg($"[gregCore][Manifest] '{validation.Manifest.Name}': " +
                        $"{validation.ValidMods.Count} mods, {validation.ValidLibrary.Count} libs, " +
                        $"{validation.ValidPlugins.Count} plugins."); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  /* manifest is optional */ }
        }

        private sealed class CoreBundle
        {
            public GregEventBus Bus;
            public GregHookBus HookBus;
            public Sdk.Metadata.GregHookCatalog Catalog;
            public Sdk.Services.GregHookCatalogService CatalogService;
            public Core.Services.GregValidationService Validation;
        }

        private static CoreBundle CreateCore(ConsoleLogger logger)
        {
            var bus = new GregEventBus(logger);
            var hookBus = new GregHookBus(logger);
            var catalog = new Sdk.Metadata.GregHookCatalog();
            var catalogService = new Sdk.Services.GregHookCatalogService(logger, catalog);
            catalogService.Initialize();
            return new CoreBundle { Bus = bus, HookBus = hookBus, Catalog = catalog, CatalogService = catalogService, Validation = new Core.Services.GregValidationService(logger) };
        }

        private static void RegisterCore(GregServiceContainer container, ConsoleLogger logger, CoreBundle core)
        {
            try
            {
                container.Register<IGregEventBus>(core.Bus);
                container.Register<GregEventBus>(core.Bus);
                container.Register<GregHookBus>(core.HookBus);
                container.Register<Sdk.Metadata.GregHookCatalog>(core.Catalog);
                container.Register<Sdk.Services.GregHookCatalogService>(core.CatalogService);
                container.Register<Core.Services.GregValidationService>(core.Validation);
                container.Register<IGregConfigService>(new GregConfigService(logger));
                container.Register<IGregPersistenceService>(new GregPersistenceService(logger));
                container.Register<IGregHookRegistry>(new GregHookRegistry(logger));
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        private sealed class SettingsBundle
        {
            public GregKeybindRegistry Keybinds;
            public GregModSettingsService ModSettings;
            public GregSettingsPersistenceService Persistence;
            public GregSettingsConflictService Conflicts;
            public GregInputBindingService InputBinding;
            public GregPluginRegistry PluginRegistry;
        }

        private static SettingsBundle CreateSettings(ConsoleLogger logger, GregEventBus bus)
        {
            var keybindRegistry = new GregKeybindRegistry(logger);
            var modSettingsService = new GregModSettingsService(logger);
            var settingsPersistence = new GregSettingsPersistenceService(logger, keybindRegistry, modSettingsService, bus);
            modSettingsService.SetPersistence(settingsPersistence);
            settingsPersistence.Load();
            var settingsConflict = new GregSettingsConflictService(logger, keybindRegistry, modSettingsService);
            var inputBinding = new GregInputBindingService(logger, keybindRegistry);
            inputBinding.SetPersistence(settingsPersistence);
            var pluginRegistry = new GregPluginRegistry(new AssemblyScanner(), logger, bus);
            return new SettingsBundle { Keybinds = keybindRegistry, ModSettings = modSettingsService, Persistence = settingsPersistence, Conflicts = settingsConflict, InputBinding = inputBinding, PluginRegistry = pluginRegistry };
        }

        private static void RegisterSettings(GregServiceContainer container, SettingsBundle s)
        {
            try
            {
                container.Register<GregKeybindRegistry>(s.Keybinds);
                container.Register<GregModSettingsService>(s.ModSettings);
                container.Register<GregSettingsPersistenceService>(s.Persistence);
                container.Register<GregSettingsConflictService>(s.Conflicts);
                container.Register<GregInputBindingService>(s.InputBinding);
                container.Register<IGregPluginRegistry>(s.PluginRegistry);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        private static void RegisterNative(GregServiceContainer container, ConsoleLogger logger, GregEventBus bus)
        {
            try
            {
                var nativeDir = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.ModsDirectory, "gregNative");
                var gameModLoaderBridge = new gregCore.Infrastructure.Plugins.Native.GameModLoaderBridge(logger);
                var gregNativePluginLoader = new gregCore.Infrastructure.Plugins.Native.GregNativePluginLoader(logger, bus);
                var nativeModService = new gregCore.Infrastructure.Plugins.Native.GregNativeModService(
                    gameModLoaderBridge, gregNativePluginLoader, logger, nativeDir);
                container.Register<Core.Abstractions.IGregNativeModService>(nativeModService);
                container.Register<gregCore.Infrastructure.Plugins.Native.GregNativeModService>(nativeModService);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        private static void RegisterUi(GregServiceContainer container, ConsoleLogger logger, SettingsBundle s, CoreBundle core)
        {
            try
            {
                var uiBridge = new GregSettingsUiBridge(logger, s.ModSettings, s.Keybinds, s.InputBinding, s.PluginRegistry);
                var hudService = new GregHudService(logger, s.Keybinds);
                var notificationService = new GregNotificationService(logger);
                var sdkApi = new Sdk.GregAPI(logger, core.HookBus, s.ModSettings, s.Keybinds, s.PluginRegistry, notificationService, core.Validation);
                container.Register<GregSettingsUiBridge>(uiBridge);
                container.Register<GregHudService>(hudService);
                container.Register<GregNotificationService>(notificationService);
                container.Register<Sdk.IGregAPI>(sdkApi);
                var harmony = new HarmonyLib.Harmony("gregCore.bootstrapper");
                Hooks.GregNativeEventHooks.Install(logger, core.HookBus, core.Bus, harmony);
                gregCore.API.GregAPI._keybindReg = s.Keybinds;
                gregCore.API.GregAPI._modSettingsService = s.ModSettings;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

        private static global::gregCore.PublicApi.GregApiContext CreateApiContext(GregServiceContainer container, ConsoleLogger logger, CoreBundle core)
        {
            var lifetime = new CancellationTokenSource();
            return new global::gregCore.PublicApi.GregApiContext {
                Logger = logger,
                EventBus = core.Bus,
                HookBus = core.HookBus,
                Config = container.GetRequired<IGregConfigService>(),
                Persist = container.GetRequired<IGregPersistenceService>(),
                Events = new global::gregCore.PublicApi.GregEventBusPublic(core.Bus),
                MainThread = new global::gregCore.PublicApi.GregMainThreadDispatcher(),
                Resources = new global::gregCore.PublicApi.GregResourceRegistry(),
                CancellationToken = lifetime.Token,
                LifetimeSource = lifetime
            };
        }

        private static void RegisterRuntime(GregServiceContainer container, ConsoleLogger logger, CoreBundle core, global::gregCore.PublicApi.GregApiContext apiContext)
        {
            try
            {
                var governor = new gregCore.Infrastructure.Performance.GregPerformanceGovernor(apiContext);
                container.Register<gregCore.Infrastructure.Performance.GregPerformanceGovernor>(governor);
                core.Bus.SetGovernor(governor);
                container.Register<IGregFfiBridge>(new Win32FfiBridge(logger, core.Bus));
                container.Register<IGregLanguageBridge>("lua", new LuaBridge(logger, core.Bus));
                container.Register<IGregLanguageBridge>("js", new JsBridge(logger, core.Bus));
                container.Register<IAssemblyScanner>(new AssemblyScanner());
                HookIntegration.Install(core.Bus, true);
                global::gregCore.PublicApi.greg._context = apiContext;
                global::gregCore.PublicApi.greg._governor = governor;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }

    private static void ValidateStartup(GregServiceContainer container)
    {
        container.GetRequired<IGregLogger>();
        container.GetRequired<IGregEventBus>();
        container.GetRequired<IGregFfiBridge>();

        // Workaround for MelonLoader naming conflict
        var melonVersion = typeof(global::MelonLoader.MelonLogger).Assembly.GetName().Version;
        if (melonVersion != null && melonVersion < new Version(0, 6, 0))
            throw new GregInitException($"MelonLoader >= 0.6.0 erforderlich, gefunden: {melonVersion}");

        var gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        if (string.IsNullOrEmpty(gameRoot)) return;

        var gameAssembly = Path.Combine(gameRoot, "MelonLoader", "Il2CppAssemblies", "Assembly-CSharp.dll");

        if (!File.Exists(gameAssembly))
            throw new GregInitException(
                $"Assembly-CSharp.dll not found: {gameAssembly}\n" +
                "Stelle sicher dass MelonLoader korrekt installiert ist.");

        var pluginDir = Path.Combine(gameRoot, "Mods");
        if (!Directory.Exists(pluginDir))
            Directory.CreateDirectory(pluginDir);
    }
}
