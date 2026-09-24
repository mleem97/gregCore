/// <file-summary>
/// Layer:       GameLayer
/// Purpose:     Injects gregCore computer shortcuts into the
///              Il2Cpp.ComputerShop main screen and keeps app pages in
///              sync. Postfixes only; all work delegates to GregComputer
///              (best-effort, never throws).
/// Maintainer:  Registry + pages: gregCore.UI.GregComputer.
/// </file-summary>

using System;
using HarmonyLib;
using gregCore.UI;

namespace gregCore.GameLayer.Patches.UI;

public static class GregComputerPatch
{
    [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.ButtonReturnMainScreen))]
    [HarmonyPostfix]
    private static void MainScreenPostfix(global::Il2Cpp.ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            GregComputer.Sync(__instance);
        }
        catch { }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.OpenShop))]
    [HarmonyPostfix]
    private static void OpenShopPostfix(global::Il2Cpp.ComputerShop __instance)
    {
        try
        {
            if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
            GregComputer.Sync(__instance);
        }
        catch { }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.CloseShop))]
    [HarmonyPostfix]
    private static void CloseShopPostfix(global::Il2Cpp.ComputerShop __instance)
    {
        try { GregComputer.OnComputerClosed(); } catch { }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.ComputerShop), nameof(global::Il2Cpp.ComputerShop.HideCanvas))]
    [HarmonyPostfix]
    private static void HideCanvasPostfix(global::Il2Cpp.ComputerShop __instance)
    {
        try { GregComputer.OnComputerClosed(); } catch { }
    }
}
