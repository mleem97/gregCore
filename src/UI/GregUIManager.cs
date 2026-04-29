using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Manages the root UI layer for gregCore.
    /// Primary: UI Toolkit (UIDocument).
    /// Fallback: UGUI Canvas if UIDocument is unavailable.
    /// </summary>
    public static class GregUIManager
    {
        private static UIDocument? _uiDocument;
        private static VisualElement? _root;
        private static GameObject? _uiRootObject;
        private static readonly Dictionary<string, VisualElement> _panels = new();

        // UGUI fallback
        private static Canvas? _uguiCanvas;
        private static GameObject? _uguiRoot;

        public static UIDocument? UIDocument => _uiDocument;
        public static VisualElement? Root => _root;
        public static bool IsInitialized => _uiRootObject != null;
        public static bool IsUsingUGUIFallback => _uguiCanvas != null;

        public static void Initialize()
        {
            if (IsInitialized) return;

            try
            {
                _uiRootObject = new GameObject("gregCore_UI_Root");
                UnityEngine.Object.DontDestroyOnLoad(_uiRootObject);

                // Attempt UI Toolkit primary path
                if (TryInitializeUIToolkit())
                {
                    MelonLogger.Msg("[gregCore] UI Toolkit root initialized.");
                }
                else
                {
                    InitializeUGUIFallback();
                    MelonLogger.Warning("[gregCore] UI Toolkit unavailable — fell back to UGUI Canvas.");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] UI Manager initialization failed: {ex.Message}");
            }
        }

        private static bool TryInitializeUIToolkit()
        {
            try
            {
                _uiDocument = _uiRootObject!.AddComponent<UIDocument>();
                _uiDocument.panelSettings = CreatePanelSettings();
                _root = _uiDocument.rootVisualElement;

                _root.style.flexGrow = 1;
                _root.style.position = Position.Absolute;
                _root.style.top = 0;
                _root.style.left = 0;
                _root.style.right = 0;
                _root.style.bottom = 0;
                _root.style.backgroundColor = Color.clear;

                UpdateInputState();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void InitializeUGUIFallback()
        {
            try
            {
                _uguiRoot = new GameObject("gregCore_UGUI_Fallback");
                UnityEngine.Object.DontDestroyOnLoad(_uguiRoot);
                _uguiRoot.transform.SetParent(_uiRootObject!.transform);

                _uguiCanvas = _uguiRoot.AddComponent<Canvas>();
                _uguiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _uguiCanvas.sortingOrder = 9999;

                var scaler = _uguiRoot.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                _uguiRoot.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] UGUI fallback initialization failed: {ex.Message}");
            }
        }

        private static PanelSettings CreatePanelSettings()
        {
            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            settings.referenceResolution = new Vector2Int(1920, 1080);
            settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            settings.match = 0.5f;
            return settings;
        }

        public static void RegisterPanel(string name, VisualElement panel)
        {
            if (_root == null) return;
            panel.style.display = DisplayStyle.None;
            _panels[name] = panel;
            _root.Add(panel);
            UpdateInputState();
        }

        public static void SetPanelActive(string name, bool active)
        {
            if (_panels.TryGetValue(name, out var panel) && panel != null)
            {
                panel.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
                UpdateInputState();
            }
        }

        public static void TogglePanel(string name)
        {
            if (_panels.TryGetValue(name, out var panel) && panel != null)
            {
                var isVisible = panel.style.display != DisplayStyle.None;
                panel.style.display = isVisible ? DisplayStyle.None : DisplayStyle.Flex;
                UpdateInputState();
            }
        }

        private static void UpdateInputState()
        {
            bool anyActive = false;
            foreach (var panel in _panels.Values)
            {
                if (panel != null && panel.style.display != DisplayStyle.None)
                {
                    anyActive = true;
                    break;
                }
            }

            if (_root != null)
            {
                _root.pickingMode = anyActive ? PickingMode.Position : PickingMode.Ignore;
            }
        }

        public static VisualElement CreateUIObject(string name)
        {
            return new VisualElement { name = name };
        }

        /// <summary>
        /// Creates a UGUI fallback GameObject under the fallback canvas.
        /// Use only when UI Toolkit is insufficient for a specific use-case.
        /// </summary>
        public static GameObject? CreateUGUIFallbackObject(string name)
        {
            if (_uguiRoot == null) return null;
            var go = new GameObject(name);
            go.transform.SetParent(_uguiRoot.transform, false);
            return go;
        }

        public static void Shutdown()
        {
            try
            {
                _panels.Clear();
                _root = null;
                _uiDocument = null;
                _uguiCanvas = null;

                if (_uiRootObject != null)
                {
                    UnityEngine.Object.Destroy(_uiRootObject);
                    _uiRootObject = null;
                }
                if (_uguiRoot != null)
                {
                    UnityEngine.Object.Destroy(_uguiRoot);
                    _uguiRoot = null;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] UI Manager shutdown failed: {ex.Message}");
            }
        }
    }
}
