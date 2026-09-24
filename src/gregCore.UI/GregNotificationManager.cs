using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI
{
    /// <summary>
    /// Toast notification system for GregCore.
    /// Displays short-lived notification banners at the edge of the screen.
    /// </summary>
    public static class GregNotificationManager
    {
        public enum GregToastType
        {
            Info = 0,
            Warning = 1,
            Error = 2,
            Custom = 3,
        }

        private static VisualElement? _container;
        private static readonly Queue<(string message, float duration)> _pending = new();
        private static readonly List<(VisualElement element, float expireTime)> _active = new();
        private const int MaxPending = 64;
        private const int MaxActive = 16;
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                var root = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Notification);
                if (root == null) return;

                _container = new VisualElement();
                _container.name = "NotificationContainer";
                _container.style.position = Position.Absolute;
                _container.style.right = 20;
                _container.style.top = 20;
                _container.style.width = 350;
                _container.style.flexDirection = FlexDirection.Column;
                root.Add(_container);

                MelonLoader.MelonLogger.Msg("[GregNotificationManager] Initialized.");
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[GregNotificationManager] Init failed: {ex.Message}");
            }
        }

        public static void Show(string message, float duration = 3f)
        {
            Show(message, GregToastType.Info, null, duration);
        }

        public static void Show(string message, GregToastType type, float duration = 3f)
        {
            Show(message, type, null, duration);
        }

        // Custom icon by name from the IconToolkit (e.g. mod-owned icons).
        public static void Show(string message, string customIcon, float duration = 3f)
        {
            Show(message, GregToastType.Custom, customIcon, duration);
        }

        public static void Show(string message, GregToastType type, string customIcon, float duration = 3f)
        {
            if (!_initialized) Initialize();

            if (_container == null)
            {
                if (_pending.Count >= MaxPending) _pending.Dequeue();
                _pending.Enqueue((message, duration));
                return;
            }

            try
            {
                ProcessPending();
                CreateToast(message, duration, TypeIcon(type, customIcon), TypeColor(type));
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[GregNotificationManager] Show failed: {ex.Message}");
            }
        }

        // Rich toast: optional cover/thumbnail + up to three text lines
        // (e.g. now-playing: small/large/small). Cover as Texture2D.
        public static void ShowRich(string lineTop, string lineTitle, string lineSub,
            Texture2D cover, string fallbackIcon, float duration = 5f)
        {
            if (!_initialized) Initialize();

            if (_container == null)
            {
                if (_pending.Count >= MaxPending) _pending.Dequeue();
                _pending.Enqueue(((lineTitle ?? "") + " " + (lineSub ?? ""), duration));
                return;
            }

            try
            {
                ProcessPending();
                CreateRichToast(lineTop, lineTitle, lineSub, cover, fallbackIcon, duration);
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[GregNotificationManager] ShowRich failed: {ex.Message}");
            }
        }

        private static void ProcessPending()
        {
            while (_pending.Count > 0 && _container != null)
            {
                var (msg, dur) = _pending.Dequeue();
                CreateToast(msg, dur);
            }
        }

        private static string TypeIcon(GregToastType type, string customIcon)
        {
            switch (type)
            {
                case GregToastType.Warning: return "warning";
                case GregToastType.Error: return "error";
                case GregToastType.Custom: return string.IsNullOrEmpty(customIcon) ? "note" : customIcon;
                default: return "info";
            }
        }

        private static Color TypeColor(GregToastType type)
        {
            switch (type)
            {
                case GregToastType.Warning: return GregUITheme.Warning;
                case GregToastType.Error: return GregUITheme.Danger;
                default: return GregUITheme.PrimaryAccent;
            }
        }

        private static void CreateToast(string message, float duration)
        {
            CreateToast(message, duration, TypeIcon(GregToastType.Info, null), TypeColor(GregToastType.Info));
        }

        private static void CreateToast(string message, float duration, string icon, Color accent)
        {
            if (_container == null) return;

            while (_active.Count >= MaxActive)
            {
                var oldest = _active[0].element;
                try { oldest?.RemoveFromHierarchy(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                _active.RemoveAt(0);
            }

            var toast = new VisualElement();
            toast.style.backgroundColor = GregUITheme.SurfaceDark;
            toast.style.borderLeftWidth = 4;
            toast.style.borderLeftColor = accent;
            toast.style.borderTopLeftRadius = 4;
            toast.style.borderTopRightRadius = 4;
            toast.style.borderBottomLeftRadius = 4;
            toast.style.borderBottomRightRadius = 4;
            toast.style.paddingLeft = 12;
            toast.style.paddingRight = 12;
            toast.style.paddingTop = 10;
            toast.style.paddingBottom = 10;
            toast.style.marginBottom = 8;
            toast.style.opacity = 0;
            toast.style.translate = new Translate(100, 0);
            toast.style.flexDirection = FlexDirection.Row;

            var iconEl = GregIconToolkit.Icon(icon, 28f);
            if (iconEl != null)
            {
                iconEl.style.marginRight = 10f;
                iconEl.style.alignSelf = Align.Center;
                toast.Add(iconEl);
            }

            var label = new Label(message);
            label.style.color = new Color(0.95f, 0.95f, 0.97f);
            label.style.fontSize = 13;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.flexGrow = 1f;
            // Toolkit default font is unusable in IL2CPP builds (text
            // invisible): assign game font as soon as available.
            try
            {
                var f = GregFontLoader.DefaultUGUIFont;
                if (f != null) label.style.unityFont = f;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            toast.Add(label);

            _container.Add(toast);
            _active.Add((toast, Time.time + duration));

            toast.schedule.Execute(new Action<TimerState>(_ =>
            {
                toast.style.opacity = 1;
                toast.style.translate = new Translate(0, 0);
            })).StartingIn(10);
        }

        private static void CreateRichToast(string lineTop, string lineTitle, string lineSub,
            Texture2D cover, string fallbackIcon, float duration)
        {
            if (_container == null) return;

            while (_active.Count >= MaxActive)
            {
                var oldest = _active[0].element;
                try { oldest?.RemoveFromHierarchy(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                _active.RemoveAt(0);
            }

            var toast = new VisualElement();
            toast.style.backgroundColor = GregUITheme.SurfaceDark;
            toast.style.borderLeftWidth = 4;
            toast.style.borderLeftColor = GregUITheme.PrimaryAccent;
            toast.style.borderTopLeftRadius = 4;
            toast.style.borderTopRightRadius = 4;
            toast.style.borderBottomLeftRadius = 4;
            toast.style.borderBottomRightRadius = 4;
            toast.style.paddingLeft = 12;
            toast.style.paddingRight = 12;
            toast.style.paddingTop = 10;
            toast.style.paddingBottom = 10;
            toast.style.marginBottom = 8;
            toast.style.opacity = 0;
            toast.style.translate = new Translate(100, 0);
            toast.style.flexDirection = FlexDirection.Row;

            VisualElement art = null;
            try
            {
                if (cover != null)
                {
                    art = new VisualElement();
                    art.style.backgroundImage = new StyleBackground(Background.FromTexture2D(cover));
                    try { art.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    art.style.width = 56f;
                    art.style.height = 56f;
                }
                else
                {
                    art = GregIconToolkit.Icon(fallbackIcon ?? "note", 56f);
                }
            }
            catch { art = null; }
            if (art != null)
            {
                art.style.marginRight = 12f;
                art.style.alignSelf = Align.Center;
                toast.Add(art);
            }

            var col = new VisualElement();
            col.style.flexDirection = FlexDirection.Column;
            col.style.flexGrow = 1f;
            Font f2 = null;
            try { f2 = GregFontLoader.DefaultUGUIFont; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            AddRichLine(col, lineTop, 11f, new Color(0.65f, 0.65f, 0.7f), false, f2);
            AddRichLine(col, lineTitle, 17f, new Color(1f, 1f, 1f), true, f2);
            AddRichLine(col, lineSub, 13f, new Color(0.88f, 0.88f, 0.88f), false, f2);
            toast.Add(col);

            _container.Add(toast);
            _active.Add((toast, Time.time + duration));

            toast.schedule.Execute(new Action<TimerState>(_ =>
            {
                toast.style.opacity = 1;
                toast.style.translate = new Translate(0, 0);
            })).StartingIn(10);
        }

        private static void AddRichLine(VisualElement parent, string text, float size, Color color, bool bold, Font font)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text)) return;
                var label = new Label(text);
                label.style.color = color;
                label.style.fontSize = size;
                label.style.whiteSpace = WhiteSpace.Normal;
                if (bold)
                {
                    try { label.style.unityFontStyleAndWeight = FontStyle.Bold; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                if (font != null)
                {
                    try { label.style.unityFont = font; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                parent.Add(label);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        public static void Update()
        {
            if (_active.Count == 0) return;

            var now = Time.time;
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var (element, expireTime) = _active[i];
                if (element == null)
                {
                    _active.RemoveAt(i);
                    continue;
                }

                if (now >= expireTime)
                {
                    try
                    {
                        element.style.opacity = 0;
                        element.style.translate = new Translate(100, 0);
                        element.schedule.Execute(new Action<TimerState>(_ =>
                        {
                            element?.RemoveFromHierarchy();
                        })).StartingIn(350);
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    _active.RemoveAt(i);
                }
            }
        }
    }
}
