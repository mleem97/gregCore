# gregCore Font AssetBundle Builder

Dieses Tool erzeugt ein AssetBundle mit FontAssets für GregCore / UI Toolkit.

## Voraussetzungen

- Unity 6000.4.x (oder 2022.3+ mit UI Toolkit)
- Die `.ttf` Dateien liegen in `StreamingAssets/Fonts/`

## Schritt-für-Schritt

### 1. Unity Projekt öffnen oder erstellen

Erstelle einen neuen Ordner für das Build-Projekt (z.B. `gregcore-font-builder/`).

### 2. Editor-Skript anlegen

Kopiere `Editor/BuildGregCoreFontBundle.cs` in das Unity-Projekt unter:
```
Assets/Editor/BuildGregCoreFontBundle.cs
```

### 3. Font-Dateien importieren

Kopiere die `.ttf` Dateien (Inter, Inter Tight, Space Grotesk) in:
```
Assets/Fonts/
```

### 4. AssetBundle bauen

Im Unity Editor:
1. **Window → gregCore → Build Font Bundle**
2. Oder über Menu: **Assets → Build gregCore Font Bundle**

Das Bundle wird erzeugt unter:
```
Assets/StreamingAssets/Fonts/gregcore_fonts.bundle
```

### 5. Bundle ins Spiel kopieren

Kopiere die erzeugte Datei in das Spielverzeichnis:
```
<Data Center>/StreamingAssets/Fonts/gregcore_fonts.bundle
```

## Bundle-Inhalt

Das Bundle enthält die Fonts als `FontAsset` (SDF-Atlas) für UI Toolkit:
- **Inter** → `Inter`
- **Inter-Italic** → `Inter-Italic`
- **Inter Tight** → `Inter-Tight`
- **Inter Tight Italic** → `Inter-Tight-Italic`
- **Space Grotesk** → `Space-Grotesk`

## Fallback

Wenn das Bundle nicht gefunden wird, verwendet GregCore System-Fonts (Arial, Segoe UI).

## Wichtig

- **Nicht** die `.ttf` Dateien direkt ins Spiel kopieren — sie müssen als `FontAsset` in einem Bundle vorliegen
- UI Toolkit in IL2CPP kann keine `.ttf` Dateien zur Laufzeit laden
- Variable Fonts werden in Unity als reguläre FontAssets importiert (die Achsen-Steuerung ist zur Zeit nicht verfügbar)
