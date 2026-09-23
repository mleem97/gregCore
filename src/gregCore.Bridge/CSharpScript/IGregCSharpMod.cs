using System;

namespace gregCore.Bridge.CSharpScript;

/// <summary>
/// Interface that C# script mods must implement.
/// The implementing class must expose a public parameterless constructor.
/// </summary>
public interface IGregCSharpMod
{
    /// <summary>Unique mod identifier (lowercase, no spaces).</summary>
    string ModId { get; }

    /// <summary>Human-readable mod name.</summary>
    string ModName { get; }

    /// <summary>SemVer string.</summary>
    string Version { get; }

    /// <summary>Called once after the mod assembly has been loaded and instantiated.</summary>
    void OnInit();

    /// <summary>Called every frame while the mod is active.</summary>
    void OnUpdate(float dt);

    /// <summary>Called when a new Unity scene finishes loading.</summary>
    void OnSceneLoaded(string sceneName);

    /// <summary>Called before the mod is unloaded (shutdown / hot-reload).</summary>
    void OnShutdown();
}
