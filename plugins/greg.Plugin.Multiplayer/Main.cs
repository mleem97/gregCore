using System;
using DataCenterModLoader;
using FrikaMF;
using FrikaMF.Plugins;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(greg.Plugin.Multiplayer.Main), "greg.Plugin.Multiplayer", ReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Plugin.Multiplayer;

/// <summary>
/// Standalone plugin hosting multiplayer bridge and plugin synchronization runtime.
/// </summary>
public sealed class Main : GregPluginBase
{
    private MultiplayerBridge _multiplayerBridge;
    private PluginSyncService _pluginSyncService;
    private bool _frameworkReady;

    /// <inheritdoc />
    public override string PluginId => "greg.Plugin.Multiplayer";

    /// <inheritdoc />
    public override string DisplayName => "FrikaMF Multiplayer Plugin";

    /// <inheritdoc />
    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(ReleaseVersion.Current);

    /// <inheritdoc />
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        if (Core.Instance != null)
            EnsureFrameworkReady();
    }

    /// <inheritdoc />
    public override void OnFrameworkReady()
    {
        EnsureFrameworkReady();
    }

    /// <inheritdoc />
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _multiplayerBridge?.OnSceneLoaded(sceneName);
    }

    /// <inheritdoc />
    public override void OnUpdate()
    {
        _multiplayerBridge?.OnUpdate(Time.deltaTime);
        _pluginSyncService?.Tick(Time.time);
    }

    /// <inheritdoc />
    public override void OnGUI()
    {
        _multiplayerBridge?.DrawGUI();
    }

    /// <inheritdoc />
    public override void OnApplicationQuit()
    {
        try
        {
            _multiplayerBridge?.Shutdown();
            Core.UnregisterMultiplayerBridge(_multiplayerBridge);
        }
        catch (Exception exception)
        {
            MelonLogger.Error($"greg.Plugin.Multiplayer shutdown failed: {exception.Message}");
        }
    }

    private void EnsureFrameworkReady()
    {
        if (_frameworkReady)
            return;

        _frameworkReady = true;

        MelonLogger.Msg("greg.Plugin.Multiplayer initializing runtime bridge and sync service.");

        _multiplayerBridge = new MultiplayerBridge(LoggerInstance);
        _pluginSyncService = new PluginSyncService(LoggerInstance);
        _pluginSyncService.Initialize();

        Core.RegisterMultiplayerBridge(_multiplayerBridge);
    }

    private static Version ParseFrameworkVersion(string version)
    {
        return Version.TryParse(version, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
    }
}
