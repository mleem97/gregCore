# Guidebook Modelling Shop Item

Ship buyable hardware with your own mesh: folder → files → spec → shop. DTO: `GregModPack.ShopItem` (`src/gregCore.Core/Mods/GregModPack.cs`).

## 1. Folder + files

```
UserData/gregCore/Mods/Lua/<modId>/my_cooler/
├── cooler.obj        (≤ 64 MB, static, triangulated)
├── cooler.png        (texture)
└── cooler_icon.png   (shop icon)
```

C# mods use a pack folder with the same content (any path you pass as `folderPath`).

## 2. Lua registration

```lua
function on_init()
    local ok = greg.items.register_shop_item("my_cooler", {
        name = "Ice Cooler", price = 499, xp = 10,
        size_u = 2, mass = 5, scale = 1,
        model = "cooler.obj", texture = "cooler.png", icon = "cooler_icon.png",
        type = "ServerCooler",
    })
    greg.ui.log_info("Cooler registered: " .. tostring(ok))
end
```

## 3. Spec reference (exact DTO mapping)

| Spec key | DTO field | Default | Notes |
|---|---|---|---|
| `name` | `ItemName` | `""` | Display name (required, non-empty in practice) |
| `price` | `Price` | `0` | Shop price |
| `xp` | `XpToUnlock` | `0` | XP gate |
| `size_u` | `SizeInU` | `1` | Rack units |
| `mass` | `Mass` | `1` | Physics mass |
| `scale` | `ModelScale` | `1` | Mesh scale multiplier |
| `model` | `ModelFile` | `""` | **Required** `.obj` filename in the folder |
| `texture` | `TextureFile` | `""` | Texture filename in the folder |
| `icon` | `IconFile` | `""` | Shop icon filename |
| `type` | `ObjectType` | `""` | Must parse as `PlayerManager.ObjectInHand` (case-insensitive); garbage → vanilla default |
| `collidersize` / `collidercenter` | `ColliderSize` / `ColliderCenter` | empty | Optional float arrays; match mesh bounds |

Keys work in `snake_case` or `PascalCase` (`LuaItemsModule` aliases + case-insensitive binding). Returns `bool`; `false` + log line on any failure — never a crash.

## 4. C# equivalent

```csharp
var dto = new GregModPack.ShopItem
{
    ItemName = "Ice Cooler", Price = 499, XpToUnlock = 10,
    SizeInU = 2, Mass = 5f, ModelScale = 1f,
    ModelFile = "cooler.obj", TextureFile = "cooler.png",
    IconFile = "cooler_icon.png", ObjectType = "ServerCooler",
};
bool ok = GregCustomItems.RegisterShopItem(folderPath, folderName, dto);
if (ok) { var prefab = GregCustomItems.GetPrefab(folderName); /* variants/overlays */ }
```

Requires `ModLoader.instance` (scene ready — register after load, not in a static constructor). `GetPrefab(folderName)` returns the loaded template prefab (`GetModPrefabByFolder`) for variants.

## 5. IDs + verify

- Item IDs from the high ranges (`100`, `1000/2000/3000`, `9001+`), never reused — saves reference them (see [[Developer Shop Items]]).
- Verify: loader log shows `Custom shop item registriert: 'Ice Cooler'`; in-game shop → “HL Mods” section → buy → checkout → place; reload the save with the mod present, then removed (vanilla must still load).

## Checkpoint

- [ ] `register_shop_item` returns true; log line present; item visible/buyable/placeable.
- [ ] Scale, size_u, and collider feel right next to vanilla hardware.

Next: [[Guidebook Modelling Static Item]].
