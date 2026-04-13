using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace gregCoreSDK.Sdk.Services;

/// <summary>
/// Bridge service for configuring Network Switches (VLANs, LACP).
/// </summary>
public static class GregSwitchConfigService
{
    private static NetworkSwitchConfiguration GetInstance()
    {
        var instance = UnityEngine.Object.FindObjectOfType<NetworkSwitchConfiguration>();
        if (instance == null)
            MelonLogger.Warning("[GregSwitchConfigService] NetworkSwitchConfiguration instance not found!");
        return instance;
    }

    public static void OpenConfig(NetworkSwitch netSwitch)
    {
        GetInstance()?.OpenConfig(netSwitch);
    }

    public static void CloseConfig()
    {
        GetInstance()?.CloseConfig();
    }

    public static void ClickPort(int portIndex)
    {
        GetInstance()?.ClickPort(portIndex);
    }

    public static void CreateLACP()
    {
        GetInstance()?.CreateLACP();
    }
}
