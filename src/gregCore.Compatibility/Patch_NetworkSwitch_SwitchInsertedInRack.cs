using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SwitchInsertedInRack))]
internal static class Patch_NetworkSwitch_SwitchInsertedInRack
{
    internal static void Postfix(NetworkSwitch? __instance, SwitchSaveData? __0)
    {
        try
        {
            string currentId = ReadSwitchId(__instance);
            int rackPosUid = ResolveRackUid(__instance, __0);
            CrashLog.Log($"SwitchInsertedInRack [diag]: switchId={currentId}, rackUid={rackPosUid}");
            if (TryRestorePending(__instance, rackPosUid, currentId)) return;
            TryRestoreInstalled(__instance, rackPosUid, currentId);
        }
        catch (Exception ex) { EventDispatcher.LogError($"SwitchInsertedInRack: {ex.Message}"); }
    }

    private static string ReadSwitchId(NetworkSwitch? instance)
    {
        try { return instance?.switchId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return ""; }
    }

    private static int ResolveRackUid(NetworkSwitch? instance, SwitchSaveData? save)
    {
        int rackPosUid = -1;
        try { rackPosUid = instance?.currentRackPosition?.rackPosGlobalUID ?? -1; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        if (rackPosUid <= 0) try { rackPosUid = instance?.rackPositionUID ?? -1; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        if (rackPosUid <= 0) try { rackPosUid = save?.rackPositionUID ?? -1; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return rackPosUid;
    }

    private static bool TryRestorePending(NetworkSwitch? instance, int rackPosUid, string currentId)
    {
        try
        {
            var pending = Patch_Rack_MarkPositionAsUsed.ConsumePendingRestore(rackPosUid, 4);
            if (!pending.HasValue) return false;
            ApplyRestoredId(instance, rackPosUid, currentId, pending.Value.objectId, "restored clone ID");
            return true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private static void TryRestoreInstalled(NetworkSwitch? instance, int rackPosUid, string currentId)
    {
        try
        {
            var installed = Patch_Rack_MarkPositionAsUsed.LookupInstalledObject(rackPosUid, 4);
            if (!installed.HasValue) return;
            ApplyRestoredId(instance, rackPosUid, currentId, installed.Value.objectId, "restored clone ID (dict fallback)");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void ApplyRestoredId(NetworkSwitch? instance, int rackPosUid, string currentId, string stableId, string label)
    {
        try
        {
            if (string.IsNullOrEmpty(stableId) || currentId == stableId) return;
            try { if (instance != null) instance.switchId = stableId; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            try { if (instance != null) instance.rackPositionUID = rackPosUid; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            CrashLog.Log($"[WorldSync] SwitchInsertedInRack: {label} '{currentId}' → '{stableId}' rackUid={rackPosUid}");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }
}
