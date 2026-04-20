using HarmonyLib;
using gregCore.GameLayer.Hooks;
using gregCore.Core.Models;
using gregCore.API;

namespace gregCore.GameLayer.Patches.Hardware;

[HarmonyPatch]
internal static class ServerPatch
{
    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.ItIsBroken))]
    [HarmonyPostfix]
    internal static void OnServerBroken(global::Il2Cpp.Server __instance)
    {
        try
        {
            var payload = new EventPayload 
            { 
                HookName = "hardware.ServerStatusChanged", 
                Data = new System.Collections.Generic.Dictionary<string, object> { { "status", "broken" } } 
            };
            HookIntegration.Emit(HookName.Create("hardware", "ServerStatusChanged"), payload);
            GregAPI.FireEvent(GregEventId.ServerBroken);
        }
        catch (System.Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnServerBroken), ex);
        }
    }

    // NOTE: Server.DeviceRepaired does not exist in the game assembly.
    // Patching RepairDevice instead which IS confirmed via reference dump.
    [HarmonyPatch(typeof(global::Il2Cpp.Server), nameof(global::Il2Cpp.Server.RepairDevice))]
    [HarmonyPostfix]
    internal static void OnServerRepaired(global::Il2Cpp.Server __instance)
    {
        try
        {
            GregAPI.FireEvent(GregEventId.ServerRepaired);
        }
        catch (System.Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnServerRepaired), ex);
        }
    }
}
