using HarmonyLib;
using MelonLoader;
using System;

namespace greg.Multiplayer.Patches
{
    /// <summary>
    /// Intercepts Rack.PlaceDevice to broadcast state to all clients.
    /// The Postfix runs AFTER the game's own logic – server authority guaranteed
    /// because only the host's execution reaches the actual placement code.
    /// </summary>
    [HarmonyPatch]
    internal static class RackPatch
    {
        // TODO: Replace "Rack" and "PlaceDevice" with actual class/method
        // names found via ILSpy on Assembly-CSharp.dll of Unity 6000.4.2f.
        // Use [HarmonyPatch(typeof(Rack), nameof(Rack.PlaceDevice))] once confirmed.
        [HarmonyPatch("Rack", "PlaceDevice")]
        [HarmonyPostfix]
        private static void PlaceDevice_Postfix(object __instance, object device)
        {
            try
            {
                if (GregRelayService.Instance == null) return;
                // Only broadcast from the host (server)
                if (!IsServer()) return;

                // Extract rack id via reflection until IL2CPP types confirmed
                int rackId = GetFieldSafe<int>(__instance, "rackId");

                var payload = new RackSyncPayload
                {
                    RackId = rackId,
                    Action = "PLACE",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                };
                GregRelayService.Instance.BroadcastRackSync(payload);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[RackPatch] PlaceDevice_Postfix error: {ex.Message}");
            }
        }

        [HarmonyPatch("Rack", "RemoveDevice")]
        [HarmonyPostfix]
        private static void RemoveDevice_Postfix(object __instance, object device)
        {
            try
            {
                if (GregRelayService.Instance == null || !IsServer()) return;
                int rackId = GetFieldSafe<int>(__instance, "rackId");
                var payload = new RackSyncPayload
                {
                    RackId = rackId,
                    Action = "REMOVE",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                };
                GregRelayService.Instance.BroadcastRackSync(payload);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[RackPatch] RemoveDevice_Postfix error: {ex.Message}");
            }
        }

        private static bool IsServer()
            => FishNet.InstanceFinder.IsServerStarted;

        private static T GetFieldSafe<T>(object obj, string fieldName)
        {
            var field = obj?.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            return field != null ? (T)field.GetValue(obj) : default;
        }
    }
}
