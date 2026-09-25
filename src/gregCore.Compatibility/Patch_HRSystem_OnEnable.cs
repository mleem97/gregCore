using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

/// <summary>
/// Patches HRSystem.OnEnable to inject custom employee cards into the HR panel.
/// </summary>
[HarmonyPatch(typeof(HRSystem), "OnEnable")]
internal static class Patch_HRSystem_OnEnable
{
    internal static void Postfix(HRSystem __instance)
    {
        try
        {
            CrashLog.Log("HRSystem.OnEnable: injecting custom employees");
            CustomEmployeeManager.InjectIntoHRSystem(__instance);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("HRSystem.OnEnable custom employee injection", ex);
        }
    }
}
