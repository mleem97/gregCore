using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI
{
    /// <summary>
    /// Game-specific UI elements for GregCore mods.
    /// Health bars, progress bars, and status indicators using UI Toolkit.
    /// </summary>
    public static class GregGameUI
    {
        private static readonly Dictionary<VisualElement, BarData> _barData = new();
        private static readonly Dictionary<VisualElement, CooldownData> _cooldownData = new();

        /// <summary>
        /// Create a health/mana bar with animated fill.
        /// </summary>
        public static VisualElement CreateBar(string label, Color fillColor, float currentValue, float maxValue)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.height = 24;
            container.style.marginBottom = 6;

            var labelElem = new Label(label);
            labelElem.style.width = 80;
            labelElem.style.color = new Color(0.88f, 0.88f, 0.88f);
            labelElem.style.fontSize = 12;
            container.Add(labelElem);

            var track = new VisualElement();
            track.style.flexGrow = 1;
            track.style.height = 12;
            track.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            track.style.borderTopLeftRadius = 6;
            track.style.borderTopRightRadius = 6;
            track.style.borderBottomLeftRadius = 6;
            track.style.borderBottomRightRadius = 6;
            track.style.overflow = Overflow.Hidden;

            var fill = new VisualElement();
            fill.name = "BarFill";
            fill.style.height = 12;
            fill.style.width = new Length(Mathf.Clamp01(currentValue / maxValue) * 100, LengthUnit.Percent);
            fill.style.backgroundColor = fillColor;
            track.Add(fill);

            var valueLabel = new Label($"{currentValue:F0}/{maxValue:F0}");
            valueLabel.style.width = 60;
            valueLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            valueLabel.style.fontSize = 11;
            valueLabel.style.unityTextAlign = TextAnchor.MiddleRight;

            container.Add(track);
            container.Add(valueLabel);

            _barData[container] = new BarData
            {
                Fill = fill,
                ValueLabel = valueLabel,
                MaxValue = maxValue
            };

            return container;
        }

        public static void UpdateBar(VisualElement barContainer, float currentValue)
        {
            if (barContainer == null) return;
            if (!_barData.TryGetValue(barContainer, out var data)) return;
            float pct = Mathf.Clamp01(currentValue / data.MaxValue);
            data.Fill.style.width = new Length(pct * 100, LengthUnit.Percent);
            data.ValueLabel.text = $"{currentValue:F0}/{data.MaxValue:F0}";
        }

        /// <summary>
        /// Create a circular cooldown overlay (radial fill).
        /// </summary>
        public static VisualElement CreateCooldownOverlay(float size = 48f)
        {
            var container = new VisualElement();
            container.style.width = size;
            container.style.height = size;
            container.style.borderTopLeftRadius = size / 2;
            container.style.borderTopRightRadius = size / 2;
            container.style.borderBottomLeftRadius = size / 2;
            container.style.borderBottomRightRadius = size / 2;
            container.style.backgroundColor = new Color(0, 0, 0, 0.6f);
            container.style.overflow = Overflow.Hidden;

            var fill = new VisualElement();
            fill.name = "CooldownFill";
            fill.style.position = Position.Absolute;
            fill.style.left = 0;
            fill.style.bottom = 0;
            fill.style.width = size;
            fill.style.height = 0;
            fill.style.backgroundColor = new Color(0, 0, 0, 0.7f);
            container.Add(fill);

            var text = new Label();
            text.name = "CooldownText";
            text.style.color = Color.white;
            text.style.fontSize = 14;
            text.style.unityFontStyleAndWeight = FontStyle.Bold;
            text.style.unityTextAlign = TextAnchor.MiddleCenter;
            text.style.position = Position.Absolute;
            text.style.left = 0;
            text.style.top = 0;
            text.style.width = size;
            text.style.height = size;
            container.Add(text);

            _cooldownData[container] = new CooldownData
            {
                Fill = fill,
                Text = text,
                Size = size
            };

            return container;
        }

        public static void UpdateCooldown(VisualElement overlay, float current, float max)
        {
            if (overlay == null) return;
            if (!_cooldownData.TryGetValue(overlay, out var data)) return;
            float pct = Mathf.Clamp01(current / max);
            data.Fill.style.height = data.Size * pct;
            data.Text.text = current > 0 ? $"{current:F1}s" : "";
            data.Text.style.display = current > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Create a floating damage number popup element.
        /// </summary>
        public static Label CreateDamagePopup(float damage, bool isCritical = false)
        {
            var label = new Label(damage.ToString("F0"));
            label.style.color = isCritical ? new Color(1f, 0.2f, 0.2f) : new Color(1f, 0.9f, 0.3f);
            label.style.fontSize = isCritical ? 28 : 20;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.textShadow = new TextShadow
            {
                offset = new Vector2(1, 1),
                blurRadius = 2,
                color = Color.black
            };
            label.pickingMode = PickingMode.Ignore;
            return label;
        }

        /// <summary>
        /// Create a quest objective update banner.
        /// </summary>
        public static VisualElement CreateObjectiveBanner(string title, string description)
        {
            var container = new VisualElement();
            container.style.backgroundColor = new Color(0.1f, 0.1f, 0.12f, 0.9f);
            container.style.borderLeftWidth = 4;
            container.style.borderLeftColor = GregUITheme.PrimaryAccent;
            container.style.paddingLeft = 16;
            container.style.paddingRight = 16;
            container.style.paddingTop = 12;
            container.style.paddingBottom = 12;
            container.style.marginBottom = 8;

            var titleLabel = new Label(title.ToUpper());
            titleLabel.style.color = GregUITheme.SecondaryColor;
            titleLabel.style.fontSize = 14;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 4;
            container.Add(titleLabel);

            var descLabel = new Label(description);
            descLabel.style.color = new Color(0.8f, 0.8f, 0.82f);
            descLabel.style.fontSize = 12;
            container.Add(descLabel);

            return container;
        }

        /// <summary>
        /// Create a minimap container.
        /// </summary>
        public static VisualElement CreateMinimap(float size = 200f)
        {
            var container = new VisualElement();
            container.style.width = size;
            container.style.height = size;
            container.style.backgroundColor = new Color(0.05f, 0.05f, 0.06f, 0.8f);
            container.style.borderTopLeftRadius = size / 2;
            container.style.borderTopRightRadius = size / 2;
            container.style.borderBottomLeftRadius = size / 2;
            container.style.borderBottomRightRadius = size / 2;
            container.style.borderLeftWidth = 2;
            container.style.borderRightWidth = 2;
            container.style.borderTopWidth = 2;
            container.style.borderBottomWidth = 2;
            container.style.borderLeftColor = GregUITheme.NeutralBorder;
            container.style.borderRightColor = GregUITheme.NeutralBorder;
            container.style.borderTopColor = GregUITheme.NeutralBorder;
            container.style.borderBottomColor = GregUITheme.NeutralBorder;
            container.style.overflow = Overflow.Hidden;

            var compass = new VisualElement();
            compass.style.position = Position.Absolute;
            compass.style.top = 4;
            compass.style.left = new Length(50, LengthUnit.Percent);
            compass.style.width = 8;
            compass.style.height = 8;
            compass.style.backgroundColor = Color.red;
            compass.style.translate = new Translate(new Length(-50, LengthUnit.Percent), 0);
            compass.style.borderTopLeftRadius = 4;
            compass.style.borderTopRightRadius = 4;
            compass.style.borderBottomLeftRadius = 4;
            compass.style.borderBottomRightRadius = 4;
            container.Add(compass);

            return container;
        }

        private class BarData
        {
            public VisualElement Fill = null!;
            public Label ValueLabel = null!;
            public float MaxValue;
        }

        private class CooldownData
        {
            public VisualElement Fill = null!;
            public Label Text = null!;
            public float Size;
        }
    }
}
