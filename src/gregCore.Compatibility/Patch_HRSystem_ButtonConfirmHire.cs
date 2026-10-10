using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonConfirmHire))]
internal static class Patch_HRSystem_ButtonConfirmHire
{
    private static bool _wasCustom;

    internal static bool Prefix(HRSystem __instance)
    {
        try
        {
            if (CustomEmployeeManager.HandleConfirmHire(__instance))
            {
                _wasCustom = true;
                return false;
            }
        }
        catch (Exception ex) { CrashLog.LogException("ButtonConfirmHire prefix", ex); }
        _wasCustom = false;
        return true;
    }

    internal static void Postfix()
    {
        if (_wasCustom) return;
        try { EventDispatcher.FireSimple(EventIds.EmployeeHired); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonConfirmHire: {ex.Message}"); }
    }
}
