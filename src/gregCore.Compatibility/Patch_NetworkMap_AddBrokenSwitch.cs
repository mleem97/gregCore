using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.AddBrokenSwitch))]
internal static class Patch_NetworkMap_AddBrokenSwitch
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSwitchBroken(); }
        catch (Exception ex) { EventDispatcher.LogError($"AddBrokenSwitch: {ex.Message}"); }
    }
}
