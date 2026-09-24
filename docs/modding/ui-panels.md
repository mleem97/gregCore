# UI-Panels mit UIToolkit (statt IMGUI)

> Verifizierte Panel-Bauweise der Team-Mods (Backplanes, MusicPlayer, Trainer).
> **Nicht** IMGUI (`OnGUI`/`GUILayout`) für Spielfenster nutzen: Im IL2CPP-Build ist
> IMGUI teils gestrippt (`Method unstripping failed`), es gibt kein EventSystem, und
> der Toolkit-Default-Font rendert unsichtbar.

## 1. Toggle-Pattern (Vorlage)

```csharp
public static bool IsVisible => _chrome != null && _chrome.IsVisible;

public static void Toggle()
{
    try
    {
        if (_chrome == null) _chrome = PanelChromeFactory.Create();
        if (_chrome == null) return;
        _chrome.Configure(false, 440f);
        bool willShow = !_chrome.IsVisible;
        if (willShow) Rebuild();   // Inhalt nur beim Öffnen neu aufbauen
        _chrome.Toggle();
        try { if (GregHost.HasCore) ReportOpenState(); } catch { }
    }
    catch (Exception ex) { Log.Error("Panel toggle failed: " + ex.Message); }
}
```

Dazu: `Refresh()` (Live-Status, vom Mod alle ~2 s nur wenn `IsVisible`),
`RouteClicks()` pro Frame (Fallback, siehe 3), `Rebuild()` baut Labels/Buttons neu.

## 2. GregPanelBuilder (Framework-Baukasten)

`gregCore.UI.GregPanelBuilder.Create("Titel").SetSize(500, 600).Build()` hängt ein
Panel in den Panel-Layer (initial versteckt, Game-Font automatisch). Wichtigste
Methoden: `Show()/Hide()/Toggle()`, `IsVisible`, `Root`, `ContentContainer`,
`AddHeadline/AddLabel/AddButton/AddSecondaryButton/AddToggle/AddSwitch/
AddSlider/AddDropdown/AddInputField/AddSpacer/AddSeparator/ClearContent`.

Eigene `VisualElement`-Bäume (wie Backplanes/MusicPlayer mit `PanelChrome`) gehen
genauso — dann selbst um Font + Click-Routing kümmern (3 + 4).

## 3. Klick-Routing (Pflicht ohne EventSystem)

Echte `ClickEvent`-Callbacks **immer** explizit registrieren:

```csharp
btn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ =>
{
    try { _lastRealClickUtc = DateTime.UtcNow; action?.Invoke(); } catch { }
}));
```

Plus manuelles Fallback pro Frame (`worldBound.Contains(pos)`, 500-ms-Doppelfeuer-Schutz),
weil ohne EventSystem sonst Klicks verloren gehen. Muster: `BackplanesOverlay.RouteClicks()`,
Framework-seitig `GregClickRouter` (+ `GregModHub.PollClicks()`).

## 4. Font (Pflicht)

```csharp
Font f = GregHost.HasCore
    ? gregCore.UI.GregFontLoader.DefaultUGUIFont   // null-tolerant nutzen!
    : ModLocalUI.ResolveFont();                    // mod-lokaler Fallback
label.style.unityFont = f;
```

Ohne Spiel-Font bleibt Toolkit-Text unsichtbar. `DefaultUGUIFont` kann null sein —
immer null-tolerant.

## 5. Input-Lock & Pause-Menü

- Mit gregCore: Menü-Optionen (`LockCamera/Movement/Interact`, `ShowCursor`) +
  `SetOpen` melden (siehe [Greg-Vertrag](gregcore-vertrag.md)) — das Framework
  sperrt/entsperrt ref-counted.
- Toggle-Taste ignorieren, solange ein Pause-/Settings-Canvas offen ist
  (Canvas-Namenscheck: `Pause`, `EscapeMenu`, `OptionsMenu`, …).
- Escape-Verhalten selbst regeln (Overlay schließen, nicht ans Spiel durchreichen).
