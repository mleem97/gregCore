using System;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Tooltip system for GregCore UI Toolkit.
    /// Displays context info when hovering over elements.
    /// </summary>
    public static class GregTooltipManager
    {
        private static VisualElement? _tooltipRoot;
        private static Label? _tooltipLabel;
        private static bool _initialized;
        private static string? _currentText;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                var root = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Tooltip);
                if (root == null) return;

                _tooltipRoot = new VisualElement();
                _tooltipRoot.name = "TooltipRoot";
                _tooltipRoot.style.position = Position.Absolute;
                _tooltipRoot.style.backgroundColor = new Color(0.08f, 0.08f, 0.1f, 0.95f);
                _tooltipRoot.style.borderLeftWidth = 1;
                _tooltipRoot.style.borderRightWidth = 1;
                _tooltipRoot.style.borderTopWidth = 1;
                _tooltipRoot.style.borderBottomWidth = 1;
                _tooltipRoot.style.borderLeftColor = GregUITheme.NeutralBorder;
                _tooltipRoot.style.borderRightColor = GregUITheme.NeutralBorder;
                _tooltipRoot.style.borderTopColor = GregUITheme.NeutralBorder;
                _tooltipRoot.style.borderBottomColor = GregUITheme.NeutralBorder;
                _tooltipRoot.style.borderTopLeftRadius = 4;
                _tooltipRoot.style.borderTopRightRadius = 4;
                _tooltipRoot.style.borderBottomLeftRadius = 4;
                _tooltipRoot.style.borderBottomRightRadius = 4;
                _tooltipRoot.style.paddingLeft = 8;
                _tooltipRoot.style.paddingRight = 8;
                _tooltipRoot.style.paddingTop = 6;
                _tooltipRoot.style.paddingBottom = 6;
                _tooltipRoot.style.maxWidth = 300;
                _tooltipRoot.style.display = DisplayStyle.None;
                _tooltipRoot.pickingMode = PickingMode.Ignore;

                _tooltipLabel = new Label();
                _tooltipLabel.style.color = new Color(0.9f, 0.9f, 0.92f);
                _tooltipLabel.style.fontSize = 12;
                _tooltipLabel.style.whiteSpace = WhiteSpace.Normal;
                _tooltipRoot.Add(_tooltipLabel);

                root.Add(_tooltipRoot);

                MelonLogger.Msg("[GregTooltipManager] Initialized.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregTooltipManager] Init failed: {ex.Message}");
            }
        }

        public static void Show(string text, Vector2 screenPosition)
        {
            if (!_initialized) Initialize();
            if (_tooltipRoot == null || _tooltipLabel == null) return;

            if (_currentText == text && _tooltipRoot.style.display == DisplayStyle.Flex)
                return;

            _currentText = text;
            _tooltipLabel.text = text;
            _tooltipRoot.style.left = screenPosition.x + 15;
            _tooltipRoot.style.top = screenPosition.y + 15;
            _tooltipRoot.style.display = DisplayStyle.Flex;

            // Clamp to screen
            var root = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Tooltip);
            if (root != null)
            {
                float maxW = root.resolvedStyle.width;
                float maxH = root.resolvedStyle.height;
                if (_tooltipRoot.style.left.value.value + _tooltipRoot.resolvedStyle.width > maxW)
                {
                    _tooltipRoot.style.left = screenPosition.x - _tooltipRoot.resolvedStyle.width - 10;
                }
                if (_tooltipRoot.style.top.value.value + _tooltipRoot.resolvedStyle.height > maxH)
                {
                    _tooltipRoot.style.top = screenPosition.y - _tooltipRoot.resolvedStyle.height - 10;
                }
            }
        }

        public static void Hide()
        {
            if (_tooltipRoot != null)
            {
                _tooltipRoot.style.display = DisplayStyle.None;
                _currentText = null;
            }
        }

        /// <summary>
        /// Attach tooltip behavior to any VisualElement.
        /// </summary>
        public static void AttachTooltip(VisualElement element, string tooltipText)
        {
            if (element == null || string.IsNullOrEmpty(tooltipText)) return;

            element.RegisterCallback<MouseEnterEvent>(new Action<MouseEnterEvent>(_ =>
            {
                Show(tooltipText, Input.mousePosition);
            }));

            element.RegisterCallback<MouseLeaveEvent>(new Action<MouseLeaveEvent>(_ =>
            {
                Hide();
            }));

            element.RegisterCallback<MouseMoveEvent>(new Action<MouseMoveEvent>(evt =>
            {
                if (_tooltipRoot != null && _tooltipRoot.style.display == DisplayStyle.Flex)
                {
                    Show(tooltipText, evt.mousePosition);
                }
            }));
        }
    }
}
