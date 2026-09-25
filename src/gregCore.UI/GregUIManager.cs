using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Global UI Manager for GregCore UI Toolkit panels.
    /// Manages the rendering lifecycle, window states, and layer organization.
    /// Replaces the legacy IMGUI-based GregUIManager.
    /// </summary>
    public static class GregUIManager
    {
        private static readonly List<GregPanelBuilder> _activePanels = new();
        private static readonly Dictionary<string, GregPanelBuilder> _namedPanels = new();
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                GregCanvasManager.Instance.Initialize();
                GregUILayerManager.Instance.Initialize();
                MelonLogger.Msg("[GregUIManager] UI Toolkit manager initialized.");
                _initialized = true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregUIManager] Initialization failed: {ex.Message}");
            }
        }

        public static void RegisterPanel(GregPanelBuilder panel)
        {
            if (panel == null) return;
            if (!_activePanels.Contains(panel))
            {
                _activePanels.Add(panel);
            }
        }

        public static void UnregisterPanel(GregPanelBuilder panel)
        {
            if (panel == null) return;
            _activePanels.Remove(panel);
        }

        public static void RegisterNamedPanel(string name, GregPanelBuilder panel)
        {
            if (string.IsNullOrEmpty(name) || panel == null) return;
            _namedPanels[name] = panel;
        }

        public static GregPanelBuilder? GetNamedPanel(string name)
        {
            _namedPanels.TryGetValue(name, out var panel);
            return panel;
        }

        public static void ShowPanel(string name)
        {
            if (_namedPanels.TryGetValue(name, out var panel))
            {
                panel.Show();
            }
        }

        public static void HidePanel(string name)
        {
            if (_namedPanels.TryGetValue(name, out var panel))
            {
                panel.Hide();
            }
        }

        public static void TogglePanel(string name)
        {
            if (_namedPanels.TryGetValue(name, out var panel))
            {
                panel.Toggle();
            }
        }

        public static void HideAllPanels()
        {
            foreach (var panel in _activePanels)
            {
                panel?.Hide();
            }
            GregUILayerManager.Instance.HideAllPanels();
        }

        /// <summary>
        /// Create a quick notification toast.
        /// </summary>
        public static void ShowNotification(string message, float duration = 3f)
        {
            GregNotificationManager.Show(message, duration);
        }

        public static bool IsAnyPanelVisible
        {
            get
            {
                foreach (var panel in _activePanels)
                {
                    if (panel != null && panel.IsVisible) return true;
                }
                return false;
            }
        }

        // Legacy shims for backward compatibility
        public static object? Root => null;
        public static void SetPanelActive(string name, bool active)
        {
            if (active) ShowPanel(name);
            else HidePanel(name);
        }

        public static void RegisterPanel(string name, object panel)
        {
            if (panel is GregPanelBuilder builder)
                RegisterNamedPanel(name, builder);
        }

        public static void Shutdown()
        {
            try
            {
                foreach (var panel in _activePanels)
                {
                    panel?.Destroy();
                }
                _activePanels.Clear();
                _namedPanels.Clear();
                GregCanvasManager.Instance.Dispose();
                _initialized = false;
                MelonLogger.Msg("[GregUIManager] Shutdown complete.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregUIManager] Shutdown failed: {ex.Message}");
            }
        }
    }
}
