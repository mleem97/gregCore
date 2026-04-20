/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Erstellt und konfiguriert den GregServiceContainer.
/// Maintainer:   Einzige Stelle wo Implementierungen an Interfaces gebunden werden. Validiert den Startup.
/// </file-summary>

using MelonLoader;
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
    public static GregServiceContainer Build(MelonLogger.Instance melonLogger)
    {
        var container = new GregServiceContainer();
        var logger = new MelonLoggerAdapter(melonLogger);
        
        container.Register<IGregLogger>(logger);
        logger.Info("gregCore v1.0.0 Bootstrap gestartet");
        
        container.Register<IGregEventBus>(new GregEventBus(logger));
        container.Register<IGregConfigService>(new GregConfigService(logger));
        container.Register<IGregPersistenceService>(new GregPersistenceService(logger));

        container.Register<IGregFfiBridge>(new Win32FfiBridge(logger, container.GetRequired<IGregEventBus>()));

        container.Register<IGregLanguageBridge>("lua", new LuaBridge(logger, container.GetRequired<IGregEventBus>()));
        container.Register<IGregLanguageBridge>("js", new JsBridge(logger, container.GetRequired<IGregEventBus>()));

        container.Register<IAssemblyScanner>(new AssemblyScanner());
        container.Register<IGregPluginRegistry>(new GregPluginRegistry(container.GetRequired<IAssemblyScanner>(), logger, container.GetRequired<IGregEventBus>()));

        HookIntegration.Install(container.GetRequired<IGregEventBus>(), logger);

        ValidateStartup(container);
        
        logger.Info("Alle Services registriert");

        return container;
    }
    
    private static void ValidateStartup(GregServiceContainer container)
    {
        container.GetRequired<IGregLogger>();
        container.GetRequired<IGregEventBus>();
        container.GetRequired<IGregFfiBridge>();

        var melonVersion = MelonLoader.MelonLoader.Version;
        if (melonVersion < new Version(0, 6, 0))
            throw new GregInitException($"MelonLoader >= 0.6.0 erforderlich, gefunden: {melonVersion}");

        var gameRoot = MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        if (string.IsNullOrEmpty(gameRoot)) return; // Could be null in tests

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
