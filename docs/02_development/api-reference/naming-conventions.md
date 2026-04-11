---
id: fmf-hook-naming
title: FMF hook and event naming
slug: /reference/fmf-hook-naming
description: Canonical naming for hooks, events, and cross-language documentation stubs.
---

# FMF hook and event naming

## Target format

All **new** public hook and event identifiers should follow:

```text
FMF.<Domain>.<EventOrHook>
```

- **`FMF`** — Fixed prefix (Greg Mod Framework / **gregFramework** hook namespace).
- **`<Domain>`** — Uppercase domain from the [approved domain list](#approved-domain-segments). Describes *where* the signal belongs in the game (player, rack, server, economy, …).
- **`<EventOrHook>`** — `PascalCase` segment(s), usually `OnSomething` for events or a verb phrase for commands.

Examples (illustrative): `FMF.RACK.CableSpinnerColorResolved`, `FMF.PLAYER.InputPoll`, `FMF.NETWORK.Cable.OnConnected`.

## Approved domain segments

Domains are **closed by default**. Add a new domain only via changelog + maintainer review.

| Domain | Scope |
|--------|--------|
| `GAMEPLAY` | Rules, scoring, simulation beats not covered elsewhere |
| `PLAYER` | Local player input, camera, UI focus |
| `EMPLOYEE` | NPC staff, hiring, schedules |
| `CUSTOMER` | Contracts, SLA, satisfaction |
| `SERVER` | In-game server racks, VMs, power |
| `RACK` | Physical rack placement, mounting |
| `NETWORK` | Cables, switches, traffic |
| `STORE` | Shop cart, checkout |
| `WORLD` | Rooms, expansion, walls |
| `UI` | HUD overlays, menus (when not `PLAYER`) |
| `SAVE` | Save/load lifecycle |
| `FRAMEWORK` | Loader, bridge, diagnostics |

## Runtime IL2CPP (MelonLoader): `greg.*`

Harmony patches in **gregFramework** emit stable **`greg.<DOMAIN>.<Action>`** strings via `GregHookName` / `GregEventDispatcher`. That surface is documented in **[greg hooks catalog (IL2CPP)](/wiki/reference/greg-hooks-catalog)** (`greg_hooks.json`, regeneration, overlap with hand-written `HarmonyPatches`). It is separate from the `FMF.*` / `FFM.*` documentation constants below.

## Native pipeline: `GregNativeEventHooks` (replaces `HookNames`)

Numeric **`EventIds`** are mapped to canonical **`greg.*`** strings in **`GregNativeEventHooks`** ([source](https://github.com/mleem97/gregFramework/blob/main/gregCore/framework/Sdk/GregNativeEventHooks.cs)), aligned with **`greg_hooks.json`** where that file names the patched method. Logging uses **`GregNativeEventHooks.Resolve(uint)`**; unknown ids → **`greg.SYSTEM.UnmappedNativeEvent`**.

**`GregCompatBridge`** ([source](https://github.com/mleem97/gregFramework/blob/main/gregCore/framework/Sdk/GregCompatBridge.cs)) redirects **deprecated** hook spellings (including older documentation-style names) to the current **`greg.*`** string — see also `legacy` entries in **`greg_hooks.json`**.

**Policy**

- New **documentation** identifiers: **`FMF.<Domain>.*`** as above.
- New **runtime** subscriptions: always **`greg.*`** via **`GregHookName.Create`** or **`GregNativeEventHooks`** constants — see [greg hooks catalog](/wiki/reference/greg-hooks-catalog).

## Cross-language stubs (documentation)

For each canonical hook, the wiki may add **non-normative** snippets:

| Language | Convention |
|----------|------------|
| C# | `FMF.Domain.OnSomething.Subscribe(...)` or string literal |
| Lua | `FMF.Domain.OnSomething:subscribe(...)` |
| Rust | `fmf::domain::on_something::subscribe(...)` |
| Python | `fmf.domain.on_something.subscribe(...)` |
| TypeScript | `FMF.Domain.OnSomething.subscribe(...)` |

Bindings are **not** auto-generated for all languages; stubs are for contributor clarity.

## Related

- [greg hooks catalog](/wiki/reference/greg-hooks-catalog) — EventId → **`greg.*`** (generated)
- [FMF hooks catalog](/wiki/reference/fmf-hook-naming) — Short overview / Redirect
- [FMF hooks](/wiki/reference/fmf-hook-naming) — declarative surface (may be superseded by the core registry)

