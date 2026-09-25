using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(Server), nameof(Server.UpdateCustomer))]
internal static class Patch_Server_UpdateCustomer
{
    internal static void Postfix(int newCustomerID)
    {
        try { EventDispatcher.FireServerCustomerChanged(newCustomerID); }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateCustomer: {ex.Message}"); }
    }
}
