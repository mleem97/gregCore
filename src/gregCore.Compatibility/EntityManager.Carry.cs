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
    /// <summary>Create a visual proxy from real game prefab, parented to entity root</summary>
    public static void CreateCarryVisual(uint entityId, uint objectInHandType)
    {
        if (!_entities.TryGetValue(entityId, out var entity)) return;
        try
        {
            LogGameItemOffsets();

            // Destroy existing proxy if any
            if (entity.CarryProxyGO != null)
            {
                UnityEngine.Object.Destroy(entity.CarryProxyGO);
                entity.CarryProxyGO = null;
            }

            EnsureHandBone(entity);

            // Try real game prefab first, fall back to primitive
            GameObject proxy = TryCreateFromGamePrefab(objectInHandType);
            if (proxy == null)
                proxy = CreateFallbackProxy(objectInHandType);

            AttachCarryProxy(entity, proxy);

            CrashLog.Log($"[EntityManager] Created carry visual type={objectInHandType} prefab={proxy != null} for entity {entity.Id} parent={proxy?.transform.parent?.name ?? "none"} (using entity root)");
        }
        catch (Exception ex) { CrashLog.LogException("EntityManager.CreateCarryVisual", ex); }
    }

    private static void EnsureHandBone(ManagedEntity entity)
    {
        // Find hand bone if not searched yet
        if (!entity.HandBoneSearched && entity.GO != null)
        {
            entity.HandBoneSearched = true;
            entity.HandBone = FindHandBone(entity.GO.transform);
            if (entity.HandBone != null)
                CrashLog.Log($"[EntityManager] Found hand bone '{entity.HandBone.name}' for entity {entity.Id}");
            else
                CrashLog.Log($"[EntityManager] No hand bone found for entity {entity.Id}");
        }
    }

    private static void AttachCarryProxy(ManagedEntity entity, GameObject proxy)
    {
        if (proxy == null) return;
        Transform parent = entity.GO.transform;
        if (parent != null)
        {
            proxy.transform.SetParent(parent, false);
            proxy.transform.localPosition = Vector3.zero;
            proxy.transform.localRotation = Quaternion.identity;
        }
        entity.CarryProxyGO = proxy;
    }

    private static void LogGameItemOffsets()
    {
        if (_gameOffsetsLogged) return;
        _gameOffsetsLogged = true;
        try
        {
            LogMoveItemPosition();
            LogObjectInHandArray();
            LogUsableObjectOffsets();
        }
        catch (Exception ex)
        {
            CrashLog.LogException("[CarryDebug] LogGameItemOffsets", ex);
        }
    }

    private static void LogMoveItemPosition()
    {
        // Log moveItemPosition from PlayerManager
        var pm = PlayerManager.instance;
        if (pm != null && pm.moveItemPosition != null)
        {
            var mip = pm.moveItemPosition;
            CrashLog.Log($"[CarryDebug] moveItemPosition localPos=({mip.localPosition.x:F3},{mip.localPosition.y:F3},{mip.localPosition.z:F3}) localRot=({mip.localEulerAngles.x:F1},{mip.localEulerAngles.y:F1},{mip.localEulerAngles.z:F1})");
        }
    }

    private static void LogObjectInHandArray()
    {
        var pm = PlayerManager.instance;
        // Log objectInHandGO array
        if (pm != null && pm.objectInHandGO != null)
        {
            CrashLog.Log($"[CarryDebug] objectInHandGO.Length={pm.objectInHandGO.Length}");
            for (int i = 0; i < pm.objectInHandGO.Length; i++)
            {
                var go = pm.objectInHandGO[i];
                if (go != null)
                    CrashLog.Log($"[CarryDebug]   [{i}] name='{go.name}' active={go.activeSelf} localPos=({go.transform.localPosition.x:F3},{go.transform.localPosition.y:F3},{go.transform.localPosition.z:F3}) localRot=({go.transform.localEulerAngles.x:F1},{go.transform.localEulerAngles.y:F1},{go.transform.localEulerAngles.z:F1}) localScale=({go.transform.localScale.x:F3},{go.transform.localScale.y:F3},{go.transform.localScale.z:F3})");
                else
                    CrashLog.Log($"[CarryDebug]   [{i}] null");
            }
        }
    }

    private static void LogUsableObjectOffsets()
    {
        // Find all UsableObject instances and log their offsets
        var usableObjects = UnityEngine.Object.FindObjectsOfType<UsableObject>();
        if (usableObjects == null) return;
        CrashLog.Log($"[CarryDebug] Found {usableObjects.Length} UsableObject instances in scene");
        // Log just a few unique types to avoid spam
        var loggedTypes = new HashSet<int>();
        foreach (var uo in usableObjects)
        {
            if (uo == null) continue;
            int typeVal = (int)uo.objectInHandType;
            if (loggedTypes.Contains(typeVal)) continue;
            loggedTypes.Add(typeVal);
            LogSingleUsableObject(uo, typeVal);
        }
    }

    private static void LogSingleUsableObject(UsableObject uo, int typeVal)
    {
        try
        {
            CrashLog.Log($"[CarryDebug] UsableObject type={typeVal} ({uo.objectInHandType}) name='{uo.gameObject.name}' offsetPivotPos=({uo.offsetPivotPosition.x:F3},{uo.offsetPivotPosition.y:F3},{uo.offsetPivotPosition.z:F3}) offsetPivotRot=({uo.offsetPivotRotation.x:F1},{uo.offsetPivotRotation.y:F1},{uo.offsetPivotRotation.z:F1})");
        }
        catch (Exception ex)
        {
            CrashLog.Log($"[CarryDebug] Failed to read UsableObject type={typeVal}: {ex.Message}");
        }
    }

    /// <summary>Try to clone a real game prefab for the carried item type</summary>
    private static GameObject TryCreateFromGamePrefab(uint objectInHandType)
    {
        try
        {
            // Check cache first
            if (_carryPrefabCache.TryGetValue(objectInHandType, out var cachedTemplate))
                return CloneCachedCarryTemplate(objectInHandType, cachedTemplate);

            var shop = UnityEngine.Object.FindObjectOfType<ComputerShop>();
            if (shop == null || shop.shopItems == null)
            {
                CrashLog.Log("[EntityManager] ComputerShop not found, using fallback proxy");
                return null;
            }

            PlayerManager.ObjectInHand targetType = (PlayerManager.ObjectInHand)(int)objectInHandType;

            return FindAndCacheCarryPrefab(shop, objectInHandType, targetType);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("EntityManager.TryCreateFromGamePrefab", ex);
            _carryPrefabCache[objectInHandType] = null;
            return null;
        }
    }

    private static GameObject CloneCachedCarryTemplate(uint objectInHandType, GameObject cachedTemplate)
    {
        if (cachedTemplate != null)
        {
            var clone = UnityEngine.Object.Instantiate(cachedTemplate);
            clone.SetActive(true);
            clone.name = $"CarryVisual_{objectInHandType}";
            return clone;
        }
        return null;
    }

    private static GameObject FindAndCacheCarryPrefab(ComputerShop shop, uint objectInHandType, PlayerManager.ObjectInHand targetType)
    {
        foreach (var shopItem in shop.shopItems)
        {
            if (shopItem == null || shopItem.shopItemSO == null) continue;
            if (shopItem.shopItemSO.itemType != targetType) continue;

            int itemID = shopItem.shopItemSO.itemID;
            var prefab = shop.GetPrefabForItem(itemID, targetType);
            if (prefab == null) continue;

            var template = UnityEngine.Object.Instantiate(prefab);
            StripToVisualOnly(template);
            template.SetActive(false);
            template.name = $"CarryTemplate_{objectInHandType}";
            UnityEngine.Object.DontDestroyOnLoad(template);
            _carryPrefabCache[objectInHandType] = template;

            CrashLog.Log($"[EntityManager] Cached carry prefab for type {objectInHandType} (itemID={itemID})");

            var instance = UnityEngine.Object.Instantiate(template);
            instance.SetActive(true);
            instance.name = $"CarryVisual_{objectInHandType}";
            return instance;
        }

        // No matching shop item found, cache null to avoid retrying
        CrashLog.Log($"[EntityManager] No shop prefab found for type {objectInHandType}");
        _carryPrefabCache[objectInHandType] = null;
        return null;
    }

    /// <summary>Create a primitive fallback when real prefab isn't available</summary>
    private static GameObject CreateFallbackProxy(uint objectInHandType)
    {
        try
        {
            var proxy = new GameObject($"CarryFallback_{objectInHandType}");
            var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Remove collider
            var col = visual.GetComponent<Collider>();
            if (col != null) UnityEngine.Object.DestroyImmediate(col);

            visual.transform.SetParent(proxy.transform, false);

            GetFallbackProxyStyle(objectInHandType, out Vector3 scale, out Color color);

            visual.transform.localScale = scale;
            ApplyFallbackProxyMaterial(visual, color);

            return proxy;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("EntityManager.CreateFallbackProxy", ex);
            return null;
        }
    }

    private static void GetFallbackProxyStyle(uint objectInHandType, out Vector3 scale, out Color color)
    {
        if (TryGetServerProxyStyle(objectInHandType, out scale, out color))
            return;
        GetPropProxyStyle(objectInHandType, out scale, out color);
    }

    private static bool TryGetServerProxyStyle(uint objectInHandType, out Vector3 scale, out Color color)
    {
        switch (objectInHandType)
        {
            case 1: // Server1U
                scale = new Vector3(0.43f, 0.045f, 0.5f);
                color = new Color(0.2f, 0.2f, 0.25f);
                return true;
            case 2: // Server2U
                scale = new Vector3(0.43f, 0.09f, 0.5f);
                color = new Color(0.2f, 0.2f, 0.25f);
                return true;
            case 3: // Server3U
                scale = new Vector3(0.43f, 0.135f, 0.5f);
                color = new Color(0.2f, 0.2f, 0.25f);
                return true;
            default:
                scale = Vector3.zero;
                color = Color.white;
                return false;
        }
    }

    private static void GetPropProxyStyle(uint objectInHandType, out Vector3 scale, out Color color)
    {
        switch (objectInHandType)
        {
            case 4: // Switch
                scale = new Vector3(0.43f, 0.045f, 0.3f);
                color = new Color(0.15f, 0.3f, 0.15f);
                break;
            case 5: // Rack
                scale = new Vector3(0.6f, 1.2f, 0.8f);
                color = new Color(0.3f, 0.3f, 0.3f);
                break;
            case 6: // CableSpinner
                scale = new Vector3(0.15f, 0.15f, 0.15f);
                color = new Color(0.4f, 0.3f, 0.1f);
                break;
            case 7: // PatchPanel
                scale = new Vector3(0.43f, 0.045f, 0.3f);
                color = new Color(0.25f, 0.25f, 0.3f);
                break;
            case 8: // SFPModule
                scale = new Vector3(0.02f, 0.01f, 0.06f);
                color = new Color(0.6f, 0.6f, 0.6f);
                break;
            case 9: // SFPBox
                scale = new Vector3(0.1f, 0.06f, 0.08f);
                color = new Color(0.35f, 0.35f, 0.4f);
                break;
            default:
                scale = new Vector3(0.3f, 0.15f, 0.4f);
                color = new Color(0.25f, 0.25f, 0.3f);
                break;
        }
    }

    private static void ApplyFallbackProxyMaterial(GameObject visual, Color color)
    {
        var renderer = visual.GetComponent<Renderer>();
        if (renderer == null) return;
        try
        {
            var mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            renderer.material = mat;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
