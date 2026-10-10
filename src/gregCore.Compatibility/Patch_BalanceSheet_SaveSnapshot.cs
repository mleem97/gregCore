using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.SaveSnapshot))]
internal static class Patch_BalanceSheet_SaveSnapshot
{
    internal static void Postfix(int __0)
    {
        try { EventDispatcher.FireMonthEnded(__0); }
        catch (Exception ex) { EventDispatcher.LogError($"SaveSnapshot: {ex.Message}"); }
    }
}
