# Guidebook Modelling Troubleshooting

Every modelling failure, with the exact log line and the fix. General debugging loop: [[Guidebook Debugging]].

| Symptom | Log line (`[gregCore][Mods] …`) | Fix |
|---|---|---|
| Registration returns false, no item | `ObjImport: Kein .obj-Pfad` | `model` must end in `.obj` (case-insensitive) |
| Registration returns false | `ObjImport: Datei nicht gefunden` | Filename typo or wrong subfolder; `model` is relative to the (sub)folder you passed |
| Registration returns false | `ObjImport: Datei zu gross (… Limit 67108864)` | Decimate/compress below 64 MB, re-export |
| Registration returns false | `bricht aus dem Pack-Ordner aus` | No `..`, `/`, or absolute paths in `model`/`subfolder` — keep files inside the mod folder |
| Registration returns false | `Import fehlgeschlagen` / `lieferte null` / `ohne Vertices` | Broken OBJ: re-export with normals + UVs, triangulated, no zero-area faces (see [[Guidebook Modelling OBJ Blender]]) |
| Registration aborted | `Mesh precheck negative — registration aborted` | Any of the above — fix the mesh, not the spec |
| `false`, folder complaint | `Pack folder missing` / `Pack folder or folder name empty` | Create the subfolder; pass its name exactly |
| `false`, no model | `No ModelFile specified` | `model` key missing/empty in the spec |
| `false` on C# early call | `ModLoader.instance not available (scene not ready yet?)` | Register after load (e.g. `OnReady`/scene state), never in a static constructor |
| Item registers but looks wrong | (no error) | `scale`/`size_u` pass-through: adjust spec, check `ColliderSize/Center` against mesh bounds |
| Item registers, surface broken | (no error) | Multi-material or missing UVs — single material + unwrapped UVs, re-export |
| Wrong shop behaviour/type | (no error) | `type` must be a valid `PlayerManager.ObjectInHand` name (case-insensitive); invalid → vanilla default |
| Old item ID reused | save/load anomalies | Never reuse IDs or GUIDs (ranges `100`, `1000/2000/3000`, `9001+`); see [[Developer Shop Items]] |

Still stuck: minimal repro (one OBJ + 5-line spec), full log excerpt with the `ObjImport:`/`CustomItems:` lines, and the exact spec table → mod tracker or Discord.
