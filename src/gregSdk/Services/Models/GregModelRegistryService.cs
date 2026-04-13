using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;

namespace greg.Sdk.Services.Models;

/// <summary>
/// Metadata container for a loaded player or NPC model.
/// </summary>
public sealed class ModelMetadata
{
    public string Name { get; }
    public string BundlePath { get; }
    public bool IsHumanoid { get; }
    public bool HasVoiceAttachment { get; }
    public DateTime LoadedAt { get; }

    public ModelMetadata(string name, string bundlePath, bool isHumanoid, bool hasVoiceAttachment, DateTime loadedAt)
    {
        Name = name;
        BundlePath = bundlePath;
        IsHumanoid = isHumanoid;
        HasVoiceAttachment = hasVoiceAttachment;
        LoadedAt = loadedAt;
    }
}

/// <summary>
/// Runtime container for a loaded model prefab and metadata.
/// </summary>
public sealed class LoadedPlayerModel
{
    public string ModelName { get; }
    public string BundlePath { get; }
    public UnityEngine.AssetBundle Bundle { get; }
    public UnityEngine.GameObject RootPrefab { get; }
    public ModelMetadata Metadata { get; }

    public LoadedPlayerModel(string modelName, string bundlePath, UnityEngine.AssetBundle bundle, UnityEngine.GameObject rootPrefab)
    {
        ModelName = modelName;
        BundlePath = bundlePath;
        Bundle = bundle;
        RootPrefab = rootPrefab;

        UnityEngine.Animator animator = rootPrefab.GetComponent<UnityEngine.Animator>();
        UnityEngine.Transform voice = rootPrefab.transform.Find("Voice");

        Metadata = new ModelMetadata(
            modelName,
            bundlePath,
            animator != null && animator.avatar != null && animator.avatar.isHuman,
            voice != null,
            DateTime.UtcNow);
    }
}

/// <summary>
/// Core SDK service for registering and accessing custom player/NPC models.
/// Extracted from gregMod.PlayerModels.
/// </summary>
public static class GregModelRegistryService
{
    private static readonly Dictionary<string, LoadedPlayerModel> Models = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object SyncRoot = new();

    public static event Action<string, ModelMetadata> OnModelRegistered;

    public static IEnumerable<string> RegisteredModels
    {
        get
        {
            lock (SyncRoot)
                return Models.Keys.ToArray();
        }
    }

    public static bool Register(LoadedPlayerModel model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.ModelName))
            return false;

        lock (SyncRoot)
        {
            Models[model.ModelName] = model;
        }

        MelonLogger.Msg($"[GregModelRegistryService] Registered model '{model.ModelName}' from '{model.BundlePath}'");
        OnModelRegistered?.Invoke(model.ModelName, model.Metadata);
        return true;
    }

    public static ModelMetadata GetMetadata(string modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName)) return null;
        lock (SyncRoot)
        {
            return Models.TryGetValue(modelName, out LoadedPlayerModel model) ? model.Metadata : null;
        }
    }

    public static LoadedPlayerModel GetLoadedModel(string modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName)) return null;
        lock (SyncRoot)
        {
            return Models.TryGetValue(modelName, out LoadedPlayerModel model) ? model : null;
        }
    }
}

