using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;

namespace gregCore.UI
{
    /// <summary>
    /// Central manager for the gregCore UGUI framework.
    /// Handles Canvas creation and EventSystem orchestration.
    /// </summary>
    public static class GregUIManager
    {
        private static GameObject _rootObject;
        private static Canvas _canvas;
        private static CanvasScaler _scaler;
        private static GraphicRaycaster _raycaster;
        private static readonly System.Collections.Generic.Dictionary<string, GameObject> _panels = new();

        public static Canvas RootCanvas => _canvas;
        public static GameObject RootObject => _rootObject;

        public static void Initialize()
        {
            if (_rootObject != null) return;

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

        public static void RegisterPanel(string name, GameObject panel)
        {
            _panels[name] = panel;
        }

        public static void SetPanelActive(string name, bool active)
        {
            if (_panels.TryGetValue(name, out var panel) && panel != null)
            {
                panel.SetActive(active);
            }
        }

        public static void TogglePanel(string name)
        {
            if (_panels.TryGetValue(name, out var panel) && panel != null)
            {
                panel.SetActive(!panel.activeSelf);
            }
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
