---
title: "Guidebook Modelling Static Item"
---
# Guidebook Modelling Static Item

Ship deco/statics with your own mesh. DTO: `GregModPack.StaticItem` — note the differences from shop items: **no price, no xp, no icon, no ID**; instead `Position`, `Rotation`, `IsKinematic`.

## 1. Folder + files

```
UserData/gregCore/Mods/Lua/<modId>/deco_plant/
├── plant.obj
└── plant.png
```

## 2. Lua registration

```lua
function on_init()
    local ok = greg.items.register_static_item("deco_plant", {
        name = "Lobby Plant",
        scale = 1,
        model = "plant.obj", texture = "plant.png",
    })
    greg.ui.log_info("Plant registered: " .. tostring(ok))
end
```

## 3. Spec reference (exact DTO mapping)

| Spec key | DTO field | Default | Notes |
|---|---|---|---|
| `name` | `ItemName` | `""` | Display name |
| `scale` | `ModelScale` | `1` | Mesh scale multiplier |
| `model` | `ModelFile` | `""` | **Required** `.obj` filename |
| `texture` | `TextureFile` | `""` | Texture filename |
| `collidersize` / `collidercenter` | `ColliderSize` / `ColliderCenter` | empty | Optional float arrays |
| `position` | `Position` | empty | Optional float array (spawn placement) |
| `rotation` | `Rotation` | empty | Optional float array (spawn placement) |
| `iskinematic` | `IsKinematic` | `false` | Kinematic physics flag |

An `icon` key is harmless (ignored — the DTO has no such field) but does nothing for statics.

## 4. C# equivalent

```csharp
var dto = new GregModPack.StaticItem
{
    ItemName = "Lobby Plant", ModelScale = 1f,
    ModelFile = "plant.obj", TextureFile = "plant.png",
    IsKinematic = true,
};
bool ok = GregCustomItems.RegisterStaticItem(folderPath, folderName, dto);
```

Same rules as shop items: folder must exist, mesh pre-check first, `ModLoader.instance` required, `GetPrefab(folderName)` for variants/overlays.

## Checkpoint

- [ ] `register_static_item` returns true; log line `Custom static item registriert`; item appears via the vanilla static flow.
- [ ] Kinematic/physics behaviour as intended; no save pollution on uninstall.

Next: [Guidebook Modelling Troubleshooting](./guidebook-modelling-troubleshooting/).
