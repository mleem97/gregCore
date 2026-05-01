using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Global UI Manager for native GregCore IMGUI windows.
    /// Manages the rendering lifecycle and window states.
    /// </summary>
    public static class GregUIManager
    {
        private static readonly List<GregUIBuilder> _activeWindows = new();
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;

            GregImGui.EnsureInitialized();

            var go = new GameObject("GregCore_UI_Host");
            UnityEngine.Object.DontDestroyOnLoad(go);
            
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<GregUiHostBehaviour>();
            go.AddComponent<GregUiHostBehaviour>();

            _initialized = true;
            MelonLogger.Msg("[GregUIManager] Native IMGUI Host initialized.");
        }

        public static void RegisterWindow(GregUIBuilder window)
        {
            if (!_activeWindows.Contains(window))
            {
                _activeWindows.Add(window);
            }
        }

        public static void UnregisterWindow(GregUIBuilder window)
        {
            _activeWindows.Remove(window);
        }

        internal static void DrawAll()
        {
            for (int i = 0; i < _activeWindows.Count; i++)
            {
                var win = _activeWindows[i];
                if (win != null && win.IsVisible)
                {
                    win.Draw();
                }
            }
        }

        // Shims for legacy modules
        public static object? Root => null;
        public static void TogglePanel(string name) { }
        public static void SetPanelActive(string name, bool active) { }
        public static void RegisterPanel(string name, object panel) { }
        public static void Shutdown() { }
    }

    /// <summary>
    /// Persistent MonoBehaviour that receives the OnGUI callback from Unity.
    /// </summary>
    internal class GregUiHostBehaviour : MonoBehaviour
    {
        public GregUiHostBehaviour(IntPtr ptr) : base(ptr) { }

        private void OnGUI()
        {
            GregUIManager.DrawAll();
        }
    }
}
