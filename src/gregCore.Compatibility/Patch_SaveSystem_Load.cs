using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.Load))]
internal static class Patch_SaveSystem_Load
{
    internal static void Postfix()
    {
        try
        {
            CustomEmployeeManager.LoadState();
            EventDispatcher.FireSimple(EventIds.GameLoaded);
            SpawnedObjectTracker.PopulateKnownServers();
        }
        catch (Exception ex) { EventDispatcher.LogError($"Load: {ex.Message}"); }
    }
}
