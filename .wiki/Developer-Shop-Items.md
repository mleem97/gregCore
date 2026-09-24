# Developer Shop Items

Sell custom hardware in the vanilla shop: stable IDs, prefab routing, cart, save compatibility.

## ID ranges (never reuse)

High, stable, unique per item: `100`, `1000/2000/3000` series, `9001+`. Never reuse an ID or GUID once shipped — saves and sidecars reference them.

## The flow

1. **Mesh check first** — `GregObjImport` validates the model before anything else; ship meshes as files in the mod folder (`GregCustomItems` + `GregObjImport`), traversal-guarded, `GetPrefab` resolved per item.
2. **Prefab routing** — prefix `GetPrefabForItem`: clone a vanilla base prefab for your `itemId` (`GregShopItems.RegisterPrefab(itemId, baseId, resolver)` / `Unregister / TryGetBaseId / TryResolvePrefab`).
3. **Shop section** — find or create the `HL Mods` section (`FindTemplate(shop, ObjectInHand)`, `FindSection(shop, "HL Mods")`), then `AddButton(template, parent, itemId, label, price, xp, guid, isCustomColor, sprite)`: clones the `ShopItem` row + new `ShopItemSO{itemName, price, xpToUnlock, itemID, itemType, sprite, isCustomColor, eol, guid}`, extends the `Il2CppReferenceArray`, dedups by GUID.
4. **Cart** — vanilla `ButtonBuyShopItem` + `ShopCartItem.Initialize / BuyAnotherItem / UpdateCartTotal`; read lines via `ReadCartLine`, mutate with `CartAddOne(OnAddClicked) / CartRemoveOne`. Per-unit spec, pending cap `12 → 200`.
5. **Checkout** — `ShopPatch` emits `greg.SYSTEM.ButtonCheckOut`; compat `CommonShopPatch` routes `ButtonShopScreen → InjectAll`, `ButtonCheckOut → OnCheckout(quantity)`.
6. **Save compat** — items persist via `GregModPack` DTOs + `GregModSave` (keyed `modFolderName`); stale-UID color fix included. Guard: `RegisterVanillaModuleMap` fallback so vanilla loads stay valid.

## Lua shortcut

```lua
greg.items.register_shop_item("my_cooler", {
    name = "Ice Cooler", price = 499, xp = 10,
    size_u = 2, mass = 5, scale = 1,
    model = "cooler.obj", texture = "cooler.png", icon = "cooler_icon.png",
    type = "ServerCooler",
})
greg.items.register_static_item("deco_plant", { name = "Plant", model = "plant.obj", ... })
-- spec keys accept snake_case or PascalCase
```

Meshes ship as files in the mod folder. Buying through the shop is covered; **direct world spawning stays C#** (out of Lua scope by design).

## C# shortcut (`GregShopItems` / `GregShop`)

`ReadDefinition / ReadItem / FindItems / UnlockItem(UnlockButton) / BuyItem(ButtonBuyItem)`, `ReadCartLine / CartAddOne / CartRemoveOne`, `FindModItems / GetModId / ReadModConfig (via GregModPack) / BuyModItem`, picker `FindPicker / IsPickerOpen / OpenPicker / CancelPicker / Get+SetPickerColor`. Note: using `GregShopItems` directly implies a hard dependency (`references/gregCore.dll`, `Private=false` off) — see `docs/modding/hard-dependency.md`.

## Checklist

- [ ] IDs from the high ranges, GUIDs unique, never recycled.
- [ ] `HL Mods` buttons dedup by GUID; cart totals update; checkout emits.
- [ ] Save → load round-trip with the mod removed still loads vanilla.
- [ ] Pending-quantity cap and per-unit spec respected.
