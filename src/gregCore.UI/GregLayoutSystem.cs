using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI
{
    /// <summary>
    /// Layout system for GregCore UI Toolkit.
    /// Provides helper methods for common layout patterns.
    /// </summary>
    public static class GregLayoutSystem
    {
        /// <summary>
        /// Create a vertical layout group with automatic spacing.
        /// </summary>
        public static VisualElement CreateVerticalGroup(float spacing = 12f, float padding = 16f)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;
            container.style.paddingLeft = padding;
            container.style.paddingRight = padding;
            container.style.paddingTop = padding;
            container.style.paddingBottom = padding;
            // Note: spacing in UI Toolkit is typically handled via margins on children
            return container;
        }

        /// <summary>
        /// Create a horizontal layout group with automatic spacing.
        /// </summary>
        public static VisualElement CreateHorizontalGroup(float spacing = 12f, float padding = 16f)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.paddingLeft = padding;
            container.style.paddingRight = padding;
            container.style.paddingTop = padding;
            container.style.paddingBottom = padding;
            return container;
        }

        /// <summary>
        /// Create a grid layout group.
        /// </summary>
        public static VisualElement CreateGridGroup(int columns, float spacing = 8f, float padding = 16f)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;
            container.style.paddingLeft = padding;
            container.style.paddingRight = padding;
            container.style.paddingTop = padding;
            container.style.paddingBottom = padding;
            container.userData = columns; // Store column count
            return container;
        }

        /// <summary>
        /// Add an element to a grid with automatic column sizing.
        /// </summary>
        public static void AddToGrid(VisualElement gridContainer, VisualElement child, int columns)
        {
            if (gridContainer == null || child == null) return;
            float percentage = 100f / columns - 1f;
            child.style.width = new Length(percentage, LengthUnit.Percent);
            child.style.marginRight = 4;
            child.style.marginBottom = 4;
            gridContainer.Add(child);
        }

        /// <summary>
        /// Create a responsive container that adapts to screen size.
        /// </summary>
        public static VisualElement CreateResponsiveContainer(float minWidth = 320, float maxWidth = 800)
        {
            var container = new VisualElement();
            container.style.minWidth = minWidth;
            container.style.maxWidth = maxWidth;
            container.style.width = new Length(90, LengthUnit.Percent);
            container.style.marginLeft = new Length(5, LengthUnit.Percent);
            container.style.marginRight = new Length(5, LengthUnit.Percent);
            return container;
        }

        /// <summary>
        /// Create a scrollable list container.
        /// </summary>
        public static ScrollView CreateScrollableList(float maxHeight = 400f)
        {
            var scrollView = new ScrollView();
            scrollView.style.maxHeight = maxHeight;
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Auto;
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            return scrollView;
        }

        /// <summary>
        /// Create an aspect-ratio-fitted container.
        /// </summary>
        public static VisualElement CreateAspectRatioContainer(float aspectRatio = 16f / 9f)
        {
            var container = new VisualElement();
            container.style.aspectRatio = aspectRatio;
            container.style.overflow = Overflow.Hidden;
            return container;
        }

        /// <summary>
        /// Apply responsive breakpoints to a container.
        /// Changes layout based on parent width.
        /// </summary>
        public static void ApplyResponsiveBreakpoints(VisualElement element,
            float mobileBreakpoint = 600,
            float tabletBreakpoint = 1024)
        {
            if (element == null) return;

            element.RegisterCallback<GeometryChangedEvent>(new Action<GeometryChangedEvent>(_ =>
            {
                float width = element.resolvedStyle.width;
                if (width < mobileBreakpoint)
                {
                    // Mobile layout
                    element.style.paddingLeft = 8;
                    element.style.paddingRight = 8;
                }
                else if (width < tabletBreakpoint)
                {
                    // Tablet layout
                    element.style.paddingLeft = 16;
                    element.style.paddingRight = 16;
                }
                else
                {
                    // Desktop layout
                    element.style.paddingLeft = 24;
                    element.style.paddingRight = 24;
                }
            }));
        }
    }
}
