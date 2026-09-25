using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

/// <summary>
/// Diagnostic hook for RackPosition.InteractOnClick — logs when a player
/// clicks on a rack slot (start of the installation coroutine).
/// </summary>
[HarmonyPatch(typeof(RackPosition), nameof(RackPosition.InteractOnClick))]
internal static class Patch_RackPosition_InteractOnClick
{
    [ThreadStatic] private static int _prevNumObjects;
    [ThreadStatic] private static int _prevObjectInHand;

    internal static void Prefix(RackPosition __instance)
    {
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null) return;
            _prevNumObjects = pm.numberOfObjectsInHand;
            _prevObjectInHand = (int)pm.objectInHand;
        }
        catch { }
    }

    internal static void Postfix(RackPosition __instance)
    {
        try
        {
            CrashLog.Log($"[WorldSync] RackPosition.InteractOnClick: posIndex={__instance.positionIndex} rackPosGlobalUID={__instance.rackPosGlobalUID}");
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] RackPosition.InteractOnClick Postfix error: {ex.Message}"); }
    }
}
