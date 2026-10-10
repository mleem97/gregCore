---
title: "Player Mod Users Guide"
---
# Player Mod Users Guide

How to use mods day-to-day once gregCore is installed: the Hub, the HUD, settings, the consoles, saves, and how to file a useful bug report.

## The Mod Hub (F1)

`GregModHub` is the central, always-available menu (`MenuId = "greg.hub"`, Dialog layer):

- **Open with F1** from anywhere. The screen dims and a `GregFramework — Mods (F1)` card lists every registered mod.
- Each entry shows the mod's menus. Buttons are honest: **Close** only appears when the mod registered a closer (`GregMenuRegistry.RegisterCloser`); otherwise you get **Open** or a display-only row.
- The **Settings** button jumps to that mod's tab in the Settings Hub (`GregSettingsHub`), if the mod registered one.
- Internal entries (`greg.*`, e.g. `greg.console`) are hidden — you only see real mods.

If F1 does nothing, check [FAQ Troubleshooting](./faq-troubleshooting/) (usually another overlay locked input, or the DLL/JSON files are misplaced).

## The key bar (HUD)

`GregHud` renders a slim key bar at the **right screen edge** (`right=8, top=64`, HUD layer, click-through):

- Every mod that calls `GregHudRegistry.Register(modId, key, label)` gets one row: **key** (bold, highlighted) + **label** (white).
- One row per mod; re-registering overwrites. Uninstalled mods disappear on refresh.
- The bar is display-only — press the shown key to open that mod's panel.

## Settings

Two settings systems exist (history matters):

1. **Modern Settings Hub** (`GregSettingsHub`, `GregModSettingsService`) — mods built against the current contract register tabs with toggles/sliders; values persist via MelonPreferences. Open it from the Mod Hub's Settings button.
2. **Legacy ModConfig panel** (`ModConfigSystem`, `gregCore.Compatibility`) — press **F8**; closes with ESC. Exists so old Data Center mods keep working. Their files live under `UserData/ModConfigs`.

If a mod documents "open settings" without saying which, try the Hub first, then F8.

## Consoles

| Console | Key | Commands | Purpose |
|---|---|---|---|
| gregCore console | Backquote (`` ` ``) | `help`, `clear`, `mods`, `menus`, `keys`, `version` | Inspect registry, menus, keybinds, version |
| Lua REPL | F12 | Any Lua using the `greg.*` API | Try API calls live: `print(greg.player.money())`, `greg.player.add_money(1000)`, `greg.ui.notify("hi")` |
| DevConsole log | — | `greg.ui.log_info / log_warning / log_error` output | Mods write here; check it when something behaves oddly |

## Saves: what to know as a player

- Saving works exactly as vanilla. gregCore's SaveGuard additionally writes per-mod sidecars (`greg_<modId>.<save>.tsv`) and a registry sidecar — the game ignores them (only `*.save` is listed), so **removing gregCore or a mod never deletes your vanilla progress**.
- Before the first modded save of each file, a permanent backup is copied to `pre-greg/`; the last 3 timestamped backups per save are kept — all under `Documents/DatacenterBackups` (Windows) or `~/Documents/DatacenterBackups` (Linux, `~/DatacenterBackups` fallback). No backup, no mod writes: if a backup fails, gregCore skips its own writes and the vanilla save proceeds untouched.
- Device identity (`gregID`, e.g. `gregID:Switch:A1B2C3…`) is invisible: screens and object names stay vanilla, IDs live in hidden fields + the inventory sidecar (`greg_inventory.<save>.tsv`). You never need to touch them — but never hand-edit sidecar files either.

## Multiplayer (co-op)

gregCore is **local-only**. Data Center owns lobby, transport, and sessions natively; gregCore adds no replacement networking ([Developer Native Coop](./developer-native-coop/)). Expect mods to affect your local game; if you play co-op, all players should run the same mod set (mod authors can declare dependencies with `GregModDeps`, and manifests can be diffed with `GetLocalManifest`/`DiffManifests`).

## Filing a useful report

1. Reproduce once, noting: game version, gregCore version (console → `version`), loader (MelonLoader/BepInEx) + version, mod list + versions (console → `mods`).
2. Attach the relevant log excerpt (`[gregCore]`, `[gregCore][HwId]`, `[DynamicPatcher]`, MelonLoader errors).
3. Screenshots help for UI issues (Hub, HUD, panels).
4. Report to the **mod's** tracker first when only one mod misbehaves; to **gregCore** when the Hub/HUD/consoles/saves themselves break. Security issues follow `SECURITY.md` (never a public issue).
