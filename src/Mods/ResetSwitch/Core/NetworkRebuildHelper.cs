using greg.Sdk.Services;

namespace greg.Mods.ResetSwitch.Core;

public static class NetworkRebuildHelper
{
    public static void ForceRebuild(string reason)
    {
        greg.Sdk.Services.GregResetSwitchService.ForceNetworkRebuild(reason);
    }
}
