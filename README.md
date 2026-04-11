# gregCore

**gregCore** — MelonLoader-based modding stack for **Data Center**: translations, Harmony hooks, event handling, a Rust/native bridge, and framework extensions.

[![Build and Release](https://github.com/MLeeM97/gregCore/actions/workflows/build.yml/badge.svg)](https://github.com/MLeeM97/gregCore/actions/workflows/build.yml)
[![Last commit](https://img.shields.io/github/last-commit/MLeeM97/gregCore/main?label=last%20commit)](https://github.com/MLeeM97/gregCore/commits/main)

![Greg Framework API Banner](https://github.com/user-attachments/assets/3e78050a-67e8-4eaa-981e-7fa5cfbc466c)

| | |
|:---|:---|
| **Repository** | [`MLeeM97/gregCore`](https://github.com/MLeeM97/gregCore) |
| **Author** | **MLeeM97** |
| **Target** | Windows x64, .NET 6 |

---

## Installation

1. Download the latest `gregCore.dll` from the [Releases](https://github.com/MLeeM97/gregCore/releases) page.
2. Copy it to your game directory: `Data Center/Mods/`.
3. Launch the game.

---

## Local Build & Development

### Prerequisites

- **.NET 6 SDK**
- **Data Center** game installed
- **MelonLoader** (net6 version) installed in the game

### Setting up References

Before building, you must sync the game's interop assemblies into the repository:

```bash
python gregTools/refresh_refs.py
```

This will copy the necessary DLLs from your Steam install into `gregLib/references/`.

### Building

Use the provided PowerShell script to build and package a release:

```powershell
./gregScripts/Build-Release.ps1
```

The output will be located in the `publish/` directory.

---

## Continuous Integration (GitHub Actions)

The repository includes a GitHub Action that attempts to build `gregCore` on every push.

**Note:** Since game assemblies are proprietary and not included in the repository, the GitHub build will skip packaging unless you provide the necessary references (e.g., via a private runner or a secure download step).

---

## Directory Structure

| Component | Path | Description |
| :--- | :--- | :--- |
| **Core Loader** | `gregModLoader/` | The main MelonLoader mod and language bridge logic. |
| **SDK** | `gregSdk/` | Core API for mods (Events, Payloads, Hooks). |
| **Scripts** | `gregScripts/` | Build and deployment scripts. |
| **Tools** | `gregTools/` | Reference refresh and catalog generation tools. |
| **References** | `gregLib/references/` | Destination for synced game DLLs (not committed). |

---

## License

gregCore is provided as-is for the Data Center modding community.
