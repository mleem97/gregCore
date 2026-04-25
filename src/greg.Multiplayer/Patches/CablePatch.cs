using HarmonyLib;
using MelonLoader;
using System;

namespace greg.Multiplayer.Patches
{
    [HarmonyPatch]
    internal static class CablePatch
    {
        // TODO: Confirm class/method via ILSpy on Assembly-CSharp.dll
        [HarmonyPatch("CableManager", "ConnectCable")]
        [HarmonyPostfix]
        private static void ConnectCable_Postfix(object __instance, object sourcePort, object targetPort)
        {
            try
            {
                if (GregRelayService.Instance == null || !FishNet.InstanceFinder.IsServerStarted) return;
                var payload = new CableSyncPayload
                {
                    Action = "CONNECT",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                };
                GregRelayService.Instance.BroadcastCableSync(payload);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[CablePatch] ConnectCable_Postfix error: {ex.Message}");
            }
        }

        [HarmonyPatch("CableManager", "DisconnectCable")]
        [HarmonyPostfix]
        private static void DisconnectCable_Postfix(object __instance, object port)
        {
            try
            {
                if (GregRelayService.Instance == null || !FishNet.InstanceFinder.IsServerStarted) return;
                var payload = new CableSyncPayload
                {
                    Action = "DISCONNECT",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                };
                GregRelayService.Instance.BroadcastCableSync(payload);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[CablePatch] DisconnectCable_Postfix error: {ex.Message}");
            }
        }
    }
}
