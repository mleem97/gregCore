using HarmonyLib;
using MelonLoader;
using System;

namespace greg.Multiplayer.Patches
{
    [HarmonyPatch]
    internal static class CablePatch
    {
        [HarmonyPatch(typeof(global::Il2Cpp.NetworkMap), nameof(global::Il2Cpp.NetworkMap.Connect))]
        [HarmonyPostfix]
        private static void ConnectCable_Postfix(global::Il2Cpp.NetworkMap __instance, string from, string to)
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

        [HarmonyPatch(typeof(global::Il2Cpp.NetworkMap), nameof(global::Il2Cpp.NetworkMap.Disconnect))]
        [HarmonyPostfix]
        private static void DisconnectCable_Postfix(global::Il2Cpp.NetworkMap __instance, string from, string to)
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
