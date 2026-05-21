using System;
using System.Reflection;
using MelonLoader;

namespace gregCore.Compatibility.FishNet
{
    /// <summary>
    /// Bridge to invoke FishNet ServerRpc methods on the Multiplayer RelayService.
    /// Uses reflection since the Multiplayer assembly is not directly referenced by core.
    /// </summary>
    public static class FishNetBridge
    {
        public static void SendServerRpc(string methodName, params object[] args)
        {
            try
            {
                var type = Type.GetType("greg.Multiplayer.GregRelayService, greg.Multiplayer");
                if (type == null) return;

                var instanceProp = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceProp == null) return;

                var instance = instanceProp.GetValue(null);
                if (instance == null) return;

                var methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if (methodInfo == null) return;

                methodInfo.Invoke(instance, args);
            }
            catch (Exception ex)
            {
                // Core mod should not crash if multiplayer RPC fails or is missing
                MelonLoader.MelonLogger.Warning($"[FishNetBridge] Failed to send ServerRpc '{methodName}': {ex.Message}");
            }
        }
    }
}
