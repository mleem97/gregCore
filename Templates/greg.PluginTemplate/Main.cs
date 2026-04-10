using System;
using DataCenterModLoader;
using gregCore;
using gregCore.Plugins;
using MelonLoader;

[assembly: MelonInfo(typeof(greg.PluginTemplate.Main), "greg.PluginTemplate", "00.01.0001", "your-name")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.PluginTemplate;

public sealed class Main : GregPluginBase
{
    private bool _registered;

    public override string PluginId => "greg.Plugin.Template";

    public override string DisplayName => "gregCore Plugin Template";

    public override Version RequiredFrameworkVersion => ParseFrameworkVersion(ReleaseVersion.Current);

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        if (Core.Instance != null)
            OnFrameworkReady();
    }

    public override void OnFrameworkReady()
    {
        if (_registered)
            return;

        _registered = true;
        MelonLogger.Msg($"{PluginId} initialized and framework-ready.");
    }

    public override void OnApplicationQuit()
    {
        _registered = false;
    }

    private static Version ParseFrameworkVersion(string version)
    {
        return Version.TryParse(version, out Version parsed)
            ? parsed
            : new Version(0, 0, 0, 0);
    }
}
