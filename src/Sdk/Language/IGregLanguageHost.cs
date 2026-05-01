using System;

namespace gregCore.Sdk.Language;

/// <summary>
/// Interface for a language host that can execute mods written in a specific language.
/// Implementations can be provided by gregCore built-ins or gregExt extensions.
/// </summary>
public interface IGregLanguageHost
{
    /// <summary>
    /// Unique string identifier for this language host (e.g. "lua", "rust", "go").
    /// Used for registry lookups and logging.
    /// </summary>
    string HostId { get; }

    /// <summary>
    /// Human-readable name of the host.
    /// </summary>
    string HostName { get; }

    /// <summary>
    /// Whether the host is currently active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// File extensions this host handles (e.g. [".lua"], [".rs", ".rmod"]).
    /// Used for script scanning.
    /// </summary>
    string[] FileExtensions { get; }

    /// <summary>
    /// Checks whether the runtime dependencies for this host are available.
    /// </summary>
    bool IsDependencyAvailable(out string detail);

    /// <summary>
    /// Activates the host and loads scripts from the given directory.
    /// </summary>
    void Activate(string modsScriptsDir);

    /// <summary>
    /// Called every frame by gregCore.
    /// </summary>
    void OnUpdate(float dt);

    /// <summary>
    /// Called when a new scene is loaded.
    /// </summary>
    void OnSceneLoaded(string sceneName);

    /// <summary>
    /// Shuts down the host and releases resources.
    /// </summary>
    void Shutdown();
}
