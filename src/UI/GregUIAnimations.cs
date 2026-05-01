using System;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace gregCore.UI
{
    /// <summary>
    /// UI animation and transition system for GregCore.
    /// Provides tween-like animations using UI Toolkit's transition system.
    /// </summary>
    public static class GregUIAnimations
    {
        private static Il2CppSystem.Collections.Generic.List<TimeValue> MakeTimeValueList(float durationMs)
        {
            var list = new Il2CppSystem.Collections.Generic.List<TimeValue>();
            list.Add(new TimeValue(durationMs, TimeUnit.Millisecond));
            return list;
        }

        private static Il2CppSystem.Collections.Generic.List<EasingFunction> MakeEasingList(EasingMode mode)
        {
            var list = new Il2CppSystem.Collections.Generic.List<EasingFunction>();
            list.Add(new EasingFunction(mode));
            return list;
        }

        /// <summary>
        /// Fade in an element over time.
        /// </summary>
        public static void FadeIn(VisualElement element, float durationMs = 300)
        {
            if (element == null) return;
            element.style.opacity = 0;
            element.style.display = DisplayStyle.Flex;
            element.schedule.Execute(new Action<TimerState>(_ =>
            {
                element.style.opacity = 1;
                element.style.transitionDuration = new StyleList<TimeValue>(MakeTimeValueList(durationMs));
                element.style.transitionTimingFunction = new StyleList<EasingFunction>(MakeEasingList(EasingMode.EaseOutCubic));
            })).StartingIn(10);
        }

        /// <summary>
        /// Fade out an element over time.
        /// </summary>
        public static void FadeOut(VisualElement element, float durationMs = 300, Action? onComplete = null)
        {
            if (element == null) return;
            element.style.opacity = 0;
            element.style.transitionDuration = new StyleList<TimeValue>(MakeTimeValueList(durationMs));
            element.schedule.Execute(new Action<TimerState>(_ =>
            {
                element.style.display = DisplayStyle.None;
                onComplete?.Invoke();
            })).StartingIn((long)durationMs + 50);
        }

        /// <summary>
        /// Slide an element in from a direction.
        /// </summary>
        public static void SlideIn(VisualElement element, SlideDirection direction, float durationMs = 400)
        {
            if (element == null) return;

            var startTranslate = direction switch
            {
                SlideDirection.Left => new Translate(new Length(-100, LengthUnit.Percent), 0),
                SlideDirection.Right => new Translate(new Length(100, LengthUnit.Percent), 0),
                SlideDirection.Top => new Translate(0, new Length(-100, LengthUnit.Percent)),
                SlideDirection.Bottom => new Translate(0, new Length(100, LengthUnit.Percent)),
                _ => new Translate(0, 0)
            };

            element.style.translate = startTranslate;
            element.style.display = DisplayStyle.Flex;
            element.schedule.Execute(new Action<TimerState>(_ =>
            {
                element.style.translate = new Translate(0, 0);
                element.style.transitionDuration = new StyleList<TimeValue>(MakeTimeValueList(durationMs));
                element.style.transitionTimingFunction = new StyleList<EasingFunction>(MakeEasingList(EasingMode.EaseOutCubic));
            })).StartingIn(10);
        }

        /// <summary>
        /// Pulse animation for highlighting elements.
        /// </summary>
        public static void Pulse(VisualElement element, float durationMs = 1000)
        {
            if (element == null) return;

            element.schedule.Execute(new Action<TimerState>(_ =>
            {
                element.style.scale = new Scale(new Vector2(1.05f, 1.05f));
                element.style.transitionDuration = new StyleList<TimeValue>(MakeTimeValueList(durationMs / 2));
                element.style.transitionTimingFunction = new StyleList<EasingFunction>(MakeEasingList(EasingMode.EaseInOutSine));
            })).StartingIn(10);

            element.schedule.Execute(new Action<TimerState>(_ =>
            {
                element.style.scale = new Scale(Vector2.one);
            })).StartingIn((long)(durationMs / 2) + 20);
        }

        /// <summary>
        /// Screen transition between two panels.
        /// </summary>
        public static void TransitionPanels(VisualElement from, VisualElement to, TransitionType type, float durationMs = 400)
        {
            if (from == null || to == null) return;

            switch (type)
            {
                case TransitionType.Fade:
                    FadeOut(from, durationMs / 2, () => FadeIn(to, durationMs / 2));
                    break;
                case TransitionType.SlideLeft:
                    SlideOut(from, SlideDirection.Left, durationMs / 2);
                    SlideIn(to, SlideDirection.Right, durationMs / 2);
                    break;
                case TransitionType.SlideRight:
                    SlideOut(from, SlideDirection.Right, durationMs / 2);
                    SlideIn(to, SlideDirection.Left, durationMs / 2);
                    break;
                case TransitionType.Zoom:
                    ZoomOut(from, durationMs / 2, () => ZoomIn(to, durationMs / 2));
                    break;
            }
        }

        private static void SlideOut(VisualElement element, SlideDirection direction, float durationMs)
        {
            if (element == null) return;

            var endTranslate = direction switch
            {
                SlideDirection.Left => new Translate(new Length(-100, LengthUnit.Percent), 0),
                SlideDirection.Right => new Translate(new Length(100, LengthUnit.Percent), 0),
                SlideDirection.Top => new Translate(0, new Length(-100, LengthUnit.Percent)),
                SlideDirection.Bottom => new Translate(0, new Length(100, LengthUnit.Percent)),
                _ => new Translate(0, 0)
            };

            element.style.translate = endTranslate;
            element.style.transitionDuration = new StyleList<TimeValue>(MakeTimeValueList(durationMs));
            element.schedule.Execute(new Action<TimerState>(_ => element.style.display = DisplayStyle.None))
                .StartingIn((long)durationMs + 50);
        }

        private static void ZoomIn(VisualElement element, float durationMs)
        {
            if (element == null) return;
            element.style.scale = new Scale(new Vector2(0.8f, 0.8f));
            element.style.opacity = 0;
            element.style.display = DisplayStyle.Flex;
            element.schedule.Execute(new Action<TimerState>(_ =>
            {
                element.style.scale = new Scale(Vector2.one);
                element.style.opacity = 1;
                element.style.transitionDuration = new StyleList<TimeValue>(MakeTimeValueList(durationMs));
                element.style.transitionTimingFunction = new StyleList<EasingFunction>(MakeEasingList(EasingMode.EaseOutBack));
            })).StartingIn(10);
        }

        private static void ZoomOut(VisualElement element, float durationMs, Action? onComplete)
        {
            if (element == null) return;
            element.style.scale = new Scale(new Vector2(0.8f, 0.8f));
            element.style.opacity = 0;
            element.style.transitionDuration = new StyleList<TimeValue>(MakeTimeValueList(durationMs));
            element.schedule.Execute(new Action<TimerState>(_ =>
            {
                element.style.display = DisplayStyle.None;
                onComplete?.Invoke();
            })).StartingIn((long)durationMs + 50);
        }
    }

    public enum SlideDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum TransitionType
    {
        Fade,
        SlideLeft,
        SlideRight,
        Zoom
    }
}
