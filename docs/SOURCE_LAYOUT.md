# Source Layout

```
gregCore/
├── src/                            # Framework source (gregCore.* assemblies)
│   ├── gregCore.Main/              # MelonLoader entry (GregCoreMod), BuildInfo
│   ├── gregCore.Core/              # Config, Diagnostics, Events, Networking, Persistence, Services
│   ├── gregCore.Shared/            # Shared utilities (DevLog, …)
│   ├── gregCore.Abstractions/      # Public API surface for mod developers
│   ├── gregCore.UI/                # UI overlays, HUD, Mod-Hub, click routing, icons
│   ├── gregCore.Bridge/            # Scripting bridges (Lua/JS/Python/Rust/Go/C#)
│   ├── gregCore.Hooks/             # Dynamic Harmony patching + hook integration
│   ├── gregCore.Patches/           # Game-specific Harmony patches
│   ├── gregCore.Mod/               # Mod registry / multi-mod runtime
│   ├── gregCore.SDK/               # SDK packs for external tooling
│   ├── gregCore.Compatibility/     # Built-in QoL/compat modules
│   ├── gregCore.GameApi/           # Generated game API surface
│   ├── GlobalUsings.cs             # Shared usings
│   ├── NullablePolyfills.cs        # Polyfills
│   ├── SubProjectUsings.cs
│   └── TestVisibility.cs
├── framework/                      # greg_hooks.json — canonical hook registry
├── game_hooks.json                 # Patchable methods from IL2CPP dump
├── references/                     # Game + MelonLoader reference assemblies
├── docs/                           # Documentation (INDEX, ARCHITECTURE, FrameworkAPI, …)
├── scripts/                        # Build, release, mirror, validation helpers
├── tests/                          # Unit tests (gregCore.Tests.csproj)
├── sdk/                            # SDK packs
├── examples/                       # Example mods (C#, Go, JS, Lua, Python, Rust)
├── templates/                      # C#/Lua mod templates (GregMod.Template.csproj)
├── sponsors/                       # Sponsor data
├── tools/                          # GameApiGenerator, coverage scanner
├── bin/ · obj/                     # Build output (gitignored)
├── Releases/                       # Packaged DLLs (gitignored)
├── .codacy/                        # Local Codacy CLI config (no CI double-run)
├── .forgejo/workflows/             # CI + mirror (identical to .gitea)
├── .gitea/workflows/               # CI + mirror (identical to .forgejo)
├── build/                          # MSBuild props/targets
├── VERSION                         # Single source of truth for version
├── gregCore.csproj                 # Project file
├── gregCore.sln                    # Solution file
├── build.sh · build.ps1            # Build scripts (dual profile)
├── publish.ps1                     # Publish script
├── CHANGELOG.md                    # Version history (Keep a Changelog)
├── QUICKSTART.md                   # Quick start guide
├── LICENSE                         # Apache 2.0
├── CONTRIBUTING.md                 # Contribution guidelines
├── AGENTS.md                       # AI agent instructions
└── README.md
```

## Key Directories

- **`src/`** — All framework source code, organized by `gregCore.*` assemblies
- **`framework/`** — Hook registry JSON used by the patching system
- **`references/`** — Game and MelonLoader reference assemblies (Il2CppInterop-Dummies)
- **`docs/`** — API docs, layout guides, architecture, changelog
- **`scripts/`** — Build helpers, mirror sync, version/contract validation
- **`examples/`** — Example mods in 6 languages
- **`templates/`** — Starter templates for C# and Lua mods
