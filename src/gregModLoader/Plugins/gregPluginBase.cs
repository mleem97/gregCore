using System;
using MelonLoader;

namespace greg.Core.Plugins;

/// <summary>
/// Base class for gregCore standalone plugins (extensions).
/// </summary>
public abstract class gregPluginBase : gregModBase
{
    /// <summary>
    /// Extension plugins always require gregCore to function.
    /// </summary>
    public override string[] RequiredDependencies => new[] { "gregCore" };

    /// <summary>
    /// Gets the plugin's unique identifier.
    /// </summary>
    public abstract string PluginId { get; }

    /// <summary>
    /// Gets the plugin's human-readable display name.
    /// </summary>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Gets the minimum required framework version.
    /// </summary>
    public abstract Version RequiredFrameworkVersion { get; }

    /// <summary>
    /// Called after the framework gregCoreLoader has fully initialized and validated plugin compatibility.
    /// </summary>
    public abstract void OnFrameworkReady();

    /// <summary>
    /// Registers the plugin with the central framework registry.
    /// </summary>
    public override void OnInitializeMod()
    {
        gregRegistry.RegisterPlugin(this);
    }
}



