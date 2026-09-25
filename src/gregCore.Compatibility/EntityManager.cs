using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.AI;
using Il2Cpp;
using Il2CppUMA;
using Il2CppUMA.CharacterSystem;
using Il2CppTMPro;

namespace DataCenterModLoader;

public class BillboardNameTag : MonoBehaviour
{
    public Transform followTarget { get; set; } = null!;
    public float offsetY { get; set; } = 1.85f;

    void Update()
    {
        if (followTarget == null) return;
        transform.position = followTarget.position + new Vector3(0, offsetY, 0);
        var cam = Camera.main;
        if (cam != null)
        {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}

public static partial class EntityManager
{
    private class ManagedEntity
    {
        public uint Id = 0;
        public GameObject GO = null!;
        public Animator? Animator;
        public NavMeshAgent? NavAgent;
        public bool WaitingForUMA = false;
        public float UMAWaitStart = 0f;
        public int SpeedParamHash = 0;
        public int WalkingParamHash = 0;
        public bool HasSpeedParam = false;
        public bool HasWalkingParam = false;
        public bool AnimParamsDiscovered;
        public int CrouchParamHash = 0;
        public int SittingParamHash = 0;
        public int CarryingParamHash = 0;
        public bool HasCrouchParam = false;
        public bool HasSittingParam = false;
        public bool HasCarryingParam = false;
        public GameObject? NameTagGO;
        public Vector3 LastPos;
        public GameObject? CarryProxyGO;
        public Transform? HandBone;
        public bool HandBoneSearched;
        public bool ColliderAdded;
    }

    private static readonly Dictionary<uint, ManagedEntity> _entities = new();
    private static uint _nextId = 1;
    private static bool _gameOffsetsLogged = false;

    public static void DestroyEntity(uint entityId)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.CarryProxyGO != null) UnityEngine.Object.Destroy(entity.CarryProxyGO);
        if (entity.NameTagGO != null) UnityEngine.Object.Destroy(entity.NameTagGO);
        if (entity.GO != null) UnityEngine.Object.Destroy(entity.GO);
        _entities.Remove(entityId);
    }

    public static void SetPosition(uint entityId, float x, float y, float z, float rotY)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.GO == null) { _entities.Remove(entityId); return; }

        // Direct transform — no NavMeshAgent involvement for remote entities
        entity.GO.transform.position = new Vector3(x, y, z);
        entity.GO.transform.eulerAngles = new Vector3(0f, rotY, 0f);
    }

    public static bool IsEntityReady(uint entityId)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return false;
        return !entity.WaitingForUMA;
    }

    public static void SetAnimation(uint entityId, float speed, bool isWalking)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.Animator == null) return;
        try
        {
            if (entity.HasSpeedParam)
            {
                // Smooth the speed to avoid jittery animation blending
                float current = entity.Animator.GetFloat(entity.SpeedParamHash);
                float smoothed = Mathf.Lerp(current, speed, Time.deltaTime * 8f);
                entity.Animator.SetFloat(entity.SpeedParamHash, smoothed);
            }
            if (entity.HasWalkingParam)
                entity.Animator.SetBool(entity.WalkingParamHash, isWalking);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    /// <summary>Set just the carry animator bool (cheap, can be called every frame)</summary>
    public static void SetCarryAnim(uint entityId, bool isCarrying)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.Animator == null || !entity.HasCarryingParam) return;
        try { entity.Animator.SetBool(entity.CarryingParamHash, isCarrying); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    /// <summary>Destroy the carry visual proxy</summary>
    public static void DestroyCarryVisual(uint entityId)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.CarryProxyGO != null)
        {
            UnityEngine.Object.Destroy(entity.CarryProxyGO);
            entity.CarryProxyGO = null;
        }
    }

    public static Vector3? GetEntityPosition(uint entityId)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return null;
        if (entity.GO == null) return null;
        return entity.GO.transform.position;
    }

    public static void AddEntityCollider(uint entityId)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.ColliderAdded || entity.GO == null) return;
        try
        {
            var capsule = entity.GO.AddComponent<CapsuleCollider>();
            capsule.center = new Vector3(0f, 0.9f, 0f);
            capsule.radius = 0.3f;
            capsule.height = 1.8f;
            entity.ColliderAdded = true;
            CrashLog.Log($"[EntityManager] Added collision capsule to entity {entity.Id}");
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"[EntityManager] Failed to add collider to entity {entity.Id}", ex);
        }
    }

    public static void SetEntityCarryTransform(uint entityId, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.CarryProxyGO == null) return;
        entity.CarryProxyGO.transform.localPosition = new Vector3(posX, posY, posZ);
        entity.CarryProxyGO.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    /// <summary>Find the right hand bone in a humanoid UMA rig</summary>
    private static Transform? FindHandBone(Transform root)
    {
        // UMA humanoid rigs use standard naming; search for right hand
        string[] handNames = { "Right Hand", "RightHand", "Hand_R", "hand_r", "R_Hand", "Bip01 R Hand" };
        foreach (var name in handNames)
        {
            var bone = FindChildRecursive(root, name);
            if (bone != null) return bone;
        }

        // Fallback: search for any transform containing "hand" and "r" (case insensitive)
        return FindChildByPattern(root, t =>
        {
            string n = t.name.ToLower();
            return n.Contains("hand") && (n.Contains("right") || (n.Contains("_r") || n.Contains(".r") || n.StartsWith("r_") || n.EndsWith(" r")));
        });
    }

    private static Transform? FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            var found = FindChildRecursive(parent.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }

    private static Transform? FindChildByPattern(Transform parent, Func<Transform, bool> predicate)
    {
        if (predicate(parent)) return parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            var found = FindChildByPattern(parent.GetChild(i), predicate);
            if (found != null) return found;
        }
        return null;
    }


    /// <summary>Cached prefab templates per ObjectInHand type (stripped visual clones)</summary>
    private static readonly Dictionary<uint, GameObject?> _carryPrefabCache = new();

    /// <summary>Strip all non-visual components from a GameObject (physics, scripts, nav)</summary>
    private static void StripToVisualOnly(GameObject go)
    {
        StripPhysicsComponents(go);
        StripScriptComponents(go);
    }

    private static void StripPhysicsComponents(GameObject go)
    {
        // Remove all colliders
        foreach (var col in go.GetComponentsInChildren<Collider>(true))
            try { UnityEngine.Object.DestroyImmediate(col); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

        // Remove all rigidbodies
        foreach (var rb in go.GetComponentsInChildren<Rigidbody>(true))
            try { UnityEngine.Object.DestroyImmediate(rb); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

        // Remove NavMeshAgents
        foreach (var nav in go.GetComponentsInChildren<NavMeshAgent>(true))
            try { UnityEngine.Object.DestroyImmediate(nav); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

        // Remove CharacterControllers
        foreach (var cc in go.GetComponentsInChildren<CharacterController>(true))
            try { UnityEngine.Object.DestroyImmediate(cc); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void StripScriptComponents(GameObject go)
    {
        // Remove all game scripts (MonoBehaviours) — keeps Transform, MeshFilter, MeshRenderer, etc.
        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
            try { UnityEngine.Object.DestroyImmediate(mb); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

        // Disable animators (don't want independent animation)
        foreach (var anim in go.GetComponentsInChildren<Animator>(true))
            try { anim.enabled = false; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void SetCrouching(uint entityId, bool isCrouching)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.Animator == null) return;
        try
        {
            if (entity.HasCrouchParam)
                entity.Animator.SetBool(entity.CrouchParamHash, isCrouching);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void SetSitting(uint entityId, bool isSitting)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.Animator == null) return;
        try
        {
            if (entity.HasSittingParam)
                entity.Animator.SetBool(entity.SittingParamHash, isSitting);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static uint GetPrefabCount()
    {
        try
        {
            var mgr = MainGameManager.instance;
            if (mgr != null && mgr.techniciansPrefabs != null)
                return (uint)mgr.techniciansPrefabs.Length;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return 0;
    }

    public static void SetEntityName(uint entityId, string name)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        if (entity.NameTagGO == null) return;
        try
        {
            var tmp = entity.NameTagGO.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = name;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void DestroyAll()
    {
        foreach (var kvp in _entities)
        {
            if (kvp.Value.CarryProxyGO != null) UnityEngine.Object.Destroy(kvp.Value.CarryProxyGO);
            if (kvp.Value.NameTagGO != null) UnityEngine.Object.Destroy(kvp.Value.NameTagGO);
            if (kvp.Value.GO != null) UnityEngine.Object.Destroy(kvp.Value.GO);
        }
        _entities.Clear();
        foreach (var kvp in _carryPrefabCache)
            if (kvp.Value != null) UnityEngine.Object.Destroy(kvp.Value);
        _carryPrefabCache.Clear();
        _nextId = 1;
    }
}
