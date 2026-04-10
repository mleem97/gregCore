# Repository tools

## `Generate-GregHookCatalog.ps1`

Generates the Docusaurus page [`wiki/docs/reference/greg-hooks-catalog.md`](../wiki/docs/reference/greg-hooks-catalog.md) from:

- `framework/FrikaMF/HookNames.cs` — string literals and `EventId` → hook mappings
- `framework/FrikaMF/EventIds.cs` — numeric event id constants

Run from repo root:

```powershell
./tools/Generate-GregHookCatalog.ps1
```

Optional:

```powershell
./tools/Generate-GregHookCatalog.ps1 -SkipChangelogNote
```

Commit the regenerated `.md` when hook names or event ids change.
