# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v1.0.0.7] - 2026-04-12
### Added
- **gregUI**: Complete UI manipulation layer for UGUI (`src/UI/`).
- **GregUIManager**: Central canvas registry and lifecycle management.
- **GregUITheme**: Design system tokens (The Luminescent Architect).
- **GregUIBuilder**: Fluent builder API for mod developers.
- **UI Components**: `GregPanel`, `GregButton`, `GregLabel`, `GregToggle`, `GregSlider`, `GregBadge`, `GregSeparator`.
- **HexViewer Integration**: UI Tree Inspector (F1), Hook Monitor (F2), Element Inspector (F3).
- **UI Hooks**: New hook category `greg.UI.*` for monitoring game UI events (MainMenu, PauseMenu, HUD, Tooltips).

### Changed
- `gregCoreLoader.OnInitializeMelon`: Now registers `GregUIManager`.
- `gregUiExtensionBridge.OnSceneLoaded`: Now notifies `GregUIManager`.

## [v1.0.0.6] - 2026-04-12
### Added
- **Game Compatibility Update**: Verified compatibility with *Data Center* patch `v1.0.45.5`.
- **VLAN Management**: Added `SetVlanAllowed`, `SetVlanDisallowed`, and `IsVlanAllowed` to `gregGameHooks` and `gregGameApi` (v9).
- **Route Evaluation**: Documented hooks for improved routing system.
- **gregReferences Synchronization**: Updated reference documentation for the new game version.

## [v1.0.0.0] - 2026-04-11
### Added
- **Phase 5 Implementation**: Initial SDK bridges for Economy and Data.
- **GregBalanceService**: Access to financial data, salaries, and income simulation.
- **GregLocalisationService**: Direct access to the game's internal translation system.
- **Roadmap Expansion**: Added Phase 8 (Unity Scripting API) and refactored all documentation to English with Material Symbols.

## [v1.0.0.0] - 2026-04-11
### Added
- **Game System Bridges**: 9 new SDK services for direct control of game systems via IL2CPP abstraction.
- **Hardware Bridges**: Direct interaction with physical server and rack objects.
### Added
- **Unity Signal Normalization**: 30+ canonical hooks in `GregNativeEventHooks` based on IL2CPP snapshots.
- **Full Category Registries**: Offizielle SDK-Registries für Server, Switches, Kunden, Mitarbeiter, SFP, Kabel, Möbel und Items.
- **Advanced Model Overrides**: Priority-based conflict resolution, author metadata, and diagnostics.
- **Language Bridge APIs**: Dynamic event registration for Lua (`on_update`, `on_gui`) and C# bridge surface for TS/JS.
- **Native Header**: Added `greg_api.h` for Rust/C mod developers to consume the v8 API table.
- **Validation & Migration**: `IContentValidator` and `IContentMigration` interfaces for mod data integrity.
- **Functional Services**: Basic implementation for `GregIPAllocationService` and `GregRackService`.

### Fixed
- Fixed build issues with nullable reference types in `gregSdk`.
- Corrected test project references and added comprehensive integration tests.

## [v1.0.0.0] - 2026-04-11
### Fixed
- **Definitive UI Removal**: Fixed incorrect file exclusions in `.csproj` and disabled bootstrap registration in `gregCore.cs` to ensure the native UI is used.
- **Debug Overlay**: Added **F5** toggle to show/hide the debug info panel.

## [v1.0.0.0] - 2026-04-11
### Fixed
- Standardized all Lua modules to camelCase (`gregUnityLuaModule`, etc.).
- Fixed static/instance member access conflicts in `gregModSettingsMenuBridge`.
- Corrected over-aggressive renaming of Unity engine components (e.g. `Camera.main`).
- Normalized `gregSdk` namespace usage across the framework.

### Changed
- Separated `gregAddons` into its own logical structure (Node.js tools and optional plugins).
- Built-in reference DLLs for out-of-the-box CI support.

## [v1.0.0.0] - 2026-04-11
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

## [v0.7.0.0] - 2026-04-10
### Fixed
- Project references and solution structure for correct DLL paths.
- README path updates.

## [v0.6.0.0] - 2026-04-09
### Added
- Language bridge architecture with Lua and TypeScript/JavaScript support.
- MoonSharp integration for Lua scripting.

## [v0.5.0.0] - 2026-04-08
### Added
- `UiExtensionBridge` for UI feature integration and modernization.
- Custom Main Menu replacement capabilities.

## [v0.4.0.0] - 2026-04-07
### Added
- Scripts for generating and parsing greg hooks from IL2CPP dumps.

## [v0.3.0.0] - 2026-04-06
### Changed
- Flat `gregFramework` workspace layout.
- Documentation alignment with the new repository structure.

## [v0.2.0.0] - 2026-04-05
### Added
- RustBridge integration for native mod support via FFI.

## [v0.1.0.0] - 2026-04-04
### Added
- Initial standalone repository setup.
- Basic MelonLoader mod structure.

