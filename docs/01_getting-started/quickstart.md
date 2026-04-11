---
title: Getting started
sidebar_label: Getting started
description: Workspace layout (gregFramework), building gregCore, hooks, and your first mod.
---

The workspace is **multi-repo**: several Git checkouts live **side by side** under a common `gregFramework/` folder. **Logical stack:** **ModManager → Framework / SDK → Plugins → Mods** — see [System architecture & documentation principles](/wiki/getting-started/architecture).

| Folder | Role |
|--------|--------|
| **`gregCore/`** | Framework: **`gregCore/gregCore.sln`**, main project **`gregCore/framework/gregCore.csproj`**; SDK under `framework/Sdk/`; MCP under `gregCore/mcp-server/` |
| **`gregMod.<Name>/`** | Gameplay mods (often `FMF.*` assemblies), **flat** next to `gregCore/` (umbrella **`gregMods/`** is deprecated) |
| **`gregExt.<Name>/`** | Framework plugins (`FFM.Plugin.*` / `greg.Plugin.*`), also flat (**`gregExtensions/`** deprecated) |
| **`gregModmanager/`** | GregModManager / Workshop UI (`WorkshopUploader.csproj`) |
| **`gregDataCenterExporter/`** | Exporter, templates, mirrors |
| **`gregWiki/`** | This documentation site (Docusaurus) |

Rust/native mods are loaded through the core FFI layer; bridge code lives under **`gregCore/framework/ModLoader/`** (including **`LanguageBridges/`** and **`LanguageBridges/LuaModules/`** for the Lua `greg.*` API).

## Build the framework

```bash
dotnet build gregCore/gregCore.sln -c Release
```

Or open **`gregCore/gregCore.sln`** in your IDE and build the **`gregCore/framework/gregCore.csproj`** output. A **Release** build produces **`gregCore.dll`** (the MelonLoader framework assembly — not a legacy `FrikaModdingFramework.dll` name).

For CI without a local game install, many projects support **`-p:CI=true`** (see each `.csproj`).

**Prerequisites:** MelonLoader **net6** assemblies and game Il2Cpp interop — either from `{Game}/MelonLoader/` or **`gregCore/lib/references/MelonLoader`** (e.g. `python gregCore/tools/refresh_refs.py`). Set **`DATA_CENTER_GAME_DIR`** if MSBuild should discover the game path.

### Quick install (matches `gregCore/README.md`)

1. Build or obtain **`gregCore.dll`**.
2. Copy it to **`Data Center/Mods/`** (same folder as other MelonLoader mods).
3. **C# mods:** additional managed mods stay under **`Data Center/Mods/`** as usual.
4. **Lua mods:** place `.lua` trees under **`Data Center/Mods/ScriptMods/lua/`** (per Language Bridges layout; same `Mods/` root as `gregCore.dll`).
5. **Rust / native mods:** place modules under **`Data Center/Mods/RustMods/`**.
6. Launch the game and confirm load order in **`MelonLoader/Latest.log`**.

## Supported modding languages

gregCore supports **three modding paths** in parallel:

| Language | Role |
|----------|--------|
| **C#** | Full MelonLoader / IL2CPP interop, Harmony, plugins, and **`gregFramework.Core`** — the default for framework extensions and heavy mods. |
| **Lua** | MoonSharp-hosted scripts via **`LuaLanguageBridge`**; game access through the **`greg.*`** API (events, Harmony bindings, Unity handles). Deploy under **`Data Center/Mods/ScriptMods/lua/`**. |
| **Rust / native** | **`FFIBridge`** native pipeline; deploy under **`Data Center/Mods/RustMods/`**. |

Policy, paths, and trade-offs: [Modding language support](/wiki/developers). Lua API tables: [Language Bridges README](https://github.com/mleem97/gregFramework/blob/main/gregCore/framework/ModLoader/LanguageBridges/README.md).

## Hooks and registries

| Topic | Where |
|-------|--------|
| **Canonical `greg.*` (JSON, Il2Cpp)** | Repo root **`greg_hooks.json`**; regenerate: **`gregCore/scripts/Generate-GregHooksFromIl2CppDump.ps1`** |
| **EventId → `greg.*` (native / FFI pipeline)** | **`GregNativeEventHooks`** — [greg hooks catalog](/wiki/reference/greg-hooks-catalog), source `gregCore/framework/Sdk/GregNativeEventHooks.cs` |
| **`FMF.*` documentation naming** | [FMF hook naming](/wiki/reference/fmf-hook-naming) |
| **Architecture** | [Greg hooks & event runtime](/wiki/development/concepts/hooks-and-events) |
| **Legacy spellings** | **`GregCompatBridge`** loads **`greg_hooks.json`** next to **`gregCore.dll`** |

## Start a new mod

1. Create **`gregMod.<Name>/`** under `gregFramework/` (or clone a template).
2. Add **`ProjectReference`** to **`gregCore/framework/gregCore.csproj`** (see **`mods/GregShowcaseMod/`**).
3. Use **`gregFramework.Core`** (`GregEventDispatcher`, `GregHookName`, `GregNativeEventHooks`, …).
4. Templates: **`gregCore/Templates/greg.BasedModTemplate/`** or mirrored templates under **`gregDataCenterExporter/Templates/`**.

Reference mod: **`mods/GregShowcaseMod/`** — [Documentation hub](/wiki/developers).

## Documentation site (`gregWiki/`)

- Markdown under **`gregWiki/docs/`**
- Docker: from **`gregWiki/`** root — see **`gregWiki/README.md`**
- MCP: [MCP references](/wiki/developers) — server code under **`gregCore/mcp-server/`**

