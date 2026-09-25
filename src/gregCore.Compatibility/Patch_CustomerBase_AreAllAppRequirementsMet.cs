using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.AreAllAppRequirementsMet))]
internal static class Patch_CustomerBase_AreAllAppRequirementsMet
{
    private static readonly HashSet<int> _satisfiedCustomers = new();

    internal static void Postfix(CustomerBase __instance, bool __result)
    {
        try
        {
            int id = __instance.customerBaseID;
            if (__result) FireSatisfied(id);
            else FireUnsatisfied(id);
        }
        catch (Exception ex) { EventDispatcher.LogError($"AreAllAppRequirementsMet: {ex.Message}"); }
    }

    private static void FireSatisfied(int id)
    {
        try
        {
            if (_satisfiedCustomers.Add(id))
                EventDispatcher.FireCustomerSatisfied(id);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void FireUnsatisfied(int id)
    {
        try
        {
            if (_satisfiedCustomers.Remove(id))
                EventDispatcher.FireCustomerUnsatisfied(id);
        }
        catch { }
    }
}
