using System;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Fluent builder for gregCore UI panels.
    /// Primary output: UI Toolkit VisualElements.
    /// Fallback output: UGUI GameObject hierarchy when UI Toolkit is insufficient.
    /// </summary>
    public class GregUIBuilder
    {
        private readonly VisualElement _root;
        private readonly VisualElement _content;
        private readonly string _panelName;
        private readonly bool _isTablet;

        private GregUIBuilder(string title, bool isTablet)
        {
            _panelName = $"Panel_{title}";
            _isTablet = isTablet;

            _root = new VisualElement
            {
                name = _panelName,
                style =
                {
                    flexDirection = FlexDirection.Column,
                    backgroundColor = GregUITheme.PanelBackground,
                    borderTopColor = GregUITheme.NeutralBorder,
                    borderBottomColor = GregUITheme.NeutralBorder,
                    borderLeftColor = GregUITheme.NeutralBorder,
                    borderRightColor = GregUITheme.NeutralBorder,
                    borderTopWidth = isTablet ? GregUITheme.BorderWidthTablet : GregUITheme.BorderWidthWidget,
                    borderBottomWidth = isTablet ? GregUITheme.BorderWidthTablet : GregUITheme.BorderWidthWidget,
                    borderLeftWidth = isTablet ? GregUITheme.BorderWidthTablet : GregUITheme.BorderWidthWidget,
                    borderRightWidth = isTablet ? GregUITheme.BorderWidthTablet : GregUITheme.BorderWidthWidget,
                    borderTopLeftRadius = GregUITheme.CornerRadius,
                    borderTopRightRadius = GregUITheme.CornerRadius,
                    borderBottomLeftRadius = GregUITheme.CornerRadius,
                    borderBottomRightRadius = GregUITheme.CornerRadius,
                    paddingTop = GregUITheme.Padding,
                    paddingBottom = GregUITheme.Padding,
                    paddingLeft = GregUITheme.Padding,
                    paddingRight = GregUITheme.Padding
                }
            };

            var header = new Label(title.ToUpper())
            {
                style =
                {
                    fontSize = 20,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = GregUITheme.SecondaryColor,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    paddingBottom = 8,
                    marginBottom = 8,
                    borderBottomColor = GregUITheme.NeutralBorder,
                    borderBottomWidth = 1
                }
            };
            _root.Add(header);

            _content = new VisualElement
            {
                name = "Content",
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    marginTop = GregUITheme.Spacing
                }
            };
            _root.Add(_content);
        }

        public static GregUIBuilder Create(string title) => CreateTablet(title);

        public static GregUIBuilder CreateTablet(string title)
        {
            var builder = new GregUIBuilder(title, true);
            builder._root.style.position = Position.Absolute;
            builder._root.style.top = 100;
            builder._root.style.left = 200;
            builder._root.style.width = 500;
            builder._root.style.height = 600;
            return builder;
        }

        public static GregUIBuilder CreateWidget(string title, float x = 50, float y = 50)
        {
            var builder = new GregUIBuilder(title, false);
            builder._root.style.position = Position.Absolute;
            builder._root.style.top = y;
            builder._root.style.left = x;
            builder._root.style.width = 300;
            builder._root.style.height = 200;
            return builder;
        }

        public GregUIBuilder SetSize(float width, float height)
        {
            _root.style.width = width;
            _root.style.height = height;
            return this;
        }

        public GregUIBuilder AddHeadline(string text)
        {
            var label = new Label(text.ToUpper())
            {
                style =
                {
                    fontSize = 16,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = GregUITheme.SecondaryColor,
                    marginTop = GregUITheme.Spacing,
                    marginBottom = 4
                }
            };
            _content.Add(label);
            return this;
        }

        public GregUIBuilder AddLabel(string text)
        {
            var label = new Label(text)
            {
                style =
                {
                    fontSize = 14,
                    color = new Color(0.88f, 0.88f, 0.88f),
                    marginBottom = 4
                }
            };
            _content.Add(label);
            return this;
        }

        public GregUIBuilder AddButton(string label, Action onClick) => AddPrimaryButton(label, onClick);

        public GregUIBuilder AddPrimaryButton(string label, Action onClick)
        {
            var button = new Button(onClick)
            {
                text = label.ToUpper(),
                style =
                {
                    backgroundColor = GregUITheme.PrimaryAccent,
                    color = Color.black,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 40,
                    marginTop = GregUITheme.Spacing,
                    borderTopLeftRadius = GregUITheme.CornerRadius,
                    borderTopRightRadius = GregUITheme.CornerRadius,
                    borderBottomLeftRadius = GregUITheme.CornerRadius,
                    borderBottomRightRadius = GregUITheme.CornerRadius,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };
            _content.Add(button);
            return this;
        }

        public GregUIBuilder AddSecondaryButton(string label, Action onClick)
        {
            var button = new Button(onClick)
            {
                text = label.ToUpper(),
                style =
                {
                    backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.5f),
                    color = GregUITheme.NeutralBorder,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    height = 40,
                    marginTop = GregUITheme.Spacing,
                    borderTopLeftRadius = GregUITheme.CornerRadius,
                    borderTopRightRadius = GregUITheme.CornerRadius,
                    borderBottomLeftRadius = GregUITheme.CornerRadius,
                    borderBottomRightRadius = GregUITheme.CornerRadius,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };
            _content.Add(button);
            return this;
        }

        public GregUIBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginTop = GregUITheme.Spacing
                }
            };

            var toggle = new Toggle
            {
                value = currentValue,
                style = { flexGrow = 0 }
            };
            toggle.RegisterValueChangedCallback<bool>(evt => onChanged?.Invoke(evt.newValue));

            var labelElement = new Label(label)
            {
                style =
                {
                    fontSize = 14,
                    color = new Color(0.88f, 0.88f, 0.88f),
                    marginLeft = 8,
                    flexGrow = 1
                }
            };

            container.Add(toggle);
            container.Add(labelElement);
            _content.Add(container);
            return this;
        }

        public GregUIBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            var container = new VisualElement { style = { marginTop = GregUITheme.Spacing } };

            var labelElement = new Label(label)
            {
                style =
                {
                    fontSize = 14,
                    color = new Color(0.88f, 0.88f, 0.88f),
                    marginBottom = 4
                }
            };

            var slider = new Slider(min, max)
            {
                value = currentValue,
                style = { flexGrow = 1 }
            };
            slider.RegisterValueChangedCallback<float>(evt => onChanged?.Invoke(evt.newValue));

            container.Add(labelElement);
            container.Add(slider);
            _content.Add(container);
            return this;
        }

        public GregUIBuilder AddInputField(string label, string defaultValue, Action<string> onChanged)
        {
            var container = new VisualElement { style = { marginTop = GregUITheme.Spacing } };

            var labelElement = new Label(label)
            {
                style =
                {
                    fontSize = 14,
                    color = new Color(0.88f, 0.88f, 0.88f),
                    marginBottom = 4
                }
            };

            var textField = new TextField
            {
                value = defaultValue,
                style =
                {
                    backgroundColor = new Color(0.1f, 0.1f, 0.1f),
                    color = Color.white,
                    height = 30
                }
            };
            textField.RegisterValueChangedCallback<string>(evt => onChanged?.Invoke(evt.newValue));

            container.Add(labelElement);
            container.Add(textField);
            _content.Add(container);
            return this;
        }

        public GregUIBuilder AddSection(string title) => AddHeadline(title);

        public GregUIBuilder AddSpacer(float height = 20f)
        {
            var spacer = new VisualElement { style = { height = height } };
            _content.Add(spacer);
            return this;
        }

        /// <summary>
        /// Builds the panel into the UI Toolkit root. This is the primary path.
        /// </summary>
        public VisualElement Build()
        {
            GregUIManager.RegisterPanel(_panelName, _root);
            return _root;
        }

        /// <summary>
        /// Fallback: builds the panel as a UGUI GameObject hierarchy.
        /// Use only when UI Toolkit is unavailable or insufficient.
        /// </summary>
        public GameObject? BuildAsUGUI()
        {
            try
            {
                var go = GregUIManager.CreateUGUIFallbackObject(_panelName);
                if (go == null) return null;

                var rect = go.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = new Vector2(_root.style.left.value.value, -_root.style.top.value.value);
                rect.sizeDelta = new Vector2(_root.style.width.value.value, _root.style.height.value.value);

                var image = go.AddComponent<UnityEngine.UI.Image>();
                image.color = GregUITheme.PanelBackground;

                MelonLogger.Warning($"[gregCore] Panel '{_panelName}' built as UGUI fallback.");
                return go;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] UGUI fallback build failed: {ex.Message}");
                return null;
            }
        }

        public GregUIBuilder Show() { _root.style.display = DisplayStyle.Flex; return this; }
        public GregUIBuilder Hide() { _root.style.display = DisplayStyle.None; return this; }
        public GregUIBuilder Toggle() { _root.style.display = _root.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None; return this; }
        public bool IsVisible => _root.style.display != DisplayStyle.None;
    }
}
