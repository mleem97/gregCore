using System;
using System.Collections.Generic;
using System.IO;
using greg.Sdk.Definitions;
using greg.Sdk.Services;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace gregCoreSDK.Core.UI;

/// <summary>
/// Singleton manager for UI replacements. Handles persistence and global application of overrides.
/// </summary>
public class GregUiReplacementManager
{
    private static GregUiReplacementManager _instance;
    public static GregUiReplacementManager Instance => _instance ??= new GregUiReplacementManager();

    private readonly GregUiReplacementService _service = new GregUiReplacementService();
    private readonly string _configPath;

    public IGregUiReplacementService Service => _service;

    private GregUiReplacementManager()
    {
        _configPath = Path.Combine(MelonEnvironment.UserDataDirectory, "gregFramework", "ui_overrides.json");
        LoadFromConfig();
    }

    public void LoadFromConfig()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                // Create example file
                var example = new List<GregUiReplacementManifest>
                {
                    new GregUiReplacementManifest
                    {
                        UiPath = "Canvas/ExamplePanel",
                        Active = false,
                        Author = "ExampleAuthor",
                        Priority = 100
                    }
                };
                Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
                File.WriteAllText(_configPath, JsonConvert.SerializeObject(example, Formatting.Indented));
                return;
            }

            var manifests = JsonConvert.DeserializeObject<List<GregUiReplacementManifest>>(File.ReadAllText(_configPath));
            if (manifests != null)
            {
                foreach (var manifest in manifests)
                {
                    _service.RegisterOverride(manifest);
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore] Failed to load UI overrides from {_configPath}: {ex.Message}");
        }
    }

    public void SaveToConfig()
    {
        // This is a bit tricky since we don't have a direct list in the service, 
        // but we can add one or just rely on the service's internal dictionary.
        // For now, we only load, but we could add saving if users want to persist changes made via API/UI.
    }

    public void ApplyOverrides()
    {
        _service.ApplyAllOverrides();
    }

    public void TryApplyOverride(GameObject go)
    {
        _service.TryApplyOverride(go);
    }
}
