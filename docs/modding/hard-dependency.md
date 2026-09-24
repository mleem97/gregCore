# Hard dependency on gregCore: when and how

> Reference implementation: `gregMod.HexViewer` v1.0.8 (deleted 351 lines
> of own UI, see its CHANGELOG). Rule of thumb and checklist below.

## Why

Additive APIs (`GregMenuBinding`, toasts, sidecars, `GregShop` reads) work
behind a `GregHost.HasCore` guard with a vanilla fallback. But anything
that *replaces* a vanilla path — shop injection (`GregShopItems`),
own UIDocument/toast/input layers, pref-gated features — cannot sit
behind a guard without dual code paths (more code, not less). If your
mod carries any of these, a hard dependency deletes real code:

| Standalone cost | Deleted by hard dep | Example |
|---|---|---|
| Own UIDocument + toast + input-guard + font layer | Toasts, panels, `GregInputLock`, `GregFontLoader` | MusicPlayer `UI/ModLocalUI.cs`, `UI/PanelChrome.cs` |
| Own IMGUI windows, scroll fallbacks, textures | `GregPanelBuilder`, toasts | HexViewer (−351 lines) |
| Hand-rolled shop injection (registry, prefixes, buttons) | `GregShopItems` | MoreSpools, MoreModules, MoreServers (~100 lines each) |
| Hand-rolled menu opener/closer/state | `GregMenuBinding.BindToggle` | works guarded too — no hard dep needed |

## Checklist (HexViewer playbook)

1. Reference: `references/gregCore.dll` symlink + csproj `Reference`
   (`Private=false`). Keep the DLL in `Mods/` next to yours at runtime.
2. Delete `GregHost.cs` and every `HasCore` guard/fallback.
3. Replace: IMGUI windows → `GregPanelBuilder` panels; event feedback →
   `GregNotificationManager.Show` (`ShowRich` with cover for swatches);
   menu wiring → `GregMenuBinding.BindToggle`; persistence →
   `GregSaveGuard.RegisterSidecar`.
4. Fail fast in `OnInitializeMelon` when the probe
   (`gregCore.UI.GregNotificationManager, gregCore`) misses: error log,
   set a `_disabled` flag, return early — and check it in `OnUpdate`
   (never let core-touching code JIT without the DLL).
5. Bump minor version, note the hard dep in README + workshop
   description (`needsgreg: true` in `metadata.json`).

## Per-mod verdicts (Sept 2026)

- **HexViewer**: done (template for everyone else).
- **MusicPlayer, Backplanes**: carry standalone UI layers —
  biggest deletion candidates, decision open.
- **NoEOL**: done (v2.0.0 hard dep; probe/guards/theme gone).
- **Speedtest**: born hard-dep; uses `GregCables.FindByServer`.
- Defensive patching: `GregPatches.TryPatchPrefix/TryPatchPostfix`
  (adopted by NoEOL `EolHider`; pattern for CableThrottle-style mods).
- **IPAM**: custom IMGUI overlay must stay (no core ImGui framework);
  already optimal (additive use only).
- **MoreSpools/Modules/Servers, NoCostShop, CableThrottle, CableTracer,
  LargerCart**: standalone by design (partly external authors) — no
  hard dep without author decision. `GregShopItems` waits for opt-in.
- **GameExport, Inventory**: additive use sufficient.
