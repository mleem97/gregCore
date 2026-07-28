# Changelog

<!-- markdownlint-disable MD024 -->

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows semantic versioning. Releases are created explicitly from a verified compatibility profile; normal pushes do not modify versions or publish artifacts.

## [Unreleased]

### Added

- Machine-readable compatibility profiles for game, Unity, IL2CPP, loader, platform, architecture and reference fingerprints.
- Runtime compatibility reports and managed-only safe mode.
- Versioned full-signature hook manifest schema with profile-specific candidates.
- Centralized idempotent IL2CPP class-injection registry.
- Loader-neutral contracts and separately buildable migration project boundaries.
- Profile, hook, fingerprint and compatibility-branch tooling.
- Manual profile-driven release workflow with maintenance branches, immutable archive branches and profile-specific tags.
- Managed compatibility-verification tests and public API baseline.

### Changed

- Dynamic hooks resolve complete method signatures and reject unresolved parameter types instead of shortening signatures.
- High-frequency hooks can remain deferred until a subscriber exists.
- Several greg hook IDs mapped to one game method share a single Harmony patch.
- `AssemblyVersion` remains stable at `1.0.0.0` for binary-compatible 1.x releases.
- Game reference packs are selected through `GREG_REFERENCE_ROOT`/`GregReferenceRoot`.
- Local deployment is opt-in and no longer contains a hardcoded Steam installation path.
- CI validates and builds only; it no longer bumps versions, creates tags or publishes releases on every push.
- BepInEx IL2CPP is no longer packaged by relabeling the MelonLoader host and remains unsupported until a dedicated adapter is verified.
- Legacy assembly resolution is restricted to exact legacy assembly names.

### Fixed

- Deferred events are no longer discarded when the performance governor budget is exhausted.
- Duplicate automated security and performance pull requests were consolidated before the architecture branch was created.

## [1.2.1] - 2026-06-28

### Changed

- Synchronized framework versions to 1.2.1.

## [1.1.0] - 2026-04-28

### Added

- Initial CI/CD release pipeline.
- API docs generation from hook manifests.
- Grid placement system (`greg.GridPlacement`).
- Multiplayer HUD.
- Lua REPL integration.
- Rust FFI host bridge.

### Fixed

- Resolved merge conflict in `GregPersistenceService.cs`.
- Fixed sponsor workflow identifier conflict.
- Fixed workflow failures caused by project paths and hardcoded version strings.

## [1.0.0] - 2026-01-01

### Added

- Initial gregCore mod framework release.
- WallRack, GridPlacement, UI and CommonShop modules.
- Harmony Prefix/Postfix hook infrastructure.
- LiteDB save engine with schema versioning.
- Unit tests.

[Unreleased]: https://github.com/mleem97/gregCore/compare/v1.2.1...HEAD
[1.2.1]: https://github.com/mleem97/gregCore/compare/v1.1.0...v1.2.1
[1.1.0]: https://github.com/mleem97/gregCore/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/mleem97/gregCore/releases/tag/v1.0.0
