using System;
using gregModLoader;
using gregModLoader;
using gregModLoader.Plugins;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(greg.Plugin.Sysadmin.gregMain), "greg.Plugin.Sysadmin", gregReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Plugin.Sysadmin;

/// <summary>
/// **Community example** — uses <see cref="gregUiExtensionBridge"/> additive events (same as gregExt.Sysadmin copy).
/// </summary>
public sealed class gregMain : gregPluginBase
{
    public override string PluginId => "greg.Plugin.Sysadmin";
    public override string DisplayName => "gregCore Sysadmin (framework UI addon example)";
    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(gregReleaseVersion.Current);

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        if (gregCoreLoader.Instance != null)
            OnFrameworkReady();
    }

    public override void OnFrameworkReady()
    {
        gregUiExtensionBridge.AfterTryModernize += OnAfterTryModernize;
        gregUiExtensionBridge.AfterUiSceneLoaded += OnAfterUiSceneLoaded;
        gregUiExtensionBridge.AfterDrawGui += OnAfterDrawGui;
        gregUiExtensionBridge.AfterSettingsOpened += OnAfterSettingsOpened;

        MelonLogger.Msg(
            "[greg.Plugin.Sysadmin] Subscribed to gregUiExtensionBridge additive hooks (see gregCore/framework/ModLoader/gregUiExtensionBridge.cs).");
    }

    public override void OnApplicationQuit()
    {
        gregUiExtensionBridge.AfterTryModernize -= OnAfterTryModernize;
        gregUiExtensionBridge.AfterUiSceneLoaded -= OnAfterUiSceneLoaded;
        gregUiExtensionBridge.AfterDrawGui -= OnAfterDrawGui;
        gregUiExtensionBridge.AfterSettingsOpened -= OnAfterSettingsOpened;
        base.OnApplicationQuit();
    }

    private static void OnAfterTryModernize(GameObject root, string sourceTag)
    {
        try
        {
            if (root == null)
                return;
            MelonLogger.Msg($"[SysadminExample] AfterTryModernize tag={sourceTag} root={root.name}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SysadminExample] AfterTryModernize: {ex.Message}");
        }
    }

    private static void OnAfterUiSceneLoaded(string sceneName)
    {
        try
        {
            MelonLogger.Msg($"[SysadminExample] AfterUiSceneLoaded scene={sceneName}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SysadminExample] AfterUiSceneLoaded: {ex.Message}");
        }
    }

    private static void OnAfterDrawGui()
    {
        try
        {
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SysadminExample] AfterDrawGui: {ex.Message}");
        }
    }

    private static void OnAfterSettingsOpened(MainMenu mainMenu)
    {
        try
        {
            MelonLogger.Msg($"[SysadminExample] AfterSettingsOpened menu={(mainMenu == null ? "null" : mainMenu.name)}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SysadminExample] AfterSettingsOpened: {ex.Message}");
        }
    }

    private static Version ParseFrameworkVersion(string version)
    {
        return Version.TryParse(version, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
    }
}




