using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonBuyWall))]
internal static class Patch_MainGameManager_ButtonBuyWall
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireWallPurchased(); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonBuyWall: {ex.Message}"); }
    }
}
