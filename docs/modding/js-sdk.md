# JS/TS SDK for script mods (UI-focused)

> Runtime: `GregJsHost` (`src/gregCore.SDK/Language/Hosts/GregJsHost.cs`,
> Jint). Types: `templates/js/greg.d.ts`. Example: `templates/js/example-mod.ts`.

## Layout

`UserData/gregCore/Mods/JS/<modId>/*.js` — one isolated Jint engine per
mod. Write TypeScript against `greg.d.ts`, compile with `tsc`, ship the
emitted `.js`. Raw `.ts` files are skipped with a warning (no transpiler
in the game).

## greg API

Log: `greg.log/warn/error`. UI: `greg.toast(msg, sec?)`,
`greg.toastRich(top, title, sub, sec?)`, `greg.notify(title, msg, sec?)`,
`greg.createPanel(title)` (chainable Toolkit builder: `AddHeadline`,
`AddLabel`, `AddButton`, `AddSecondaryButton`, `AddToggle`, `AddSlider`,
`AddSeparator`, `AddSpacer`, `ClearContent`, `Show/Hide/Toggle`),
`greg.bindMenu(id, toggle, isOpen)`, `greg.reportMenu(id, open)`.
Settings: `greg.registerToggle/registerSlider/registerKey`.
Hooks: `greg.on(name, fn)` (same bus as Lua/C#).

Lifecycle (define when needed): `onUpdate(dt)`, `onSceneLoaded(name)`.
Throwing callbacks are disabled after the first failure (no log spam).

## HotLoad (main menu only)

Edit + save → change queued (500 ms debounce) → applied on next
main-menu entry (old engine dropped, fresh engine, files re-executed).
Never mid-game. Deleted mod dirs unload. Memory cap per engine: 16 MB.

Limits: only the `greg` object is a stable contract (direct .NET access
via AllowClr may break across versions); scripts must not Harmony-patch
— use hooks + `greg` instead.
