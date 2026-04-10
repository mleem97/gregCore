using System;
using DataCenterModLoader;
using FrikaMF;
using FrikaMF.Plugins;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(greg.Plugin.WebUIBridge.Main), "greg.Plugin.WebUIBridge", ReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Plugin.WebUIBridge;

/// <summary>
/// **Community example** — same behaviour as <c>gregExt.WebUIBridge</c> copy.
/// </summary>
public sealed class Main : GregPluginBase
{
    public override string PluginId => "greg.Plugin.WebUIBridge";
    public override string DisplayName => "FrikaMF WebUI Bridge (framework UI addon example)";
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
            "[greg.Plugin.WebUIBridge] Subscribed to UiExtensionBridge hooks; web replacement remains in FrikaMF core.");
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
            MelonLogger.Msg($"[WebUIBridgeExample] AfterTryModernize tag={sourceTag} root={root.name}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[WebUIBridgeExample] AfterTryModernize: {ex.Message}");
        }
    }

    private static void OnAfterUiSceneLoaded(string sceneName)
    {
        try
        {
            MelonLogger.Msg($"[WebUIBridgeExample] AfterUiSceneLoaded scene={sceneName}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[WebUIBridgeExample] AfterUiSceneLoaded: {ex.Message}");
        }
    }

    private static void OnAfterDrawGui()
    {
        try
        {
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[WebUIBridgeExample] AfterDrawGui: {ex.Message}");
        }
    }

    private static void OnAfterSettingsOpened(MainMenu mainMenu)
    {
        try
        {
            bool web = UiExtensionBridge.GetWebBridgeEnabled();
            MelonLogger.Msg(
                $"[WebUIBridgeExample] AfterSettingsOpened menu={(mainMenu == null ? "null" : mainMenu.name)} webBridgeEnabled={web}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[WebUIBridgeExample] AfterSettingsOpened: {ex.Message}");
        }
    }

    private static Version ParseFrameworkVersion(string version)
    {
        return Version.TryParse(version, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
    }
}
