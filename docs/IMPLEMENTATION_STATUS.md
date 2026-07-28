# Implementation status

## Implemented on `refactor/il2cpp-version-neutral`

- Compatibility profile schema and current Data Center profile.
- Runtime fingerprint checks for Unity line, platform, architecture, required file sizes and SHA-256 hashes.
- Managed-only safe mode before class injection, UI adapters and game Harmony patches.
- Full-signature hook manifest v2 with profile-specific candidates.
- Strict type/signature resolution, ambiguity reporting and legacy-manifest conversion.
- Lazy activation for high-frequency hooks and subscriber-aware payload creation.
- Single Harmony patch per game method/patch kind with fan-out to stable greg hook IDs.
- Centralized, idempotent IL2CPP type registration with constructor validation.
- Restricted legacy assembly redirect behavior.
- Stable 1.x assembly version and append-only API policy.
- Loader-neutral contracts and real, separately buildable project boundaries.
- Configurable local reference packs and opt-in deployment.
- Explicit release workflow and Unity/game/framework maintenance/archive branch naming.
- Compatibility/profile/hook validation tools and managed verifier tests.
- Consolidated security and performance PR changes on `main` before branch creation.

## Staged migration

The new `gregCore.*` projects are real buildable project boundaries, but most production types still compile into the legacy `gregCore.dll` host. Source extraction is intentionally incremental so existing namespaces, assembly loading and third-party mods are not broken in one commit.

The intended order is:

1. Move pure contracts into `gregCore.Abstractions` and `gregCore.SDK` while retaining forwarding facades.
2. Move managed services into `gregCore.Core` and `gregCore.Shared`.
3. Move IL2CPP/Harmony code into `gregCore.Hooks`, `gregCore.Patches` and `gregCore.Compatibility`.
4. Reduce `gregCore.Mod` to the MelonLoader lifecycle host.
5. Add a separately compiled BepInEx IL2CPP host only after a pinned runtime is tested.

## External verification still required

The repository cannot manufacture or infer these values safely:

- exact Unity patch version used by the installed game;
- SHA-256 of the local `Assembly-CSharp.dll`, Unity modules, MelonLoader and Il2CppInterop binaries;
- `global-metadata.dat` fingerprint and metadata version;
- successful in-game class injection and required hook smoke tests;
- a pinned, independently built BepInEx IL2CPP host.

Use `scripts/capture_compat_profile.py` against a legal local installation, update the profile and hook-manifest hashes, then run the game smoke test. The release workflow blocks verified releases until the exact Unity version and required hashes are present.

## CI infrastructure state

The draft PR's GitHub Actions validation jobs currently terminate before checkout or any command step. GitHub exposes no executed steps and no downloadable job logs for those failures. This means the branch has not received a trustworthy hosted build result yet. The PR remains draft until runners execute normally and the Windows/Linux build and test matrix completes.
