using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Canvas render modes for the root UI layer.
    /// </summary>
    public enum GregCanvasMode
    {
        ScreenSpaceOverlay,
        ScreenSpaceCamera,
        WorldSpace
    }

    /// <summary>
    /// Central canvas manager for GregCore UI.
    /// Handles root PanelSettings, UIDocument lifecycle, and render mode switching.
    /// </summary>
    public sealed class GregCanvasManager : IDisposable
    {
        private static GregCanvasManager? _instance;
        public static GregCanvasManager Instance => _instance ??= new GregCanvasManager();

        private readonly Dictionary<string, UIDocument> _documents = new();
        private readonly Dictionary<string, GameObject> _roots = new();
        private GameObject? _canvasHost;
        private PanelSettings? _panelSettings;
        private bool _disposed;

        private GregCanvasManager() { }

        /// <summary>
        /// Initialize the canvas manager with default panel settings.
        /// </summary>
        public void Initialize()
        {
            if (_disposed) return;

            try
            {
                _canvasHost = new GameObject("GregCore_CanvasHost");
                UnityEngine.Object.DontDestroyOnLoad(_canvasHost);

                // Create shared PanelSettings for UI Toolkit
                _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                _panelSettings.name = "GregCore_PanelSettings";
                _panelSettings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("DefaultTheme");
                _panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                _panelSettings.referenceResolution = new Vector2Int(1920, 1080);
                _panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                _panelSettings.match = 0.5f;
                _panelSettings.sortingOrder = 1000;

                MelonLogger.Msg("[GregCanvasManager] Initialized with ScreenSpaceOverlay defaults.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregCanvasManager] Init failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Create or retrieve a named UI layer (HUD, Dialog, Overlay, etc.).
        /// </summary>
        public UIDocument? GetOrCreateLayer(string layerName, int sortingOrder)
        {
            if (_disposed) return null;
            if (_canvasHost == null) Initialize();

            if (_documents.TryGetValue(layerName, out var existing))
            {
                if (existing != null && existing.gameObject != null)
                    return existing;
                _documents.Remove(layerName);
            }

            try
            {
                var go = new GameObject($"GregUI_{layerName}");
                go.transform.SetParent(_canvasHost?.transform, false);
                UnityEngine.Object.DontDestroyOnLoad(go);

                var doc = go.AddComponent<UIDocument>();
                doc.panelSettings = _panelSettings;

                // Set sorting via panelSettings override if needed
                if (sortingOrder != 0)
                {
                    var ps = ScriptableObject.CreateInstance<PanelSettings>();
                    ps.name = $"GregCore_PS_{layerName}";
                    ps.themeUss = _panelSettings?.themeUss;
                    ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                    ps.referenceResolution = new Vector2Int(1920, 1080);
                    ps.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                    ps.match = 0.5f;
                    ps.sortingOrder = sortingOrder;
                    doc.panelSettings = ps;
                }

                _documents[layerName] = doc;
                _roots[layerName] = go;

                MelonLogger.Msg($"[GregCanvasManager] Layer '{layerName}' created (order={sortingOrder}).");
                return doc;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregCanvasManager] Failed to create layer '{layerName}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get the root VisualElement for a layer.
        /// </summary>
        public VisualElement? GetRoot(string layerName)
        {
            if (_documents.TryGetValue(layerName, out var doc))
            {
                return doc?.rootVisualElement;
            }
            return null;
        }

        /// <summary>
        /// Set layer visibility.
        /// </summary>
        public void SetLayerVisible(string layerName, bool visible)
        {
            if (_roots.TryGetValue(layerName, out var go))
            {
                if (go != null)
                    go.SetActive(visible);
            }
        }

        /// <summary>
        /// Destroy a layer and clean up.
        /// </summary>
        public void DestroyLayer(string layerName)
        {
            if (_roots.TryGetValue(layerName, out var go))
            {
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                }
                _roots.Remove(layerName);
            }
            _documents.Remove(layerName);
        }

        /// <summary>
        /// Apply the resolved game font to a VisualElement and its children.
        /// </summary>
        public void ApplyGameFont(VisualElement element)
        {
            if (element == null) return;
            GregFontLoader.ApplyFontTo(element);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var kvp in _roots)
            {
                if (kvp.Value != null)
                    UnityEngine.Object.Destroy(kvp.Value);
            }
            _roots.Clear();
            _documents.Clear();

            if (_canvasHost != null)
                UnityEngine.Object.Destroy(_canvasHost);

            if (_panelSettings != null)
                UnityEngine.Object.Destroy(_panelSettings);
        }
    }
}
