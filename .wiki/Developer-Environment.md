# Developer Environment

One-time setup for building gregCore itself and C#/Lua mods. (Oxide equivalent: "Development Environment".)

## Requirements

- **.NET 6 SDK** (project targets `net6.0`; on hosts with only .NET 8/10 use `DOTNET_ROLL_FORWARD=Major` for tests).
- **Local Data Center install + MelonLoader run once** — populates `MelonLoader/Il2CppAssemblies/` and `MelonLoader/net6/`.
- **Game reference assemblies** in `references/` (Il2CppInterop dummies copied from your local install — see `QUICKSTART.md` §2). The framework was built on Linux (Proton-GE); Windows works identically.
- Python 3 for `scripts/` helpers (`validate_version.py`, `generate_api_docs.py`, `validate_contracts.py`).

## Clone + build

```bash
git clone https://github.com/mleem97/gregCore.git
cd gregCore
dotnet build -c Release          # artifact: bin/Release/net6.0/gregCore.dll
# or dual-profile: ./build.sh --both   (Releases/ output)
```

```bash
DOTNET_ROLL_FORWARD=Major dotnet test   # only needed without a .NET 6 runtime
python3 scripts/validate_version.py 1.2.3
```

Single source of truth for the version is the `VERSION` file (currently `1.2.3`); `BuildInfo` stamps `-dev.0` on dev builds.

## Key paths (repo root)

| Path | What is here |
|---|---|
| `src/` | Framework source, `gregCore.*` assemblies (see [[Core Overview]]) |
| `framework/greg_hooks.json` | Canonical hook registry (manifest v2, 2 pinned lifecycle hooks) |
| `game_hooks.json` | 1,850 patchable methods in 22 groups (IL2CPP dump) |
| `references/` | Game + loader assemblies (your local copy; never commit binaries) |
| `examples/` | Working mods in 6 languages (Lua ×4, C#, JS, Python, Rust, Go) |
| `templates/csharp`, `templates/lua/example-mod` | Starter templates |
| `sdk/packs/` | `greg-{go,js,lua,python,rust}-sdk.zip` (built via `scripts/Create-SDK-Packs.ps1`) |
| `tools/` | `GameApiGenerator` (+ `regenerate.sh/.ps1`), `GregCoverageScanner`, `FontAssetBundleBuilder`, `check-coverage.sh` |
| `scripts/` | 27 helpers: build, release, mirror (`gregMirror.Sync.sh`), deploy (`Deploy-Release-ToDataCenter.ps1`, `Deploy-Release-ToWorkshop.ps1`), validation |
| `tests/` | `gregCore.Tests` (unit tests) |
| `.forgejo/workflows`, `.gitea/workflows` | Identical CI + mirror (Forgejo = push source, GitHub = passive mirror) |
| `.codacy/` | Local CLI config (`lizard`, `opengrep`, `pylint`, `trivy`) — no duplicate CI run |
| `.wiki/` | This wiki (synced via `scripts/Sync-Wiki.ps1`) |

## Lua-only setup (no .NET needed)

- Install the game + gregCore per [[Player Installation]].
- Write in any editor; iterate with the **F12 REPL** + hot-reload (`on_reload()`).
- Start from `templates/lua/example-mod/{main.lua, mod.json}` or `examples/Lua/starter_template/`.

## C# mod setup

- Start from `templates/csharp/` (`ExampleMod.cs`, `GregHost.cs` with the `HasCore` probe, `GregMod.Template.csproj`).
- Reference `gregCore.dll` with `Private=false` (soft dependency — see `docs/modding/hard-dependency.md` for when a hard dependency pays; the HexViewer playbook removed 351 lines by going hard).
- Keep the JIT-split: never let a missing gregCore crash your mod at JIT time — probe `GregHost.HasCore` first.

## Decompiler / dump workflow (Oxide "Publicizer / Using Decompiler" equivalent)

1. After a **game update**: run the game once with MelonLoader, re-copy interop assemblies into `references/`.
2. Regenerate the API surface: `tools/GameApiGenerator/regenerate.sh` (writes `src/gregCore.GameApi/Generated/`).
3. Regenerate hooks if methods changed: `scripts/Generate-GregHooksFromIl2CppDump.ps1` (refreshes `game_hooks.json`); review into `framework/greg_hooks.json`.
4. Validate: `scripts/validate_contracts.py` (coverage semantics per `docs/modding/api/coverage.md`) + `python3 scripts/validate_version.py <VERSION>` + full test run.
5. Test vanilla-first: disable all mods, confirm the game loads, then enable yours.
