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

        private GregUIBuilder(string title)
        {
            _title = title;
        }

        public static GregUIBuilder Create(string title)
        {
            return new GregUIBuilder(title);
        }

        public static GregUIBuilder CreateTablet(string title) => Create(title);

        public static GregUIBuilder CreateWidget(string title, float x = 50, float y = 50)
        {
            var builder = new GregUIBuilder(title);
            builder._panelBuilder = GregPanelBuilder.Create(title)
                .SetSize(320, 220)
                .SetPosition(x, y);
            return builder;
        }

        public GregUIBuilder SetSize(float width, float height)
        {
            _panelBuilder?.SetSize(width, height);
            return this;
        }

        public void SetContentArea(Rect area)
        {
            _panelBuilder?.SetPosition(area.x, area.y);
            _panelBuilder?.SetSize(area.width, area.height);
        }

        public void ResetActions()
        {
            _panelBuilder?.ClearContent();
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
            if (_panelBuilder == null)
            {
                _panelBuilder = GregPanelBuilder.Create(_title).SetSize(500, 600);
            }
            _panelBuilder.Build();
            GregUIManager.RegisterPanel(_panelBuilder);
            return this;
        }

        public GregUIBuilder AddHeadline(string text)
        {
            _panelBuilder?.AddHeadline(text);
            return this;
        }

        public GregUIBuilder AddLabel(string text)
        {
            _panelBuilder?.AddLabel(text);
            return this;
        }

        public GregUIBuilder AddButton(string label, Action onClick)
        {
            _panelBuilder?.AddButton(label, onClick);
            return this;
        }

        public GregUIBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            _panelBuilder?.AddToggle(label, currentValue, onChanged);
            return this;
        }

        public GregUIBuilder AddSwitch(string label, bool currentValue, Action<bool> onChanged)
        {
            _panelBuilder?.AddSwitch(label, currentValue, onChanged);
            return this;
        }

        public GregUIBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            _panelBuilder?.AddSlider(label, min, max, currentValue, onChanged);
            return this;
        }

        public GregUIBuilder AddSpacer(float height = 20f)
        {
            _panelBuilder?.AddSpacer(height);
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
            if (_panelBuilder == null) Build();
            _panelBuilder?.Toggle();
            _isVisible = _panelBuilder?.IsVisible ?? !_isVisible;
        }
    }
}
