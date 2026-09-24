using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// UI layer types for sorting and organization.
    /// </summary>
    public enum GregUILayerType
    {
        Background = -1000,
        HUD = 0,
        Panel = 1000,
        Dialog = 2000,
        Overlay = 3000,
        Tooltip = 4000,
        Notification = 5000
    }

    /// <summary>
    /// Manages UI layers with proper sorting and visibility.
    /// Each layer gets its own UIDocument for isolation.
    /// </summary>
    public sealed class GregUILayerManager : IDisposable
    {
        private static GregUILayerManager? _instance;
        public static GregUILayerManager Instance => _instance ??= new GregUILayerManager();

        private readonly Dictionary<GregUILayerType, string> _layerNames = new()
        {
            [GregUILayerType.Background] = "Background",
            [GregUILayerType.HUD] = "HUD",
            [GregUILayerType.Panel] = "Panel",
            [GregUILayerType.Dialog] = "Dialog",
            [GregUILayerType.Overlay] = "Overlay",
            [GregUILayerType.Tooltip] = "Tooltip",
            [GregUILayerType.Notification] = "Notification"
        };

        private readonly Dictionary<string, VisualElement> _layerRoots = new();
        private readonly HashSet<string> _visibleLayers = new();
        private bool _initialized;
        private bool _disposed;

        private GregUILayerManager() { }

        public void Initialize()
        {
            if (_initialized || _disposed) return;
            _initialized = true;

            try
            {
                GregCanvasManager.Instance.Initialize();

                foreach (var kvp in _layerNames)
                {
                    var doc = GregCanvasManager.Instance.GetOrCreateLayer(kvp.Value, (int)kvp.Key);
                    if (doc?.rootVisualElement != null)
                    {
                        _layerRoots[kvp.Value] = doc.rootVisualElement;
                        doc.rootVisualElement.pickingMode = PickingMode.Ignore;
                    }
                }

                // HUD is visible by default
                SetLayerVisible(GregUILayerType.HUD, true);

                MelonLogger.Msg("[GregUILayerManager] All layers initialized.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregUILayerManager] Init failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Get the root VisualElement for a specific layer type.
        /// </summary>
        public VisualElement? GetLayerRoot(GregUILayerType layer)
        {
            if (_layerNames.TryGetValue(layer, out var name))
            {
                _layerRoots.TryGetValue(name, out var root);
                return root;
            }
            return null;
        }

        /// <summary>
        /// Toggle layer visibility.
        /// </summary>
        public void SetLayerVisible(GregUILayerType layer, bool visible)
        {
            if (!_layerNames.TryGetValue(layer, out var name)) return;

            GregCanvasManager.Instance.SetLayerVisible(name, visible);

            if (visible)
                _visibleLayers.Add(name);
            else
                _visibleLayers.Remove(name);
        }

        public bool IsLayerVisible(GregUILayerType layer)
        {
            return _layerNames.TryGetValue(layer, out var name) && _visibleLayers.Contains(name);
        }

        /// <summary>
        /// Hide all layers except HUD.
        /// </summary>
        public void HideAllPanels()
        {
            foreach (var kvp in _layerNames)
            {
                if (kvp.Key != GregUILayerType.HUD && kvp.Key != GregUILayerType.Background)
                {
                    SetLayerVisible(kvp.Key, false);
                }
            }
        }

        /// <summary>
        /// Add a VisualElement to a specific layer.
        /// </summary>
        public void AddToLayer(GregUILayerType layer, VisualElement element)
        {
            var root = GetLayerRoot(layer);
            if (root != null && element != null)
            {
                root.Add(element);
                root.pickingMode = PickingMode.Position;
            }
        }

        /// <summary>
        /// Remove a VisualElement from its parent layer.
        /// </summary>
        public void RemoveFromLayer(VisualElement element)
        {
            element?.RemoveFromHierarchy();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _layerRoots.Clear();
            _visibleLayers.Clear();
        }
    }
}
