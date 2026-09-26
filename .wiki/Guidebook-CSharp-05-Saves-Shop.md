# Guidebook CSharp 05 Saves Shop

Persist state, address devices, and sell custom items. Theory: [[Core Save Engine]], [[Developer Hardware IDs Inventory]], [[Developer Shop Items]].

## Sidecars: hook your state into SaveGuard

```csharp
GregSaveGuard.RegisterSidecar("shift_helper_cs",
    save: () => SerializeMyState(),     // -> string
    load: data => RestoreMyState(data)); // <- string
GregSaveGuard.RegisterVanillaModuleMap("shift_helper_cs",
    toVanilla: id => MapToVanillaOrNull(id), fallbackId: 0);
```

Writes land in `<saveDir>/greg_shift_helper_cs.<save>.tsv` (atomic `.tmp` + `.bak`); backups in `Documents/DatacenterBackups/` (`pre-greg/` permanent + 3 rolling) — and only after a fresh backup succeeded (guarantee, see [[Core Save Engine]]). Sanitize (`SanitizeSaveData`, e.g. SFP `prefabID` clamp) keeps vanilla loads valid.

Typed options: `GregConfigService.LoadConfig<T> / SaveConfig<T>`; `GregPersistenceService` (`%AppData%/gregCore/Saves/<key>.json`, traversal-guarded); `GregModSave` (`Create / Fill / Read / ReadAll / Upsert / Remove` on `SaveData.modItemData`, keyed by `modFolderName`); `GregSaveEngine` LiteDB (`gregSave_<guid>.greg.db`, `greg_meta` + `grid_state`, writes gated by `GregFeatureGuard.SaveEngine.Write`).

## Devices: inventory UIDs, never labels

`GregEntityInventory` (`gregCore.Infrastructure.Persistence`) rebuilds on every load; entries are `Entry{Kind, NativeKey, Uid, Hint}` over `InventoryKind{Server, Switch, Router, Firewall, PatchPanel, Cable, SfpModule, LacpGroup}`:

```csharp
if (!GregEntityInventory.IsReady) return;   // rebuilt in the LoadNetworkState postfix
var servers = GregEntityInventory.GetAll(GregEntityInventory.InventoryKind.Server);
int n = GregEntityInventory.Count(GregEntityInventory.InventoryKind.Server);
bool ok = GregEntityInventory.TryFindLive(uid, out var go);              // uid -> live GameObject
bool known = GregEntityInventory.TryGetUid(GregEntityInventory.InventoryKind.Router, nativeKey, out var routerUid);
GregEntityInventory.Rebuilt += () => { /* re-resolve your cached handles here */ };
string report = GregEntityInventory.Verify();   // Dump() prints the whole table
```

Servers/switches/patch panels reuse `gregID:<Type>:<12HEX>` (`ServerPrefix`, `SwitchPrefix`, `PatchPanelPrefix`); cables/LACP use deterministic vanilla-derived UIDs (`UidPrefix = "gregUID:"`); routers/firewalls/SFP persist via `greg_inventory.<save>.tsv`. Tune via MelonPreferences `gregCore.EntityInventory` (`Enabled`, `VerboseLogging`, `DumpOnRebuild`). Keep `gameObject.name` vanilla; never invent a second ID scheme.

## Shop items: IDs, prefab, section, cart

ID ranges (never reuse): `100`, `1000/2000/3000` series, `9001+`.

```csharp
GregShopItems.RegisterPrefab(itemId, baseItemId, resolver);
GregShopItems.UnregisterPrefab(itemId);
GregShopItems.TryGetBaseId(itemId, out var baseId);
GregShopItems.TryResolvePrefab(itemId, out var prefab);
var section = GregShopItems.FindSection(shop, "HL Mods");   // created if missing
GregShopItems.AddButton(new GregShopItems.ShopButtonSpec { Template = template, Parent = section, ItemId = itemId, Label = label, Price = price, XpToUnlock = xp, Guid = guid, IsCustomColor = isCustomColor, Sprite = sprite });
GregShop.CartAddOne(...); GregShop.CartRemoveOne(...);      // stay on vanilla cart behavior
// persist via GregModPack DTOs (ShopItem/StaticItem/DllRef/Snapshot) + GregModSave
```

Checkout emits `greg.SYSTEM.ButtonCheckOut`; compat `CommonShopPatch` routes `ButtonShopScreen → InjectAll`, `ButtonCheckOut → OnCheckout(quantity)`. Cap pending quantity (`12 → 200`), per-unit spec, GUID-deduped buttons.

## Exercises

1. Register a sidecar round-trip; confirm save → load → mod-removed vanilla load.
2. Resolve three devices via inventory; subscribe `Rebuilt` and re-resolve.
3. Add one shop item end-to-end (mesh → prefab → `HL Mods` → cart → checkout → reload).

## Checkpoint

- [ ] State survives reload; devices addressed by UID; one custom item buyable.

Next: [[Guidebook CSharp Complete Project]] — ShiftHelper in C#.
