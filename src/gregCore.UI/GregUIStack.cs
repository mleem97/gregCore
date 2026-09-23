using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Base interface for all UI screens.
    /// </summary>
    public interface IGregUIScreen
    {
        string ScreenId { get; }
        string ScreenTitle { get; }
        bool IsVisible { get; }
        void Show();
        void Hide();
        void BuildUI(VisualElement container);
    }

    /// <summary>
    /// Screen stack manager for push/pop navigation.
    /// Supports modal dialogs, panels, and overlay screens.
    /// </summary>
    public sealed class GregUIStack
    {
        private static GregUIStack? _instance;
        public static GregUIStack Instance => _instance ??= new GregUIStack();

        private readonly Stack<IGregUIScreen> _stack = new();
        private readonly Dictionary<string, IGregUIScreen> _registry = new();
        private readonly List<VisualElement> _modalBlockers = new();
        private VisualElement? _blockerRoot;

        private GregUIStack() { }

        /// <summary>
        /// Register a screen for later use.
        /// </summary>
        public void Register(IGregUIScreen screen)
        {
            if (screen == null) return;
            _registry[screen.ScreenId] = screen;
        }

        public void Unregister(string screenId)
        {
            _registry.Remove(screenId);
        }

        /// <summary>
        /// Push a screen onto the stack and show it.
        /// </summary>
        public void Push(IGregUIScreen screen)
        {
            if (screen == null) return;

            // Hide current screen if any
            if (_stack.Count > 0)
            {
                var current = _stack.Peek();
                current.Hide();
            }

            _stack.Push(screen);
            ShowScreen(screen);
        }

        /// <summary>
        /// Push a registered screen by ID.
        /// </summary>
        public void Push(string screenId)
        {
            if (_registry.TryGetValue(screenId, out var screen))
            {
                Push(screen);
            }
            else
            {
                MelonLogger.Warning($"[GregUIStack] Screen '{screenId}' not registered.");
            }
        }

        /// <summary>
        /// Pop the current screen and show the previous one.
        /// </summary>
        public void Pop()
        {
            if (_stack.Count == 0) return;

            var current = _stack.Pop();
            current.Hide();

            if (_stack.Count > 0)
            {
                ShowScreen(_stack.Peek());
            }
            else
            {
                // No more screens - hide modal blocker
                HideModalBlocker();
            }
        }

        /// <summary>
        /// Pop all screens and clear the stack.
        /// </summary>
        public void PopAll()
        {
            while (_stack.Count > 0)
            {
                _stack.Pop().Hide();
            }
            HideModalBlocker();
        }

        /// <summary>
        /// Get the current top screen.
        /// </summary>
        public IGregUIScreen? Peek()
        {
            return _stack.Count > 0 ? _stack.Peek() : null;
        }

        public int Count => _stack.Count;

        private void ShowScreen(IGregUIScreen screen)
        {
            try
            {
                var root = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Panel);
                if (root == null) return;

                screen.Show();
                ShowModalBlocker();

                MelonLogger.Msg($"[GregUIStack] Showing '{screen.ScreenTitle}' ({screen.ScreenId}).");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregUIStack] ShowScreen failed: {ex.Message}");
            }
        }

        private void ShowModalBlocker()
        {
            if (_blockerRoot == null)
            {
                var root = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Overlay);
                if (root == null) return;

                _blockerRoot = new VisualElement();
                _blockerRoot.name = "ModalBlocker";
                _blockerRoot.style.position = Position.Absolute;
                _blockerRoot.style.left = 0;
                _blockerRoot.style.top = 0;
                _blockerRoot.style.right = 0;
                _blockerRoot.style.bottom = 0;
                _blockerRoot.style.backgroundColor = new Color(0, 0, 0, 0.5f);
                _blockerRoot.pickingMode = PickingMode.Position;
                _blockerRoot.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ => Pop()));
                root.Add(_blockerRoot);
            }
            else
            {
                _blockerRoot.style.display = DisplayStyle.Flex;
            }
        }

        private void HideModalBlocker()
        {
            if (_blockerRoot != null)
            {
                _blockerRoot.style.display = DisplayStyle.None;
            }
        }
    }
}
