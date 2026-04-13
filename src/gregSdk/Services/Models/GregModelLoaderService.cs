using System;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

namespace greg.Sdk.Services.Models;

/// <summary>
/// Core SDK service for loading custom player models from asset bundles.
/// Extracted from gregMod.PlayerModels.
/// </summary>
public static class GregModelLoaderService
{
    private static readonly string StreamingModelsRoot = Path.Combine(
        MelonEnvironment.GameRootDirectory,
        "DataCenter_Data",
        "StreamingAssets",
        "gregMod",
        "PlayerModels");

    private static readonly string ModsModelsRoot = Path.Combine(
        MelonEnvironment.ModsDirectory,
        "gregMod",
        "PlayerModels");

    private static LoadedPlayerModel LoadFromStreamingAssets(string modelName)
    {
        string bundlePath = Path.Combine(StreamingModelsRoot, modelName + ".bundle");
        return LoadFromPath(modelName, bundlePath, "StreamingAssets");
    }

    private static LoadedPlayerModel LoadFromModsPath(string modelName)
    {
        string bundlePath = Path.Combine(ModsModelsRoot, modelName + ".bundle");
        return LoadFromPath(modelName, bundlePath, "ModsPath");
    }

    public static LoadedPlayerModel LoadModel(string modelName)
    {
        LoadedPlayerModel model = LoadFromStreamingAssets(modelName);
        return model ?? LoadFromModsPath(modelName);
    }

    private static LoadedPlayerModel LoadFromPath(string modelName, string bundlePath, string source)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            return null;

        if (!File.Exists(bundlePath))
        {
            MelonLogger.Warning($"[GregModelLoaderService] {source} bundle missing: '{bundlePath}'");
            return null;
        }

        try
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                MelonLogger.Error($"[GregModelLoaderService] Failed to load bundle: '{bundlePath}'");
                return null;
            }

            GameObject rootPrefab = bundle.LoadAsset<GameObject>("PlayerModel_Root");
            if (rootPrefab == null)
            {
                MelonLogger.Error($"[GregModelLoaderService] Bundle '{bundlePath}' missing required prefab 'PlayerModel_Root'.");
                bundle.Unload(unloadAllLoadedObjects: false);
                return null;
            }

            Animator animator = rootPrefab.GetComponent<Animator>();
            if (animator == null || animator.avatar == null || !animator.avatar.isHuman)
            {
                MelonLogger.Error($"[GregModelLoaderService] Prefab in '{bundlePath}' needs a humanoid Animator avatar.");
                bundle.Unload(unloadAllLoadedObjects: false);
                return null;
            }

            var loaded = new LoadedPlayerModel(modelName, bundlePath, bundle, rootPrefab);
            GregModelRegistryService.Register(loaded);
            return loaded;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregModelLoaderService] Exception while loading '{bundlePath}': {ex.Message}");
            return null;
        }
    }
}

