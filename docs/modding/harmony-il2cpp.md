# Harmony Style + IL2CPP Pitfalls

> Patch rules of the team mods: smallest surface, defensive methods, zero cost
> per frame. Everything here is as written in code (Backplanes, MoreModules,
> MoreSpools, HexViewer).

## 1. Keep the patch surface minimal

- `harmony.PatchAll(typeof(Patches))` (explicit) or `HarmonyInstance.PatchAll()`.
- One job per patch; heavy work goes in dedicated classes
  (Backplanes: thin `Patches`, thick `CatalogInjector`).
- Every patch method: `try/catch` + null checks, a mod must **never** take down
  the game thread. Missing targets (after game updates) fail silently in Harmony —
  actively remove dead code (v1.x `ShopCartItem.AddSpawnedItem` hook).
- Prefix returns: `true` = original runs, `false` = swallow (only with reason,
  e.g. custom IDs in the `GetPrefabForItem` prefix with `ref __result`).

## 2. No per-frame reflection

- Cache `Type.GetType` probes **once** (`bool? _hasCore`).
- Gate scans/sweeps by timestamp (1/s repair window, 2 s panel refresh,
  0.1 s HUD throttle, 30 s rescans) instead of per frame.
- `FindObjectsOfType` only in windows/on button press (Verify/Repair), never in `OnUpdate`.
- Hold state via events instead of polling (template `PortSpeedMemory`, Backplanes):
  register each port once with target speed (instance ID + speed), re-assert drift
  in `InsertSFP`/`SetConnectionSpeed` postfixes via direct field write.
  Self-cleaning stale entries (destroyed cables, recycled IDs) via reference check,
  map size bounded by sweep threshold.
- Material animation (template `RgbAnimator`, Backplanes): only touch tracked slots
  (pointer key + material/property), drop dead servers via liveness check,
  time-based hue rotation — never walk all renderers per frame.

## 3. Using IL2CPP types correctly

- Arrays are fixed: copy via `new Il2CppReferenceArray<ShopItem>(old.Length + 1)`
  instead of resize (shop, slot arrays).
- `Il2CppSystem.Nullable<Color>` for optional colors
  (`cartItem.Initialize(shop, name, id, price, type, noCustomColor)`).
- Don't cache and hold objects forever: IL2CPP GC invalidates native pointers —
  rebuild on demand instead (MoreModules template rule) or use pointer keys
  (`RepairGuard` by native `IntPtr`, **not** by boxed wrapper —
  otherwise stack overflow via missed re-entrancy guards).
- Inactive template holders (`SetActive(false)` + `DontDestroyOnLoad`) so the
  `UsableObject` tracker ignores templates; `activeInHierarchy` checks before access.
- Delegates to the game via `DelegateSupport.ConvertDelegate<T>`; some events
  exist only as `add_/remove_` methods (e.g. Unity log feed).
- `renderer.materials` clones (names gain ` (Instance)`); never recolor shared
  materials directly.

## 4. After game updates: compatibility routine

1. Clear only generated caches, let interop assemblies regenerate.
2. Start once without optional mods, then re-add in controlled groups.
3. Verify hook signatures against the live assembly (fields/methods from
   section 3), delete dead hooks. Keep UserData configs and saves.
