/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        MelonMod Entry Point.
/// Maintainer:   So klein wie möglich halten. Max 50 Zeilen. Kein Business-Logic.
/// </file-summary>

using MelonLoader;

[assembly: MelonInfo(typeof(gregCore.GameLayer.Bootstrap.GregCoreLoader), "gregCore", "1.0.0", "TeamGreg")]
[assembly: MelonGame("", "Data Center")]

namespace gregCore.GameLayer.Bootstrap;

public sealed class GregCoreLoader : MelonMod
{
    private GregServiceContainer? _container;
    private IGregLogger? _logger;

    public override void OnInitializeMelon()
    {
        _container = GregBootstrapper.Build(LoggerInstance);
        _logger = _container.GetRequired<IGregLogger>();
        _container.GetRequired<IGregPluginRegistry>().LoadAll();
    }

    public override void OnUpdate()
    {
        _container?.Get<gregCore.Infrastructure.Performance.GregPerformanceGovernor>()?.OnUpdate();
        _container?.Get<GregEventBus>()?.FlushDeferredEvents();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) =>
        _container?.GetRequired<IGregEventBus>()
                   .Publish(HookName.Create("lifecycle", "SceneLoaded").Full,
                            EventPayloadBuilder.ForScene(buildIndex, sceneName));

    public override void OnApplicationQuit()
    {
        if (_container == null || _logger == null) return;
        
        _logger.Info("Führe Graceful Shutdown durch...");
        _container.Dispose();
        _logger.Info("Shutdown abgeschlossen.");
    }
}
