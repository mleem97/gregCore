using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace greg.Core.UI;

/// <summary>
/// Central registry for all gregUI elements.
/// Lifecycle: Init → Register → OnSceneLoad → Teardown
/// </summary>
public static class GregUIManager
{
    private static readonly Dictionary<string, IGregUIElement> _registry = new();
    private static Canvas _rootCanvas;

    public static bool IsReady => _rootCanvas != null;

    public static void Init()
    {
        // Hook into scene load to find root canvas
        // The game uses StaticUIElements which has a canvasStatic.
        // We'll also look for any canvas if that fails.
    }

    public static void OnSceneLoaded(string sceneName)
    {
        // Search for the root canvas. 
        // Based on analysis, "canvasStatic" is a strong candidate, or just find any root canvas.
        _rootCanvas = GameObject.FindObjectOfType<Canvas>();
        
        if (_rootCanvas == null)
        {
            MelonLogger.Warning($"[GregUIManager] Root canvas not found in scene: {sceneName}");
            return;
        }

        MelonLogger.Msg($"[GregUIManager] Root canvas found: {_rootCanvas.name}");

        // Re-attach all registered panels
        foreach (var element in _registry.Values)
        {
            try 
            {
                element.Attach(_rootCanvas);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregUIManager] Failed to attach element {element.PanelId}: {ex.Message}");
            }
        }
    }

    public static void Register(string panelId, IGregUIElement element)
    {
        _registry[panelId] = element;
        if (_rootCanvas != null)
        {
            element.Attach(_rootCanvas);
        }
    }

    public static T Get<T>(string panelId) where T : class, IGregUIElement
    {
        return _registry.TryGetValue(panelId, out var el) ? el as T : null;
    }

    public static void Teardown()
    {
        foreach (var el in _registry.Values)
        {
            try { el.Destroy(); } catch { }
        }
        _registry.Clear();
        _rootCanvas = null;
    }
}
