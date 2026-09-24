# UI Panels with UIToolkit (not IMGUI)

> Verified panel construction of the team mods (Backplanes, MusicPlayer, Trainer).
> Do **not** use IMGUI (`OnGUI`/`GUILayout`) for game windows: in IL2CPP builds
> IMGUI is partly stripped (`Method unstripping failed`), there is no EventSystem,
> and the toolkit default font renders invisible.

## 1. Toggle pattern (template)

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
        if (willShow) Rebuild();   // rebuild content only when opening
        _chrome.Toggle();
        try { if (GregHost.HasCore) ReportOpenState(); } catch { }
    }
    catch (Exception ex) { Log.Error("Panel toggle failed: " + ex.Message); }
}
```

Plus: `Refresh()` (live status, polled by the mod ~every 2 s only while `IsVisible`),
`RouteClicks()` per frame (fallback, see 3), `Rebuild()` rebuilds labels/buttons.

## 2. GregPanelBuilder (framework kit)

`gregCore.UI.GregPanelBuilder.Create("Title").SetSize(500, 600).Build()` attaches a
panel to the panel layer (initially hidden, game font automatic). Key methods:
`Show()/Hide()/Toggle()`, `IsVisible`, `Root`, `ContentContainer`,
`AddHeadline/AddLabel/AddButton/AddSecondaryButton/AddToggle/AddSwitch/
AddSlider/AddDropdown/AddInputField/AddSpacer/AddSeparator/ClearContent`.

Custom `VisualElement` trees (like Backplanes/MusicPlayer with `PanelChrome`) work
the same — then handle font + click routing yourself (3 + 4).

## 2b. Text input (IL2CPP-safe)

`AddInputField` uses `TextField` → stripped `TextEditor` → hard crash
(`[Obsolete]`). Use `AddSafeInputField(label, value, onChanged,
multiline, maxLength)` instead → `GregSafeTextField` (label rendering + caret +
`Changed` event, fields reachable via `SafeInputFields`). The owning mod
calls `field.PumpFrame()` per frame while focused and manages focus itself
(click-to-focus, tab switching, `FocusField()`/`BlurField()`); `GregKeyPump`
holds backspace-repeat state per field. Reference: gregMod.NotesHUD
(`SafeTextPump` as standalone variant without gregCore.dll).

## 3. Click routing (mandatory without EventSystem)

Always register real `ClickEvent` callbacks explicitly:

```csharp
btn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ =>
{
    try { _lastRealClickUtc = DateTime.UtcNow; action?.Invoke(); } catch { }
}));
```

Plus manual fallback per frame (`worldBound.Contains(pos)`, 500 ms double-fire guard),
because clicks get lost without an EventSystem. Template: `BackplanesOverlay.RouteClicks()`,
framework-side `GregClickRouter` (+ `GregModHub.PollClicks()`).

## 4. Font (mandatory)

```csharp
Font f = GregHost.HasCore
    ? gregCore.UI.GregFontLoader.DefaultUGUIFont   // null-tolerant use!
    : ModLocalUI.ResolveFont();                    // mod-local fallback
label.style.unityFont = f;
```

Without the game font, toolkit text stays invisible. `DefaultUGUIFont` can be null —
always use it null-tolerantly.

## 5. Input lock & pause menu

- With gregCore: menu options (`LockCamera/Movement/Interact`, `ShowCursor`) +
  report `SetOpen` (see [Greg Contract](greg-contract.md)) — the framework
  locks/unlocks ref-counted.
- Ignore the toggle key while a pause/settings canvas is open
  (canvas name check: `Pause`, `EscapeMenu`, `OptionsMenu`, …).
- Handle Escape yourself (close overlay, don't pass through to the game).
