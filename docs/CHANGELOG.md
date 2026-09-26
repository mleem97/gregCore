# Changelog
<!-- markdownlint-disable MD024 -- no-duplicate-heading does not apply to Keep-a-Changelog version sections -->

All notable changes to gregCore are documented here.

## [Unreleased]

### Added

- Keybind auto-resolve (`GregKeybindRegistry`): colliding or game-reserved
  (`Escape`, `F1`) toggle requests move to the first free `FallbackPool` key
  (`F12…F2`, then navigation keys), loudly logged + `AutoResolved` flag +
  persisted. Pool exhausted → `HasConflict` flag as before. New query
  `FindFreeKey()`. Docs: `docs/modding/keybinds.md`. Tests:
  `GregKeybindAutoResolveTests` (8 cases).

### Fixed

- Hardware-ID system: deterministic gregIDs (SHA-256 of the legacy device ID
  instead of per-load random GUIDs), so live objects and save endpoints
  correlate by construction and route evaluation survives game builds that
  bind save entries to live objects differently. Previously healed saves
  keep working (prefix skip on both sides).
- Hardware-ID rewrites are now gated on the GregDoctor fingerprint verdict
  (`GregGameCompat.HwIdRewritesAllowed`): on unknown/unsupported game builds
  device IDs pass through untouched (route-safe vanilla passthrough) with one
  loud `[gregCore][HwId] DISABLED ...` warning, instead of renumbering devices
  that cannot be verified. Fixes silent customer disconnects after loading
  vanilla saves on newer game builds (e.g. 1.1.10 with gregCore 1.2.3).

### Added

- `GregGameCompat` boot latch (supported-build verdict for fail-safe patches)
  with unit tests (`GregGameCompatTests`: latch semantics, ID determinism,
  schema, 500-ID uniqueness, empty-input fallback).
- Troubleshooting entry for UNSUPPORTED_GAME_BUILD + broken routes
  (`docs/troubleshooting/doctor.md`); hardware-ids doc covers the
  deterministic scheme and the safe-mode gate.

## [1.1.0] - 2025-06-28

### Changed

- Template repository restructure
- Removed tracked build artifacts (Releases/, *.zip)
- Removed temp/scratch files
- Added LICENSE (Apache 2.0)
- Added CONTRIBUTING.md
- Rewrote README to match gregMod template format
- Added version branch step to CI workflow
- Renamed release.yml → docs.yml (docs-only workflow)

## [1.0.0] - 2025-01-01

### Added

- Initial framework release
- Harmony-based runtime patching system
- UI overlay and widget management
- Save engine with versioning (LiteDB)
- Multi-mod architecture with dependency resolution
- Lua, JS and Python scripting bridges
- Native Data Center co-op compatibility boundary
- CI/CD pipeline with auto-versioning
