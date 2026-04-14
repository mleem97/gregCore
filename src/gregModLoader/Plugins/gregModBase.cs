using System;
using MelonLoader;

namespace greg.Core.Plugins;

/// <summary>
/// Unified base class for all gregCore ecosystem mods and plugins.
/// Handles automatic dependency resolution and lifecycle management.
/// </summary>
public abstract class gregModBase : MelonMod
{
    /// <summary>
    /// Gets the list of required assembly/DLL names (without extension).
    /// If any are missing, the mod will not initialize.
    /// </summary>
    public virtual string[] RequiredDependencies => Array.Empty<string>();

    /// <summary>
    /// Gets the list of optional assembly/DLL names (without extension).
    /// If missing, a warning is logged but the mod still initializes.
    /// </summary>
    public virtual string[] OptionalDependencies => Array.Empty<string>();

    /// <summary>
    /// Indicates if the dependency check failed.
    /// If true, all lifeycle methods (Update, GUI, etc.) are short-circuited.
    /// </summary>
    protected bool IsDependencySearchFailed { get; private set; }

    public sealed override void OnInitializeMelon()
    {
        if (!gregDependencyResolver.CheckDependencies(LoggerInstance, MelonAssembly.Assembly.GetName().Name, RequiredDependencies, OptionalDependencies))
        {
            IsDependencySearchFailed = true;
            return;
        }

        try
        {
            OnInitializeMod();
        }
        catch (Exception ex)
        {
            LoggerInstance.Error($"Unhandled exception during OnInitializeMod: {ex.Message}");
            IsDependencySearchFailed = true;
        }
    }

    /// <summary>
    /// Actual initialization logic for the mod. Called only if dependencies are satisfied.
    /// </summary>
    public virtual void OnInitializeMod() { }

    public override void OnUpdate() { if (!IsDependencySearchFailed) OnUpdateMod(); }
    public virtual void OnUpdateMod() { }

    public override void OnFixedUpdate() { if (!IsDependencySearchFailed) OnFixedUpdateMod(); }
    public virtual void OnFixedUpdateMod() { }

    public override void OnLateUpdate() { if (!IsDependencySearchFailed) OnLateUpdateMod(); }
    public virtual void OnLateUpdateMod() { }

    public override void OnGUI() { if (!IsDependencySearchFailed) OnGuiMod(); }
    public virtual void OnGuiMod() { }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) { if (!IsDependencySearchFailed) OnSceneLoadedMod(buildIndex, sceneName); }
    public virtual void OnSceneLoadedMod(int buildIndex, string sceneName) { }

    public override void OnApplicationQuit() { if (!IsDependencySearchFailed) OnApplicationQuitMod(); }
    public virtual void OnApplicationQuitMod() { }
}
