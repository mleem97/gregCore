using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(Player), nameof(Player.UpdateReputation))]
internal static class Patch_Player_UpdateReputation
{
    private static float _oldRep;

    internal static void Prefix(Player __instance)
    {
        try { _oldRep = __instance.reputation; }
        catch { _oldRep = 0f; }
    }

    internal static void Postfix(Player __instance)
    {
        try
        {
            float newRep = __instance.reputation;
            if (Math.Abs(newRep - _oldRep) > 0.001f)
                EventDispatcher.FireValueChanged(EventIds.ReputationChanged, _oldRep, newRep, newRep - _oldRep);
        }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateReputation postfix: {ex.Message}"); }
    }
}
