using System;
using System.Collections.Generic;
using UnityEngine;

namespace greg.Core.UI.Components;

public class GregUIBuilder
{
    private string   _panelId;
    private string   _title       = string.Empty;
    private Vector2  _size        = new(400, 300);
    private GregUIAnchor _anchor  = GregUIAnchor.Center;
    private bool     _draggable   = true;
    private bool     _closable    = true;

    private readonly List<IGregUIComponent> _components = new();

    private GregUIBuilder(string panelId) => _panelId = panelId;

    public static GregUIBuilder Panel(string panelId) => new(panelId);

    public GregUIBuilder Title(string title)          { _title   = title;  return this; }
    public GregUIBuilder Size(float w, float h)       { _size    = new(w, h); return this; }
    public GregUIBuilder Position(GregUIAnchor anchor){ _anchor  = anchor; return this; }
    public GregUIBuilder Draggable(bool v = true)     { _draggable = v;   return this; }
    public GregUIBuilder Closable(bool v = true)      { _closable  = v;   return this; }

    public GregUIBuilder AddButton(string label, Action onClick, GregButtonStyle style = GregButtonStyle.Primary)
    {
        _components.Add(new GregButton { Label = label, OnClick = onClick, Style = style });
        return this;
    }

    public GregUIBuilder AddLabel(string text, Color? color = null, float fontSize = 12)
    {
        _components.Add(new GregLabel { Text = text, Color = color ?? GregUITheme.OnSurface, FontSize = fontSize });
        return this;
    }

    public GregUIBuilder AddToggle(string label, bool initial, Action<bool> onChange)
    {
        _components.Add(new GregToggle { Label = label, Value = initial, OnChange = onChange });
        return this;
    }

    public GregUIBuilder AddSeparator()
    {
        _components.Add(new GregSeparator());
        return this;
    }

    public GregPanel Build()
    {
        var panel = new GregPanel
        {
            PanelId    = _panelId,
            Title      = _title,
            Size       = _size,
            Anchor     = _anchor,
            Draggable  = _draggable,
            Closable   = _closable,
            Components = _components,
        };
        GregUIManager.Register(_panelId, panel);
        return panel;
    }
}

