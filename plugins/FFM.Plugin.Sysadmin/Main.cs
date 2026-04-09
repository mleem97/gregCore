using System;
using DataCenterModLoader;
using FrikaMF;
using FrikaMF.Plugins;
using MelonLoader;

[assembly: MelonInfo(typeof(FFM.Plugin.Sysadmin.Main), "FFM.Plugin.Sysadmin", ReleaseVersion.Current, "mleem97")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace FFM.Plugin.Sysadmin;

/// <summary>
/// Legacy optional plugin: Sysadmin UI handlers now live in FrikaMF core (<c>UiExtensionBootstrap</c>).
/// Keeping this Melon preserves older installs and Workshop layouts that still ship the DLL.
/// </summary>
public sealed class Main : FFMPluginBase
{
    public override string PluginId => "FFM.Plugin.Sysadmin";
    public override string DisplayName => "FrikaMF Sysadmin Plugin (legacy shim)";
    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(ReleaseVersion.Current);

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        if (Core.Instance != null)
            OnFrameworkReady();
    }

    public override void OnFrameworkReady()
    {
        MelonLogger.Msg("FFM.Plugin.Sysadmin: no-op — UI modernizer/settings bridge is registered by FrikaMF core.");
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
