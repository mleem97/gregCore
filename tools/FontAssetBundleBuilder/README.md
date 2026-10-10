# gregCore Font AssetBundle Builder

This tool builds an AssetBundle with font assets for GregCore / UI Toolkit.

## Prerequisites

- Unity 6000.4.x (or 2022.3+ with UI Toolkit)
- The `.ttf` files live in `StreamingAssets/Fonts/`

## Step by step

### 1. Open or create a Unity project

Create a new folder for the build project (e.g. `gregcore-font-builder/`).

### 2. Add the editor script

Copy `Editor/BuildGregCoreFontBundle.cs` into the Unity project under:
```
Assets/Editor/BuildGregCoreFontBundle.cs
```

### 3. Import font files

Copy the `.ttf` files (Inter, Inter Tight, Space Grotesk) into:
```
Assets/Fonts/
```

### 4. Build the AssetBundle

In the Unity Editor:
1. **Window → gregCore → Build Font Bundle**
2. Or via menu: **Assets → Build gregCore Font Bundle**

The bundle is created at:
```
Assets/StreamingAssets/Fonts/gregcore_fonts.bundle
```

### 5. Copy the bundle into the game

Copy the built file into the game directory:
```
<Data Center>/StreamingAssets/Fonts/gregcore_fonts.bundle
```

## Bundle contents

The bundle contains the fonts as `FontAsset` (SDF atlas) for UI Toolkit:
- **Inter** → `Inter`
- **Inter-Italic** → `Inter-Italic`
- **Inter Tight** → `Inter-Tight`
- **Inter Tight Italic** → `Inter-Tight-Italic`
- **Space Grotesk** → `Space-Grotesk`

## Fallback

If the bundle is not found, GregCore uses system fonts (Arial, Segoe UI).

## Important

- Do **not** copy the `.ttf` files directly into the game — they must exist as `FontAsset` inside a bundle
- UI Toolkit under IL2CPP cannot load `.ttf` files at runtime
- Variable fonts import in Unity as regular font assets (axis control is currently unavailable)
