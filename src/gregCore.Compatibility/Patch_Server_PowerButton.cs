using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(Server), nameof(Server.PowerButton))]
internal static class Patch_Server_PowerButton
{
    internal static void Postfix(Server __instance)
    {
        try { EventDispatcher.FireServerPowered(__instance.isOn); }
        catch (Exception ex) { EventDispatcher.LogError($"PowerButton: {ex.Message}"); }
    }
}
