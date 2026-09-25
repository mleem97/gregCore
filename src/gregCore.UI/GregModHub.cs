/// <file-summary>
/// Layer:       UI
/// Purpose:     Central mod hub (F1). Lists all registered mods and
///              menus with Open/Close buttons (via opener).
///              Renders into the dialog layer. Clicks: RegisterCallback +
///              GregClickRouter fallback.
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
    public static string MenuId { get; } = "greg.hub";
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
            try { layer = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.Dialog); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (layer == null) return;
            EnsureOverlay(layer);
            if (_overlay == null) return;
            _overlay.style.display = DisplayStyle.Flex;
            _open = true;
            GregMenuRegistry.SetOpen(MenuId, true);
            try { GregInputLock.Refresh(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            Rebuild();
        }
        catch (System.Exception ex) /* ignored: defensive best-effort (CONVENTIONS.md) */
        {
            MelonLogger.Warning($"[gregCore][UI] Hub open failed: {ex.Message}");
        }
    }

    public static void Close()
    {
        try
        {
            if (_overlay != null) _overlay.style.display = DisplayStyle.None;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        _open = false;
        GregMenuRegistry.SetOpen(MenuId, false);
        try { GregInputLock.Refresh(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void EnsureOverlay(VisualElement layer)
    {
        _overlay = layer.Q<VisualElement>(OverlayName);
        Font font = null;
        try { font = GregFontLoader.DefaultUGUIFont; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
        card.style.maxWidth = new Length(92, LengthUnit.Percent);
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
        if (font != null) { try { title.style.unityFont = font; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
        card.Add(title);

        var hint = new Label("Open/close menus here · Settings (toggles) on F8 or per mod via \"Settings\"");
        hint.style.color = new Color(0.6f, 0.65f, 0.7f, 1f);
        hint.style.fontSize = 11;
        hint.style.marginBottom = 8;
        hint.style.whiteSpace = WhiteSpace.Normal;
        if (font != null) { try { hint.style.unityFont = font; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  } }
        card.Add(hint);

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

    // Per frame from the menu tick: manual click routing (fallback if
    // no EventSystem delivers). Real callbacks take precedence via timestamp.
    public static void PollClicks()
    {
        if (!_open || _clickables.Count == 0) return;
        try { GregClickRouter.RouteClicks(_clickables, ref _lastRealClickUtc); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
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
            try { font = GregFontLoader.DefaultUGUIFont; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            var mods = Core.Mods.GregModRegistry.All().ToList();
            var menus = GregMenuRegistry.Snapshot();

            var byMod = GroupMenusByMod(menus, mods, out var orphan);
            AddModSections(font, mods, byMod);
            AddOrphanMenus(font, orphan);
            AddEmptyNotice(font);
        }
        catch (System.Exception ex) /* ignored: defensive best-effort (CONVENTIONS.md) */
        {
            MelonLogger.Warning($"[gregCore][UI] Hub rebuild failed: {ex.Message}");
        }
    }

    private static Dictionary<string, List<GregMenuRegistry.MenuInfo>> GroupMenusByMod(
        System.Collections.Generic.IReadOnlyList<GregMenuRegistry.MenuInfo> menus, List<Core.Mods.GregModRegistry.Entry> mods,
        out List<GregMenuRegistry.MenuInfo> orphan)
    {
        var byMod = new Dictionary<string, List<GregMenuRegistry.MenuInfo>>();
        orphan = new List<GregMenuRegistry.MenuInfo>();
        foreach (var m in menus)
        {
            if (m.MenuId == MenuId) continue;
            // Framework-internal menus (e.g. greg.console) are operated via hotkey
            // and do not belong in the hub list.
            if (m.MenuId != null && m.MenuId.StartsWith("greg.",
                System.StringComparison.OrdinalIgnoreCase)) continue;
            string owner = FindMenuOwner(mods, m.MenuId);
            if (owner == null) orphan.Add(m);
            else
            {
                if (!byMod.TryGetValue(owner, out var l)) { l = new List<GregMenuRegistry.MenuInfo>(); byMod[owner] = l; }
                l.Add(m);
            }
        }
        return byMod;
    }

    private static string FindMenuOwner(List<Core.Mods.GregModRegistry.Entry> mods, string menuId)
    {
        foreach (var mod in mods)
        {
            if (mod == null || mod.Menus == null) continue;
            if (System.Array.Exists(mod.Menus, id =>
                string.Equals(id, menuId, System.StringComparison.OrdinalIgnoreCase)))
                return mod.Name;
        }
        return null;
    }

    private static void AddModSections(Font font, List<Core.Mods.GregModRegistry.Entry> mods,
        Dictionary<string, List<GregMenuRegistry.MenuInfo>> byMod)
    {
        foreach (var mod in mods.OrderBy(x => x != null ? x.Name : string.Empty))
        {
            if (mod == null) continue;
            var row = Row(font, $"{mod.Name}  v{mod.Version}", null, null);
            row.style.marginBottom = 2;
            _body.Add(row);
            if (byMod.TryGetValue(mod.Name, out var ml))
                foreach (var m in ml)
                {
                    var r = MenuRow(font, m, mod.Name);
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
    }

    private static void AddOrphanMenus(Font font, List<GregMenuRegistry.MenuInfo> orphan)
    {
        foreach (var m in orphan)
        {
            var r = MenuRow(font, m, null);
            r.style.marginBottom = 6;
            _body.Add(r);
        }
    }

    private static void AddEmptyNotice(Font font)
    {
        if (_body.childCount != 0) return;
        var empty = new Label("No mods registered.");
        empty.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        empty.style.fontSize = 13;
        if (font != null) { try { empty.style.unityFont = font; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
        _body.Add(empty);
    }

    private static VisualElement MenuRow(Font font, GregMenuRegistry.MenuInfo m, string ownerModName)
    {
        string captured = m.MenuId;
        string settingsTab = null;
        try { settingsTab = greg.UI.Settings.GregSettingsHub.FindTabForMenu(captured, ownerModName); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        System.Action settingsAction = null;
        if (!string.IsNullOrEmpty(settingsTab))
        {
            string capturedTab = settingsTab;
            settingsAction = () =>
            {
                try { Close(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                try { greg.UI.Settings.GregSettingsHub.ShowTab(capturedTab); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                try { Rebuild(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            };
        }

        // Honest buttons: "Close" only with a registered closer (and only
        // then is the open state reliable). Otherwise "Open" without
        // status display instead of a fake "Closed".
        if (m.HasCloser)
        {
            if (m.Open)
                return Row(font, $"  {m.MenuId}  [Open]", "Close",
                    () => { GregMenuRegistry.TryClose(captured); Rebuild(); }, settingsAction);
            if (m.HasOpener)
                return Row(font, $"  {m.MenuId}  [Closed]", "Open",
                    () => { GregMenuRegistry.TryOpen(captured); Rebuild(); }, settingsAction);
            return Row(font, $"  {m.MenuId}  [Closed]", null, null, settingsAction);
        }
        if (m.HasOpener)
            return Row(font, $"  {m.MenuId}", "Open",
                () => { GregMenuRegistry.TryOpen(captured); Rebuild(); }, settingsAction);
        return Row(font, $"  {m.MenuId}", null, null, settingsAction);
    }

    private static VisualElement Row(Font font, string text, string buttonLabel, System.Action onClick,
        System.Action settingsAction = null)
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
        if (font != null) { try { lab.style.unityFont = font; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ } }
        row.Add(lab);

        if (buttonLabel != null && onClick != null)
        {
            row.Add(MakeButton(font, buttonLabel, onClick));
        }
        if (settingsAction != null)
        {
            row.Add(MakeButton(font, "Settings", settingsAction));
        }
        return row;
    }

    private static Button MakeButton(Font font, string buttonLabel, System.Action onClick)
    {
        var btn = new Button();
        btn.text = buttonLabel;
        btn.style.backgroundColor = new Color(0.04f, 0.51f, 0.63f, 1f);
        btn.style.color = Color.white;
        btn.style.fontSize = 12;
        if (font != null) { try { btn.style.unityFont = font; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  } }
        btn.RegisterCallback<ClickEvent>(new System.Action<ClickEvent>(_ =>
        {
            try
            {
                _lastRealClickUtc = GregClickRouter.MarkRealClick();
                onClick();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        }));
        _clickables.Add(new GregClickRouter.Clickable { Element = btn, Action = onClick });
        return btn;
    }
}
