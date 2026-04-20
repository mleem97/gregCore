/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Erstellt und konfiguriert den GregServiceContainer.
/// Maintainer:   Einzige Stelle wo Implementierungen an Interfaces gebunden werden. Validiert den Startup.
/// </file-summary>

using gregCore.Infrastructure.Logging;
using gregCore.Infrastructure.Config;
using gregCore.Infrastructure.Ffi;
using gregCore.Infrastructure.Plugins;
using gregCore.Infrastructure.Scripting.Lua;
using gregCore.Infrastructure.Scripting.Js;
using gregCore.GameLayer.Hooks;

namespace gregCore.GameLayer.Bootstrap;

internal static class GregBootstrapper
{
    public static GregServiceContainer Build(global::MelonLoader.MelonLogger.Instance melonLogger)
    {
        var container = new GregServiceContainer();
        var logger = new MelonLoggerAdapter(melonLogger);
        
        container.Register<IGregLogger>(logger);
        logger.Info("gregCore v1.0.0 Bootstrap gestartet");
        
        var bus = new GregEventBus(logger);
        container.Register<IGregEventBus>(bus);
        container.Register<IGregConfigService>(new GregConfigService(logger));
        container.Register<IGregPersistenceService>(new GregPersistenceService(logger));

        var apiContext = new global::gregCore.PublicApi.GregApiContext {
            Logger = logger,
            EventBus = bus,
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
        container.Register<IGregPluginRegistry>(new GregPluginRegistry(container.GetRequired<IAssemblyScanner>(), logger, bus));

        HookIntegration.Install(bus, logger);
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
