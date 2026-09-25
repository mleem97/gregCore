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

public static partial class EntityManager
{
    public static uint SpawnCharacter(uint prefabIdx, float x, float y, float z, float rotY, string name)
    {
        try
        {
            GameObject go = null;
            var mgr = MainGameManager.instance;
            if (mgr != null && mgr.techniciansPrefabs != null && mgr.techniciansPrefabs.Length > 0)
            {
                int idx = (int)(prefabIdx % (uint)mgr.techniciansPrefabs.Length);
                var prefab = mgr.techniciansPrefabs[idx];

                bool prefabWasActive = prefab.activeSelf;
                prefab.SetActive(false);
                go = UnityEngine.Object.Instantiate(prefab);
                if (prefabWasActive) prefab.SetActive(true);
            }
            else
            {
                return SpawnCapsuleFallback(x, y, z, name);
            }

            if (go == null) return 0;

            go.SetActive(false);

            var spawnPos = new Vector3(x, y, z);

            go.transform.position = spawnPos;
            go.transform.eulerAngles = new Vector3(0, rotY, 0);

            StripSpawnedPrefabComponents(go);

            go.SetActive(true);

            Animator animator = go.GetComponentInChildren<Animator>();
            if (animator != null)
                animator.applyRootMotion = false;

            return RegisterSpawnedCharacter(go, name, spawnPos, animator);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("EntityManager.SpawnCharacter", ex);
            return 0;
        }
    }

    private static uint SpawnCapsuleFallback(float x, float y, float z, string name)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.transform.position = new Vector3(x, y, z);
        var col = go.GetComponent<Collider>();
        if (col != null) UnityEngine.Object.Destroy(col);

        uint capsuleId = _nextId++;
        var capsuleEntity = new ManagedEntity
        {
            Id = capsuleId,
            GO = go,
            WaitingForUMA = false,
        };
        go.name = $"Entity_{capsuleId}";
        AddNameTag(go, name, capsuleEntity);
        _entities[capsuleId] = capsuleEntity;
        CrashLog.Log($"[EntityManager] Spawned capsule fallback entity {capsuleId} '{name}'");
        return capsuleId;
    }

    private static void StripSpawnedPrefabComponents(GameObject go)
    {
        var navCheck = go.GetComponent<NavMeshAgent>();
        DisableNonUmaBehaviours(go);
        DestroySpawnPhysicsComponents(go, navCheck);
    }

    private static bool ShouldKeepSpawnBehaviour(string typeName)
    {
        return typeName.Contains("UMA") || typeName.Contains("DynamicCharacter") ||
            typeName.Contains("Avatar") || typeName.Contains("Generator") ||
            typeName == "Animator" || typeName.Contains("Renderer");
    }

    private static void DisableNonUmaBehaviours(GameObject go)
    {
        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb == null) continue;
            string typeName = mb.GetIl2CppType().Name;
            if (ShouldKeepSpawnBehaviour(typeName))
                continue;
            try { mb.enabled = false; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static void DestroySpawnPhysicsComponents(GameObject go, NavMeshAgent navCheck)
    {
        if (navCheck != null)
            try { UnityEngine.Object.DestroyImmediate(navCheck); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        DestroySpawnComponents<CharacterController>(go);
        DestroySpawnComponents<Collider>(go);
        DestroySpawnComponents<Rigidbody>(go);
        DestroySpawnComponents<NavMeshAgent>(go);
    }

    private static void DestroySpawnComponents<T>(GameObject go) where T : Component
    {
        foreach (var c in go.GetComponentsInChildren<T>(true))
            try { UnityEngine.Object.DestroyImmediate(c); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static uint RegisterSpawnedCharacter(GameObject go, string name, Vector3 spawnPos, Animator animator)
    {
        uint id = _nextId++;
        go.name = $"Entity_{id}";

        var entity = new ManagedEntity
        {
            Id = id,
            GO = go,
            Animator = animator,
            NavAgent = null, // NavMeshAgent destroyed — remote entities don't need pathfinding
            WaitingForUMA = true,
            UMAWaitStart = Time.time,
            LastPos = spawnPos,
        };

        AddNameTag(go, name, entity);
        _entities[id] = entity;

        CrashLog.Log($"[EntityManager] Spawned entity {id} '{name}' at ({spawnPos.x:F1},{spawnPos.y:F1},{spawnPos.z:F1}) anim={animator != null}");
        return id;
    }

    public static void Update()
    {
        foreach (var kvp in _entities)
        {
            var entity = kvp.Value;
            if (!entity.WaitingForUMA) continue;
            if (entity.GO == null) continue;

            int rendererCount;
            bool meshReady = TryGetUmaMeshReady(entity.GO, out rendererCount);
            if (!meshReady) continue;

            FinalizeUmaReadyEntity(entity, rendererCount);
        }
    }

    private static bool TryGetUmaMeshReady(GameObject go, out int rendererCount)
    {
        rendererCount = 0;
        var umaData = go.GetComponentInChildren<UMAData>(true);
        if (umaData == null || !umaData.isOfficiallyCreated)
            return false;

        try
        {
            var rends = umaData.GetRenderers();
            if (rends != null)
            {
                for (int r = 0; r < rends.Length; r++)
                {
                    var smr = rends[r];
                    if (smr != null && smr.sharedMesh != null)
                        rendererCount++;
                }
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"[EntityManager] UMAData.GetRenderers() error: {ex.Message}", ex);
        }
        return rendererCount > 0;
    }

    private static void FinalizeUmaReadyEntity(ManagedEntity entity, int rendererCount)
    {
        CrashLog.Log($"[EntityManager] UMA mesh ready for entity {entity.Id} {rendererCount} renderer");

        foreach (var mb in entity.GO.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb == null) continue;
            string typeName = mb.GetIl2CppType().Name;
            if (typeName == "Animator" || typeName.Contains("Renderer")) continue;
            try { mb.enabled = false; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        entity.WaitingForUMA = false;

        // Disable NavMeshAgent — remote entities don't need pathfinding
        if (entity.NavAgent != null && entity.NavAgent.enabled)
            entity.NavAgent.enabled = false;

        DiscoverAnimatorParams(entity);
        entity.AnimParamsDiscovered = true;
    }

    private static void DiscoverAnimatorParams(ManagedEntity entity)
    {
        if (entity.AnimParamsDiscovered) return;
        if (entity.Animator == null)
            entity.Animator = entity.GO.GetComponentInChildren<Animator>();
        if (entity.Animator == null) return;

        entity.Animator.applyRootMotion = false;
        try
        {
            foreach (var param in entity.Animator.parameters)
                ClassifyAnimatorParam(entity, param);
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"[EntityManager] Animator param discovery error: {ex.Message}", ex);
        }
    }

    private static void ClassifyAnimatorParam(ManagedEntity entity, AnimatorControllerParameter param)
    {
        string lower = param.name.ToLower();
        TryClassifySpeedParam(entity, param, lower);
        TryClassifyWalkingParam(entity, param, lower);
        ClassifyPoseAnimatorParam(entity, param, lower);
    }

    private static void TryClassifySpeedParam(ManagedEntity entity, AnimatorControllerParameter param, string lower)
    {
        if (entity.HasSpeedParam) return;
        if (param.type != AnimatorControllerParameterType.Float) return;
        if (!IsSpeedParamName(lower)) return;
        entity.SpeedParamHash = param.nameHash;
        entity.HasSpeedParam = true;
    }

    private static bool IsSpeedParamName(string lower)
    {
        return lower.Contains("speed") || lower.Contains("velocity") || lower.Contains("move") || lower.Contains("forward");
    }

    private static void TryClassifyWalkingParam(ManagedEntity entity, AnimatorControllerParameter param, string lower)
    {
        if (entity.HasWalkingParam) return;
        if (param.type != AnimatorControllerParameterType.Bool) return;
        if (!IsWalkingParamName(lower)) return;
        entity.WalkingParamHash = param.nameHash;
        entity.HasWalkingParam = true;
    }

    private static bool IsWalkingParamName(string lower)
    {
        return lower.Contains("walk") || lower.Contains("moving") || lower.Contains("run");
    }

    private static void ClassifyPoseAnimatorParam(ManagedEntity entity, AnimatorControllerParameter param, string lower)
    {
        TryClassifyCrouchParam(entity, param, lower);
        TryClassifySittingParam(entity, param, lower);
        TryClassifyCarryingParam(entity, param, lower);
    }

    private static void TryClassifyCrouchParam(ManagedEntity entity, AnimatorControllerParameter param, string lower)
    {
        if (entity.HasCrouchParam) return;
        if (param.type != AnimatorControllerParameterType.Bool) return;
        if (!lower.Contains("crouch")) return;
        entity.CrouchParamHash = param.nameHash;
        entity.HasCrouchParam = true;
    }

    private static void TryClassifySittingParam(ManagedEntity entity, AnimatorControllerParameter param, string lower)
    {
        if (entity.HasSittingParam) return;
        if (param.type != AnimatorControllerParameterType.Bool) return;
        if (lower != "issitting" && !lower.Contains("sitting")) return;
        entity.SittingParamHash = param.nameHash;
        entity.HasSittingParam = true;
    }

    private static void TryClassifyCarryingParam(ManagedEntity entity, AnimatorControllerParameter param, string lower)
    {
        if (entity.HasCarryingParam) return;
        if (param.type != AnimatorControllerParameterType.Bool) return;
        if (!lower.Contains("carry") && !lower.Contains("carrying")) return;
        entity.CarryingParamHash = param.nameHash;
        entity.HasCarryingParam = true;
    }

    private static void AddNameTag(GameObject parent, string name, ManagedEntity entity)
    {
        try
        {
            float scale = ComputeNameTagScale(parent);
            float fontSize = 5f;
            float rectW = 70f;
            float rectH = 10f;

            var canvasGO = new GameObject($"NameTag_Entity_{entity.Id}");
            canvasGO.transform.position = parent.transform.position + new Vector3(0, 1.75f, 0);

            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var canvasRect = canvasGO.GetComponent<RectTransform>();
            if (canvasRect != null)
                canvasRect.sizeDelta = new Vector2(rectW, rectH);

            canvasGO.transform.localScale = new Vector3(scale, scale, scale);

            SetupNameTagBackground(canvasGO);
            SetupNameTagText(canvasGO, name, fontSize);

            var bb = canvasGO.AddComponent<BillboardNameTag>();
            bb.followTarget = parent.transform;
            bb.offsetY = 1.85f;

            entity.NameTagGO = canvasGO;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("EntityManager.AddNameTag", ex);
        }
    }

    private static float ComputeNameTagScale(GameObject parent)
    {
        float scale = 0.01f;
        try
        {
            var parentScale = parent.transform.lossyScale;
            if (parentScale.x > 0.001f)
            {
                float compensate = 1f / parentScale.x;
                scale *= compensate;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return scale;
    }

    private static void SetupNameTagBackground(GameObject canvasGO)
    {
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);

        var bgImage = bgGO.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.45f);

        var bgRect = bgGO.GetComponent<RectTransform>();
        if (bgRect != null)
        {
            bgRect.anchorMin = new Vector2(0f, 0f);
            bgRect.anchorMax = new Vector2(1f, 1f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
        }
    }

    private static void SetupNameTagText(GameObject canvasGO, string name, float fontSize)
    {
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(canvasGO.transform, false);

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = name;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.enableWordWrapping = false;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = new Color32(0, 0, 0, 200);

        var rect = textGO.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
