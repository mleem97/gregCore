# Guidebook Release

From working mod to a release others can install. Companion: [[Developer Publishing]].

## 1. Pre-release checklist (all languages)

- [ ] Unique mod ID; identity complete (Lua `mod.json`, C# `[GregMod]` + registry, JS folder name = mod ID).
- [ ] Cold start + HotLoad clean; no errors; log confirms registration (`Loaded mod` / `Reloaded`).
- [ ] Save round-trip: save → load → remove mod → vanilla still loads (Lua/C# with state; JS trivially safe — no state).
- [ ] UI truthful: Hub Open/Close, HUD row ↔ real hotkey, input restored after close.
- [ ] Co-op scope documented (SP-only vs co-op-tested; identical mod sets for co-op).
- [ ] Diagnostics green: `[DynamicPatcher]` applied, no `[HwId]` error spam, `dotnet test` (C#), `validate_contracts.py` if you touch hooks.

## 2. Versioning

- SemVer everywhere: manifest `Version`, `[GregMod]` version, JS folder/version noted in README.
- Framework floor: document the gregCore version you built against (1.2.3) in the README (Lua `ApiVersion`/`Loader` fields and C# `[GregDependsOn("gregCore", "1.2.3")]` state it in-repo).
- Never reuse shop/item IDs or GUIDs; never rename a shipped custom event — add a new one.

## 3. Packaging per language

| Language | Ship | Installs to |
|---|---|---|
| Lua | Zip of `<modId>/` (`mod.json` + `main.lua` + modules; no absolute paths) | `UserData/gregCore/Mods/Lua/<modId>/` |
| C# | DLL (+ declared deps); soft-dep unless consciously hard | Per `Deploy-Release-ToDataCenter.ps1` conventions |
| JS/TS | Folder `<modId>/` with compiled `*.js` (+ `greg.d.ts`-checked `.ts` sources in your repo) | `UserData/gregCore/Mods/JS/<modId>/` (legacy flat `UserLibs/Js/` still loads) |

Custom-item meshes/textures/icons ship as files in the mod/pack folder (mesh pre-check + traversal guard apply).

## 4. Release flow

`dev → pre-release → main`; Conventional Commits (`feat:`, `fix:`, `docs:`…); README + docs + `CHANGELOG.md` (Unreleased) updated with every feature; Workshop copy via `steamdesc.md` + `Deploy-Release-ToWorkshop.ps1` / `Update-ReleaseMetadata.ps1`. Security reports via `SECURITY.md` — never public issues.

## 5. Post-release (after each game update)

1. Vanilla-first test (no mods → loads).
2. Re-copy interop assemblies (C#); regenerate `GameApi` + hooks if methods changed; `validate_contracts.py`.
3. Re-run your chapter checkpoints; bump the documented framework floor when you adopt new APIs.
