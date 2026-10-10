using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.AutoSave))]
internal static class Patch_SaveSystem_AutoSave
{
    internal static void Postfix()
    {
        try
        {
            CustomEmployeeManager.SaveState();
            EventDispatcher.FireGameAutoSaved();
        }
        catch (Exception ex) { EventDispatcher.LogError($"AutoSave: {ex.Message}"); }
    }
}
