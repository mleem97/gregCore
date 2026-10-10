using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(Rack), nameof(Rack.ButtonUnmountRack))]
internal static class Patch_Rack_ButtonUnmountRack
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireRackUnmounted(); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonUnmountRack: {ex.Message}"); }
    }
}
