using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

public partial class GameAPIManager
{
    private Il2Cpp.RackPosition FindRackPosition(int rackUid)
    {
        for (int attempt = 0; attempt < 2; attempt++)
        {
            var found = SearchRackPositionOnce(rackUid);
            if (found != null) return found;

            if (attempt == 0)
            {
                CrashLog.Log($"[WorldSync] FindRackPosition: uid={rackUid} not found, reassigning UIDs…");
                GameHooks.EnsureAllRackPositionUIDs();
            }
        }
        return null;
    }

    private static Il2Cpp.RackPosition SearchRackPositionOnce(int rackUid)
    {
        var positions = UnityEngine.Object.FindObjectsOfType<Il2Cpp.RackPosition>();
        foreach (var rp in positions)
        {
            try { if (rp.rackPosGlobalUID == rackUid) return rp; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return null;
    }

    private int RackInstallBookkeeping(IntPtr ptr, Il2Cpp.RackPosition rackPos, byte objectType, string logPrefix)
    {
        switch (objectType)
        {
            case 0: // Server1U
            case 1: // (alias)
            case 2: // Server7U
            case 3: // Server3U
                return InstallServerBookkeeping(ptr, rackPos, logPrefix);
            case 4: // NetworkSwitch
                return InstallSwitchBookkeeping(ptr, rackPos, logPrefix);
            case 7: // PatchPanel
                return InstallPatchPanelBookkeeping(ptr, rackPos, logPrefix);
            default:
                CrashLog.Log($"[WorldSync] {logPrefix}: unknown objectType={objectType}, no bookkeeping");
                return 1;
        }
    }

    private static int InstallServerBookkeeping(IntPtr ptr, Il2Cpp.RackPosition rackPos, string logPrefix)
    {
        int sizeInU = 1;
        var server = new Il2Cpp.Server(ptr);
        try
        {
            sizeInU = server.sizeInU > 0 ? server.sizeInU : 1;
            int rackUid = rackPos.rackPosGlobalUID;

            var sd = new Il2Cpp.ServerSaveData();
            sd.serverID = server.ServerID;
            sd.rackPositionUID = rackUid;
            sd.serverType = server.serverType;
            sd.position = rackPos.transform.position;
            sd.rotation = rackPos.transform.rotation;
            try { sd.isOn = server.isOn; } catch { sd.isOn = false; }
            try { sd.isBroken = server.isBroken; } catch { sd.isBroken = false; }

            server.ServerInsertedInRack(sd);
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: ServerInsertedInRack failed: {ex.Message}");
        }

        UpdateInstalledServerFields(server, rackPos, logPrefix);
        return sizeInU;
    }

    private static void UpdateInstalledServerFields(Il2Cpp.Server server, Il2Cpp.RackPosition rackPos, string logPrefix)
    {
        try
        {
            server.rackPositionUID = rackPos.rackPosGlobalUID;
            server.currentRackPosition = rackPos;
            server.objectInHands = false;
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: server field update failed: {ex.Message}");
        }
    }

    private static int InstallSwitchBookkeeping(IntPtr ptr, Il2Cpp.RackPosition rackPos, string logPrefix)
    {
        int sizeInU = 1;
        try
        {
            var sw = new Il2Cpp.NetworkSwitch(ptr);
            int rackUid = rackPos.rackPosGlobalUID;
            sizeInU = sw.sizeInU > 0 ? sw.sizeInU : 1;

            var sd = new Il2Cpp.SwitchSaveData();
            sd.switchID = sw.switchId;
            sd.rackPositionUID = rackUid;
            sd.switchType = sw.switchType;
            sd.position = rackPos.transform.position;
            sd.rotation = rackPos.transform.rotation;
            try { sd.isOn = sw.isOn; } catch { sd.isOn = false; }
            try { sd.isBroken = sw.isBroken; } catch { sd.isBroken = false; }
            // sd.label = sw.label; // Label seems removed in Unity 6 version of NetworkSwitch
            sd.label = "";

            sw.SwitchInsertedInRack(sd);
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: SwitchInsertedInRack failed: {ex.Message}");
        }

        UpdateInstalledSwitchFields(ptr, rackPos, logPrefix);
        return sizeInU;
    }

    private static void UpdateInstalledSwitchFields(IntPtr ptr, Il2Cpp.RackPosition rackPos, string logPrefix)
    {
        try
        {
            var sw = new Il2Cpp.NetworkSwitch(ptr);
            sw.rackPositionUID = rackPos.rackPosGlobalUID;
            sw.currentRackPosition = rackPos;
            sw.objectInHands = false;
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: switch field update failed: {ex.Message}");
        }
    }

    private static int InstallPatchPanelBookkeeping(IntPtr ptr, Il2Cpp.RackPosition rackPos, string logPrefix)
    {
        int sizeInU = 1;
        try
        {
            var pp = new Il2Cpp.PatchPanel(ptr);
            int rackUid = rackPos.rackPosGlobalUID;
            sizeInU = pp.sizeInU > 0 ? pp.sizeInU : 1;

            var sd = new Il2Cpp.PatchPanelSaveData();
            sd.patchPanelID = pp.patchPanelId;
            sd.rackPositionUID = rackUid;
            sd.patchPanelType = pp.patchPanelType;
            sd.position = rackPos.transform.position;
            sd.rotation = rackPos.transform.rotation;

            pp.InsertedInRack(sd);
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: PatchPanel.InsertedInRack failed: {ex.Message}");
        }

        UpdateInstalledPatchPanelFields(ptr, rackPos, logPrefix);
        return sizeInU;
    }

    private static void UpdateInstalledPatchPanelFields(IntPtr ptr, Il2Cpp.RackPosition rackPos, string logPrefix)
    {
        try
        {
            var pp = new Il2Cpp.PatchPanel(ptr);
            pp.rackPositionUID = rackPos.rackPosGlobalUID;
            pp.currentRackPosition = rackPos;
            pp.objectInHands = false;
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: patchpanel field update failed: {ex.Message}");
        }
    }

    private void RackUninstallBookkeeping(IntPtr ptr, byte objectType, string logPrefix)
    {
        switch (objectType)
        {
            case 0: // Server1U
            case 1: // (alias)
            case 2: // Server7U
            case 3: // Server3U
                ClearServerRackFields(ptr, logPrefix);
                break;
            case 4: // NetworkSwitch
                ClearSwitchRackFields(ptr, logPrefix);
                break;
            case 7: // PatchPanel
                ClearPatchPanelRackFields(ptr, logPrefix);
                break;
            default:
                CrashLog.Log($"[WorldSync] {logPrefix}: unknown objectType={objectType}, no bookkeeping");
                break;
        }
    }

    private static void ClearServerRackFields(IntPtr ptr, string logPrefix)
    {
        try
        {
            var server = new Il2Cpp.Server(ptr);
            server.rackPositionUID = 0;
            server.currentRackPosition = null;
            server.objectInHands = true;
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: server field clear failed: {ex.Message}");
        }
    }

    private static void ClearSwitchRackFields(IntPtr ptr, string logPrefix)
    {
        try
        {
            var sw = new Il2Cpp.NetworkSwitch(ptr);
            sw.rackPositionUID = 0;
            sw.currentRackPosition = null;
            sw.objectInHands = true;
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: switch field clear failed: {ex.Message}");
        }
    }

    private static void ClearPatchPanelRackFields(IntPtr ptr, string logPrefix)
    {
        try
        {
            var pp = new Il2Cpp.PatchPanel(ptr);
            pp.rackPositionUID = 0;
            pp.currentRackPosition = null;
            pp.objectInHands = true;
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] {logPrefix}: patchpanel field clear failed: {ex.Message}");
        }
    }
}
