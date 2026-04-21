---
title: "F8 Settings Hub"
description: "How to use and extend the F8 Settings Hub"
---

# F8 Settings Hub

## Was ist das F8 Settings Hub?
Der F8 Hub ist das zentrale Mod-Konfigurationsmenü für alle gregCore-Mods. Es basiert auf dem `GregUIBuilder` (OnGUI) und nutzt das Luminescent Architect Design System. Kein Mod sollte mehr ein eigenes, abgetrenntes Fenster für Einstellungen bauen.

## Standard-Tabs
- **[Framework]**: Zeigt Core-Infos und Debug-Toggles.
- **[Grid]**: Einstellungen für das `greg.GridPlacement`.
- **[SaveEngine]**: Einstellungen für `greg.SaveEngine`.
- **[Languages]**: Status der registrierten Skriptsprachen.
- **[Debug]**: Diagnose.

## Modder: Eigenen Tab registrieren

```csharp
// Eigenen Tab im F8-Menü registrieren
GregSettingsHub.RegisterTab("myMod.settings", "My Mod", (builder) => {
    builder.AddLabel("My Mod v1.0.0")
           .AddToggle("Enable Feature X", config.FeatureX, v => config.FeatureX = v)
           .AddSlider("Speed", 0.1f, 5f, config.Speed, v => config.Speed = v)
           .AddButton("Reset to Defaults", ResetConfig);
});

// Tab beim Mod-Shutdown entfernen
GregSettingsHub.UnregisterTab("myMod.settings");
```

## Vanilla-Save-Banner
Wird ein Vanilla-Save geladen, erscheint im F8-Menü ein Warnbanner `Vanilla Save detected — Grid Placement disabled`. Dies signalisiert dem User, dass Game-Breaking-Features deaktiviert wurden, um den Vanilla-Save nicht zu korrumpieren.
