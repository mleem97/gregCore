# Steam Workshop template (Data Center)

Steam AppID: **4170200**.

## Files

- `workshop_item.vdf` — template for `steamcmd` / `upload.sh` in [`tools/steam-workshop-upload/`](../tools/steam-workshop-upload/README.md).
- `preview.png` — **add your own** 512×512 (or Steam-required size), under 1 MB recommended.

## Usage

1. Copy `workshop_item.vdf` into your mod project (e.g. `HexMod/workshop/hexmod.vdf`).
2. Replace `{{MOD_TITLE}}`, `{{MOD_DESCRIPTION}}`, `{{CHANGE_NOTES}}`, and paths `contentfolder` / `previewfile`.
3. Build your mod output into the folder referenced by `contentfolder`.
4. Run the upload script from `tools/steam-workshop-upload/` (see script README).

**Security:** do not commit Steam credentials. Prefer interactive login or CI secrets.
