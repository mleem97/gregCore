# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0-pre.5] - 2026-04-11
### Added
- **Game System Bridges**: 9 neue SDK-Services zur direkten Steuerung von Spielsystemen via IL2CPP-Abstraktion.
- **GregGameManagerService**: Kontrolle über UI-Canvas und Speichersystem.
- **GregPlayerManagerService**: Cursor-Locking und Movement-Kontrolle.
- **GregShopService**: Steuerung des In-Game-Shops und Spawn-Punkte.
- **GregSwitchConfigService**: Zugriff auf Switch-OS (VLAN, LACP).
- **GregCustomerBaseService**: Manipulation von Kunden-Apps und IP-Management.
- **GregTechnicianService**: Mitarbeiter-Dispatching und Job-Queues.
- **GregTimeService**: Uhrzeit-Simulation und Abfragen.
- **Hardware Bridges**: Direkte Interaktion mit physischen Server- und Rack-Objekten.

## [1.0.0-pre.4] - 2026-04-11
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

## [1.0.0-pre.3] - 2026-04-11
### Fixed
- **Definitive UI Removal**: Fixed incorrect file exclusions in `.csproj` and disabled bootstrap registration in `gregCore.cs` to ensure the native UI is used.
- **Debug Overlay**: Added **F5** toggle to show/hide the debug info panel.

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
