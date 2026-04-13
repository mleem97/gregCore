using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace greg.Sdk.Services.Models;

/// <summary>
/// Core SDK service for swapping player and NPC models at runtime.
/// Extracted from gregMod.PlayerModels.
/// </summary>
public static class GregModelSwapperService
{
    private static readonly Dictionary<string, string> AssignedModelsByPlayer = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> PersistentNpcModels = new(StringComparer.OrdinalIgnoreCase);

    // --- PLAYERS ---

    public static void AssignModelToPlayer(string playerId, string modelName)
    {
        if (string.IsNullOrWhiteSpace(playerId) || string.IsNullOrWhiteSpace(modelName))
            return;
        AssignedModelsByPlayer[playerId] = modelName;
    }

    public static void ReapplyPlayerSceneAssignments()
    {
        if (AssignedModelsByPlayer.Count == 0) return;
        foreach (var pair in AssignedModelsByPlayer)
            RefreshPlayerModel(pair.Key);
    }

    public static void RefreshPlayerModel(string playerId)
    {
        if (string.IsNullOrWhiteSpace(playerId)) return;
        if (!AssignedModelsByPlayer.TryGetValue(playerId, out string modelName)) return;

        LoadedPlayerModel loadedModel = GregModelRegistryService.GetLoadedModel(modelName);
        if (loadedModel == null)
        {
            MelonLogger.Warning($"[GregModelSwapperService] Assigned model '{modelName}' for player '{playerId}' not loaded.");
            return;
        }

        GameObject target = FindGameObjectObject(playerId);
        if (target == null) return;

        ApplyModel(target, loadedModel, true);
    }

    // --- NPCS ---

    public static void ReplaceNPCModel(string npcId, string modelName, bool persistent = true)
    {
        if (string.IsNullOrWhiteSpace(npcId) || string.IsNullOrWhiteSpace(modelName))
            return;
        if (persistent)
            PersistentNpcModels[npcId] = modelName;
        ApplyReplacementNPC(npcId, modelName);
    }

    public static void RevertNPC(string npcId)
    {
        if (!string.IsNullOrWhiteSpace(npcId))
            PersistentNpcModels.Remove(npcId);
    }

    public static void ReapplyPersistentNPCReplacements()
    {
        foreach (var pair in PersistentNpcModels)
            ApplyReplacementNPC(pair.Key, pair.Value);
    }

    private static void ApplyReplacementNPC(string npcId, string modelName)
    {
        LoadedPlayerModel model = GregModelRegistryService.GetLoadedModel(modelName);
        if (model == null) return;

        GameObject npc = FindGameObjectObject(npcId);
        if (npc == null) return;

        ApplyModel(npc, model, false);
    }

    // --- SHARED ---

    private static GameObject FindGameObjectObject(string identifier)
    {
        var candidates = GameObject.FindObjectsOfType<GameObject>();
        for (int i = 0; i < candidates.Length; i++)
        {
            var c = candidates[i];
            if (c == null) continue;
            if (string.Equals(c.name, identifier, StringComparison.OrdinalIgnoreCase)
                || c.name.IndexOf(identifier, StringComparison.OrdinalIgnoreCase) >= 0)
                return c;
        }
        return null;
    }

    private static void ApplyModel(GameObject target, LoadedPlayerModel model, bool isPlayer)
    {
        if (target == null || model?.RootPrefab == null) return;

        SkinnedMeshRenderer sourceRenderer = model.RootPrefab.GetComponentInChildren<SkinnedMeshRenderer>(true);
        SkinnedMeshRenderer targetRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>(true);

        if (sourceRenderer == null || targetRenderer == null)
        {
            MelonLogger.Warning($"[GregModelSwapperService] Missing renderer applying '{model.ModelName}' to '{target.name}'.");
            return;
        }

        targetRenderer.sharedMesh = sourceRenderer.sharedMesh;
        targetRenderer.sharedMaterials = sourceRenderer.sharedMaterials;

        if (isPlayer)
        {
            Transform[] targetBones = targetRenderer.bones;
            Transform[] sourceBones = sourceRenderer.bones;
            if (sourceBones != null && sourceBones.Length > 0 && targetBones != null && targetBones.Length == sourceBones.Length)
                targetRenderer.bones = targetBones;

            Animator targetAnimator = target.GetComponentInChildren<Animator>(true);
            if (targetAnimator != null)
                targetRenderer.rootBone = targetAnimator.transform;
        }
        else
        {
            Animator npcAnimator = target.GetComponentInChildren<Animator>(true);
            Animator modelAnimator = model.RootPrefab.GetComponentInChildren<Animator>(true);
            if (npcAnimator != null && modelAnimator != null && modelAnimator.avatar != null && !modelAnimator.avatar.isHuman)
                MelonLogger.Warning($"[GregModelSwapperService] NPC '{target.name}' model '{model.ModelName}' has non-humanoid avatar.");
        }

        MelonLogger.Msg($"[GregModelSwapperService] Applied '{model.ModelName}' to {(isPlayer ? "player" : "NPC")} '{target.name}'.");
    }
}
