# gregCore - Agent Guide

Welcome to the `gregCore` repository. This document outlines the project architecture, commands, conventions, and constraints to help AI agents work effectively in this codebase.

## 1. Project Overview

`gregCore` is a modding framework for the game "Data Center". It acts as a bridge between the game (running on Unity IL2CPP with MelonLoader) and various modding languages (C#, Lua, JS, Rust, Go). 

**Critical Constraint:** All runtime/gameplay-facing components MUST remain compatible with **.NET 6.0**. Do not upgrade the target framework beyond `net6.0`, as it will break Unity IL2CPP + MelonLoader compatibility.

## 2. Essential Commands & Scripts

The project uses PowerShell scripts for most build and deployment tasks. You can find them in the `scripts/` directory.

- **Build Release:** `pwsh scripts/Build-Release.ps1`
  - This builds `src/gregCore.csproj` in Release configuration and packages it into `publish/gregCore-vX.Y.Z.zip`.
- **Manual Build:** `dotnet build src/gregCore.csproj -c Release`
- **Hooks Generation:** `pwsh scripts/Generate-GregHooksFromIl2CppDump.ps1`
- **Workshop Deployment:** `pwsh scripts/Deploy-Release-ToWorkshop.ps1`

When `CI=true` is set, the project uses stub DLLs from `ci-stubs/` instead of actual MelonLoader references.

## 3. Architecture & Layers

The framework is strictly layered (as detailed in `modding_core_architecture_summary.md`):

1. **Unity Game Layer:** Game types/methods patched via Harmony (`src/GameLayer/Patches/`).
2. **Core Layer (gregCore):** 
   - `src/GameLayer/Bootstrap/gregCoreLoader.cs` (MelonMod entry point)
   - FFI Bridge (`Win32FfiBridge.cs`) and API Table (`GameApiTable.cs`)
   - Event Dispatching (`GregEventBus`, `NativeEventHooks.cs`)
3. **Plugin Layer:** Plugin registry and dependency resolver (`src/Infrastructure/Plugins/`).
4. **Language Bridges:** Hosts for script isolation (`src/Infrastructure/Scripting/Lua`, `Js`).
5. **Mod Layer:** User-created mods (C#, native DLLs, or scripts).

### Event / Hooking Pipeline
1. `HarmonyPatches` intercept Unity methods (Prefix/Postfix).
2. Data is extracted into primitive/struct DTOs.
3. Dispatched via `GregEventBus` using canonical hook names: `greg.<Domain>.<Event>`.
4. Mappings are defined in `assets/greg_hooks.json` and loaded dynamically via `GregHookRegistry` (`IGregHookRegistry`), providing stable hashed Event-IDs for the FFI.

## 4. Conventions and Rules

- **Language:** Respond in **technical German** when summarizing intent before code changes, unless a file or repository policy explicitly requires English.
- **ABI & FFI Boundaries:**
  - Struct order/types are ABI-critical. Always use `[StructLayout(LayoutKind.Sequential)]`.
  - Pass stable primitive types (`int`, `uint`, `float`, `byte[]`, `IntPtr`) across the bridge.
  - No implicit Unity/IL2CPP object references in public Bridge DTOs.
- **Error Handling:** Every boundary (FFI, JSON parse, network) must be wrapped in `try/catch` + logging. Failure in a mod must remain isolated and not crash the core loop.
- **`MISSING.md` Rule:** If a required API abstraction is missing during extension work, you MUST create a `MISSING.md` file in the relevant module folder. It must include a `.gitignore` header (`!MISSING.md`). (See `docs/MODDING_MelonLoader_gregCore.md` for the template).
- **Wiki Currency Check:** At the end of every change request, you MUST verify whether relevant wiki pages in `docs/` are up to date and list them if updates are required.

## 5. Directory Structure

- `src/Core/`: Core interfaces, exceptions, and models.
- `src/GameLayer/`: Harmony patches and bootstrap/entry point logic.
- `src/PublicApi/`: Public SDK used by C# mods (Attributes, Mod base classes).
- `src/Infrastructure/`: Implementations for Networking (MCP, Sync), FFI, UI, Config, Scripting (MoonSharp Lua, JS), Automation, and Performance Governors.
- `scripts/`: PowerShell automation for builds, packaging, and hook generation.
- `assets/`: Contains `greg_hooks.json`.
- `docs/`: Extensive project documentation (Tutorials, Hooks Catalog, Architecture).
- `lib/`: Third-party dependencies and references (e.g., MoonSharp, MelonLoader stubs).