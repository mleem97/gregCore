---
title: System architecture & documentation principles
sidebar_label: Architecture principles
description: Canonical stack model (ModManager ? Framework ? Plugins ? Mods), priorities, and how wiki pages should align.
---

# System architecture & documentation principles

This page is the **canonical reference** for how gregFramework documentation describes the stack: **ModManager (MAUI) ? modding framework / SDK ? plugins ? mods**, plus priorities (stability first), the hook-proxy idea, and **authoring rules**. All wiki content must be written in **English** only.

Detail pages (individual mods, plugins, releases) should align with this model without repeating the full narrative each time — **link here** for the big picture.

## Layer model (target architecture)

Describe the runtime as a **layered system**, not a flat list of DLLs:

| Layer | Role | Typical workspace artifacts |
|--------|--------|------------------------------|
| **1. ModManager (front-end)** | UI to enable/disable mods and plugins, ordering, configuration, game state (e.g. no save loaded, level loading). Talks to the framework through **well-defined** interfaces (shared library, config files, IPC, named pipes, HTTP — depending on implementation). | `gregModmanager/` — **Gregtools Modmanager** (MAUI, e.g. `WorkshopUploader.csproj`). |
| **2. Modding framework / SDK** | Stable API surface for plugins and mods: lifecycle, events, versioning, dependencies, logging, error handling. Hooks Unity / MelonLoader / IL2CPP and **maps** low-level events to **framework events** (hook proxy). | `gregCore/` — e.g. `framework/` (**`gregCore` runtime**), Harmony integration, **native FFI** (`FfiBridge`), hook registry. |
| **3. Plugins** | Extend the framework (new services, hook types, optional ModManager UI). Clear extension points. | `greg.Plugin.*`, repos **`gregExt.<Name>/`**. |
| **4. Mods** | User extensions via the **documented** framework API; avoid direct IL2CPP details where possible; load in isolation; soft-fail on errors. | `greg.*`, repos **`gregMod.<Name>/`**. |

**Mnemonic:** `ModManager ? Framework ? Plugins ? Mods`.

### Hook proxy and hotloading (concept)

- The framework should map **Unity / IL2CPP events** (MelonLoader hooks, patches) to **stable, named framework events** (e.g. level loaded, scene changed, update) so mods do not couple to concrete Unity signatures.
- **Hotloading** mods is a target state: load only in **safe** states (e.g. no active save, menu), re-bind on level change — exact rules live in framework code and should appear in technical articles **only** when anchored in the repo.

These wiki pages do **not** mandate a specific implementation; they **align** authors and readers on the same vocabulary.

## Technical context (expertise areas)

Documentation and reviews in the gregFramework space typically assume:

- **C# / .NET** (modern language features, best practices)
- **Unity with IL2CPP**
- **MelonLoader** and modular **greg** / **greg** stacks
- **.NET MAUI** for the ModManager (deployment, installer, release vs debug issues)
- Debugging, logging, tracing, crash analysis (including outside the IDE)

## Priorities (when trade-offs arise)

When documentation or API design must choose, use this **order**:

1. **Stability and fault tolerance** — faulty mods must not tear down the whole system arbitrarily; clear error paths and logging.
2. **Clean architecture and maintainability** — clear layers, documented interfaces.
3. **Developer experience** — understandable APIs, hooks, logging for mod authors.
4. **Performance and low invasiveness** toward the game.
5. **Extensibility and long-term compatibility** — versioning, dependency rules.

## Rules for wiki authors

- **Terminology:** Always name the layer (ModManager, framework, plugin, mod). Do not conflate “plugin” and “mod” without context.
- **Language:** **English only** for all user-facing documentation in `docs/`, the homepage, and UI strings in this site.
- **Repos:** Keep paths such as `gregCore/`, `gregMod.*`, `gregExt.*`, `gregModmanager/` consistent with the [Workspace map](/wiki/getting-started/architecture) and [Repository architecture](/wiki/development/concepts/hooks-and-events).
- **No invented APIs:** New pages must not promise hooks or events that are not evidenced in core/registry — link to [greg hooks](/wiki/reference/greg-hook-naming) and the [Hooks catalog](/wiki/reference/greg-hooks-catalog).
- **Cross-links:** Entry [Developers & contributors](/wiki/developers), architecture [Repository architecture](/wiki/development/concepts/hooks-and-events), language rule [Modding language support](/wiki/developers).

## See also

- [Repository architecture](/wiki/development/concepts/hooks-and-events) — multi-repo layout and core
- [Getting started](/wiki/getting-started/quickstart) — workspace and build
- [Mods — Framework](/mods) — runtime from mod authors’ perspective
- [Plugins overview](/mods) — `greg.Plugin.*`
- [Mod developers](/wiki/developers)


