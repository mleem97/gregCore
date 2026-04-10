# gregCore

**Frika Mod Framework (FrikaMF)** — MelonLoader-based modding stack for **Data Center**: translations, Harmony hooks, event handling, a Rust/native bridge, plugin extensions, and optional tooling.

[![Last commit](https://img.shields.io/github/last-commit/mleem97/gregCore/main?label=last%20commit)](https://github.com/mleem97/gregCore/commits/main)

![Greg Framework API Banner](https://github.com/user-attachments/assets/3e78050a-67e8-4eaa-981e-7fa5cfbc466c)

---

## Part of gregFramework

This directory is part of the **[gregFramework](https://github.com/mleem97/gregFramework)** workspace. Clone sibling repositories side by side so each project lives at `gregFramework/<RepoName>/`. See the workspace [README](https://github.com/mleem97/gregFramework/blob/master/README.md) for the full layout and migration notes.

**Remote:** [`mleem97/gregCore`](https://github.com/mleem97/gregCore) — on-disk path: `gregFramework/gregCore/`.

**Mod, plugin, and extension code** shipped by authors must be implemented in **C#** — see [Modding language (C# only)](https://github.com/mleem97/gregWiki/blob/main/docs/reference/modding-language-requirement.md) in **gregWiki**.

---

## Installation and setup

### Prerequisites

- A compatible installation of **Data Center**
- **MelonLoader** configured for your game build
- The framework DLL in your game `Mods` folder

### Quick install

1. Build or download `FrikaModdingFramework.dll`.
2. Copy it to: `Data Center/Mods/`.
3. (Optional) Add companion mods or plugins.
4. Launch the game and verify load order in `MelonLoader/Latest.log`.

---

## Directory structure

### In-game layout

- **C# mods:** `Data Center/Mods/`
- **Rust/native mods:** `Data Center/Mods/RustMods/`
- **Content packs:** `Data Center/Data Center_Data/StreamingAssets/Mods/`

### Repository layout (standalone)

| Component | Path | Description |
| :--- | :--- | :--- |
| **Framework (runtime)** | [`framework/`](framework/) | Build with [`framework/FrikaMF.csproj`](framework/FrikaMF.csproj) or [`FrikaMF.sln`](FrikaMF.sln). |
| **FFM plugins** | [`plugins/`](plugins/) | Runtime plugin layer projects (`FFM.Plugin.*`). |
| **Templates** | [`Templates/`](Templates/) | Mod and plugin template projects and scaffolds. |
| **MCP server** | [`mcp-server/`](mcp-server/) | Model Context Protocol server implementation and tooling docs. |
| **Scripts** | [`scripts/`](scripts/) | Build, deploy, and version scripts. |
| **Tools** | [`tools/`](tools/) | Scanners, hook generation, and helper tooling. |
| **References** | [`lib/references/`](lib/references/) | MelonLoader and game reference assemblies for local builds. |

---

## Documentation

- **Reference assemblies workflow:** [`lib/references/README.md`](lib/references/README.md)
- **MCP server details:** [`mcp-server/README.md`](mcp-server/README.md)
- **Template usage:** [`Templates/README.md`](Templates/README.md)
- **Tooling overview:** [`tools/README.md`](tools/README.md)

---

## Developer tooling

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

## Troubleshooting

- **Mod not loading?** Check `MelonLoader/Latest.log`.
- **Build reference errors?** Ensure MelonLoader interop DLLs exist or set `DATA_CENTER_GAME_DIR`.

---

## Contributing

This standalone repository is the cleaned framework target after monorepo split and migration.

If you contribute changes, keep runtime compatibility on **.NET 6** and preserve layer boundaries (`Core SDK` → `Plugin layer` → `Mod layer`).
