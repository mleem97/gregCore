using System;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(Server), nameof(Server.ServerInsertedInRack))]
internal static class Patch_Server_ServerInsertedInRack
{
    internal static void Postfix(Server? __instance, ServerSaveData? __0)
    {
        try
        {
            string instanceId = ReadInstanceId(__instance);
            byte objectType = ReadObjectType(__instance);
            int rackPosUid = ResolveRackUid(__instance, __0);
            CrashLog.Log($"ServerInsertedInRack [diag]: instanceId={instanceId}, type={objectType}, rackUid={rackPosUid}");
            if (TryRestorePending(__instance, rackPosUid, instanceId)) return;
            TryRestoreInstalled(__instance, rackPosUid, instanceId);
        }
        catch (Exception ex) { EventDispatcher.LogError($"ServerInsertedInRack: {ex.Message}"); }
    }

    private static string ReadInstanceId(Server? instance)
    {
        try { return instance?.ServerID ?? ""; } catch { return ""; }
    }

    private static byte ReadObjectType(Server? instance)
    {
        try { return (byte)(instance?.serverType ?? 0); } catch { return 0; }
    }

    private static int ResolveRackUid(Server? instance, ServerSaveData? save)
    {
        int rackPosUid = -1;
        try { rackPosUid = instance?.currentRackPosition?.rackPosGlobalUID ?? -1; } catch { }
        if (rackPosUid <= 0) try { rackPosUid = instance?.rackPositionUID ?? -1; } catch { }
        if (rackPosUid <= 0) try { rackPosUid = save?.rackPositionUID ?? -1; } catch { }
        return rackPosUid;
    }

    private static bool TryRestorePending(Server? instance, int rackPosUid, string instanceId)
    {
        try
        {
            var pending = Patch_Rack_MarkPositionAsUsed.ConsumePendingRestore(rackPosUid, 0);
            if (!pending.HasValue) return false;
            ApplyRestoredId(instance, rackPosUid, instanceId, pending.Value.objectId, "restored clone ID");
            return true;
        }
        catch { return false; }
    }

    private static void TryRestoreInstalled(Server? instance, int rackPosUid, string instanceId)
    {
        try
        {
            var installed = Patch_Rack_MarkPositionAsUsed.LookupInstalledObject(rackPosUid, 0);
            if (!installed.HasValue) return;
            ApplyRestoredId(instance, rackPosUid, instanceId, installed.Value.objectId, "restored clone ID (dict fallback)");
        }
        catch { }
    }

    private static void ApplyRestoredId(Server? instance, int rackPosUid, string instanceId, string stableId, string label)
    {
        try
        {
            if (string.IsNullOrEmpty(stableId) || instanceId == stableId) return;
            try { if (instance != null) instance.ServerID = stableId; } catch { }
            try { if (instance != null) instance.rackPositionUID = rackPosUid; } catch { }
            CrashLog.Log($"[WorldSync] ServerInsertedInRack: {label} '{instanceId}' → '{stableId}' rackUid={rackPosUid}");
        }
        catch { }
    }

    internal static int FindServerPrefabIndex(Server srv)
    {
        try
        {
            var mgr = MainGameManager.instance;
            if (mgr?.serverPrefabs == null) return 0;
            string prefix = StripCloneSuffix(srv.gameObject?.name ?? "");
            for (int i = 0; i < mgr.serverPrefabs.Count; i++)
            {
                try { if (mgr.serverPrefabs[i]?.name == prefix) return i; } catch { }
            }
        }
        catch { }
        return 0;
    }

    private static string StripCloneSuffix(string srvName)
    {
        try
        {
            if (srvName.EndsWith("(Clone)")) srvName = srvName.Substring(0, srvName.Length - 7);
            int idx = srvName.LastIndexOf('_');
            return idx > 0 ? srvName.Substring(0, idx) : srvName;
        }
        catch { return srvName; }
    }
}
