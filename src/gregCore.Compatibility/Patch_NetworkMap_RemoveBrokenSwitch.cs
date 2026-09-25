using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveBrokenSwitch))]
internal static class Patch_NetworkMap_RemoveBrokenSwitch
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSwitchRepaired(); }
        catch (Exception ex) { EventDispatcher.LogError($"RemoveBrokenSwitch: {ex.Message}"); }
    }
}
