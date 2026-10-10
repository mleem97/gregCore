# Keybinds — no mod bites another mod's keys

> Registry detects conflicts; since this change it also resolves them.
> Code: `src/gregCore.UI/Infrastructure/Settings/GregKeybindRegistry.cs`,
> model: `src/gregCore.Abstractions/KeybindEntry.cs`, polling:
> `GregInputBindingService`, persistence: `GregSettingsPersistenceService`
> (`gregCore_Keybinds.json`), validation: `GregSettingsConflictService`.

## Problem

Every mod picked its own toggle key (`F7` here, `F7` there) and mods bit
each other — e.g. UnityExplorer and gregMod.Trainer both fired on `F7`.
The registry flagged `HasConflict`, but nothing moved: two actions stayed
on one key.

## Design (register → auto-resolve → persist)

Mods register toggles (C#: `Sdk.GregAPI.RegisterKeybind`, Lua/Python/JS via
the SDK bridges) with `ModId + ActionId + DefaultKey + OnPress`. On register:

1. Persisted `CurrentKey` wins (survives sessions via `gregCore_Keybinds.json`).
2. Otherwise the default applies — **unless** it is game-reserved
   (`Escape`, `F1`) or already taken by another entry.
3. Then the entry moves to the first free key of `FallbackPool`
   (`F12…F2`, then `Insert/Home/End/Delete/PageUp/PageDown`), loudly logged
   (`auto-resolved … old -> new`) and flagged `AutoResolved`.
4. Pool exhausted → entry keeps its key and `CheckConflicts` flags
   `HasConflict` as before (manual fix in settings).

`GregInputBindingService` polls `CurrentKey`, so resolved keys work end to
end without mod changes. `Rebind`/`ResetToDefault` keep working; manual
rebinds can still collide on purpose (flagged, never force-moved).

## Adoption (first adopter: gregMod.Trainer)

`gregMod.Trainer` registers its panel toggle (`RegisterCoreExtras`, JIT-split,
standalone fallback intact): with gregCore it hands polling to
`GregInputBindingService` (no double-toggle — own polling switches off via
`CoreHandlesToggle`) and adopts the effective, possibly auto-resolved key for
HUD + panel labels. Without gregCore nothing changes. Pattern for other mods:
register in the core-gated method, gate own polling on success, adopt the
effective key for display.

## Limits (read before relying on it)

- The allocator sees **registered entries + game-reserved keys only**. Mods
  that never register (own MelonPreferences toggles, third-party mods like
  UnityExplorer) stay invisible — adoption is what makes the fleet quiet.
  Roadmap: migrate Greg toggles onto `RegisterKeybind`; external tools keep
  their own guards (e.g. `gregPlugin.ExplorerBridge` pre-seed).
- `KeyCode`-based (legacy). New-Input-System `Key` users map through
  `GregInputBindingService.KeyCodeToKey`.
- `Backquote` (MelonLoader console default on some setups) is deliberately
  NOT reserved and NOT in the pool — unverified, see COMPATIBILITY notes.
