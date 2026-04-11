using System;
using gregModLoader;
using MelonLoader;

[assembly: MelonInfo(typeof(greg.BasedModTemplate.gregMain), "greg.BasedModTemplate", "00.01.0001", "your-name")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.BasedModTemplate;

public sealed class gregMain : MelonMod
{
    private bool _frameworkReady;

    public override void OnInitializeMelon()
    {
        if (gregCoreLoader.Instance == null)
        {
            LoggerInstance.Warning("gregCore runtime not available yet. Waiting for game update loop.");
            return;
        }

        _frameworkReady = true;
        LoggerInstance.Msg("greg.BasedModTemplate initialized.");
    }

    public override void OnUpdate()
    {
        if (_frameworkReady)
            return;

        if (gregCoreLoader.Instance == null)
            return;

        _frameworkReady = true;
        LoggerInstance.Msg("greg gregCoreLoader detected. Mod is now active.");
    }

    public override void OnApplicationQuit()
    {
        _frameworkReady = false;
    }
}



