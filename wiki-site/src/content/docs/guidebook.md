---
title: "Guidebook — Build Mods with C#, Lua, and JS"
---
# Guidebook — Build Mods with C#, Lua, and JS

A hands-on guidebook that takes you from zero to a published Data Center mod. Pick **one** track — Lua, C#, or JavaScript — and build the same kind of mod in each. More languages (Python, Rust, Go, C# scripts) are in preview: see [Guidebook Next Languages](./guidebook-next-languages/).

> Framework version: gregCore 1.2.3 · Game: Data Center 1.0.50.15 · Unity 6000.5 IL2CPP · Loaders: MelonLoader 0.7.2+ / BepInEx 6+ · License: Apache-2.0. Everything here is English-only.

## Which track?

| Track | Status | You need | You get | Start |
|---|---|---|---|---|
| **Lua** | Production, full API | Any text editor | Fastest iteration, REPL, hot-reload, full `greg.*` API | [Guidebook Lua 01 First Mod](./guidebook-lua-01-first-mod/) |
| **C#** | Production, full power | .NET 6 SDK | World editing, custom Harmony patches, max performance | [Guidebook CSharp 01 Setup](./guidebook-csharp-01-setup/) |
| **JS** | Production, UI-focused SDK | Any text editor + `tsc` for TS | Toasts, Toolkit panels, F1 menu, settings, hooks via Jint — HotLoad in main menu | [Guidebook JS 01 Setup](./guidebook-js-01-setup/) |

Rule of thumb: fast iteration and game-state automation → **Lua**. Spawning, cable operations, custom patches, heavy logic → **C#**. You already think in JavaScript and want UI mods fast (panels, toasts, menus, settings) → **JS** (and read its limits first).

## How the guidebook is organized

- **Foundations** (everyone): [Guidebook Prerequisites](./guidebook-prerequisites/) — install, verify, folder layout.
- **Lua track**: 01 First Mod → 02 Events → 03 Timers + Storage → 04 UI → 05 Game Systems → [Guidebook Lua Complete Project](./guidebook-lua-complete-project/) (one fully annotated mod).
- **C# track**: 01 Setup → 02 Lifecycle → 03 UI → 04 Patches → 05 Saves + Shop → [Guidebook CSharp Complete Project](./guidebook-csharp-complete-project/) → [Guidebook CSharp 06 Deploy Debug](./guidebook-csharp-06-deploy-debug/) (deploy + debug).
- **JS track**: 01 Setup → [Guidebook JS 02 Project](./guidebook-js-02-project/) (panel + settings + menu project).
- **Modelling track** (OBJ, static only): [Guidebook Modelling Overview](./guidebook-modelling-overview/) → OBJ + Blender → Shop Item → Static Item → Troubleshooting.
- **Shipping** (everyone): [Guidebook Debugging](./guidebook-debugging/) (all languages) → [Guidebook Computer UI](./guidebook-computer-ui/) (shortcuts + apps) → [Guidebook Porting Matrix](./guidebook-porting-matrix/) (same feature in 3 languages) → [Guidebook Release](./guidebook-release/) (package, version, Workshop) → [Guidebook Next Languages](./guidebook-next-languages/).

Each chapter ends with a checkpoint: something runnable you can verify in-game before moving on.

## What you will build

Every track converges on the same sample mod — **ShiftHelper**: it greets you with your balance, logs money changes, repairs broken servers on a timer, keeps a setting, shows a small panel, and cleans up after itself. Lua builds it with the full API, C# mirrors it with registry + panel + patch, JS builds it with toasts + Toolkit panel + settings (same feature, script speed).

## Reference companions

- API + hook catalog: [Hooks Reference](./hooks-reference/) · concepts: [Core Events](./core-events/) · saves: [Core Save Engine](./core-save-engine/) · internals: [Core Overview](./core-overview/)
- Language reality check: [Developer Scripting Bridges](./developer-scripting-bridges/) · rules that keep setups stable: [Developer Best Practices](./developer-best-practices/) · stuck: [FAQ Troubleshooting](./faq-troubleshooting/)
- In-repo sources: `templates/lua/example-mod/`, `templates/csharp/`, `examples/Lua/starter_template`, `templates/js/` (`greg.d.ts` + `example-mod.ts`).
