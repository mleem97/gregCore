using System;
using gregModLoader;
using gregModLoader;
using gregModLoader.Plugins;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(greg.Plugin.Multiplayer.gregMain), "greg.Plugin.Multiplayer", gregReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Plugin.Multiplayer;

/// <summary>
/// Standalone plugin hosting multiplayer bridge and plugin synchronization runtime.
/// </summary>
public sealed class gregMain : gregPluginBase
{
    private gregMultiplayerBridge _multiplayerBridge;
    private PluginSyncService _pluginSyncService;
    private bool _frameworkReady;

    /// <inheritdoc />
    public override string PluginId => "greg.Plugin.Multiplayer";

    /// <inheritdoc />
    public override string DisplayName => "gregCore Multiplayer Plugin";

    /// <inheritdoc />
    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(gregReleaseVersion.Current);

    /// <inheritdoc />
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        if (gregCoreLoader.Instance != null)
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
            gregCoreLoader.UnregisterMultiplayerBridge(_multiplayerBridge);
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

        _multiplayerBridge = new gregMultiplayerBridge(LoggerInstance);
        _pluginSyncService = new PluginSyncService(LoggerInstance);
        _pluginSyncService.Initialize();

        gregCoreLoader.RegisterMultiplayerBridge(_multiplayerBridge);
    }

    private static Version ParseFrameworkVersion(string version)
    {
        return Version.TryParse(version, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
    }
}




