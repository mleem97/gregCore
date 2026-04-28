using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI
{
    public static class GregUIManager
    {
        private static UIDocument? _uiDocument;
        private static VisualElement? _root;
        private static readonly Dictionary<string, VisualElement> _panels = new();

        public static UIDocument UIDocument => _uiDocument!;
        public static VisualElement Root => _root!;

        public static void Initialize()
        {
            if (_uiDocument != null) return;

            var go = new GameObject("gregCore_UI_Root");
            UnityEngine.Object.DontDestroyOnLoad(go);

            _uiDocument = go.AddComponent<UIDocument>();
            _uiDocument.panelSettings = CreatePanelSettings();
            _root = _uiDocument.rootVisualElement;

            // Default styles
            _root.style.flexGrow = 1;
            _root.style.position = Position.Absolute;
            _root.style.top = 0;
            _root.style.left = 0;
            _root.style.right = 0;
            _root.style.bottom = 0;
            _root.style.backgroundColor = Color.clear;

            UpdateInputState();
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

            // Update root picking mode for input blocking
            if (_root != null)
            {
                _root.pickingMode = anyActive ? PickingMode.Position : PickingMode.Ignore;
            }
        }

        public static VisualElement CreateUIObject(string name)
        {
            var element = new VisualElement
            {
                name = name
            };
            return element;
        }
    }
}
