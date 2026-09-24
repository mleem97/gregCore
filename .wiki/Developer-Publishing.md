# Developer Publishing

From working mod to Workshop release. (Oxide equivalent: "Plugin Guidelines" release half + "Server lifecycle".)

## 1. Pre-release checklist

- [ ] Unique mod ID; `mod.json` (`id / name / version / entry / min_framework_version`) or `[GregMod]` + `[GregDependsOn]` filled.
- [ ] [[Developer Best Practices]] §6 diagnostics green (doctor, patcher log, contracts, version, tests, Codacy local).
- [ ] Save round-trip tested: save → load → remove mod → vanilla still loads.
- [ ] Co-op scope documented (SP-only vs co-op-tested, identical-set requirement).
- [ ] README documents: install, settings, events emitted (`<modId>.*`), hotkeys, troubleshooting.
- [ ] `CHANGELOG.md` (Unreleased) entry; Conventional Commits on the branch (`feat/…`, `fix/…`, `docs/…` off current `main`); docs (`README.md` + `docs/`) synced.

## 2. Versioning

- Framework truth: `VERSION` file (`BuildInfo` stamps `-dev.0`). Mods: SemVer in manifest/attribute; gate with `min_framework_version` / `GregDependsOn`.
- Never reuse shop/item IDs or GUIDs across versions (see [[Developer Shop Items]]); never rename a shipped custom event — add a new one ([[Developer Events Hooks Guide]]).

## 3. Packaging

- **Lua:** zip the mod folder (`mod.json` + `main.lua` + modules + `data/` defaults, no absolute paths). Players extract to `UserData/gregCore/Mods/Lua/<modId>/`.
- **C#:** ship the DLL (+ declared deps); soft-dep on `gregCore.dll` unless you consciously went hard-dep (see `docs/modding/hard-dependency.md`).
- **Custom items:** meshes/textures/icons as files in the mod folder or pack folder (`GregCustomItems` mesh pre-check, traversal guard).
- Repo scripts: `scripts/Package-SdkAssets.ps1`, `Create-SDK-Packs.ps1`, `Publish-LocalRelease.ps1`, `Deploy-Release-ToDataCenter.ps1` (local test) and `Deploy-Release-ToWorkshop.ps1` / `Copy-WorkshopUploaderToGame.ps1` / `Update-ReleaseMetadata.ps1` + `steamdesc.md` for Workshop copy.

## 4. Release flow (framework convention, recommended for mods)

`dev → pre-release → main`; GitHub Releases carry the downloads (dev builds are never presented as stable). Mirror/CI (`.forgejo` = source, GitHub = passive mirror via `gregMirror.Sync.sh`) and branch protection (`docs/maintainers/branch-protection.md`) apply to gregCore itself — mirror the discipline: small reviewable PRs, screenshots/logs for UI/behavior changes, security reports via `SECURITY.md` (never public issues).

## 5. Post-release

- Watch loader logs for `[DynamicPatcher]` failures after each **game update**, then follow [[Developer Harmony IL2CPP]] “After a game update” (re-copy interop, regenerate `GameApi` + hooks, validate, vanilla-first test).
- Keep `min_framework_version` accurate when you adopt new APIs; announce dependency changes in the changelog.
