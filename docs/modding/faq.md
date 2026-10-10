# FAQ: Typical Mod Bugs

> Symptoms from real mod sessions — with cause and fix.

## Panel opens, but text invisible

Toolkit default font without game font. Fix: apply `GregFontLoader.DefaultUGUIFont`
(null-tolerant) to all labels/buttons, mod-local fallback without core
(see [UI Panels](ui-panels.md)).

## Clicks on panel buttons get lost

No EventSystem in the game. Fix: explicit `RegisterCallback<ClickEvent>` **plus**
manual `RouteClicks()` fallback per frame (500 ms double-fire guard).

## `Method unstripping failed` in the log

IMGUI/stripped methods called in the IL2CPP build (hit HexViewer).
Fix: switch to UIToolkit, no IMGUI windows for game functions.

## Crash without gregCore (JIT TypeLoad)

gregCore types in methods that also run without core. Fix: `GregHost.HasCore` probe,
gregCore code only in separate methods, only called when `true`.

## Stack overflow on save load (0xC00000FD)

Re-entrancy guard with `HashSet<object>` over **boxed** Il2Cpp wrappers never hits
(every access boxes anew). Fix: guard by native `IntPtr` + depth cap.

## Bought custom items arrive white / only 1 of N colored

- Shop overlay hides the vanilla color picker → own picker or restore vanilla flow
  (no direct purchase without color).
- Vanilla calls `ApplyColorToSpawnedItem` with stale UID (e.g. always 1):
  redirect to freshest checkout spawn; `spawnedItems` is never cleared
  (don't recolor stale keys!). Spread line quantity over consecutive spawns,
  force-apply uncolored ones.
- Modules/ports: never write `sfpTypeInserted` without a real module (phantom module
  blocks real modules + renders nothing).

## Ports stuck at 1 Gbps

Effective rate = `min(server port, module, cable, switch port)`. Raise empty ports
early via `CableLink.Start` postfix to tier cap; re-assert once after module insert
(`InsertSFP` postfix, raise only). Otherwise never touch connected ports.
Measure instead of guessing: audit connected ports read-only (cap/module/cable/far end).

## RGB/material animation eats frames

Walking all renderers per frame + `materials` access (clones!) doesn't scale.
Fix (Backplanes `RgbAnimator` pattern): only touch registered slots
(pointer key + material/property), drop dead servers via liveness check,
time-based hue rotation.

## Bulk purchases (30+ units) misassigned

Price-peek as primary key fails at shared price points. Fix: checkout snapshot
with one spec **per unit** in cart order (expand quantity), prefab family as
drift correction, generous pending cap + expiry, verify afterwards
(see [Shop Items](shop-items.md), section 4).

## Duplicate shop buttons / double purchases

Two mods own the same ID ranges (MoreModules vs. MoreServers vs.
RealisticModules). Fix: `RegisteredMelons` check, loser stays inert
(`s_disabledBySibling`, log error).

## Game update broke hooks

Dead targets fail silently in Harmony. Fix routine: regenerate generated caches,
start once without optional mods, verify hook signatures against live assembly,
delete dead hooks (see [Harmony + IL2CPP](harmony-il2cpp.md)).

## Trolley flies around when loaded

Mass too small. Fix: `Rigidbody.mass` multiplier + `angularDrag` multiplier
(LargerCart `CartStabilizer` pattern, configurable, once per scene).
