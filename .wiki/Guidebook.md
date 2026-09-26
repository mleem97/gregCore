# Guidebook — Build Mods with C#, Lua, and JS

A hands-on guidebook that takes you from zero to a published Data Center mod. Pick **one** track — Lua, C#, or JavaScript — and build the same kind of mod in each. More languages (Python, Rust, Go, C# scripts) are in preview: see [[Guidebook Next Languages]].

> Framework version: gregCore 1.2.3 · Game: Data Center 1.0.50.15 · Unity 6000.5 IL2CPP · Loaders: MelonLoader 0.7.2+ / BepInEx 6+ · License: Apache-2.0. Everything here is English-only.

## Which track?

| Track | Status | You need | You get | Start |
|---|---|---|---|---|
| **Lua** | Production, full API | Any text editor | Fastest iteration, REPL, hot-reload, full `greg.*` API | [[Guidebook Lua 01 First Mod]] |
| **C#** | Production, full power | .NET 6 SDK | World editing, custom Harmony patches, max performance | [[Guidebook CSharp 01 Setup]] |
| **JS** | Production, UI-focused SDK | Any text editor + `tsc` for TS | Toasts, Toolkit panels, F1 menu, settings, hooks via Jint — HotLoad in main menu | [[Guidebook JS 01 Setup]] |

Rule of thumb: fast iteration and game-state automation → **Lua**. Spawning, cable operations, custom patches, heavy logic → **C#**. You already think in JavaScript and want UI mods fast (panels, toasts, menus, settings) → **JS** (and read its limits first).

## How the guidebook is organized

- **Foundations** (everyone): [[Guidebook Prerequisites]] — install, verify, folder layout.
- **Lua track**: 01 First Mod → 02 Events → 03 Timers + Storage → 04 UI → 05 Game Systems → [[Guidebook Lua Complete Project]] (one fully annotated mod).
- **C# track**: 01 Setup → 02 Lifecycle → 03 UI → 04 Patches → 05 Saves + Shop → [[Guidebook CSharp Complete Project]] → [[Guidebook CSharp 06 Deploy Debug]] (deploy + debug).
- **JS track**: 01 Setup → [[Guidebook JS 02 Project]] (panel + settings + menu project).
- **Rust track**: 01 Setup → [[Guidebook Rust 02 Project]] (native events + hooks project).
- **Modelling track** (OBJ, static only): [[Guidebook Modelling Overview]] → OBJ + Blender → Shop Item → Static Item → Troubleshooting.
- **Shipping** (everyone): [[Guidebook Debugging]] (all languages) → [[Guidebook Computer UI]] (shortcuts + apps) → [[Guidebook Porting Matrix]] (same feature in 3 languages) → [[Guidebook Release]] (package, version, Workshop) → [[Guidebook Next Languages]].

Each chapter ends with a checkpoint: something runnable you can verify in-game before moving on.

## What you will build

Every track converges on the same sample mod — **ShiftHelper**: it greets you with your balance, logs money changes, repairs broken servers on a timer, keeps a setting, shows a small panel, and cleans up after itself. Lua builds it with the full API, C# mirrors it with registry + panel + patch, JS builds it with toasts + Toolkit panel + settings (same feature, script speed).

## Reference companions

- API + hook catalog: [[Hooks Reference]] · concepts: [[Core Events]] · saves: [[Core Save Engine]] · internals: [[Core Overview]]
- Language reality check: [[Developer Scripting Bridges]] · rules that keep setups stable: [[Developer Best Practices]] · stuck: [[FAQ Troubleshooting]]
- In-repo sources: `templates/lua/example-mod/`, `templates/csharp/`, `examples/Lua/starter_template`, `templates/js/` (`greg.d.ts` + `example-mod.ts`).
