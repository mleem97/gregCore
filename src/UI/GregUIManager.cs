using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2CppInterop.Runtime.Attributes;
using System.Collections.Generic;

namespace gregCore.UI
{
    public static class GregUIManager
    {
        private static GameObject _rootObject;
        private static Canvas _canvas;
        private static CanvasScaler _scaler;
        private static GraphicRaycaster _raycaster;
        private static readonly Dictionary<string, GameObject> _panels = new();

        public static Canvas RootCanvas => _canvas;
        public static GameObject RootObject => _rootObject;

        public static void Initialize()
        {
            if (_rootObject != null) return;

            GenerateAssets();

            _rootObject = new GameObject("gregCore_UI_Root");
            UnityEngine.Object.DontDestroyOnLoad(_rootObject);

            _canvas = _rootObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 999;

            _scaler = _rootObject.AddComponent<CanvasScaler>();
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = new Vector2(1920, 1080);

            _raycaster = _rootObject.AddComponent<GraphicRaycaster>();

            EnsureEventSystem();
        }

        private static void GenerateAssets()
        {
            int size = 64;
            float radius = 24f; // Moderate roundedness
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var colors = new Color[size * size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dx = Mathf.Max(0, radius - x, x - (size - 1 - radius));
                    float dy = Mathf.Max(0, radius - y, y - (size - 1 - radius));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    
                    if (dist > radius) colors[y * size + x] = Color.clear;
                    else colors[y * size + x] = Color.white;
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            
            // Create 9-sliced sprite
            GregUITheme.RoundedSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        }

        public static void RegisterPanel(string name, GameObject panel) => _panels[name] = panel;

        public static void SetPanelActive(string name, bool active)
        {
            if (_panels.TryGetValue(name, out var panel) && panel != null)
                panel.SetActive(active);
        }

        public static void TogglePanel(string name)
        {
            if (_panels.TryGetValue(name, out var panel) && panel != null)
                panel.SetActive(!panel.activeSelf);
        }

        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
            {
                var eventSystemObj = new GameObject("gregCore_EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                UnityEngine.Object.DontDestroyOnLoad(eventSystemObj);
            }
        }

        public static GameObject CreateUIObject(string name, GameObject parent = null)
        {
            var obj = new GameObject(name);
            obj.layer = LayerMask.NameToLayer("UI");
            obj.transform.SetParent(parent?.transform ?? _rootObject.transform, false);
            return obj;
        }
    }
}
