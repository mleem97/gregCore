using System;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

namespace greg.Plugin.PlayerModels;

/// <summary>
/// Loads custom player model bundles from supported runtime paths.
/// </summary>
public static class ModelLoader
{
    private static readonly string StreamingModelsRoot = Path.Combine(
        MelonEnvironment.GameRootDirectory,
        "DataCenter_Data",
        "StreamingAssets",
        "gregCore",
        "PlayerModels");

    private static readonly string LegacyStreamingModelsRoot = Path.Combine(
        MelonEnvironment.GameRootDirectory,
        "DataCenter_Data",
        "StreamingAssets",
        "gregCore",
        "PlayerModels");

    private static readonly string ModsModelsRoot = Path.Combine(
        MelonEnvironment.ModsDirectory,
        "gregCore",
        "PlayerModels");

    private static readonly string LegacyModsModelsRoot = Path.Combine(
        MelonEnvironment.ModsDirectory,
        "gregCore",
        "PlayerModels");

    /// <summary>
    /// Loads a model from StreamingAssets, with required validation.
    /// </summary>
    public static LoadedPlayerModel LoadFromStreamingAssets(string modelName)
    {
        string bundlePath = Path.Combine(StreamingModelsRoot, modelName + ".bundle");
        LoadedPlayerModel model = LoadFromPath(modelName, bundlePath, "StreamingAssets/gregCore");
        if (model != null)
            return model;

        string legacyBundlePath = Path.Combine(LegacyStreamingModelsRoot, modelName + ".bundle");
        return LoadFromPath(modelName, legacyBundlePath, "StreamingAssets/gregCore (legacy)");
    }

    /// <summary>
    /// Loads a model from the mod-local fallback path.
    /// </summary>
    public static LoadedPlayerModel LoadFromModsPath(string modelName)
    {
        string bundlePath = Path.Combine(ModsModelsRoot, modelName + ".bundle");
        LoadedPlayerModel model = LoadFromPath(modelName, bundlePath, "Mods/gregCore");
        if (model != null)
            return model;

        string legacyBundlePath = Path.Combine(LegacyModsModelsRoot, modelName + ".bundle");
        return LoadFromPath(modelName, legacyBundlePath, "Mods/gregCore (legacy)");
    }

    /// <summary>
    /// Loads a model using the configured fallback order: StreamingAssets first, then Mods path.
    /// </summary>
    public static LoadedPlayerModel LoadModel(string modelName)
    {
        LoadedPlayerModel streamingModel = LoadFromStreamingAssets(modelName);
        if (streamingModel != null)
            return streamingModel;

        return LoadFromModsPath(modelName);
    }

    private static LoadedPlayerModel LoadFromPath(string modelName, string bundlePath, string source)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            return null;

        if (!File.Exists(bundlePath))
        {
            MelonLogger.Warning($"greg.PlayerModels: {source} bundle missing '{bundlePath}'");
            return null;
        }

        try
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                MelonLogger.Error($"greg.PlayerModels: failed to load bundle '{bundlePath}'");
                return null;
            }

            GameObject rootPrefab = bundle.LoadAsset<GameObject>("PlayerModel_Root");
            if (rootPrefab == null)
            {
                MelonLogger.Error($"greg.PlayerModels: bundle '{bundlePath}' missing required prefab 'PlayerModel_Root'.");
                bundle.Unload(unloadAllLoadedObjects: false);
                return null;
            }

            Animator animator = rootPrefab.GetComponent<Animator>();
            if (animator == null || animator.avatar == null || !animator.avatar.isHuman)
            {
                MelonLogger.Error($"greg.PlayerModels: prefab 'PlayerModel_Root' in '{bundlePath}' needs a humanoid Animator avatar.");
                bundle.Unload(unloadAllLoadedObjects: false);
                return null;
            }

            var loaded = new LoadedPlayerModel(modelName, bundlePath, bundle, rootPrefab);
            GregModelRegistry.Register(loaded);
            return loaded;
        }
        catch (Exception exception)
        {
            MelonLogger.Error($"greg.PlayerModels: exception while loading '{bundlePath}': {exception.Message}");
            return null;
        }
    }
}
