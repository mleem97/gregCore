using DataCenterModLoader;
using MelonLoader;

[assembly: MelonInfo(typeof(gregCore.Sysadmin.Mod.Main), "gregCore.Sysadmin.Mod", "0.1.0", "gregCore")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace gregCore.Sysadmin.Mod;

public sealed class Main : MelonMod
{
    private bool _ready;

    public override void OnInitializeMelon()
    {
        _ready = Core.Instance != null;
        if (_ready)
            LoggerInstance.Msg("[gregCore][Sysadmin][C#] initialized");
    }

    public override void OnUpdate()
    {
        if (_ready)
            return;

        if (Core.Instance == null)
            return;

        _ready = true;
        LoggerInstance.Msg("[gregCore][Sysadmin][C#] runtime detected");
    }

    public override void OnApplicationQuit()
    {
        _ready = false;
    }
}
