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
    // ── v13: World Object Sync ────────────────────────────────────────

    static string ReadUtf8(IntPtr ptr, uint len)
    {
        if (ptr == IntPtr.Zero || len == 0) return "";
        byte[] buf = new byte[len];
        Marshal.Copy(ptr, buf, 0, (int)len);
        int end = Array.IndexOf(buf, (byte)0);
        if (end < 0) end = (int)len;
        return System.Text.Encoding.UTF8.GetString(buf, 0, end).Trim();
    }

    private ulong FindHandleByStableId(string targetId)
    {
        try
        {
            ulong handle = FindServerHandleByStableId(targetId);
            if (handle != 0) return handle;
            handle = FindSwitchHandleByStableId(targetId);
            if (handle != 0) return handle;
            return FindPatchPanelHandleByStableId(targetId);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("FindHandleByStableId", ex);
        }
        return 0;
    }

    private static ulong FindServerHandleByStableId(string targetId)
    {
        foreach (var srv in UnityEngine.Resources.FindObjectsOfTypeAll<Server>())
        {
            try
            {
                if (srv.gameObject.scene.name == null) continue;
                if ((srv.ServerID ?? "") == targetId) return (ulong)srv.Pointer.ToInt64();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return 0;
    }

    private static ulong FindSwitchHandleByStableId(string targetId)
    {
        foreach (var sw in UnityEngine.Resources.FindObjectsOfTypeAll<NetworkSwitch>())
        {
            try
            {
                if (sw.gameObject.scene.name == null) continue;
                if ((sw.switchId ?? "") == targetId) return (ulong)sw.Pointer.ToInt64();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return 0;
    }

    private static ulong FindPatchPanelHandleByStableId(string targetId)
    {
        foreach (var pp in UnityEngine.Resources.FindObjectsOfTypeAll<PatchPanel>())
        {
            try
            {
                if (pp.gameObject.scene.name == null) continue;
                if ((pp.patchPanelId ?? "") == targetId) return (ulong)pp.Pointer.ToInt64();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return 0;
    }

    uint WorldGetObjectCountImpl()
    {
        // Phase 4 stub
        return 0;
    }

    uint WorldGetObjectHashesImpl(IntPtr buf, uint maxCount)
    {
        // Phase 4 stub
        return 0;
    }

    uint WorldGetObjectStateImpl(IntPtr id, uint idLen, IntPtr buf, uint bufMax)
    {
        // Phase 4 stub
        return 0;
    }


    private static string ReadSpawnDesiredId(IntPtr outId, uint outMax)
    {
        string desiredId = null;
        if (outId != IntPtr.Zero && outMax > 0)
        {
            byte firstByte = Marshal.ReadByte(outId);
            if (firstByte != 0)
            {
                desiredId = ReadUtf8(outId, outMax);
            }
        }
        return desiredId;
    }

    private static UnityEngine.GameObject ResolveSpawnPrefab(Il2Cpp.MainGameManager mgr, byte objectType, int prefabId)
    {
        if (objectType == 4)
            return ResolveSwitchSpawnPrefab(mgr, prefabId);
        if (objectType == 7)
            return ResolvePatchPanelSpawnPrefab(mgr, prefabId);
        return ResolveServerSpawnPrefab(mgr, prefabId);
    }

    private static UnityEngine.GameObject ResolveSwitchSpawnPrefab(Il2Cpp.MainGameManager mgr, int prefabId)
    {
        try
        {
            if (mgr.switchesPrefabs != null && prefabId >= 0 && prefabId < mgr.switchesPrefabs.Count)
                return mgr.switchesPrefabs[prefabId];
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] SpawnObject: switchesPrefabs lookup failed: {ex.Message}");
        }
        return null;
    }

    private static UnityEngine.GameObject ResolvePatchPanelSpawnPrefab(Il2Cpp.MainGameManager mgr, int prefabId)
    {
        try
        {
            return mgr.GetPatchPanelPrefab(prefabId);
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] SpawnObject: GetPatchPanelPrefab failed: {ex.Message}");
        }
        return null;
    }

    private static UnityEngine.GameObject ResolveServerSpawnPrefab(Il2Cpp.MainGameManager mgr, int prefabId)
    {
        try
        {
            if (mgr.serverPrefabs != null && prefabId >= 0 && prefabId < mgr.serverPrefabs.Count)
                return mgr.serverPrefabs[prefabId];
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] SpawnObject: serverPrefabs lookup failed: {ex.Message}");
        }
        return null;
    }

    private static string ConfigureSpawnedObject(UnityEngine.GameObject go, byte objectType, string desiredId)
    {
        string resultId = go.name;

        if (objectType == 4)
            resultId = ConfigureSpawnedSwitch(go, desiredId, resultId);
        else if (objectType == 7)
            resultId = ConfigureSpawnedPatchPanel(go, desiredId, resultId);
        else
            resultId = ConfigureSpawnedServer(go, desiredId, resultId);

        return resultId;
    }

    private static string ConfigureSpawnedSwitch(UnityEngine.GameObject go, string desiredId, string fallbackId)
    {
        string resultId = fallbackId;
        try
        {
            var switchComp = go.GetComponent<Il2Cpp.NetworkSwitch>();
            if (switchComp != null)
            {
                if (!string.IsNullOrEmpty(desiredId))
                {
                    switchComp.switchId = desiredId;
                    resultId = desiredId;
                    CrashLog.Log($"[WorldSync] SpawnObject: set switchId to '{desiredId}'");
                }
                else
                {
                    resultId = switchComp.switchId ?? go.name;
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] SpawnObject: switch setup failed: {ex.Message}");
        }

        ResetSpawnedRigidBody(go);
        return resultId;
    }

    private static string ConfigureSpawnedPatchPanel(UnityEngine.GameObject go, string desiredId, string fallbackId)
    {
        string resultId = fallbackId;
        try
        {
            var ppComp = go.GetComponent<Il2Cpp.PatchPanel>();
            if (ppComp != null)
            {
                if (!string.IsNullOrEmpty(desiredId))
                {
                    ppComp.patchPanelId = desiredId;
                    resultId = desiredId;
                    CrashLog.Log($"[WorldSync] SpawnObject: set patchPanelId to '{desiredId}'");
                }
                else
                {
                    resultId = BuildPatchPanelFallbackId(go, ppComp);
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] SpawnObject: patchpanel setup failed: {ex.Message}");
        }

        ResetSpawnedRigidBody(go);
        return resultId;
    }

    private static string BuildPatchPanelFallbackId(UnityEngine.GameObject go, Il2Cpp.PatchPanel ppComp)
    {
        string ppId = ppComp.patchPanelId ?? "";
        if (string.IsNullOrEmpty(ppId))
        {
            int instId = go.GetInstanceID();
            string objName = go.name ?? "PatchPanel";
            if (objName.EndsWith("(Clone)"))
                objName = objName.Substring(0, objName.Length - 7);
            ppId = $"{objName}_{instId}";
            ppComp.patchPanelId = ppId;
        }
        return ppId;
    }

    private static string ConfigureSpawnedServer(UnityEngine.GameObject go, string desiredId, string fallbackId)
    {
        string resultId = fallbackId;
        // Server setup
        try
        {
            var serverComp = go.GetComponent<Il2Cpp.Server>();
            if (serverComp != null)
            {
                if (!string.IsNullOrEmpty(desiredId))
                {
                    serverComp.ServerID = desiredId;
                    resultId = desiredId;
                    CrashLog.Log($"[WorldSync] SpawnObject: set ServerID to '{desiredId}'");
                }
                else
                {
                    resultId = serverComp.ServerID ?? go.name;
                }

                ResetServerRigidBody(serverComp);
            }
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] SpawnObject: server setup failed: {ex.Message}");
        }
        return resultId;
    }

    private static void ResetServerRigidBody(Il2Cpp.Server serverComp)
    {
        try
        {
            var rb = serverComp.rb;
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = UnityEngine.Vector3.zero;
                rb.angularVelocity = UnityEngine.Vector3.zero;
                rb.WakeUp();
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void ResetSpawnedRigidBody(UnityEngine.GameObject go)
    {
        try
        {
            var rb = go.GetComponent<UnityEngine.Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.velocity = UnityEngine.Vector3.zero;
                rb.angularVelocity = UnityEngine.Vector3.zero;
                rb.WakeUp();
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void WriteSpawnResultId(IntPtr outId, uint outMax, string resultId)
    {
        if (outId == IntPtr.Zero || outMax == 0) return;
        for (uint i = 0; i < outMax && i < 128; i++)
            Marshal.WriteByte(outId, (int)i, 0);

        var bytes = System.Text.Encoding.UTF8.GetBytes(resultId);
        int copyLen = Math.Min(bytes.Length, (int)outMax - 1);
        Marshal.Copy(bytes, 0, outId, copyLen);
        Marshal.WriteByte(outId, copyLen, 0);
    }

    int WorldDestroyObjectImpl(IntPtr id, uint idLen)
    {
        // Phase 3 stub
        string objId = ReadUtf8(id, idLen);
        CrashLog.Log($"[WorldSync] DestroyObject stub: id={objId}");
        return 0;
    }
}
