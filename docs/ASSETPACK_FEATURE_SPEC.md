# AssetPack Import System for GregCore

## Description
Design and implement a runtime asset loading system that allows GregCore mods to import custom AssetBundles (or loose asset packs) at runtime without requiring Unity Editor access. This enables modders to ship textures, fonts, audio, UI skins, and other assets alongside their DLLs.

## Problem Statement
Currently, GregCore mods can only distribute C# DLLs. Any custom assets (fonts, textures, sounds, prefabs) require the modder to either:
- Hardcode everything procedurally (limited flexibility)
- Rely solely on `StreamingAssets` loose files (no compression, no dependency management)
- Require end-users to manually place files in correct folder structures (error-prone)

An **AssetPack Import System** would allow mods to ship `.assetpack` / `.bundle` files that GregCore loads, validates, and exposes via a unified API.

## Goals
- Load compressed asset packs from `Mods/Assets/` or `StreamingAssets/Mods/`
- Support multiple formats: Unity AssetBundles (primary), ZIP archives (fallback), loose folders
- Version validation (pack declares target GregCore version, Unity version, game version)
- Dependency resolution (Pack A requires Pack B)
- Hot-reload support during development
- IL2CPP-safe (no Editor-only APIs at runtime)

## Non-Goals
- Editor-time asset baking (modders use their own Unity project for that)
- Runtime AssetBundle creation (only loading)
- Modifying game assets (read-only import)

## Proposed Architecture

```
Mods/
├── gregCore.dll
├── gregMod.MyMod.dll
└── Assets/
    └── MyMod/
        ├── manifest.json          # AssetPack manifest
        ├── MyMod_Textures.bundle  # Unity AssetBundle
        ├── MyMod_Audio.bundle
        └── Fonts/
            └── Inter-Regular.ttf  # Loose files (fallback)
```

### Manifest Schema (`manifest.json`)
```json
{
  "packId": "com.teamgreg.mymod.assets",
  "name": "MyMod Asset Pack",
  "version": "1.0.0",
  "targetGregCore": "1.1.0",
  "targetUnity": "6000.4.4f1",
  "targetGame": "Data Center",
  "bundles": [
    {
      "name": "textures",
      "file": "MyMod_Textures.bundle",
      "type": "assetbundle"
    }
  ],
  "looseFiles": [
    {
      "path": "Fonts/Inter-Regular.ttf",
      "type": "font"
    }
  ],
  "dependencies": [
    "com.teamgreg.gregcore.assets"
  ]
}
```

### API Surface
```csharp
// Load an asset pack by ID
var pack = GregAssetPack.Load("com.teamgreg.mymod.assets");

// Get a texture from an AssetBundle
Texture2D icon = pack.GetAsset<Texture2D>("textures", "icon_main");

// Get a loose font file
Font font = pack.GetFont("Inter-Regular");

// Enumerate all packs
foreach (var p in GregAssetPack.GetAll())
    MelonLogger.Msg($"Loaded pack: {p.Name}");
```

## Technical Challenges

| Challenge | Proposed Solution |
|-----------|-------------------|
| IL2CPP AssetBundle loading | Use `AssetBundle.LoadFromFile()` (works in IL2CPP) |
| Type safety for loaded assets | Generic `GetAsset<T>()` with `TryCast<T>()` for IL2CPP types |
| Memory management | Reference counting + `AssetBundle.Unload(false)` on mod unload |
| Version mismatches | Manifest validation before loading, graceful skip |
| Cross-platform paths | `Path.Combine` with `Application.streamingAssetsPath` |
| Duplicate asset names | Namespacing by packId: `"packId/bundleName/assetName"` |

## Reference Implementation

```csharp
public class GregAssetPack
{
    public string PackId { get; }
    public string Name { get; }
    public Version Version { get; }

    private readonly Dictionary<string, AssetBundle> _bundles = new();
    private readonly string _basePath;

    public static GregAssetPack Load(string packId)
    {
        // 1. Find manifest
        // 2. Validate versions
        // 3. Load bundles
        // 4. Register in global cache
    }

    public T GetAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
    {
        if (!_bundles.TryGetValue(bundleName, out var bundle))
            return null;

        var asset = bundle.LoadAsset(assetName);
        return asset.TryCast<T>();
    }

    public void Unload()
    {
        foreach (var bundle in _bundles.Values)
            bundle.Unload(false);
        _bundles.Clear();
    }
}
```

## Development Phases

### Phase 1: Foundation (MVP)
- [ ] `GregAssetPack` class with manifest parsing
- [ ] AssetBundle loading from `Mods/Assets/{packId}/`
- [ ] `GetAsset<T>()` with IL2CPP-safe casting
- [ ] Basic validation (file exists, manifest valid)

### Phase 2: Loose File Support
- [ ] Load `.ttf`, `.png`, `.wav` from loose folders
- [ ] Runtime Texture2D creation from raw bytes
- [ ] Runtime AudioClip creation

### Phase 3: Advanced Features
- [ ] Dependency resolution graph
- [ ] ZIP archive support (extract to temp)
- [ ] Hot-reload (`FileSystemWatcher` in dev mode)
- [ ] Asset override system (pack replaces game assets)

### Phase 4: Integration
- [ ] SettingsHub loads UI skins from AssetPack
- [ ] HexViewer loads custom fonts from AssetPack
- [ ] ModMenu auto-discovers asset packs on startup

## File Locations (Proposed)
```
GregCore/
├── src/
│   └── Assets/
│       ├── GregAssetPack.cs           # Main pack class
│       ├── GregAssetManifest.cs       # Manifest parser
│       ├── GregAssetRegistry.cs       # Global registry
│       └── Loaders/
│           ├── AssetBundleLoader.cs
│           ├── LooseFileLoader.cs
│           └── ZipArchiveLoader.cs
└── docs/
    └── ASSETPACK_GUIDE.md             # Modder documentation
```

## Related Files
- `GregCore/docs/FONT_PACK_GUIDE.md` — Fonts are Phase 2 loose files
- `GregCore/src/UI/GregUIManager.cs` — Consumer of asset packs
- `GregMods/gregMod.HexViewer/` — First consumer for custom fonts/skins

## Acceptance Criteria
- [ ] Asset pack loads from `Mods/Assets/{packId}/manifest.json`
- [ ] AssetBundle `.bundle` files load and expose assets via API
- [ ] Loose `.ttf` files load as `Font` objects
- [ ] Version mismatch logs warning and skips pack
- [ ] Missing dependency logs error and skips pack
- [ ] Mod unload triggers `AssetPack.Unload()` → no memory leaks
- [ ] IL2CPP compatibility verified (no Editor APIs)

## Priority
**Medium-High** — Blocks long-term mod customizability but not current bugfixes.

## Status
📝 Design Phase — Ready for architecture review

## Estimated Effort
**8–12 hours** (Phase 1 only: 3–4 hours)

---

*This feature enables the ecosystem: mods ship DLL + AssetPack together, GregCore manages loading, modders focus on content.*
