using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonCustomerChosen))]
internal static class Patch_MainGameManager_ButtonCustomerChosen
{
    internal static void Postfix(int __0)
    {
        try { EventDispatcher.FireCustomerAccepted(__0); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonCustomerChosen: {ex.Message}"); }
    }
}
