# Repository tools

## `Generate-GregHookCatalog.ps1`

Generates the Docusaurus page [`gregWiki/docs/reference/greg-hooks-catalog.md`](../../gregWiki/docs/reference/greg-hooks-catalog.md) from:

- `framework/Sdk/GregNativeEventHooks.cs` — canonical `greg.*` strings for the native / FFI pipeline
- `framework/ModLoader/EventDispatcher.cs` — `EventIds` numeric constants

Run from repo root:

```powershell
./tools/Generate-GregHookCatalog.ps1
```

Optional:

```powershell
./tools/Generate-GregHookCatalog.ps1 -SkipChangelogNote
```

Commit the regenerated `.md` when hook names or event ids change.
