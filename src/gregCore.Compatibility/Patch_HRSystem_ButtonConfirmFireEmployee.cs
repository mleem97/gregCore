using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonConfirmFireEmployee))]
internal static class Patch_HRSystem_ButtonConfirmFireEmployee
{
    private static bool _wasCustom;

    internal static bool Prefix(HRSystem __instance)
    {
        try
        {
            if (CustomEmployeeManager.HandleConfirmFire(__instance))
            {
                _wasCustom = true;
                return false;
            }
        }
        catch (Exception ex) { CrashLog.LogException("ButtonConfirmFireEmployee prefix", ex); }
        _wasCustom = false;
        return true;
    }

    internal static void Postfix()
    {
        if (_wasCustom) return;
        try { EventDispatcher.FireSimple(EventIds.EmployeeFired); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonConfirmFireEmployee: {ex.Message}"); }
    }
}
