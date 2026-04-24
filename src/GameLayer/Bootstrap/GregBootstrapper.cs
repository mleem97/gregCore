/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Erstellt und konfiguriert den GregServiceContainer.
/// Maintainer:   Einzige Stelle wo Implementierungen an Interfaces gebunden werden. Validiert den Startup.
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
    public static GregServiceContainer Build(global::MelonLoader.MelonLogger.Instance melonLogger)
    {
        // Initialize Assembly Resolver for subfolder dependencies
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
            try {
                var name = new AssemblyName(args.Name).Name;
                var depDir = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.ModsDirectory, "gregDependencies");
                var targetPath = Path.Combine(depDir, name + ".dll");
                
                if (File.Exists(targetPath)) {
                    return Assembly.LoadFrom(targetPath);
                }
            } catch { }
            return null;
        };

        var container = new GregServiceContainer();

        // Initialize static Logger
        var logger = new ConsoleLogger(melonLogger);

        container.Register<IGregLogger>(logger);

        greg.Logging.GregLogger.Initialize(melonLogger);
        greg.Logging.GregLogger.Msg("Bootstrapper starting...", "BOOT");

        // Note: The banner is printed in GregCoreMod.cs now as requested
        /*
        greg.Logging.GregLogger.Box(new[] {
            "gregCore v1.1.0 (Production-Grade)",
            "MelonLoader Framework initialized",
            "PRO-Edition Active"
        });
        */

        var bus = new GregEventBus(logger);
        var hookBus = new GregHookBus(logger);
        var catalog = new Sdk.Metadata.GregHookCatalog();
        var catalogService = new Sdk.Services.GregHookCatalogService(logger, catalog);
        catalogService.Initialize();

        var validationService = new Core.Services.GregValidationService(logger);

        container.Register<IGregEventBus>(bus);
        container.Register<GregHookBus>(hookBus);
        container.Register<Sdk.Metadata.GregHookCatalog>(catalog);
        container.Register<Sdk.Services.GregHookCatalogService>(catalogService);
        container.Register<Core.Services.GregValidationService>(validationService);
        container.Register<IGregConfigService>(new GregConfigService(logger));
        container.Register<IGregPersistenceService>(new GregPersistenceService(logger));
        container.Register<IGregHookRegistry>(new GregHookRegistry(logger));

        // --- Settings Subsystem ---
        var keybindRegistry = new GregKeybindRegistry(logger);
        var modSettingsService = new GregModSettingsService(logger);

        var settingsPersistence = new GregSettingsPersistenceService(logger, keybindRegistry, modSettingsService, bus);
        modSettingsService.SetPersistence(settingsPersistence); // Link back for lazy injection
        settingsPersistence.Load();

        var settingsConflict = new GregSettingsConflictService(logger, keybindRegistry, modSettingsService);
        var inputBinding = new GregInputBindingService(logger, keybindRegistry);
        inputBinding.SetPersistence(settingsPersistence);

        var pluginRegistry = new GregPluginRegistry(new AssemblyScanner(), logger, bus);

        var uiBridge = new GregSettingsUiBridge(logger, modSettingsService, keybindRegistry, inputBinding, pluginRegistry);
        var hudService = new GregHudService(logger, keybindRegistry);
        var notificationService = new GregNotificationService(logger);

        var sdkApi = new Sdk.GregAPI(logger, hookBus, modSettingsService, keybindRegistry, pluginRegistry, notificationService, validationService);

        container.Register<GregKeybindRegistry>(keybindRegistry);
        container.Register<GregModSettingsService>(modSettingsService);
        container.Register<GregSettingsPersistenceService>(settingsPersistence);
        container.Register<GregSettingsConflictService>(settingsConflict);
        container.Register<GregInputBindingService>(inputBinding);
        container.Register<IGregPluginRegistry>(pluginRegistry);
        container.Register<GregSettingsUiBridge>(uiBridge);
        container.Register<GregHudService>(hudService);
        container.Register<GregNotificationService>(notificationService);
        container.Register<Sdk.IGregAPI>(sdkApi);

        // --- Harmony Initialization ---
        Hooks.GregNativeEventHooks.Install(logger, hookBus);

        // Link globally for legacy/mod compatibility
        gregCore.API.GregAPI._keybindReg = keybindRegistry;
        gregCore.API.GregAPI._modSettingsService = modSettingsService;
        // --------------------------

        var apiContext = new global::gregCore.PublicApi.GregApiContext {
            Logger = logger,
            EventBus = bus,
            HookBus = hookBus,
            Config = container.GetRequired<IGregConfigService>(),
            Persist = container.GetRequired<IGregPersistenceService>()
        };

        var governor = new gregCore.Infrastructure.Performance.GregPerformanceGovernor(apiContext);
        container.Register<gregCore.Infrastructure.Performance.GregPerformanceGovernor>(governor);
        bus.SetGovernor(governor);

        container.Register<IGregFfiBridge>(new Win32FfiBridge(logger, bus));

        container.Register<IGregLanguageBridge>("lua", new LuaBridge(logger, bus));
        container.Register<IGregLanguageBridge>("js", new JsBridge(logger, bus));

        container.Register<IAssemblyScanner>(new AssemblyScanner());

        HookIntegration.Install(bus, true);
        global::gregCore.PublicApi.greg._context = apiContext;
        global::gregCore.PublicApi.greg._governor = governor;

        ValidateStartup(container);

        logger.Info("Alle Services registriert");

        return container;
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
                $"Assembly-CSharp.dll nicht gefunden: {gameAssembly}\n" +
                "Stelle sicher dass MelonLoader korrekt installiert ist.");

        var pluginDir = Path.Combine(gameRoot, "Mods");
        if (!Directory.Exists(pluginDir))
            Directory.CreateDirectory(pluginDir);
    }
}
