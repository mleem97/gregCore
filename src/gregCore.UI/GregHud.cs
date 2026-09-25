/// <file-summary>
/// Layer:      UI
/// Purpose:     Key HUD on the right edge (like the game hints below).
///              Display only (no input), built from GregHudRegistry.
///              Renders into the HUD layer (GregUILayerManager).
/// </file-summary>

using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Runtime UI over live game UIDocument; needs running game.")]
public static class GregHud
{
    private const string ContainerName = "greg-hud";
    private static VisualElement _container;
    private static float _fontRetryAt;

    public static void Refresh()
    {
        try
        {
            VisualElement layer = null;
            try { layer = GregUILayerManager.Instance.GetLayerRoot(GregUILayerType.HUD); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (layer == null) return;

            EnsureContainer(layer);
            _container.Clear();

            Font font = ResolveFont();
            foreach (var e in GregHudRegistry.All())
            {
                if (e == null) continue;
                _container.Add(BuildLine(e, font));
            }
            _container.style.display = _container.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] HUD-Refresh failed: {ex.Message}");
        }
    }

    private static void EnsureContainer(VisualElement layer)
    {
        _container = layer.Q<VisualElement>(ContainerName);
        if (_container != null) return;
        _container = new VisualElement { name = ContainerName };
        _container.style.position = Position.Absolute;
        _container.style.right = 8;
        _container.style.top = 64;
        _container.style.flexDirection = FlexDirection.Column;
        _container.style.alignItems = Align.FlexEnd;
        _container.pickingMode = PickingMode.Ignore;
        layer.Add(_container);
    }

    private static Font ResolveFont()
    {
        Font font = null;
        try { font = GregFontLoader.DefaultUGUIFont; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        if (font == null && Time.realtimeSinceStartup > _fontRetryAt)
            _fontRetryAt = Time.realtimeSinceStartup + 2f;
        return font;
    }

    private static VisualElement BuildLine(GregHudRegistry.Entry e, Font font)
    {
        var line = new VisualElement();
        line.style.flexDirection = FlexDirection.Row;
        line.style.alignItems = Align.Center;
        line.style.backgroundColor = new Color(0.03f, 0.10f, 0.14f, 0.78f);
        line.style.paddingLeft = 7;
        line.style.paddingRight = 7;
        line.style.paddingTop = 2;
        line.style.paddingBottom = 2;
        line.style.marginBottom = 3;
        line.style.borderTopLeftRadius = 3;
        line.style.borderTopRightRadius = 3;
        line.style.borderBottomLeftRadius = 3;
        line.style.borderBottomRightRadius = 3;
        line.pickingMode = PickingMode.Ignore;

        var key = new Label(e.Key ?? string.Empty);
        key.style.color = new Color(0.53f, 0.81f, 0.92f, 1f);
        key.style.fontSize = 13;
        key.style.unityFontStyleAndWeight = FontStyle.Bold;
        key.style.marginRight = 6;
        key.pickingMode = PickingMode.Ignore;

        var lab = new Label(e.Label ?? string.Empty);
        lab.style.color = Color.white;
        lab.style.fontSize = 12;
        lab.pickingMode = PickingMode.Ignore;

        if (font != null)
        {
            try
            {
                key.style.unityFont = font;
                lab.style.unityFont = font;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }

        line.Add(key);
        line.Add(lab);
        return line;
    }

    // From the menu tick: ensure container + font retry (max. every 2s).
    public static void Tick()
    {
        try
        {
            if (_container == null)
            {
                if (GregHudRegistry.All().Count > 0) Refresh();
                return;
            }
            if (Time.realtimeSinceStartup >= _fontRetryAt)
            {
                Font font = null;
                try { font = GregFontLoader.DefaultUGUIFont; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                if (font != null)
                {
                    _fontRetryAt = Time.realtimeSinceStartup + 30f;
                    Refresh();
                }
                else
                {
                    _fontRetryAt = Time.realtimeSinceStartup + 2f;
                }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
