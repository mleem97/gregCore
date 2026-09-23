/// <file-summary>
/// Schicht:      UI
/// Zweck:        Wiederverwendbares Mod-Panel (gregCore.UI Baukasten).
///               Theming ueber GregUITheme, Sichtbarkeit + Slide-Animation
///               + Draggable-Header. Der Inhalt (Content-Container) wird vom
///               Mod befuellt; Klick-Zustellung laeuft ueber GregUIInputSystem
///               (echte Callbacks) oder mod-seitiges Hit-Routing als Fallback.
///               Ticks laufen zentral ueber GregMenuRegistry.Tick.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Runtime UI shell over live Unity scene; needs running game.")]
public sealed class GregPanel
{
    private enum SlideState { Hidden, Shown, SlidingIn, SlidingOut }

    private static readonly List<GregPanel> _all = new List<GregPanel>();
    private const float SlideDuration = 0.28f;
    private const float DockMargin = 16f;

    public string MenuId { get; }
    public VisualElement Root { get; private set; }
    public VisualElement Content { get; private set; }
    public GregMenuOptions Options { get; private set; }
    public bool IsVisible { get; private set; }

    private SlideState _slide = SlideState.Hidden;
    private float _slideT;
    private VisualElement _dragHandle;
    private bool _dragging;
    private Vector2 _dragOffset;

    private GregPanel(string menuId)
    {
        MenuId = menuId;
        Options = new GregMenuOptions();
    }

    public static GregPanel GetOrCreate(string menuId, string title, GregMenuOptions options)
    {
        lock (_all)
        {
            foreach (var p in _all)
            {
                if (p != null && p.MenuId == menuId)
                {
                    if (options != null) p.Configure(options);
                    return p;
                }
            }
            var panel = new GregPanel(menuId);
            if (options != null) panel.Options = options.Clone();
            panel.BuildChrome(title);
            GregMenuRegistry.RegisterMenu(menuId, panel.Options);
            _all.Add(panel);
            return panel;
        }
    }

    public void Configure(GregMenuOptions options)
    {
        if (options == null) return;
        Options = options.Clone();
        try
        {
            if (Root != null) Root.style.width = Options.PanelWidth;
        }
        catch { }
        GregMenuRegistry.RegisterMenu(MenuId, Options);
        if (IsVisible) SnapPosition();
    }

    public void SetDragHandle(VisualElement handle)
    {
        _dragHandle = handle;
    }

    public void Show()
    {
        try
        {
            if (Root == null) return;
            IsVisible = true;
            GregMenuRegistry.SetOpen(MenuId, true);
            if (Options.SlideFromRight && !Options.Draggable)
            {
                _slide = SlideState.SlidingIn;
                _slideT = 0f;
                Root.style.display = DisplayStyle.Flex;
                Root.style.left = Screen.width;
            }
            else
            {
                _slide = SlideState.Shown;
                Root.style.display = DisplayStyle.Flex;
                SnapPosition();
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Panel-Show fehlgeschlagen ({MenuId}): {ex.Message}");
        }
    }

    public void Hide()
    {
        try
        {
            IsVisible = false;
            GregMenuRegistry.SetOpen(MenuId, false);
            _dragging = false;
            if (Root == null) return;
            if (_slide == SlideState.Shown || _slide == SlideState.SlidingIn)
            {
                if (Options.SlideFromRight && !Options.Draggable)
                {
                    _slide = SlideState.SlidingOut;
                    _slideT = 0f;
                    return;
                }
            }
            _slide = SlideState.Hidden;
            Root.style.display = DisplayStyle.None;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Panel-Hide fehlgeschlagen ({MenuId}): {ex.Message}");
        }
    }

    public void Toggle()
    {
        if (IsVisible) Hide();
        else Show();
    }

    // Inhalt neu befuellen (Mod baut Buttons/Labels in den Container).
    public void RebuildContent(Action<VisualElement> build)
    {
        try
        {
            if (Content == null || build == null) return;
            Content.Clear();
            build(Content);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Panel-Rebuild fehlgeschlagen ({MenuId}): {ex.Message}");
        }
    }

    internal static void TickAll(float dt)
    {
        GregPanel[] copy;
        lock (_all) { copy = _all.ToArray(); }
        foreach (var p in copy)
        {
            try { p?.Tick(dt); } catch { }
        }
    }

    private void Tick(float dt)
    {
        if (Root == null) return;
        try
        {
            if (_slide == SlideState.SlidingIn || _slide == SlideState.SlidingOut)
            {
                _slideT += dt / SlideDuration;
                float target = DockLeft();
                if (_slide == SlideState.SlidingIn)
                {
                    if (_slideT >= 1f)
                    {
                        Root.style.left = target;
                        _slide = SlideState.Shown;
                    }
                    else
                    {
                        float from = Screen.width;
                        Root.style.left = from + (target - from) * EaseOut(_slideT);
                    }
                }
                else
                {
                    if (_slideT >= 1f)
                    {
                        _slide = SlideState.Hidden;
                        Root.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        float from = DockLeft();
                        Root.style.left = from + (Screen.width - from) * EaseOut(_slideT);
                    }
                }
                return;
            }
            if (IsVisible && Options.Draggable) TickDrag();
        }
        catch { }
    }

    private void TickDrag()
    {
        try
        {
            if (_dragHandle == null || _slide != SlideState.Shown) return;
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse == null) return;
            Vector2 pos = mouse.position.ReadValue();
            pos.y = Screen.height - pos.y;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                try
                {
                    Rect b = _dragHandle.worldBound;
                    if (b.width > 0f && b.height > 0f && b.Contains(pos))
                    {
                        _dragging = true;
                        Rect r = Root.worldBound;
                        _dragOffset = new Vector2(pos.x - r.x, pos.y - r.y);
                    }
                }
                catch { }
                return;
            }
            if (_dragging)
            {
                if (mouse.leftButton.isPressed)
                {
                    float nx = pos.x - _dragOffset.x;
                    float ny = pos.y - _dragOffset.y;
                    if (nx < 0f) nx = 0f;
                    if (ny < 0f) ny = 0f;
                    Root.style.left = nx;
                    Root.style.top = ny;
                }
                else
                {
                    _dragging = false;
                }
            }
        }
        catch { }
    }

    private float DockLeft()
    {
        try
        {
            float w = Options.PanelWidth;
            float x = Screen.width - w - DockMargin;
            return x < 0f ? 0f : x;
        }
        catch { return 0f; }
    }

    private void SnapPosition()
    {
        try
        {
            if (Root == null) return;
            if (Options.SlideFromRight)
            {
                Root.style.left = DockLeft();
                Root.style.top = 60f;
            }
            else
            {
                Root.style.left = 16f;
                Root.style.top = 60f;
            }
        }
        catch { }
    }

    private static float EaseOut(float t)
    {
        if (t < 0f) t = 0f;
        if (t > 1f) t = 1f;
        return 1f - (1f - t) * (1f - t);
    }

    private void BuildChrome(string title)
    {
        VisualElement layer = null;
        try { layer = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Panel); } catch { }
        if (layer == null)
        {
            MelonLogger.Error("[gregCore][UI] Panel-Layer nicht verfuegbar.");
            return;
        }

        var root = new VisualElement();
        root.name = "GregPanel_" + MenuId;
        root.style.position = Position.Absolute;
        root.style.width = Options.PanelWidth;
        root.style.backgroundColor = GregUITheme.PanelBackground;
        root.style.borderTopLeftRadius = GregUITheme.CornerRadius;
        root.style.borderTopRightRadius = GregUITheme.CornerRadius;
        root.style.borderBottomLeftRadius = GregUITheme.CornerRadius;
        root.style.borderBottomRightRadius = GregUITheme.CornerRadius;
        root.style.borderLeftWidth = GregUITheme.BorderWidth;
        root.style.borderRightWidth = GregUITheme.BorderWidth;
        root.style.borderTopWidth = GregUITheme.BorderWidth;
        root.style.borderBottomWidth = GregUITheme.BorderWidth;
        root.style.borderLeftColor = GregUITheme.NeutralBorder;
        root.style.borderRightColor = GregUITheme.NeutralBorder;
        root.style.borderTopColor = GregUITheme.NeutralBorder;
        root.style.borderBottomColor = GregUITheme.NeutralBorder;
        root.style.paddingLeft = GregUITheme.Padding;
        root.style.paddingRight = GregUITheme.Padding;
        root.style.paddingTop = 12f;
        root.style.paddingBottom = 12f;
        root.style.display = DisplayStyle.None;

        var header = new Label(string.IsNullOrEmpty(title) ? MenuId.ToUpper() : title.ToUpper());
        try { GregUITheme.ApplyTextStyle(header, true); } catch { }
        root.Add(header);
        _dragHandle = header;

        var content = new VisualElement();
        content.name = "Content";
        content.style.flexGrow = 1f;
        root.Add(content);

        layer.Add(root);
        Root = root;
        Content = content;
        SnapPosition();
    }
}
