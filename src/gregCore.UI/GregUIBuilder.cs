using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// UI Builder that delegates to GregPanelBuilder (UI Toolkit).
    /// Maintains a similar fluent API for backward compatibility.
    /// Legacy IMGUI rendering has been removed.
    /// </summary>
    public class GregUIBuilder
    {
        private readonly string _title;
        private GregPanelBuilder? _panelBuilder;
        private bool _isVisible;
        private float _width = 500f;
        private float _height = 600f;

        private GregUIBuilder(string title)
        {
            _title = title;
            _panelBuilder = GregPanelBuilder.Create(title);
        }

        public static GregUIBuilder Create(string title)
        {
            return new GregUIBuilder(title);
        }

        public static GregUIBuilder CreateTablet(string title) => Create(title);

        public static GregUIBuilder CreateWidget(string title, float x = 50, float y = 50)
        {
            var builder = new GregUIBuilder(title);
            builder._panelBuilder!
                .Build()
                .SetSize(320, 220)
                .SetPosition(x, y);
            return builder;
        }

        public GregUIBuilder SetSize(float width, float height)
        {
            _width = width;
            _height = height;
            EnsurePanelBuilt().SetSize(width, height);
            return this;
        }

        public void SetContentArea(Rect area)
        {
            EnsurePanelBuilt()
                .SetPosition(area.x, area.y)
                .SetSize(area.width, area.height);
        }

        public void ResetActions()
        {
            EnsurePanelBuilt().ClearContent();
        }

        public void Draw()
        {
            // No-op: UI Toolkit handles rendering automatically
        }

        public void DrawContent()
        {
            // No-op: UI Toolkit handles rendering automatically
        }

        public GregUIBuilder Build()
        {
            var panel = EnsurePanelBuilt().SetSize(_width, _height);
            GregUIManager.RegisterPanel(panel);
            return this;
        }

        public GregUIBuilder AddHeadline(string text)
        {
            EnsurePanelBuilt().AddHeadline(text);
            return this;
        }

        public GregUIBuilder AddLabel(string text)
        {
            EnsurePanelBuilt().AddLabel(text);
            return this;
        }

        public GregUIBuilder AddButton(string label, Action onClick)
        {
            EnsurePanelBuilt().AddButton(label, onClick);
            return this;
        }

        public GregUIBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            EnsurePanelBuilt().AddToggle(label, currentValue, onChanged);
            return this;
        }

        public GregUIBuilder AddSwitch(string label, bool currentValue, Action<bool> onChanged)
        {
            EnsurePanelBuilt().AddSwitch(label, currentValue, onChanged);
            return this;
        }

        public GregUIBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            EnsurePanelBuilt().AddSlider(label, min, max, currentValue, onChanged);
            return this;
        }

        public GregUIBuilder AddSpacer(float height = 20f)
        {
            EnsurePanelBuilt().AddSpacer(height);
            return this;
        }

        public GregUIBuilder AddPrimaryButton(string label, Action onClick) => AddButton(label, onClick);
        public GregUIBuilder AddSecondaryButton(string label, Action onClick) => AddButton(label, onClick);
        public GregUIBuilder AddSection(string title) => AddHeadline(title);

        public bool IsVisible
        {
            get => _panelBuilder?.IsVisible ?? _isVisible;
            set
            {
                _isVisible = value;
                if (_panelBuilder != null)
                {
                    if (value) _panelBuilder.Show();
                    else _panelBuilder.Hide();
                }
            }
        }

        public void Toggle()
        {
            EnsurePanelBuilt();
            _panelBuilder?.Toggle();
            _isVisible = _panelBuilder?.IsVisible ?? !_isVisible;
        }

        private GregPanelBuilder EnsurePanelBuilt()
        {
            _panelBuilder ??= GregPanelBuilder.Create(_title);
            _panelBuilder.Build();
            return _panelBuilder;
        }
    }
}
