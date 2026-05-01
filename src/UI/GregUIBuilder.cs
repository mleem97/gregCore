using System;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// Procedural IMGUI Builder. 
    /// Replaces the Hybrid UI Toolkit system with a robust OnGUI-based approach.
    /// Supports nested drawing for tabbed layouts and modern switches.
    /// </summary>
    public class GregUIBuilder
    {
        private readonly string _title;
        private Rect _rect;
        private Rect _contentArea;
        private readonly List<Action<Rect>> _drawActions = new();
        private float _currentY = 10;
        private const float PADDING_SIDE = 20;
        private const float SPACING = 12;
        private bool _isVisible;
        private Vector2 _scrollPos;

        private GregUIBuilder(string title, Rect rect)
        {
            _title = title;
            _rect = rect;
            _contentArea = rect;
        }

        public static GregUIBuilder Create(string title)
        {
            return new GregUIBuilder(title, new Rect(Screen.width / 2 - 250, Screen.height / 2 - 300, 500, 600));
        }

        public static GregUIBuilder CreateTablet(string title) => Create(title);

        public static GregUIBuilder CreateWidget(string title, float x = 50, float y = 50)
        {
            return new GregUIBuilder(title, new Rect(x, y, 320, 220));
        }

        public GregUIBuilder SetSize(float width, float height)
        {
            _rect.width = width;
            _rect.height = height;
            _contentArea.width = width;
            _contentArea.height = height;
            return this;
        }

        public void SetContentArea(Rect area)
        {
            _contentArea = area;
        }

        public void ResetActions()
        {
            _drawActions.Clear();
            _currentY = 10;
        }

        public void Draw()
        {
            if (!_isVisible) return;
            GregImGui.EnsureInitialized();
            GregImGui.DrawWindowFrame(_rect, _title);
            DrawContent();
        }

        public void DrawContent()
        {
            GregImGui.EnsureInitialized();

            // Custom skin for styled scrollbars
            var prevSkin = GUI.skin;
            GUI.skin = GregImGui.GetCleanSkin();

            // Scrolling container for content
            var viewRect = new Rect(0, 0, _contentArea.width - 25, _currentY + 50);
            _scrollPos = GUI.BeginScrollView(_contentArea, _scrollPos, viewRect, false, true);

            // Positioning relative to scroll view
            // Using slightly wider element width for better typography
            var elementRect = new Rect(PADDING_SIDE, 0, _contentArea.width - (PADDING_SIDE * 2) - 10, 25);
            
            foreach (var action in _drawActions)
            {
                action(elementRect);
            }

            GUI.EndScrollView();
            GUI.skin = prevSkin;
        }

        public GregUIBuilder Build()
        {
            GregUIManager.RegisterWindow(this);
            return this;
        }

        public GregUIBuilder AddHeadline(string text)
        {
            float y = _currentY;
            _drawActions.Add(r => GUI.Label(new Rect(r.x, y, r.width, 30), text.ToUpper(), GregImGui.stHeader));
            _currentY += 30 + SPACING;
            return this;
        }

        public GregUIBuilder AddLabel(string text)
        {
            float y = _currentY;
            _drawActions.Add(r => GUI.Label(new Rect(r.x, y, r.width, 24), text, GregImGui.stLabel));
            _currentY += 24 + (SPACING / 2);
            return this;
        }

        public GregUIBuilder AddButton(string label, Action onClick)
        {
            float y = _currentY;
            _drawActions.Add(r => {
                if (GUI.Button(new Rect(r.x, y, r.width, 40), label, GregImGui.stButton))
                    onClick?.Invoke();
            });
            _currentY += 40 + SPACING;
            return this;
        }

        public GregUIBuilder AddToggle(string label, bool currentValue, Action<bool> onChanged)
        {
            float y = _currentY;
            _drawActions.Add(r => {
                bool next = GUI.Toggle(new Rect(r.x, y, r.width, 25), currentValue, "  " + label, GregImGui.stToggle);
                if (next != currentValue) onChanged?.Invoke(next);
            });
            _currentY += 25 + SPACING;
            return this;
        }

        public GregUIBuilder AddSwitch(string label, bool currentValue, Action<bool> onChanged)
        {
            float y = _currentY;
            _drawActions.Add(r => {
                GUI.Label(new Rect(r.x, y, r.width - 60, 25), label, GregImGui.stLabel);
                bool next = GregImGui.DrawSwitch(new Rect(r.x + r.width - 45, y, 44, 25), currentValue);
                if (next != currentValue) onChanged?.Invoke(next);
            });
            _currentY += 25 + SPACING;
            return this;
        }

        public GregUIBuilder AddSlider(string label, float min, float max, float currentValue, Action<float> onChanged)
        {
            float y = _currentY;
            _drawActions.Add(r => {
                GUI.Label(new Rect(r.x, y, r.width, 20), $"{label}: {currentValue:F2}", GregImGui.stLabel);
                float next = GUI.HorizontalSlider(new Rect(r.x, y + 24, r.width, 20), currentValue, min, max);
                if (Mathf.Abs(next - currentValue) > 0.001f) onChanged?.Invoke(next);
            });
            _currentY += 48 + SPACING;
            return this;
        }

        public GregUIBuilder AddSpacer(float height = 20f)
        {
            _currentY += height;
            return this;
        }

        public GregUIBuilder AddPrimaryButton(string label, Action onClick) => AddButton(label, onClick);
        public GregUIBuilder AddSecondaryButton(string label, Action onClick) => AddButton(label, onClick);
        public GregUIBuilder AddSection(string title) => AddHeadline(title);

        public bool IsVisible 
        { 
            get => _isVisible; 
            set => _isVisible = value; 
        }
        
        public void Toggle() => _isVisible = !_isVisible;
    }
}
