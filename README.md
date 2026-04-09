# GREG FRAMEWORK

## DATA CENTER MODDING API

[![.NET CI](https://github.com/mleem97/gregFramework/actions/workflows/dotnet-ci.yml/badge.svg?branch=master)](https://github.com/mleem97/gregFramework/actions/workflows/dotnet-ci.yml)
[![Commit Lint](https://github.com/mleem97/gregFramework/actions/workflows/commitlint.yml/badge.svg?branch=master)](https://github.com/mleem97/gregFramework/actions/workflows/commitlint.yml)
[![Last Commit](https://img.shields.io/github/last-commit/mleem97/gregFramework/master)](https://github.com/mleem97/gregFramework/commits/master)

![Greg Framework API Banner](https://github.com/user-attachments/assets/3e78050a-67e8-4eaa-981e-7fa5cfbc466c)

**FrikaMF** (Frika Mod Framework) is a MelonLoader-based modding stack for **Data Center**.
It is the **framework core** (`gregCore`): translations, Harmony hooks, event handling, a Rust/native bridge, plugin extensions, and optional tooling.

**Mod / plugin / extension logic** shipped by authors must be implemented in **C#** — see [Modding language (C# only)](https://github.com/mleem97/gregWiki/blob/main/docs/reference/modding-language-requirement.md) in **gregWiki**.

---

## 📦 Installation & Setup

### Prerequisites

To use this framework, you need:

- A compatible installation of **Data Center**
- **MelonLoader** configured for your game build
- The framework DLL in your game `Mods` folder

### Quick Install Guide

1. Build or download `FrikaModdingFramework.dll`.
2. Copy it to: `Data Center/Mods/`.
3. (Optional) Add companion mods/plugins.
4. Launch the game and verify load order in `MelonLoader/Latest.log`.

---

## 📂 Directory Structure

### In-Game Layout

- **C# mods:** `Data Center/Mods/`
- **Rust/native mods:** `Data Center/Mods/RustMods/`
- **Content packs:** `Data Center/Data Center_Data/StreamingAssets/Mods/`

### Repository Layout (Standalone)

| Component | Path | Description |
| :--- | :--- | :--- |
| **Framework (runtime)** | [`framework/`](framework/) | Build with [`framework/FrikaMF.csproj`](framework/FrikaMF.csproj) or [`FrikaMF.sln`](FrikaMF.sln). |
| **FFM Plugins** | [`plugins/`](plugins/) | Runtime plugin layer projects (`FFM.Plugin.*`). |
| **Templates** | [`Templates/`](Templates/) | Mod/plugin template projects and scaffolds. |
| **MCP Server** | [`mcp-server/`](mcp-server/) | Model Context Protocol server implementation and tooling docs. |
| **Scripts** | [`scripts/`](scripts/) | Build/deploy/version scripts. |
| **Tools** | [`tools/`](tools/) | Scanners, hook generation, and helper tooling. |
| **References** | [`lib/references/`](lib/references/) | MelonLoader/game reference assemblies for local builds. |

---

## 📖 Documentation & References

- **Reference assemblies workflow:** [`lib/references/README.md`](lib/references/README.md)
- **MCP server details:** [`mcp-server/README.md`](mcp-server/README.md)
- **Template usage:** [`Templates/README.md`](Templates/README.md)
- **Tooling overview:** [`tools/README.md`](tools/README.md)

---

## 🛠 Developer Tooling

- **Refresh interop DLLs:**

  ```bash
  python tools/refresh_refs.py
  ```

- **Diff game assembly metadata:**

  ```bash
  python tools/diff_assembly_metadata.py
  ```

- **Regenerate hook catalog:**

  ```powershell
  ./tools/Generate-FmfHookCatalog.ps1
  ```

---

## ⚠️ Troubleshooting

- **Mod not loading?** Check `MelonLoader/Latest.log`.
- **Build reference errors?** Ensure MelonLoader interop DLLs exist or set `DATA_CENTER_GAME_DIR`.

---

## 🤝 Contributing

This standalone repository is the cleaned framework target after monorepo split/migration.

If you contribute changes, keep runtime compatibility on `.NET 6` and preserve layer boundaries (`Core SDK` -> `Plugin Layer` -> `Mod Layer`).
