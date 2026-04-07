# Vendored MelonLoader references (Live-Sync)

This folder mirrors `MelonLoader/net6` and `MelonLoader/Il2CppAssemblies` from your **Data Center** install.

- **Do not commit** `*.dll` files (see `.gitignore`).
- Run from the repo root:

```bash
python tools/refresh_refs.py
```

Or set `DATA_CENTER_GAME_DIR` to your game root, or pass `--game-dir "C:\Program Files (x86)\Steam\steamapps\common\Data Center"`.

After sync, `FrikaMF.csproj` will prefer `lib/references/MelonLoader` when `net6/MelonLoader.dll` exists, so IDEs and CI can build without a Steam path.

Optional: `python tools/refresh_refs.py --watch` polls for interop changes after game updates.

See also: `MANIFEST.txt` (generated) and `tools/diff_assembly_metadata.py`.
