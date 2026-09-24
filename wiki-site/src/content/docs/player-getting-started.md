---
title: "Player Getting Started"
---
# Player Getting Started

This page is for **players** (mod users). No programming required. If you want to *write* mods, start at [Developer Getting Started](./developer-getting-started/) instead.

## What gregCore is

gregCore is a framework mod for the game **Data Center**. It does three things for you as a player:

1. **Loads other mods** — Lua scripts, C# mods, and multi-language extensions all run through gregCore's registry (`GregModRegistry`), with dependency checks (`GregModDeps`) so missing requirements are reported instead of crashing.
2. **Gives you one control center** — press **F1** anywhere in game for the Mod Hub (`GregModHub`), which lists every registered mod and its menus.
3. **Protects your saves** — the SaveGuard (`GregSaveGuard`) backs up vanilla saves before writing and keeps mod data in separate sidecar files, so uninstalling a mod never corrupts your save.

## What you need

| Requirement | Notes |
|---|---|
| Data Center (Steam), version 1.0.50.15 | The supported game version for gregCore 1.2.3 |
| Windows x64 or Linux x64 | macOS is not supported (no Apple test device on the team — contributors welcome) |
| MelonLoader 0.7.2+ **or** BepInEx 6+ | Exactly one loader; both are supported |
| gregCore 1.2.3 release ZIP | From the GitHub Releases page, matching your loader + OS |

## The 3-step path

1. **Install** — [Player Installation](./player-installation/) (extract one ZIP into the game folder, launch once).
2. **Verify** — launch the game, check the MelonLoader log for `HWID SYSTEM ACTIVE` and the `[DynamicPatcher]` lines, press **F1** to see the Mod Hub.
3. **Add mods** — [Player Mod Users Guide](./player-mod-users-guide/) (where Lua mods live, how the HUD key bar works, how to open mod settings, how to report issues).

## Key bindings (defaults)

| Key | What happens | Implementation |
|---|---|---|
| **F1** | Opens/closes the central Mod Hub | `GregModHub` (`MenuId = "greg.hub"`) |
| **F12** | Opens the Lua console (REPL) | `LuaRepl` in `gregCore.SDK` |
| **Backquote (`)** | Opens the gregCore console (`greg.console`: `help`, `clear`, `mods`, `menus`, `keys`, `version`) | `gregCore` console module |
| **F8** | Legacy mod-config panel (compatibility layer) | `ModConfigSystem` in `gregCore.Compatibility` |

Mods can register their own hotkeys, which appear automatically in the key bar on the right screen edge (`GregHud`). Pressing a mod's key opens its panel; the Mod Hub always shows what is available.

## Where things live after first launch

First launch creates the working directories automatically:

- `Mods/gregCore.dll` + `game_hooks.json` + `greg_hooks.json` — the framework itself (MelonLoader layout; BepInEx uses `BepInEx/plugins/gregCore/`).
- `UserData/gregCore/Mods/Lua/<modId>/` — Lua mods (each with `main.lua`, optional `mod.json`, sandboxed `data/` folder).
- `UserData/gregCore/Mods/Scripts/` — general script directory.
- `Documents/DatacenterBackups` — automatic save backups (`pre-greg/` permanent copy + timestamped folders, max 3 per save; Linux: `~/Documents/…`, fallback `~/…`).

## Getting help

- In-game first: Mod Hub (F1) → mod entry → Settings tab; console (`` ` ``) → `mods`, `menus`, `keys`, `version`.
- Logs: MelonLoader log (look for `[gregCore]`, `[gregCore][HwId]`, `[DynamicPatcher]`).
- Still stuck: [FAQ Troubleshooting](./faq-troubleshooting/) → Discord (`discord.gg/greg`) → GitHub issue (include version, loader, log excerpt).
