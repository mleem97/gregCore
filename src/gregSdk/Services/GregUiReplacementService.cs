using System;
using System.Collections.Generic;
using greg.Sdk.Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace greg.Sdk.Services;

/// <summary>
/// Implementation of the UI replacement and override service.
/// </summary>
public class GregUiReplacementService : IGregUiReplacementService
{
    private readonly Dictionary<string, GregUiReplacementManifest> _overrides = new Dictionary<string, GregUiReplacementManifest>();

    public void RegisterOverride(GregUiReplacementManifest manifest)
    {
        if (manifest == null || string.IsNullOrEmpty(manifest.UiPath)) return;

        if (_overrides.TryGetValue(manifest.UiPath, out var existing))
        {
            if (manifest.Priority > existing.Priority)
            {
                _overrides[manifest.UiPath] = manifest;
            }
        }
        else
        {
            _overrides[manifest.UiPath] = manifest;
        }
    }

    public void UnregisterOverride(string uiPath)
    {
        if (string.IsNullOrEmpty(uiPath)) return;
        _overrides.Remove(uiPath);
    }

    public void ApplyAllOverrides()
    {
        // This is expensive, but necessary if we want to catch everything in the scene.
        // We'll iterate through all roots and apply.
        foreach (var root in UnityEngine.Object.FindObjectsOfType<GameObject>(true))
        {
            // Only check objects that are likely UI (have a RectTransform)
            if (root.GetComponent<RectTransform>() != null)
            {
                TryApplyOverride(root);
            }
        }
    }

    public void TryApplyOverride(GameObject gameObject)
    {
        if (gameObject == null) return;

        string path = GetGameObjectPath(gameObject);
        if (_overrides.TryGetValue(path, out var manifest))
        {
            ApplyManifestToGameObject(gameObject, manifest);
        }
    }

    public bool HasOverride(string uiPath, out GregUiReplacementManifest manifest)
    {
        return _overrides.TryGetValue(uiPath, out manifest);
    }

    private void ApplyManifestToGameObject(GameObject gameObject, GregUiReplacementManifest manifest)
    {
        try
        {
            if (manifest.Active.HasValue)
            {
                gameObject.SetActive(manifest.Active.Value);
            }

            var transform = gameObject.transform;

            if (manifest.LocalPosition.HasValue)
            {
                transform.localPosition = manifest.LocalPosition.Value;
            }

            if (manifest.LocalEulerAngles.HasValue)
            {
                transform.localEulerAngles = manifest.LocalEulerAngles.Value;
            }

            if (manifest.LocalScale.HasValue)
            {
                transform.localScale = manifest.LocalScale.Value;
            }

            if (manifest.Color.HasValue)
            {
                var img = gameObject.GetComponent<Image>();
                if (img != null) img.color = manifest.Color.Value;

                var text = gameObject.GetComponent<TextMeshProUGUI>();
                if (text != null) text.color = manifest.Color.Value;
            }

            if (!string.IsNullOrEmpty(manifest.Text))
            {
                var text = gameObject.GetComponent<TextMeshProUGUI>();
                if (text != null) text.text = manifest.Text;

                var legacyText = gameObject.GetComponent<Text>();
                if (legacyText != null) legacyText.text = manifest.Text;
            }
        }
        catch (Exception ex)
        {
            GregDiagnostics.LogContentError("UiOverride", manifest.UiPath, $"Failed to apply override from {manifest.Author}: {ex.Message}");
        }
    }

    private string GetGameObjectPath(GameObject gameObject)
    {
        string path = gameObject.name;
        Transform parent = gameObject.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}
