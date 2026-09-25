using System;
using System.Runtime.InteropServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

public partial class GameAPIManager
{
    // Native FFI entry points: signatures are fixed by the C ABI on the
    // native side (see GameAPI.Bind.cs delegate wiring). They intentionally
    // carry wide parameter lists — excluded from the parameter-count rule
    // in .codacy/codacy.yaml. Orchestrator bodies delegate to small
    // helpers in GameAPI.World.cs / GameAPI.Sync.cs / GameAPI.Place.cs.

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S107:Methods should not have too many parameters", Justification = "Native FFI ABI fixes this signature; see GameAPI.Bind.cs delegate wiring.")]
    int WorldSpawnObjectImpl(byte objectType, int prefabId, float x, float y, float z, float rotX, float rotY, float rotZ, float rotW, IntPtr outId, uint outMax)
    {
        try
        {
            string desiredId = ReadSpawnDesiredId(outId, outMax);

            CrashLog.Log($"[WorldSync] SpawnObject: type={objectType}, prefab={prefabId}, desiredId='{desiredId ?? "(none)"}', pos=({x:F1},{y:F1},{z:F1})");

            var mgr = Il2Cpp.MainGameManager.instance;
            if (mgr == null)
            {
                CrashLog.Log("[WorldSync] SpawnObject: MainGameManager is null");
                return 0;
            }

            UnityEngine.GameObject prefab = ResolveSpawnPrefab(mgr, objectType, prefabId);

            if (prefab == null)
            {
                CrashLog.Log($"[WorldSync] SpawnObject: no prefab found for type={objectType} prefabId={prefabId}");
                return 0;
            }

            var pos = new UnityEngine.Vector3(x, y, z);
            var rot = new UnityEngine.Quaternion(rotX, rotY, rotZ, rotW);
            var go = UnityEngine.Object.Instantiate(prefab, pos, rot);
            if (go == null)
            {
                CrashLog.Log("[WorldSync] SpawnObject: Instantiate returned null");
                return 0;
            }

            try
            {
                if (mgr.parentUsableObjects != null)
                    go.transform.SetParent(mgr.parentUsableObjects, true);
            }
            catch (Exception ex)
            {
                CrashLog.Log($"[WorldSync] SpawnObject: parenting failed (non-fatal): {ex.Message}");
            }

            string resultId = ConfigureSpawnedObject(go, objectType, desiredId);

            WriteSpawnResultId(outId, outMax, resultId);

            try { SpawnedObjectTracker.RegisterRemoteSpawn(go.GetInstanceID(), resultId); }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            CrashLog.Log($"[WorldSync] SpawnObject: created '{resultId}' (type={objectType}, prefab={prefabId}) OK");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("WorldSpawnObjectImpl", ex);
            return 0;
        }
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S107:Methods should not have too many parameters", Justification = "Native FFI ABI fixes this signature; see GameAPI.Bind.cs delegate wiring.")]
    int WorldConnectCableImpl(int cableId, byte startType, float sx, float sy, float sz, IntPtr startDevice, uint startDeviceLen, byte endType, float ex, float ey, float ez, IntPtr endDevice, uint endDeviceLen)
    {
        // Phase 3 stub
        CrashLog.Log($"[WorldSync] ConnectCable stub: cableId={cableId}");
        return 0;
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S107:Methods should not have too many parameters", Justification = "Native FFI ABI fixes this signature; see GameAPI.Bind.cs delegate wiring.")]
    int WorldDropObjectImpl(IntPtr id, uint idLen, float x, float y, float z, float rotX, float rotY, float rotZ, float rotW)
    {
        try
        {
            string objId = ReadUtf8(id, idLen);
            CrashLog.Log($"[WorldSync] DropObject: id={objId}, pos=({x:F1},{y:F1},{z:F1})");

            ulong handle = FindHandleByStableId(objId);
            if (handle == 0)
            {
                CrashLog.Log($"[WorldSync] DropObject: '{objId}' not found");
                return 0;
            }

            var comp = ResolveComponent(handle);
            if (comp == null)
            {
                CrashLog.Log($"[WorldSync] DropObject: component null for '{objId}'");
                return 0;
            }

            // ── 1. Lock physics WHILE STILL INACTIVE ────────────────────────
            var rb = LockDropRigidBody(comp);

            // ── 2. Clear objectInHands BEFORE activation ─────────────────────
            TryResetObjectInHands(handle);

            // ── 3. Teleport & reparent WHILE STILL INACTIVE ─────────────────
            TeleportDropToPose(comp, x, y, z, rotX, rotY, rotZ, rotW);

            // ── 4. Activate at the correct position ─────────────────────────
            ActivateDropComponent(comp);

            // ── 5. Flush new position into PhysX broadphase ─────────────────
            UnityEngine.Physics.SyncTransforms();

            // ── 6. Release physics ───────────────────────────────────────────
            ReleaseDropRigidBody(rb, handle);

            CrashLog.Log($"[WorldSync] DropObject: reactivated '{objId}' at ({x:F1},{y:F1},{z:F1})");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("WorldDropObjectImpl", ex);
            return 0;
        }
    }
}
