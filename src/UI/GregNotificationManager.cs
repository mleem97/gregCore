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
        private static VisualElement? _container;
        private static readonly Queue<(string message, float duration)> _pending = new();
        private static readonly List<(VisualElement element, float expireTime)> _active = new();
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
            if (!_initialized) Initialize();

            if (_container == null)
            {
                _pending.Enqueue((message, duration));
                return;
            }

            try
            {
                ProcessPending();
                CreateToast(message, duration);
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[GregNotificationManager] Show failed: {ex.Message}");
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

        private static void CreateToast(string message, float duration)
        {
            if (_container == null) return;

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

            var label = new Label(message);
            label.style.color = new Color(0.95f, 0.95f, 0.97f);
            label.style.fontSize = 13;
            label.style.whiteSpace = WhiteSpace.Normal;
            toast.Add(label);

            _container.Add(toast);
            _active.Add((toast, Time.time + duration));

            toast.schedule.Execute(new Action<TimerState>(_ =>
            {
                toast.style.opacity = 1;
                toast.style.translate = new Translate(0, 0);
            })).StartingIn(10);
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
                    catch { }
                    _active.RemoveAt(i);
                }
            }
        }
    }
}
