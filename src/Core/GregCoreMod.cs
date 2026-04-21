using System;
using MelonLoader;
using gregCore.Core.Abstractions;
using gregCore.GameLayer.Bootstrap;
using gregCore.Infrastructure.Logging;
using gregCore.Sdk.Language;

[assembly: MelonInfo(typeof(gregCore.Core.GregCoreMod), "gregCore", "1.1.0", "TeamGreg")]
[assembly: MelonGame("", "Data Center")]
[assembly: MelonOptionalDependencies("Python.Runtime", "RustBridge", "JS.Runtime.Binding")]

namespace gregCore.Core;

/// <summary>
/// Der zentrale Einstiegspunkt des Frameworks (Prod-Layer: Core).
/// Verantwortlich für Lifecycle, Service-Orchestrierung und globale Initialisierung.
/// </summary>
public sealed class GregCoreMod : MelonMod
{
    private GregServiceContainer? _container;
    private IGregLogger? _logger;

    public override void OnInitializeMelon()
    {
        // 1. Bootstrapping
        _container = GregBootstrapper.Build(LoggerInstance);
        _logger = _container.GetRequired<IGregLogger>();

        _logger.Info("gregCore Core-Modus wird initialisiert...");

        // 2. Global API Init
        gregCore.API.GregAPI.Initialize();

        // 3. Plugin Loading
        _container.GetRequired<IGregPluginRegistry>().LoadAll();

        // 4. Script Host Scan + Activation (on-demand)
        string scriptsDir = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.ModsDirectory, "Scripts");
        GregLanguageRegistry.ScanAndActivate(scriptsDir);

        _logger.Success("gregCore v1.1.0 (Production-Grade) erfolgreich geladen.");
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
        _logger?.Info($"Szene geladen: {sceneName} (Index: {buildIndex})");
        
        // Notify Event Bus
        _container?.GetRequired<IGregEventBus>()
                   .Publish("greg.lifecycle.SceneLoaded", 
                            Core.Events.EventPayloadBuilder.ForScene(buildIndex, sceneName));

        GregLanguageRegistry.OnSceneLoaded(sceneName);
                            
        gregCore.API.GregAPI.FireEvent(gregCore.API.GregEventId.GameLoaded);
    }

    public override void OnApplicationQuit()
    {
        _logger?.Info("gregCore wird beendet...");
        GregLanguageRegistry.Shutdown();
        _container?.Dispose();
    }
}
