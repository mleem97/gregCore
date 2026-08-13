# Source Layout

```
gregCore.Framework/
├── src/                            # Framework + mod source code
│   ├── Core/                       # GregCoreMod.cs — MelonLoader/BepInEx entry point
│   ├── Infrastructure/             # Config, logging, persistence, DI
│   ├── GameLayer/                  # Harmony patches for game classes
│   ├── UI/                         # UI Toolkit overlay, widgets
│   ├── API/                        # Public API surface for mod developers
│   ├── Bridge/                     # Scripting bridges (Lua, JS, Python)
│   ├── Save/                       # Save engine with versioning (LiteDB)
│   ├── Shop/                       # Custom shop system
│   ├── Employee/                   # Employee management
│   ├── Rack/                       # Wall rack and grid placement
│   ├── Network/                    # Native game-network read-only adapters
│   ├── Diagnostics/                # Debug and logging infrastructure
│   └── ...                         # 27 modules total
├── framework/                      # greg_hooks.json — canonical hook registry
├── game_hooks.json                 # Patchable methods from IL2CPP dump
├── lib/                            # Reference assemblies
│   ├── references/MelonLoader/     # Game stubs + MelonLoader DLLs
│   └── MoonSharp/                  # MoonSharp interpreter source (reference)
├── docs/                           # Documentation
│   ├── FrameworkAPI.md             # Auto-generated hook reference
│   ├── SOURCE_LAYOUT.md            # This file
│   └── CHANGELOG.md                # Version history
├── scripts/                        # Build and code-generation helpers
├── ci-stubs/                       # CI stub generation
├── tests/                          # Unit tests
├── sdk/                            # SDK packs
├── examples/                       # Example mods (C#, Go, JS, Lua, Python, Rust)
├── sponsors/                       # Sponsor data
├── tools/                          # Utility tools
├── .github/workflows/              # CI pipeline
├── build/                          # MSBuild props/targets
├── VERSION                         # Single source of truth for version
├── gregCore.csproj                 # Project file
├── gregCore.sln                    # Solution file
├── build.ps1                       # Windows build script
├── build.sh                        # Linux build script
├── publish.ps1                     # Publish script
├── game_hooks.json                 # Game hook definitions
├── AGENTS.md                       # AI agent instructions
├── CHANGELOG.md                    # Version history (root)
├── QUICKSTART.md                   # Quick start guide
├── LICENSE                         # Apache 2.0
├── CONTRIBUTING.md                 # Contribution guidelines
└── README.md
```

## Key Directories

- **`src/`** — All framework source code, organized by module
- **`framework/`** — Hook registry JSON used by the patching system
- **`lib/references/`** — Game and MelonLoader reference assemblies (not committed)
- **`docs/`** — Auto-generated API docs, layout guides, changelog
- **`scripts/`** — Build helpers, hook generation, CI utilities
- **`examples/`** — Example mods in 6 languages
