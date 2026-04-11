# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0-pre.2] - 2026-04-11
### Fixed
- Standardized all Lua modules to camelCase (`gregUnityLuaModule`, etc.).
- Fixed static/instance member access conflicts in `gregModSettingsMenuBridge`.
- Corrected over-aggressive renaming of Unity engine components (e.g. `Camera.main`).
- Normalized `gregSdk` namespace usage across the framework.

### Changed
- Separated `gregAddons` into its own logical structure (Node.js tools and optional plugins).
- Built-in reference DLLs for out-of-the-box CI support.

## [1.0.0-pre.1] - 2026-04-11
### Added
- Missing SDK APIs for all 4 languages (C#, Rust, Lua, TS/JS).
- HUD and Targeting bridge modules for Lua.
- Advanced event subscription model.
- Windows x64 and .NET 6 as primary targets.

### Changed
- **Major Refactor**: Renamed all directories and files to follow the `greg[Name]` camelCase convention.
- Global namespace update from `DataCenterModLoader` to `gregModLoader`.
- Global namespace update from `AssetExporter` to `gregAssetExporter`.
- Standardized author name to `MLeeM97`.

## [0.7.0] - 2026-04-10
### Fixed
- Project references and solution structure for correct DLL paths.
- README path updates.

## [0.6.0] - 2026-04-09
### Added
- Language bridge architecture with Lua and TypeScript/JavaScript support.
- MoonSharp integration for Lua scripting.

## [0.5.0] - 2026-04-08
### Added
- `UiExtensionBridge` for UI feature integration and modernization.
- Custom Main Menu replacement capabilities.

## [0.4.0] - 2026-04-07
### Added
- Scripts for generating and parsing greg hooks from IL2CPP dumps.

## [0.3.0] - 2026-04-06
### Changed
- Flat `gregFramework` workspace layout.
- Documentation alignment with the new repository structure.

## [0.2.0] - 2026-04-05
### Added
- RustBridge integration for native mod support via FFI.

## [0.1.0] - 2026-04-04
### Added
- Initial standalone repository setup.
- Basic MelonLoader mod structure.
