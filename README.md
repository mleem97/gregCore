# 🟢 **GREG Framework** — gregCore

> Part of the [GREG Universe](https://gregframework.eu) — The primary SDK and runtime backbone for *Data Center*.

---

## 📖 **Overview**
**gregCore** is the foundational assembly for the Greg Framework. It provides the normalized event bus, cross-language bridges (Lua, TS, Rust), and typsafe SDK services to interact with the game's internal IL2CPP systems.

- **Language:** C# (Managed)
- **Layer:** Framework / SDK
- **Version:** `v1.0.0-pre.5`
- **Primary Artifact:** `gregCore.dll`

---

## 🚀 **Quick Start**

### Installation
1. Download the latest **[v1.0.0-pre.5 Release](https://github.com/mleem97/gregCore/releases/tag/v1.0.0-pre.5)**.
2. Place `gregCore.dll` and `MoonSharp.Interpreter.dll` in your `Data Center/Mods/` folder.
3. (Optional) For native modding, include `greg_api.h` in your project.

### Build from Source
```bash
dotnet build gregCore.csproj -c Release
```

---

## 🛠️ **Features in v1.0.0-pre.5**

- **Unity Signal Normalization:** 30+ canonical hooks for UI, Input, and World events.
- **Typed Registries:** Official SDK registries for all content categories (Servers, Switches, etc.).
- **Engine Bridges:** High-level services for Shop, Technician, and Time control.
- **Language Parity:** Identical API surface for C#, Lua, Rust, and TypeScript.

---

## 🌍 **Repositories & Remotes**
This repository uses a dual-push setup for maximum redundancy.

- **GitHub (Primary):** [mleem97/gregCore](https://github.com/mleem97/gregCore)
- **Gitea (Mirror):** [git.datacentermods.com/teamGreg/gregCore](https://git.datacentermods.com/teamGreg/gregCore)

---

## 🤝 **Contributing**
All code changes must follow the `camelCase` naming convention. Ensure that any new Unity engine integration is properly normalized into a `greg.*` hook constant in `GregNativeEventHooks.cs`.

---
*© 2026 teamGreg | Developed by [mleem97](https://github.com/mleem97)*
