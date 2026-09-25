using System;
using HarmonyLib;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.InsertedInRack))]
internal static class Patch_PatchPanel_InsertedInRack
{
    internal static void Postfix(PatchPanel? __instance, PatchPanelSaveData? __0)
    {
        try
        {
            string currentId = ReadPanelId(__instance);
            int rackPosUid = ResolveRackUid(__instance, __0);
            CrashLog.Log($"PatchPanel.InsertedInRack [diag]: patchPanelId={currentId}, rackUid={rackPosUid}");
            if (TryRestorePending(__instance, rackPosUid, currentId)) return;
            TryRestoreInstalled(__instance, rackPosUid, currentId);
        }
        catch (Exception ex) { EventDispatcher.LogError($"PatchPanel.InsertedInRack: {ex.Message}"); }
    }

    private static string ReadPanelId(PatchPanel? instance)
    {
        try { return instance?.patchPanelId ?? ""; } catch { return ""; }
    }

    private static int ResolveRackUid(PatchPanel? instance, PatchPanelSaveData? save)
    {
        int rackPosUid = -1;
        try { rackPosUid = instance?.currentRackPosition?.rackPosGlobalUID ?? -1; } catch { }
        if (rackPosUid <= 0) try { rackPosUid = instance?.rackPositionUID ?? -1; } catch { }
        if (rackPosUid <= 0) try { rackPosUid = save?.rackPositionUID ?? -1; } catch { }
        return rackPosUid;
    }

    private static bool TryRestorePending(PatchPanel? instance, int rackPosUid, string currentId)
    {
        try
        {
            var pending = Patch_Rack_MarkPositionAsUsed.ConsumePendingRestore(rackPosUid, 7);
            if (!pending.HasValue) return false;
            ApplyRestoredId(instance, rackPosUid, currentId, pending.Value.objectId, "restored clone ID");
            return true;
        }
        catch { return false; }
    }

    private static void TryRestoreInstalled(PatchPanel? instance, int rackPosUid, string currentId)
    {
        try
        {
            var installed = Patch_Rack_MarkPositionAsUsed.LookupInstalledObject(rackPosUid, 7);
            if (!installed.HasValue) return;
            ApplyRestoredId(instance, rackPosUid, currentId, installed.Value.objectId, "restored clone ID (dict fallback)");
        }
        catch { }
    }

    private static void ApplyRestoredId(PatchPanel? instance, int rackPosUid, string currentId, string stableId, string label)
    {
        try
        {
            if (string.IsNullOrEmpty(stableId) || currentId == stableId) return;
            try { if (instance != null) instance.patchPanelId = stableId; } catch { }
            try { if (instance != null) instance.rackPositionUID = rackPosUid; } catch { }
            CrashLog.Log($"[WorldSync] PatchPanel.InsertedInRack: {label} '{currentId}' → '{stableId}' rackUid={rackPosUid}");
        }
        catch { }
    }
}
