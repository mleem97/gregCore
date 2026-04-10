using System;
using DataCenterModLoader;
using gregCore;
using gregCore.Plugins;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(greg.Plugin.Sysadmin.Main), "greg.Plugin.Sysadmin", ReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Plugin.Sysadmin;

/// <summary>
/// **Community example** — uses <see cref="UiExtensionBridge"/> additive events (same as gregExt.Sysadmin copy).
/// </summary>
public sealed class Main : GregPluginBase
{
    public override string PluginId => "greg.Plugin.Sysadmin";
    public override string DisplayName => "gregCore Sysadmin (framework UI addon example)";
    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(ReleaseVersion.Current);

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        if (Core.Instance != null)
            OnFrameworkReady();
    }

    public override void OnFrameworkReady()
    {
        UiExtensionBridge.AfterTryModernize += OnAfterTryModernize;
        UiExtensionBridge.AfterUiSceneLoaded += OnAfterUiSceneLoaded;
        UiExtensionBridge.AfterDrawGui += OnAfterDrawGui;
        UiExtensionBridge.AfterSettingsOpened += OnAfterSettingsOpened;

        MelonLogger.Msg(
            "[greg.Plugin.Sysadmin] Subscribed to UiExtensionBridge additive hooks (see gregCore/framework/ModLoader/UiExtensionBridge.cs).");
    }

    public override void OnApplicationQuit()
    {
        UiExtensionBridge.AfterTryModernize -= OnAfterTryModernize;
        UiExtensionBridge.AfterUiSceneLoaded -= OnAfterUiSceneLoaded;
        UiExtensionBridge.AfterDrawGui -= OnAfterDrawGui;
        UiExtensionBridge.AfterSettingsOpened -= OnAfterSettingsOpened;
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
