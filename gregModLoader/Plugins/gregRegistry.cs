using System;
using System.Collections.Generic;
using MelonLoader;

namespace gregModLoader.Plugins;

/// <summary>
/// Central runtime registry for standalone gregCore plugins.
/// </summary>
public static class gregRegistry
{
    private static readonly Dictionary<string, gregPluginBase> Plugins = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object SyncRoot = new();

    /// <summary>
    /// Returns all currently registered plugins by plugin ID.
    /// </summary>
    public static IReadOnlyDictionary<string, gregPluginBase> RegisteredPlugins => Plugins;

    /// <summary>
    /// Registers a plugin instance with the framework.
    /// </summary>
    /// <param name="plugin">Plugin instance to register.</param>
    public static void RegisterPlugin(gregPluginBase plugin)
    {
        if (plugin == null)
            throw new ArgumentNullException(nameof(plugin));

        if (string.IsNullOrWhiteSpace(plugin.PluginId))
            throw new ArgumentException("PluginId must not be empty.", nameof(plugin));

        lock (SyncRoot)
        {
            if (Plugins.TryGetValue(plugin.PluginId, out gregPluginBase existing))
            {
                if (ReferenceEquals(existing, plugin))
                    return;

                MelonLogger.Warning($"gregRegistry duplicate PluginId '{plugin.PluginId}' ignored.");
                return;
            }

            Plugins[plugin.PluginId] = plugin;
        }

        MelonLogger.Msg($"gregRegistry registered plugin: {plugin.PluginId} ({plugin.DisplayName})");
    }

    /// <summary>
    /// Notifies all compatible plugins that gregCoreLoader initialization is complete.
    /// </summary>
    public static void NotifyFrameworkReady()
    {
        gregPluginBase[] snapshot;
        lock (SyncRoot)
        {
            snapshot = new gregPluginBase[Plugins.Count];
            Plugins.Values.CopyTo(snapshot, 0);
        }

        Version currentVersion = GetCurrentFrameworkVersion();
        for (int pluginIndex = 0; pluginIndex < snapshot.Length; pluginIndex++)
        {
            gregPluginBase plugin = snapshot[pluginIndex];
            if (plugin == null)
                continue;

            if (plugin.RequiredFrameworkVersion > currentVersion)
            {
                MelonLogger.Warning($"gregRegistry skipped plugin '{plugin.PluginId}' due to required version {plugin.RequiredFrameworkVersion} > {currentVersion}.");
                continue;
            }

            try
            {
                plugin.OnFrameworkReady();
                MelonLogger.Msg($"gregRegistry framework-ready delivered: {plugin.PluginId}");
            }
            catch (Exception exception)
            {
                MelonLogger.Error($"gregRegistry plugin '{plugin.PluginId}' failed in OnFrameworkReady: {exception.Message}");
            }
        }
    }

    private static Version GetCurrentFrameworkVersion()
    {
        string versionString = gregReleaseVersion.Current;
        return Version.TryParse(versionString, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
    }
}


