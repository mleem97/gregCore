---
title: "Guidebook Modelling OBJ Blender"
---
# Guidebook Modelling OBJ Blender

The OBJ contract enforced by `GregObjImport` (`src/gregCore.Core/Mods/GregObjImport.cs`) plus a Blender export checklist that satisfies it. Read this before modelling anything.

## The enforced contract (exact)

| Rule | Source | Failure log |
|---|---|---|
| Extension must be `.obj` (case-insensitive) | `TryValidatePath` | `Kein .obj-Pfad` |
| File must exist and be ≤ **64 MB** (`MaxObjBytes`) | `TryValidatePath` | `Datei nicht gefunden` / `Datei zu gross` |
| Path must stay inside the pack/mod folder (`..` escapes rejected) | `ImportMeshForPack` | `bricht aus dem Pack-Ordner aus` |
| Vanilla `ObjImporter.ImportOBJ` must return non-null with `vertexCount > 0` | `ImportMesh` | `Import fehlgeschlagen` / `lieferte null` / `ohne Vertices` |

All paths funnel through `ImportMeshForPack(folderPath, modelFile)` before `GregCustomItems` touches the vanilla `ModLoader` — a negative pre-check aborts registration (`Mesh-Vorabcheck negativ — Registrierung abgebrochen`). Warnings are prefixed `[gregCore][Mods] ObjImport:` / `CustomItems:`.

## Blender export checklist

1. **Strip animation**: delete armatures, bake/remove shape keys, no skinning — none of it survives `ImportOBJ`. Keep one static mesh (joined with Ctrl+J where sensible).
2. **Apply transforms**: Ctrl+A → All Transforms. Model in meters (Unity units); set origin to the base center of the object.
3. **Triangulate**: OBJ is triangles downstream — add a Triangulate modifier or Faces → Triangulate so *you* control the result.
4. **Clean geometry**: remove doubles, recalculate normals outside, no zero-area faces (they inflate vertex counts or import as holes).
5. **UV + single material**: unwrapped UVs, one material with your texture assigned. Multi-material and vertex-color-only meshes arrive with missing/broken surfaces — verify in-game, keep it to one material.
6. **Export**: Wavefront OBJ, with normals + UVs written, default object grouping. Keep the file ASCII and well under 64 MB (decimate before export, not after a failure).
7. **Name files plainly**: `cooler.obj`, `cooler.png`, `cooler_icon.png` — referenced by filename in the spec (`model`, `texture`, `icon`).

## Scale sanity

- `ModelScale` multiplies at load; `SizeInU` declares rack units; start with `scale = 1`, `size_u` matching the real device height, and adjust after seeing it next to vanilla hardware.
- Colliders are manual numbers (`ColliderSize`, `ColliderCenter`) — match them to the mesh bounds; a wrong collider feels like a broken model even when the mesh is perfect.

## Checkpoint

- [ ] OBJ imports with vertices (test the exact file, not a re-export).
- [ ] ≤ 64 MB, inside the mod folder, plain filenames.
- [ ] No armature/animation data left in the blend file.

Next: [Guidebook Modelling Shop Item](./guidebook-modelling-shop-item/).
