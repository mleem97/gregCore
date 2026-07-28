# Compatibility profiles

`main` is tied to the profile referenced by `compat/current.json`. A profile describes the complete tested runtime tuple rather than only a Unity marketing version:

- gregCore version line
- game and game build
- exact or partial Unity version
- IL2CPP metadata version when known
- loader and Il2CppInterop versions
- platform and architecture
- required reference assemblies, sizes and SHA-256 hashes
- supported runtime capabilities
- hook manifest version

## Verification levels

1. **Declared** — profile JSON is valid.
2. **Size verified** — required binaries exist and match the recorded sizes.
3. **Hash verified** — all recorded SHA-256 values match.
4. **Runtime verified** — class injection and required hooks pass a game smoke test.

A profile without exact Unity patch information or hashes is not allowed to claim universal compatibility. It may remain the current development reference, but gregCore must enter safe mode when the runtime fingerprint differs.

## Safe mode

Safe mode keeps managed services, logging, configuration, the public API and diagnostics available while disabling game-specific class injection and critical Harmony patches. Optional hooks are enabled only after their complete signatures resolve unambiguously.

## Adding a version

1. Copy the closest profile under `compat/profiles/`.
2. Record exact Unity, game, loader and interop versions.
3. Run `scripts/capture_compat_profile.py` against the legal local installation to populate sizes and hashes.
4. Generate and validate the hook manifest.
5. Run the compatibility CI and an in-game smoke test.
6. Create a maintenance branch only after the profile is verified.
