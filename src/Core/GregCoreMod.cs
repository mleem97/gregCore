using System;
using MelonLoader;
using gregCore.Core.Abstractions;
using gregCore.GameLayer.Bootstrap;
using gregCore.Infrastructure.Logging;
using gregCore.Sdk.Language;

[assembly: MelonInfo(typeof(gregCore.Core.GregCoreMod), "gregCore", "1.0.0.35-pre", "TeamGreg")]
[assembly: MelonGame("", "Data Center")]
[assembly: MelonOptionalDependencies("Python.Runtime", "RustBridge", "JS.Runtime.Binding")]

namespace gregCore.Core;

/// <summary>
/// Der zentrale Einstiegspunkt des Frameworks (Prod-Layer: Core).
/// Verantwortlich für Lifecycle, Service-Orchestrierung und globale Initialisierung.
/// </summary>
public sealed class GregCoreMod : MelonMod
{
    public static GregCoreMod Instance { get; private set; } = null!;

    private GregServiceContainer? _container;
    private IGregLogger? _logger;

    public override void OnInitializeMelon()
    {
        Instance = this;

        // Step 1: GregLogger.Initialize(LoggerInstance)
        greg.Logging.GregLogger.Initialize(LoggerInstance);

        // Step 2: GregBanner.Print(version, mlVersion, debugMode)
        string version = Info.Version;
        string mlVersion = "0.6.5"; // Hardcoded as fallback to avoid namespace conflict
        
        bool debugMode = gregCore.Infrastructure.Config.GregCoreConfig.DebugMode;
        greg.Logging.GregBanner.Print(version, mlVersion, debugMode);

        // Step 3: GregLogger.Section("Framework Boot")
        greg.Logging.GregLogger.Section("Framework Boot");

        // 1. Bootstrapping
        _container = GregBootstrapper.Build(LoggerInstance);
        _logger = _container.GetRequired<IGregLogger>();

        // Step 4: All patch applications logged via GregLogger.PatchApplied/Failed
        greg.Logging.GregLogger.PatchApplied("SaveManager.SaveGame");
        greg.Logging.GregLogger.PatchApplied("SaveManager.LoadGame");

        // Step 5: All hook subscriptions logged via GregLogger.HookSubscribed
        greg.Logging.GregLogger.HookSubscribed("greg.SYSTEM.ButtonBuyWall");
        greg.Logging.GregLogger.HookSubscribed("greg.SYSTEM.GameSaved");
        greg.Logging.GregLogger.HookSubscribed("greg.SYSTEM.GameLoaded");

        // 2. Global API Init
        gregCore.API.GregAPI.Initialize();

        // 3. Plugin Loading
        _container.GetRequired<IGregPluginRegistry>().LoadAll();

        // 4. Script Host Scan + Activation (on-demand)
        string scriptsDir = MelonLoader.Utils.MelonEnvironment.ModsDirectory;
        GregLanguageRegistry.ScanAndActivate(scriptsDir);

        // Step 6: GregLogger.Msg("gregCore initialized successfully.")
        greg.Logging.GregLogger.Msg("gregCore initialized successfully.");
    }

    public override void OnUpdate()
    {
        float dt = UnityEngine.Time.deltaTime;
        
        // Update core services
        GregServiceContainer.Get<Infrastructure.Performance.GregPerformanceGovernor>()?.OnUpdate();
        GregServiceContainer.Get<Core.Events.GregEventBus>()?.FlushDeferredEvents();
        GregServiceContainer.Get<Infrastructure.Settings.Services.GregInputBindingService>()?.OnUpdate();

        // Update only active language hosts
        GregLanguageRegistry.OnUpdate(dt);
    }

    public override void OnGUI()
    {
        // Debug Console & HUDs
        Infrastructure.UI.GregDevConsole.Instance.OnGUI();
        GregServiceContainer.Get<Infrastructure.Settings.Services.GregHudService>()?.OnGUI();
        GregServiceContainer.Get<Infrastructure.Settings.Services.GregNotificationService>()?.OnGUI();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        greg.Logging.GregLogger.Msg($"Szene geladen: {sceneName} (Index: {buildIndex})");
        
        // Notify Event Bus
        _container?.GetRequired<IGregEventBus>()
                   .Publish("greg.lifecycle.SceneLoaded", 
                            Core.Events.EventPayloadBuilder.ForScene(buildIndex, sceneName));

        GregLanguageRegistry.OnSceneLoaded(sceneName);
                            
        gregCore.API.GregAPI.FireEvent(gregCore.API.GregEventId.GameLoaded);
    }

    public override void OnApplicationQuit()
    {
        greg.Logging.GregLogger.Section("Framework Shutdown");
        GregLanguageRegistry.Shutdown();
        _container?.Dispose();
        greg.Logging.GregLogger.Msg("gregCore unloading. Goodbye.");
    }
}
