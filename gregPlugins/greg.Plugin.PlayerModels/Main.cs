using System;
using gregModLoader;
using gregModLoader.Hooks;
using gregModLoader.Plugins;
using MelonLoader;

[assembly: MelonInfo(typeof(greg.Plugin.PlayerModels.gregMain), "greg.Plugin.PlayerModels", gregReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Plugin.PlayerModels;

/// <summary>
/// Standalone plugin that exposes runtime player and NPC model replacement services.
/// </summary>
public sealed class gregMain : gregPluginBase
{
    private static gregMain _instance;
    private bool _hookSampleRegistered;

    /// <summary>
    /// Gets the active plugin instance.
    /// </summary>
    public static gregMain Instance => _instance;

    /// <inheritdoc />
    public override string PluginId => "greg.Plugin.PlayerModels";

    /// <inheritdoc />
    public override string DisplayName => "gregCore Player Models Plugin";

    /// <inheritdoc />
    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(gregReleaseVersion.Current);

    /// <inheritdoc />
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        _instance = this;

        if (gregModLoader.gregCoreLoader.Instance != null)
            OnFrameworkReady();
    }

    /// <inheritdoc />
    public override void OnFrameworkReady()
    {
        greg.PlayerModels.API.Initialize();
        RegisterHookBinderSample();
        MelonLogger.Msg("greg.Plugin.PlayerModels initialized.");
    }

    /// <inheritdoc />
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        PlayerModelSwapper.ReapplySceneAssignments();
        NPCModelReplacer.ReapplyPersistentReplacements();
    }

    private static Version ParseFrameworkVersion(string version)
    {
        return Version.TryParse(version, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
    }

    private void RegisterHookBinderSample()
    {
        if (_hookSampleRegistered)
            return;

        _hookSampleRegistered = true;

        HookBinder.OnAfter("greg.Server.OnPowerButton", OnServerPowerButtonHookAfter);
        MelonLogger.Msg("greg.Plugin.PlayerModels: HookBinder sample registered for greg.Server.OnPowerButton.");
    }

    private static void OnServerPowerButtonHookAfter(HookContext context)
    {
        if (context?.Method == null)
            return;

        MelonLogger.Msg($"[HookSample] {context.HookName} triggered by {context.Method.DeclaringType?.Name}.{context.Method.Name}");
    }
}




