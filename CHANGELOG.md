# Changelog

<!-- markdownlint-disable MD024 -->

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows `MAJOR.MINOR.PATCH` — patch is auto-incremented on every push to `main`.

## [Unreleased]

### Changed

- Initial unreleased section.

## [1.1.0] - 2026-04-28

### Added
- Full CI/CD pipeline: auto version bump, win+linux × MelonLoader+BepInEx releases
- API docs auto-generation from `game_hooks.json` and `framework/greg_hooks.json`
- `scripts/generate_api_docs.py` — generates `docs/FrameworkAPI.md`
- Grid placement system (`greg.GridPlacement`)
- Multiplayer HUD (`src/UI`)
- Lua REPL integration
- Rust FFI host bridge

### Fixed
- Resolved merge conflict in `GregPersistenceService.cs`
- Fixed `sponsor-tier-sync.yml` (`core` identifier conflict in github-script)
- Fixed workflow failures (incorrect project paths, hardcoded version strings)

## [1.0.0] - 2026-01-01

### Added
- gregCore mod framework initial release
- Multiple mods: WallRack, GridPlacement, UI, CommonShop, etc.
- Harmony hooking system (Prefix/Postfix)
- Save engine with versioning (LiteDB)
- Unit tests

[Unreleased]: https://github.com/mleem97/gregCore/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/mleem97/gregCore/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/mleem97/gregCore/releases/tag/v1.0.0
