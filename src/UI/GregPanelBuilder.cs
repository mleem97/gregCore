using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// UI Toolkit-based panel builder.
    /// Replaces the legacy IMGUI GregUIBuilder with modern VisualElement hierarchies.
    /// Supports fluent API for constructing panels at runtime.
    /// </summary>
    public sealed class GregPanelBuilder
    {
        private readonly string _title;
        private VisualElement? _root;
        private VisualElement? _contentContainer;
        private ScrollView? _scrollView;
        private VisualElement? _header;
        private bool _isVisible;
        private bool _isBuilt;

        private GregPanelBuilder(string title)
        {
            _title = title;
        }

        public static GregPanelBuilder Create(string title)
        {
            return new GregPanelBuilder(title);
        }

        /// <summary>
        /// Build the panel structure. Call once before adding content.
        /// </summary>
        public GregPanelBuilder Build(GregUILayerType layer = GregUILayerType.Panel)
        {
            if (_isBuilt) return this;
            _isBuilt = true;

            try
            {
                var layerRoot = GregUILayerManager.Instance.GetLayerRoot(layer);
                if (layerRoot == null)
                {
                    MelonLogger.Error($"[GregPanelBuilder] Layer root for {layer} not available.");
                    return this;
                }

                _root = new VisualElement();
                _root.name = $"Panel_{_title}";
                _root.style.position = Position.Absolute;
                _root.style.left = new Length(50, LengthUnit.Percent);
                _root.style.top = new Length(50, LengthUnit.Percent);
                _root.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
                _root.style.width = 500;
                _root.style.height = 600;
                _root.style.backgroundColor = GregUITheme.PanelBackground;
                _root.style.borderTopLeftRadius = GregUITheme.CornerRadius;
                _root.style.borderTopRightRadius = GregUITheme.CornerRadius;
                _root.style.borderBottomLeftRadius = GregUITheme.CornerRadius;
                _root.style.borderBottomRightRadius = GregUITheme.CornerRadius;
                _root.style.borderLeftWidth = GregUITheme.BorderWidth;
                _root.style.borderRightWidth = GregUITheme.BorderWidth;
                _root.style.borderTopWidth = GregUITheme.BorderWidth;
                _root.style.borderBottomWidth = GregUITheme.BorderWidth;
                _root.style.borderLeftColor = GregUITheme.NeutralBorder;
                _root.style.borderRightColor = GregUITheme.NeutralBorder;
                _root.style.borderTopColor = GregUITheme.NeutralBorder;
                _root.style.borderBottomColor = GregUITheme.NeutralBorder;
                _root.pickingMode = PickingMode.Position;

                // Header
                _header = new VisualElement();
                _header.name = "PanelHeader";
                _header.style.height = GregUITheme.HeaderHeight;
                _header.style.backgroundColor = new Color(0, 0, 0, 0.2f);
                _header.style.borderBottomWidth = 3;
                _header.style.borderBottomColor = GregUITheme.PrimaryAccent;
                _header.style.flexDirection = FlexDirection.Row;
                _header.style.alignItems = Align.Center;
                _header.style.paddingLeft = GregUITheme.Padding;
                _header.style.paddingRight = GregUITheme.Padding;

                var titleLabel = new Label(_title.ToUpper());
                titleLabel.style.color = GregUITheme.SecondaryColor;
                titleLabel.style.fontSize = 20;
                titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                titleLabel.style.flexGrow = 1;
                _header.Add(titleLabel);

                // Close button
                var closeBtn = new Button();
                closeBtn.text = "X";
                closeBtn.style.width = 30;
                closeBtn.style.height = 30;
                closeBtn.style.backgroundColor = Color.clear;
                closeBtn.style.color = GregUITheme.SecondaryColor;
                closeBtn.style.borderLeftWidth = 0;
                closeBtn.style.borderRightWidth = 0;
                closeBtn.style.borderTopWidth = 0;
                closeBtn.style.borderBottomWidth = 0;
                closeBtn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ => Hide()));
                _header.Add(closeBtn);

                _root.Add(_header);

                // Scrollable content
                _scrollView = new ScrollView();
                _scrollView.name = "PanelContent";
                _scrollView.style.flexGrow = 1;
                _scrollView.style.paddingLeft = GregUITheme.Padding;
                _scrollView.style.paddingRight = GregUITheme.Padding;
                _scrollView.style.paddingTop = GregUITheme.Padding;
                _scrollView.style.paddingBottom = GregUITheme.Padding;
                _scrollView.verticalScrollerVisibility = ScrollerVisibility.Auto;
                _scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

                _contentContainer = _scrollView.contentContainer;
                _root.Add(_scrollView);

                layerRoot.Add(_root);
                _root.style.display = DisplayStyle.None;

                // Apply game font
                GregCanvasManager.Instance.ApplyGameFont(_root);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[GregPanelBuilder] Build failed: {ex.Message}");
            }

            return this;
        }

        public GregPanelBuilder SetSize(float width, float height)
        {
            if (_root != null)
            {
                _root.style.width = width;
                _root.style.height = height;
            }
            return this;
        }

        public GregPanelBuilder SetPosition(float x, float y)
        {
            if (_root != null)
            {
                _root.style.left = x;
                _root.style.top = y;
                _root.style.translate = new Translate(0, 0);
            }
            return this;
        }

        public GregPanelBuilder AddHeadline(string text)
        {
            if (_contentContainer == null) return this;

            var label = new Label(text.ToUpper());
            label.style.color = GregUITheme.SecondaryColor;
            label.style.fontSize = 18;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginTop = GregUITheme.Spacing;
            label.style.marginBottom = GregUITheme.Spacing / 2;
            _contentContainer.Add(label);
            return this;
        }

        public GregPanelBuilder AddLabel(string text)
        {
            if (_contentContainer == null) return this;

            var label = new Label(text);
            label.style.color = new Color(0.88f, 0.88f, 0.88f);
            label.style.fontSize = 14;
            label.style.marginBottom = GregUITheme.Spacing / 2;
            _contentContainer.Add(label);
            return this;
        }

        public GregPanelBuilder AddButton(string label, Action onClick)
        {
            if (_contentContainer == null) return this;

            var btn = new Button();
            btn.text = label;
            btn.style.height = 40;
            btn.style.marginBottom = GregUITheme.Spacing;
            GregUITheme.ApplyPrimaryButtonStyle(btn);
            btn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ => onClick?.Invoke()));
            _contentContainer.Add(btn);
            return this;
        }

        public GregPanelBuilder AddSecondaryButton(string label, Action onClick)
        {
            if (_contentContainer == null) return this;

            var btn = new Button();
            btn.text = label;
            btn.style.height = 40;
            btn.style.marginBottom = GregUITheme.Spacing;
            GregUITheme.ApplySecondaryButtonStyle(btn);
            btn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ => onClick?.Invoke()));
            _contentContainer.Add(btn);
            return this;
        }

        public GregPanelBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            if (_contentContainer == null) return this;

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginBottom = GregUITheme.Spacing;
            row.style.height = 30;

            var lbl = new Label(label);
            lbl.style.color = new Color(0.88f, 0.88f, 0.88f);
            lbl.style.fontSize = 14;
            lbl.style.flexGrow = 1;
            row.Add(lbl);

            var toggle = new Toggle();
            toggle.value = currentValue;
            toggle.RegisterCallback<ChangeEvent<bool>>(new Action<ChangeEvent<bool>>(evt =>
            {
                onChanged?.Invoke(evt.newValue);
            }));
            row.Add(toggle);

            _contentContainer.Add(row);
            return this;
        }

        public GregPanelBuilder AddSwitch(string label, bool currentValue, Action<bool> onChanged)
        {
            if (_contentContainer == null) return this;

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginBottom = GregUITheme.Spacing;
            row.style.height = 30;

            var lbl = new Label(label);
            lbl.style.color = new Color(0.88f, 0.88f, 0.88f);
            lbl.style.fontSize = 14;
            lbl.style.flexGrow = 1;
            row.Add(lbl);

            var toggle = new Toggle();
            toggle.value = currentValue;
            toggle.RegisterCallback<ChangeEvent<bool>>(new Action<ChangeEvent<bool>>(evt =>
            {
                onChanged?.Invoke(evt.newValue);
            }));
            row.Add(toggle);

            _contentContainer.Add(row);
            return this;
        }

        public GregPanelBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            if (_contentContainer == null) return this;

            var container = new VisualElement();
            container.style.marginBottom = GregUITheme.Spacing;

            var lbl = new Label($"{label}: {currentValue:F2}");
            lbl.style.color = new Color(0.88f, 0.88f, 0.88f);
            lbl.style.fontSize = 14;
            lbl.style.marginBottom = 4;
            container.Add(lbl);

            var slider = new Slider(min, max);
            slider.value = currentValue;
            slider.style.height = 20;
            slider.RegisterCallback<ChangeEvent<float>>(new Action<ChangeEvent<float>>(evt =>
            {
                lbl.text = $"{label}: {evt.newValue:F2}";
                onChanged?.Invoke(evt.newValue);
            }));
            container.Add(slider);

            _contentContainer.Add(container);
            return this;
        }

        public GregPanelBuilder AddDropdown(string label, List<string> options, int currentIndex, Action<int> onChanged)
        {
            if (_contentContainer == null || options == null || options.Count == 0) return this;

            var container = new VisualElement();
            container.style.marginBottom = GregUITheme.Spacing;

            var lbl = new Label(label);
            lbl.style.color = new Color(0.88f, 0.88f, 0.88f);
            lbl.style.fontSize = 14;
            lbl.style.marginBottom = 4;
            container.Add(lbl);

            var dropdown = new DropdownField();
            var il2cppList = new Il2CppSystem.Collections.Generic.List<string>();
            foreach (var opt in options)
                il2cppList.Add(opt);
            dropdown.choices = il2cppList;
            dropdown.index = Math.Clamp(currentIndex, 0, options.Count - 1);
            dropdown.RegisterCallback<ChangeEvent<string>>(new Action<ChangeEvent<string>>(evt =>
            {
                onChanged?.Invoke(dropdown.index);
            }));
            container.Add(dropdown);

            _contentContainer.Add(container);
            return this;
        }

        public GregPanelBuilder AddInputField(string label, string currentValue, Action<string> onChanged)
        {
            if (_contentContainer == null) return this;

            var container = new VisualElement();
            container.style.marginBottom = GregUITheme.Spacing;

            var lbl = new Label(label);
            lbl.style.color = new Color(0.88f, 0.88f, 0.88f);
            lbl.style.fontSize = 14;
            lbl.style.marginBottom = 4;
            container.Add(lbl);

            var textField = new TextField();
            textField.value = currentValue;
            textField.style.height = 30;
            textField.RegisterCallback<ChangeEvent<string>>(new Action<ChangeEvent<string>>(evt =>
            {
                onChanged?.Invoke(evt.newValue);
            }));
            container.Add(textField);

            _contentContainer.Add(container);
            return this;
        }

        public GregPanelBuilder AddSpacer(float height = 20f)
        {
            if (_contentContainer == null) return this;

            var spacer = new VisualElement();
            spacer.style.height = height;
            _contentContainer.Add(spacer);
            return this;
        }

        public GregPanelBuilder AddSeparator()
        {
            if (_contentContainer == null) return this;

            var sep = new VisualElement();
            sep.style.height = 1;
            sep.style.backgroundColor = GregUITheme.NeutralBorder;
            sep.style.marginTop = GregUITheme.Spacing;
            sep.style.marginBottom = GregUITheme.Spacing;
            sep.style.opacity = 0.3f;
            _contentContainer.Add(sep);
            return this;
        }

        public GregPanelBuilder ClearContent()
        {
            _contentContainer?.Clear();
            return this;
        }

        public void Show()
        {
            if (_root != null)
            {
                _root.style.display = DisplayStyle.Flex;
                _isVisible = true;
                OverlayManager.SetVisible(_title, true);
            }
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.style.display = DisplayStyle.None;
                _isVisible = false;
                OverlayManager.SetVisible(_title, false);
            }
        }

        public void Toggle()
        {
            if (_isVisible) Hide();
            else Show();
        }

        public bool IsVisible => _isVisible;

        public VisualElement? Root => _root;

        public VisualElement? ContentContainer => _contentContainer;

        public void Destroy()
        {
            if (_root != null)
            {
                _root.RemoveFromHierarchy();
                _root = null;
            }
            _isBuilt = false;
        }
    }
}
