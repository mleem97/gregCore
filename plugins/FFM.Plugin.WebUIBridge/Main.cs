using System;
using DataCenterModLoader;
using FrikaMF;
using FrikaMF.Plugins;
using MelonLoader;

[assembly: MelonInfo(typeof(FFM.Plugin.WebUIBridge.Main), "FFM.Plugin.WebUIBridge", ReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace FFM.Plugin.WebUIBridge;

/// <summary>
/// Legacy optional plugin: Web UI wiring now lives in FrikaMF core (<c>UiExtensionBootstrap</c>).
/// Keeping this Melon preserves older installs and Workshop layouts that still ship the DLL.
/// </summary>
public sealed class Main : FFMPluginBase
{
    public override string PluginId => "FFM.Plugin.WebUIBridge";
    public override string DisplayName => "FrikaMF WebUI Bridge Plugin (legacy shim)";
    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(ReleaseVersion.Current);

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        if (Core.Instance != null)
            OnFrameworkReady();
    }

    public override void OnFrameworkReady()
    {
        MelonLogger.Msg("FFM.Plugin.WebUIBridge: no-op — DC2 web UI is registered by FrikaMF core.");
    }

    public override void OnApplicationQuit()
    {
        // Unregister is owned by DataCenterModLoader.Core shutdown.
    }

    private static Version ParseFrameworkVersion(string version)
    {
        return Version.TryParse(version, out Version parsed) ? parsed : new Version(0, 0, 0, 0);
    }
}
