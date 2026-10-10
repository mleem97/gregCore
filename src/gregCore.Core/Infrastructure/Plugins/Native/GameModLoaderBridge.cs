/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Observes the game's own Il2Cpp.ModLoader (DllEntry/IModPlugin system).
/// Maintainer:  Read-only access + LoadAllMods reload. No private methods via reflection.
/// </file-summary>

using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Plugins.Native;

public sealed class GameModLoaderBridge
{
    private readonly IGregLogger _logger;

    public GameModLoaderBridge(IGregLogger logger)
    {
        _logger = logger.ForContext("GameModLoaderBridge");
    }

    public bool TryGetLoader(out global::Il2Cpp.ModLoader loader)
    {
        loader = null!;
        try
        {
            var instance = global::Il2Cpp.ModLoader.instance;
            if (instance == null || instance.Pointer == IntPtr.Zero) return false;
            loader = instance;
            return true;
        }
        catch (Exception ex)
        {
            _logger.Warning($"ModLoader.instance not available: {ex.Message}");
            return false;
        }
    }

    public GameModLoaderSnapshot Refresh()
    {
        if (!TryGetLoader(out var loader))
            return new GameModLoaderSnapshot { IsAvailable = false };

        var snapshots = new List<NativePluginSnapshot>();
        var nextModId = -1;
        try
        {
            var plugins = loader.loadedPlugins;
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    try
                    {
                        if (plugin == null || plugin.Pointer == IntPtr.Zero) continue;
                        var managedType = plugin.GetType();
                        snapshots.Add(new NativePluginSnapshot
                        {
                            TypeName = managedType.FullName ?? "unknown",
                            AssemblyName = managedType.Assembly.GetName().Name ?? "unknown"
                        });
                    }
                    catch { /* skip single plugin */ }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Warning($"loadedPlugins could not be read: {ex.Message}");
        }

        try { nextModId = loader.nextModID; }
        catch { /* optional */ }

        return new GameModLoaderSnapshot
        {
            IsAvailable = true,
            LoadedPluginCount = snapshots.Count,
            NextModId = nextModId,
            LoadedPlugins = snapshots
        };
    }

    /// <summary>
    /// Asks the game loader to load all ModPacks again.
    /// Loads mods at game-start time; no real hot-reload of individual DLLs.
    /// </summary>
    public bool RequestReload()
    {
        if (!TryGetLoader(out var loader)) return false;
        try
        {
            loader.LoadAllMods();
            _logger.Info("Game ModLoader reload requested (LoadAllMods).");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error("Game ModLoader reload failed", ex);
            return false;
        }
    }
}
