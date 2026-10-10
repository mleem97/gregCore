# Guidebook Modelling Overview

Bring your own 3D models into Data Center as buyable hardware or static deco. **OBJ only, static only** — no animation, no rigging, no skinned meshes. If you can export a clean static OBJ from Blender, you can do this track.

> Pipeline code: `GregObjImport` (validation) → `GregCustomItems` (vanilla `ModLoader` loading) → `GregModPack` (DTOs) → shop/cart/save. Lua surface: `greg.items`. Full spec tables: [[Guidebook Modelling Shop Item]] and [[Guidebook Modelling Static Item]].

## The pipeline (same for every item)

```
Blender (or any modeller)
  → model.obj (≤ 64 MB, must import with vertexCount > 0)
  → mod folder: model.obj + texture.png + icon.png
  → mesh pre-check (GregObjImport: .obj only, no ".." escapes, vanilla ObjImporter trial)
  → DTO (ShopItem / StaticItem: names, files, scale, size, mass, type)
  → vanilla ModLoader.LoadShopItem / LoadStaticItem
  → shop ("HL Mods") or static placement, prefabs via GetPrefab
```

Every step is best-effort with a clear log line (`[gregCore][Mods] ObjImport:` / `CustomItems:`). A failed pre-check **aborts registration with a warning** instead of a silent vanilla drop — read the log first when an item is missing.

## Static only — what that means

- The importer is the vanilla `Il2Cpp.ObjImporter.ImportOBJ`: pure triangle soup. Armatures, animations, shape keys, and skinning do not survive — strip them before export (see [[Guidebook Modelling OBJ Blender]]).
- Physics comes from data, not the mesh: `Mass`, `ColliderSize`, `ColliderCenter` (and `IsKinematic` for statics) in the DTO. The mesh is visual; the collider is numbers you provide.
- Placement behaviour (rack units via `SizeInU`, buying via shop, spawning via vanilla loader) is game-owned; your model rides along.

## The two item kinds

| | Shop item | Static item |
|---|---|---|
| Lua | `greg.items.register_shop_item(subfolder, spec)` | `greg.items.register_static_item(subfolder, spec)` |
| C# | `GregCustomItems.RegisterShopItem(folderPath, folderName, dto)` | `GregCustomItems.RegisterStaticItem(folderPath, folderName, dto)` |
| DTO | `GregModPack.ShopItem` (price, xp, size_u, mass, type, icon…) | `GregModPack.StaticItem` (scale, position, rotation, kinematic…) |
| Shows up | Shop, “HL Mods” section, cart, checkout | Static/deco placement via the vanilla loader |
| Needs | Unique high ID (`100`, `1000/2000/3000`, `9001+`), valid `ObjectType` | A pack folder + model file; no ID, no price |

Spec keys accept `snake_case` or `PascalCase` (Lua aliases + case-insensitive DTO binding — verified in `LuaItemsModule`).

## What you need

- Blender (or equivalent) + this track's export checklist ([[Guidebook Modelling OBJ Blender]]).
- A text editor (Lua) or .NET 6 SDK (C#) — same prerequisites as the other tracks ([[Guidebook Prerequisites]]).
- Patience for one test loop: export → register → check log → look in-game → adjust scale → repeat.

## Track order

1. [[Guidebook Modelling OBJ Blender]] — the OBJ contract + export checklist (read first, saves hours).
2. [[Guidebook Modelling Shop Item]] — buyable hardware end-to-end.
3. [[Guidebook Modelling Static Item]] — deco/statics end-to-end.
4. [[Guidebook Modelling Troubleshooting]] — every failure mode with log line + fix.
