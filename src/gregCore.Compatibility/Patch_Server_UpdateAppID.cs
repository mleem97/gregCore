using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(Server), nameof(Server.UpdateAppID))]
internal static class Patch_Server_UpdateAppID
{
    internal static void Postfix(int _appID)
    {
        try { EventDispatcher.FireServerAppChanged(_appID); }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateAppID: {ex.Message}"); }
    }
}
