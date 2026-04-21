using System;
using HarmonyLib;
using greg.Logging;

namespace greg.WallRack.Integration
{
    [HarmonyPatch(typeof(Il2Cpp.NetworkSwitchConfiguration), "Awake")]
    internal static class CustomerDeviceSwapPatch
    {
        private static readonly GregModLogger _log = new GregModLogger("WallRack");

        [HarmonyPostfix]
        private static void Postfix(Il2Cpp.NetworkSwitchConfiguration __instance)
        {
            try
            {
                if (!frameworkSdk.GregFeatureGuard.IsEnabled("WallRack")) return;

                // Sync Vanilla Ref
                // _log.Debug($"NetworkSwitch {__instance.name} initialized. Binding vanillaRef.");
                
            }
            catch (Exception ex)
            {
                _log.Error("Prefix threw an exception -- original method will run.", ex);
            }
        }
    }
}
