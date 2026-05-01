# Font-Pack Integration Guide for GregCore Mods (IL2CPP / MelonLoader)

**Context:** Unity 6000.4+ | IL2CPP | MelonLoader 0.7+ | Blackbox Modding (no game source access)
**Purpose:** Add custom fonts to your mod that work at runtime via StreamingAssets + UGUI

---

## Overview

Since you do **not** have access to the game's Unity project, you create a **separate Unity project** to prepare font assets. The runtime mod loads `.ttf`/`.otf` files directly from `StreamingAssets/Fonts/` — no Editor baking required.

**Why `.ttf` and not SDF Font Assets?**
- UI Toolkit SDF Font Assets (`.asset`) can **only** be generated in the Unity Editor
- IL2CPP at runtime cannot generate SDF atlases
- UGUI `UnityEngine.UI.Text` accepts raw `.ttf` files via `new Font(path)`

---

## Step 1: Create a Unity Asset Project

1. Install **Unity 6000.4.x** (match the game's Unity version exactly)
2. Create a new 3D project: `GregMod_Assets`
3. In `Edit → Project Settings → Player`:
   - **Color Space:** Match the game (usually Linear)
   - **Api Compatibility:** .NET Standard 2.1

---

## Step 2: Import Google Fonts

### Download Fonts
1. Go to [fonts.google.com](https://fonts.google.com)
2. Select fonts (recommendation for a starter pack):
   - **Inter** — UI / body text
   - **JetBrains Mono** — monospace / logs
   - **Oswald** — headlines
   - **Roboto** — general UI fallback
3. Click "Get font" → Download
4. Unzip the downloaded files

### Import into Unity
1. In your Unity project, create folder: `Assets/Fonts/`
2. Copy the `.ttf` files into `Assets/Fonts/`
3. Unity auto-imports them as `TrueTypeFontImporter`

### Configure for Runtime
For each imported font:
1. Select the `.ttf` in the Project window
2. In the Inspector:
   - **Font Size:** 16 (default, can be overridden at runtime)
   - **Rendering Mode:** Smooth
   - **Character:** Unicode (includes all glyphs)
   - **Include Font Data:** ✅ **ENABLED** (critical!)
   - **Ascent Calculation Mode:** Legacy 2.0
3. Click **Apply**

> ⚠️ **CRITICAL:** If "Include Font Data" is disabled, the font will not work when loaded via `new Font(path)` at runtime.

---

## Step 3: Prepare the StreamingAssets Structure

Your mod will load fonts from the game's `StreamingAssets` folder at runtime. You need to prepare this structure.

### Option A: Direct File Copy (Recommended)

Create this folder structure **outside** Unity (e.g., in your mod repo):

```
GregMod.FontPack/
├── Fonts/
│   ├── Inter-Regular.ttf
│   ├── Inter-Bold.ttf
│   ├── JetBrainsMono-Regular.ttf
│   ├── Oswald-Bold.ttf
│   └── Roboto-Regular.ttf
└── manifest.json
```

### Option B: AssetBundle (Advanced)

If you want Unity to compress/manage fonts:

1. In Unity, select all font files
2. At the bottom of the Inspector, click **AssetBundle → New**
3. Name it: `gregmod_fonts`
4. Build the bundle:
   ```csharp
   // Editor script: Assets/Editor/BuildFontBundle.cs
   using UnityEditor;
   using System.IO;

   public class BuildFontBundle
   {
       [MenuItem("GregMod/Build Font Bundle")]
       static void Build()
       {
           string outDir = "../GameFramework-Monorepo/GregMods/gregMod.MyMod/Assets";
           Directory.CreateDirectory(outDir);

           BuildPipeline.BuildAssetBundles(
               outDir,
               BuildAssetBundleOptions.None,
               BuildTarget.StandaloneWindows64
           );
       }
   }
   ```
5. The output `gregmod_fonts` file goes into your mod's distribution

> **Note:** For IL2CPP mods, direct `.ttf` files are simpler and more reliable than AssetBundles. Use AssetBundles only if you have many assets beyond fonts.

---

## Step 4: Deploy to Game Directory

Your mod's installation script (or manual install) must copy fonts to:

```
C:\Program Files (x86)\Steam\steamapps\common\Data Center\StreamingAssets\Fonts\
```

### PowerShell Deploy Script (add to your build pipeline)

```powershell
$gamePath = "C:\Program Files (x86)\Steam\steamapps\common\Data Center"
$modFonts = ".\Fonts"

$targetDir = Join-Path $gamePath "StreamingAssets\Fonts"
New-Item -ItemType Directory -Force -Path $targetDir | Out-Null

Copy-Item "$modFonts\*.ttf" $targetDir -Force
Write-Host "Fonts deployed to $targetDir"
```

---

## Step 5: Runtime Font Loader (C# Code)

Add this to your mod (e.g., `GregCore/src/UI/GregFontLoader.cs`):

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;

namespace gregCore.UI
{
    public static class GregFontLoader
    {
        private static readonly Dictionary<string, Font> _cache = new();
        private static readonly string FontDir;

        static GregFontLoader()
        {
            FontDir = Path.Combine(
                Application.streamingAssetsPath,
                "Fonts"
            );
        }

        /// <summary>Load a .ttf font by name (without extension).</summary>
        public static Font Load(string fontName)
        {
            if (_cache.TryGetValue(fontName, out var cached))
                return cached;

            try
            {
                // 1. Try StreamingAssets/Fonts/{name}.ttf
                string ttfPath = Path.Combine(FontDir, $"{fontName}.ttf");
                if (File.Exists(ttfPath))
                {
                    var font = new Font(ttfPath);
                    _cache[fontName] = font;
                    MelonLogger.Msg($"[FontLoader] Loaded '{fontName}' from StreamingAssets");
                    return font;
                }

                // 2. Try .otf variant
                string otfPath = Path.Combine(FontDir, $"{fontName}.otf");
                if (File.Exists(otfPath))
                {
                    var font = new Font(otfPath);
                    _cache[fontName] = font;
                    MelonLogger.Msg($"[FontLoader] Loaded '{fontName}' (.otf) from StreamingAssets");
                    return font;
                }

                // 3. Fallback: OS system font
                var sysFont = Font.CreateDynamicFontFromOSFont(fontName, 16);
                if (sysFont != null)
                {
                    _cache[fontName] = sysFont;
                    MelonLogger.Warning($"[FontLoader] '{fontName}' not found in StreamingAssets, using OS fallback");
                    return sysFont;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[FontLoader] Failed to load '{fontName}': {ex.Message}");
            }

            // 4. Ultimate fallback: Arial (guaranteed to exist)
            MelonLogger.Error($"[FontLoader] All fallbacks failed for '{fontName}', using Arial");
            return Font.CreateDynamicFontFromOSFont("Arial", 16);
        }

        /// <summary>Apply a loaded font to a UGUI Text component.</summary>
        public static void Apply(Text textComponent, string fontName, int fontSize = 14)
        {
            if (textComponent == null) return;
            var font = Load(fontName);
            if (font != null)
            {
                textComponent.font = font;
                textComponent.fontSize = fontSize;
            }
        }

        public static bool HasFont(string fontName)
        {
            if (_cache.ContainsKey(fontName)) return true;
            return File.Exists(Path.Combine(FontDir, $"{fontName}.ttf"))
                || File.Exists(Path.Combine(FontDir, $"{fontName}.otf"));
        }
    }
}
```

---

## Step 6: Apply Fonts in Your Mod

### In HexViewerHud (UGUI Text)

Replace the `CreateLabel` method's font assignment:

```csharp
private static Text CreateLabel(Transform parent, string name, int fontSize,
    FontStyle style, Color color, TextAnchor align)
{
    var go = new GameObject(name);
    go.transform.SetParent(parent, false);
    go.AddComponent<RectTransform>();
    var txt = go.AddComponent<Text>();

    // Apply custom font
    txt.font = GregFontLoader.Load("Inter-Regular");
    txt.fontSize = fontSize;
    txt.fontStyle = style;
    txt.color = color;
    txt.alignment = align;
    txt.text = "—";
    return txt;
}
```

### In GregSettingsHub (after UGUI migration)

```csharp
// After creating the Text component:
var titleText = titleGo.AddComponent<Text>();
GregFontLoader.Apply(titleText, "Oswald-Bold", 20);
```

---

## Step 7: Build & Distribute

### Project Structure in Your Mod Repo

```
GameFramework-Monorepo/
├── GregMods/
│   └── gregMod.MyMod/
│       ├── Assets/
│       │   └── Fonts/
│       │       ├── Inter-Regular.ttf
│       │       └── ...
│       ├── MyMod.cs
│       └── MyMod.csproj
└── Tools/
    └── deploy-fonts.ps1
```

### Build Steps

1. **Build the mod DLL:**
   ```bash
   dotnet build gregMod.MyMod.csproj -c Release
   ```

2. **Deploy DLL + Fonts:**
   ```powershell
   # deploy.ps1
   $game = "C:\Program Files (x86)\Steam\steamapps\common\Data Center"

   # Copy mod DLL
   Copy-Item "bin\Release\net6.0\gregMod.MyMod.dll" "$game\Mods\" -Force

   # Copy fonts
   $fontDir = "$game\StreamingAssets\Fonts"
   New-Item -ItemType Directory -Force -Path $fontDir
   Copy-Item "Assets\Fonts\*.ttf" $fontDir -Force
   ```

---

## Compliance Checklist (AGENTS.md)

- [ ] Font files are `.ttf` or `.otf` (NOT `.asset` SDF fonts)
- [ ] Fonts placed in `StreamingAssets/Fonts/` (NOT in built assets)
- [ ] `Include Font Data` was enabled when exporting from Unity (if Unity was used)
- [ ] Runtime loader uses `new Font(path)` with full path via `Application.streamingAssetsPath`
- [ ] OS fallback via `Font.CreateDynamicFontFromOSFont()` implemented
- [ ] No Unity Editor-only APIs used at runtime (e.g., no `FontAsset.CreateFontAsset`)

---

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| Text invisible / squares | Wrong font path | Check `Application.streamingAssetsPath` at runtime |
| Font looks pixelated | Font size too small | Use font size ≥ 14 or enable `Best Fit` on Text |
| Font not found | File not deployed | Verify `.ttf` exists in `StreamingAssets/Fonts/` |
| Chinese/Japanese missing | Font lacks glyphs | Use a CJK font (e.g., Noto Sans CJK) |

---

## Recommended Starter Font Pack

| Font | Use Case | File Size |
|------|----------|-----------|
| Inter-Regular.ttf | Body text, labels | ~250 KB |
| Inter-Bold.ttf | Headlines, buttons | ~260 KB |
| JetBrainsMono-Regular.ttf | Monospace / logs | ~110 KB |
| Oswald-Bold.ttf | Big headlines | ~40 KB |
| NotoSansCJK-Regular.ttc | Chinese/Japanese/Korean | ~17 MB |

**Total (without CJK):** ~660 KB — perfectly fine to ship with your mod.

---

**Last Updated:** 2026-04-30
**Applies to:** Unity 6000.4+ IL2CPP | MelonLoader 0.7+ | GregCore 1.1.0+
