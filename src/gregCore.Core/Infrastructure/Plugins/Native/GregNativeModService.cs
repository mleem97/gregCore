/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Facade over game ModLoader observation and GregCore-native plugins.
/// Maintainer:  Single entry point for native mod management.
/// </file-summary>

using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Plugins.Native;

public sealed class GregNativeModService : IGregNativeModService
{
    private readonly GameModLoaderBridge _bridge;
    private readonly GregNativePluginLoader _loader;
    private readonly IGregLogger _logger;
    private readonly string _nativeDir;

    public GregNativeModService(GameModLoaderBridge bridge, GregNativePluginLoader loader, IGregLogger logger, string nativeDir)
    {
        _bridge = bridge;
        _loader = loader;
        _logger = logger.ForContext("NativeModService");
        _nativeDir = nativeDir;
    }

    public GameModLoaderSnapshot RefreshGameSnapshot()
    {
        var snapshot = _bridge.Refresh();
        _logger.Info($"Game-ModLoader: available={snapshot.IsAvailable}, plugins={snapshot.LoadedPluginCount}");
        return snapshot;
    }

    public IReadOnlyList<GregNativePluginSnapshot> GetGregNativePlugins() => _loader.GetLoaded();

    public int LoadGregNativePlugins()
    {
        try
        {
            if (!Directory.Exists(_nativeDir)) Directory.CreateDirectory(_nativeDir);
            var count = _loader.LoadAll(_nativeDir);
            _logger.Info($"{count} Greg-native plugins loaded ({_nativeDir}).");
            return count;
        }
        catch (Exception ex)
        {
            _logger.Error("Greg-native plugin loading failed", ex);
            return 0;
        }
    }

    public bool UnloadGregNativePlugin(string id) => _loader.Unload(id);

    public bool RequestGameReload() => _bridge.RequestReload();

    public void Shutdown() => _loader.Shutdown();
}
