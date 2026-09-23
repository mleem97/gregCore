/// <file-summary>
/// Schicht:      UI
/// Zweck:        Zentrales Mod-Hub (F1). Listet alle registrierten Mods und
///               Menues mit Oeffnen/Schliessen-Knoepfen (ueber Opener).
///               Rendert in den Dialog-Layer. Klicks: RegisterCallback +
///               GregClickRouter-Fallback.
/// </file-summary>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Runtime UI over live game UIDocument; needs running game.")]
public static class GregModHub
{
    public const string MenuId = "greg.hub";
    private const string OverlayName = "greg-hub";
    private const string BodyName = "greg-hub-body";

    private static bool _open;
    private static VisualElement _overlay;
    private static VisualElement _body;
    private static float _lastRebuild = -10f;
    private static readonly List<GregClickRouter.Clickable> _clickables = new List<GregClickRouter.Clickable>();
    private static System.DateTime _lastRealClickUtc = System.DateTime.MinValue;

    public static bool IsOpen => _open;

    public static void Toggle()
    {
        if (_open) Close();
        else Open();
    }

    public static void Open()
    {
        try
        {
            VisualElement layer = null;
            try { layer = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Dialog); } catch { }
            if (layer == null) return;
            EnsureOverlay(layer);
            if (_overlay == null) return;
            _overlay.style.display = DisplayStyle.Flex;
            _open = true;
            GregMenuRegistry.SetOpen(MenuId, true);
            try { GregInputLock.Refresh(); } catch { }
            Rebuild();
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Hub-Oeffnen fehlgeschlagen: {ex.Message}");
        }
    }

    public static void Close()
    {
        try
        {
            if (_overlay != null) _overlay.style.display = DisplayStyle.None;
        }
        catch { }
        _open = false;
        GregMenuRegistry.SetOpen(MenuId, false);
        try { GregInputLock.Refresh(); } catch { }
    }

    private static void EnsureOverlay(VisualElement layer)
    {
        _overlay = layer.Q<VisualElement>(OverlayName);
        Font font = null;
        try { font = GregFontLoader.DefaultUGUIFont; } catch { }
        if (_overlay != null) return;

        _overlay = new VisualElement { name = OverlayName };
        _overlay.style.position = Position.Absolute;
        _overlay.style.left = 0; _overlay.style.top = 0;
        _overlay.style.right = 0; _overlay.style.bottom = 0;
        _overlay.style.backgroundColor = new Color(0f, 0f, 0f, 0.45f);
        _overlay.style.alignItems = Align.Center;
        _overlay.style.justifyContent = Justify.Center;
        _overlay.style.display = DisplayStyle.None;
        layer.Add(_overlay);

        var card = new VisualElement();
        card.style.width = 420;
        card.style.maxHeight = 560;
        card.style.backgroundColor = new Color(0.03f, 0.10f, 0.14f, 0.97f);
        card.style.borderTopLeftRadius = 8;
        card.style.borderTopRightRadius = 8;
        card.style.borderBottomLeftRadius = 8;
        card.style.borderBottomRightRadius = 8;
        card.style.paddingLeft = 16; card.style.paddingRight = 16;
        card.style.paddingTop = 14; card.style.paddingBottom = 14;
        _overlay.Add(card);

        var title = new Label("GregFramework — Mods  (F1)");
        title.style.color = new Color(0.53f, 0.81f, 0.92f, 1f);
        title.style.fontSize = 17;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.marginBottom = 8;
        if (font != null) { try { title.style.unityFont = font; } catch { } }
        card.Add(title);

        var scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.style.flexGrow = 1;
        scroll.style.minHeight = 200;
        card.Add(scroll);

        _body = new VisualElement { name = BodyName };
        _body.style.flexDirection = FlexDirection.Column;
        scroll.Add(_body);
    }

    public static void Refresh()
    {
        if (_open && Time.realtimeSinceStartup - _lastRebuild > 1.0f) Rebuild();
    }

    // Pro Frame aus dem Menue-Tick: manuelles Klick-Routing (Fallback falls
    // kein EventSystem zustellt). Echte Callbacks haben via Zeitstempel Vorrang.
    public static void PollClicks()
    {
        if (!_open || _clickables.Count == 0) return;
        try { GregClickRouter.RouteClicks(_clickables, ref _lastRealClickUtc); } catch { }
    }

    private static void Rebuild()
    {
        _lastRebuild = Time.realtimeSinceStartup;
        if (_body == null) return;
        try
        {
            _body.Clear();
            _clickables.Clear();
            Font font = null;
            try { font = GregFontLoader.DefaultUGUIFont; } catch { }
            var mods = Core.Mods.GregModRegistry.All().ToList();
            var menus = GregMenuRegistry.Snapshot();

            var byMod = new Dictionary<string, List<GregMenuRegistry.MenuInfo>>();
            var orphan = new List<GregMenuRegistry.MenuInfo>();
            foreach (var m in menus)
            {
                if (m.MenuId == MenuId) continue;
                // Framework-interne Menues (z.B. greg.console) sind per Taste
                // bedienbar und gehoeren nicht in die Hub-Liste.
                if (m.MenuId != null && m.MenuId.StartsWith("greg.",
                    System.StringComparison.OrdinalIgnoreCase)) continue;
                string owner = null;
                foreach (var mod in mods)
                {
                    if (mod == null || mod.Menus == null) continue;
                    if (System.Array.Exists(mod.Menus, id =>
                        string.Equals(id, m.MenuId, System.StringComparison.OrdinalIgnoreCase)))
                    { owner = mod.Name; break; }
                }
                if (owner == null) orphan.Add(m);
                else
                {
                    if (!byMod.TryGetValue(owner, out var l)) { l = new List<GregMenuRegistry.MenuInfo>(); byMod[owner] = l; }
                    l.Add(m);
                }
            }

            foreach (var mod in mods.OrderBy(x => x != null ? x.Name : string.Empty))
            {
                if (mod == null) continue;
                var row = Row(font, $"{mod.Name}  v{mod.Version}", null, null);
                row.style.marginBottom = 2;
                _body.Add(row);
                if (byMod.TryGetValue(mod.Name, out var ml))
                    foreach (var m in ml)
                    {
                        var r = MenuRow(font, m);
                        r.style.marginBottom = 6;
                        _body.Add(r);
                    }
                else
                {
                    var pad = new VisualElement();
                    pad.style.height = 4;
                    _body.Add(pad);
                }
            }
            foreach (var m in orphan)
            {
                var r = MenuRow(font, m);
                r.style.marginBottom = 6;
                _body.Add(r);
            }

            if (_body.childCount == 0)
            {
                var empty = new Label("Keine Mods registriert.");
                empty.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                empty.style.fontSize = 13;
                if (font != null) { try { empty.style.unityFont = font; } catch { } }
                _body.Add(empty);
            }
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] Hub-Rebuild fehlgeschlagen: {ex.Message}");
        }
    }

    private static VisualElement MenuRow(Font font, GregMenuRegistry.MenuInfo m)
    {
        string captured = m.MenuId;
        // Ehrliche Buttons: "Schliessen" nur mit registriertem Closer (und nur
        // dann ist auch der Offen-Status verlaesslich). Sonst "Oeffnen" ohne
        // Status-Anzeige statt gelogenem "Zu".
        if (m.HasCloser)
        {
            if (m.Open)
                return Row(font, $"  {m.MenuId}  [Offen]", "Schliessen",
                    () => { GregMenuRegistry.TryClose(captured); Rebuild(); });
            if (m.HasOpener)
                return Row(font, $"  {m.MenuId}  [Zu]", "Oeffnen",
                    () => { GregMenuRegistry.TryOpen(captured); Rebuild(); });
            return Row(font, $"  {m.MenuId}  [Zu]", null, null);
        }
        if (m.HasOpener)
            return Row(font, $"  {m.MenuId}", "Oeffnen",
                () => { GregMenuRegistry.TryOpen(captured); Rebuild(); });
        return Row(font, $"  {m.MenuId}", null, null);
    }

    private static VisualElement Row(Font font, string text, string buttonLabel, System.Action onClick)
    {
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.alignItems = Align.Center;
        row.style.justifyContent = Justify.SpaceBetween;

        var lab = new Label(text);
        lab.style.color = Color.white;
        lab.style.fontSize = 13;
        lab.style.flexGrow = 1;
        lab.style.marginRight = 8;
        if (font != null) { try { lab.style.unityFont = font; } catch { } }
        row.Add(lab);

        if (buttonLabel != null && onClick != null)
        {
            var btn = new Button();
            btn.text = buttonLabel;
            btn.style.backgroundColor = new Color(0.04f, 0.51f, 0.63f, 1f);
            btn.style.color = Color.white;
            btn.style.fontSize = 12;
            if (font != null) { try { btn.style.unityFont = font; } catch { } }
            btn.RegisterCallback<ClickEvent>(new System.Action<ClickEvent>(_ =>
            {
                try
                {
                    GregClickRouter.MarkRealClick(ref _lastRealClickUtc);
                    onClick();
                }
                catch { }
            }));
            _clickables.Add(new GregClickRouter.Clickable { Element = btn, Action = onClick });
            row.Add(btn);
        }
        return row;
    }
}
