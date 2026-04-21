---
title: Scripting Language Support
description: Übersicht über alle von gregCore unterstützten Script-Sprachen inklusive Aktivierungslogik und Host-Mapping.
slug: /scripting-language-support
---

## Überblick

`gregCore` unterscheidet strikt:

- **Plugins**: MelonLoader-Assemblies (`*.dll`)
- **Scripts**: Sprachdateien in `Mods/Scripts` (`*.lua`, `*.py`, `*.rs`, `*.rmod`, `*.js`, `*.ts`, `*.cs`)

> ⚠️ WIKI↔CODE CONFLICT: Ältere Bridge-Implementierungen nutzen intern noch `Plugins/<Lang>`-Ordner. Die Aktivierungslogik scannt jetzt `Mods/Scripts` als Source of Truth.

## Aktivierungsmodell (On-Demand)

Beim Start (`OnInitializeMelon`) ruft `gregCore` auf:

```csharp
// Signatur
public static void ScanAndActivate(string modsScriptsDir)
```

Die Registry aktiviert ausschließlich Hosts, für die Dateien erkannt wurden.

```csharp
// Signatur
public static bool IsActive(Language lang)
```

```csharp
// Signatur
public static IGregLanguageHost GetHost(Language lang)
```

## Sprach-Matrix

| Sprache | Muster in `Mods/Scripts` | Abhängigkeit in `gregCore` | Host | Status |
|---|---|---|---|---|
| Lua | `*.lua` | MoonSharp `2.0.0` | `GregLuaHost` | Aktivierbar |
| Rust | `*.rs`, `*.rmod` | Rust-Bridge (FFI/ABI-Layer) | `GregRustHost` | Aktivierbar |
| Python | `*.py` | Python-Host-Bindings (`pythonnet` / `Python.Runtime`) | `GregPythonHost` | Aktivierbar bei Runtime |
| C# Script | `*.cs` | Roslyn (falls vorhanden) | `GregCSharpScriptHost` | [UNVERIFIED] Ausführungslayer |
| JS / TS | `*.js`, `*.ts` | Jint JS-Runtime-Binding | `GregJsHost` | `*.js` aktiv, `*.ts` benötigt Transpile |

## Einstieg je Sprache

### Lua

- Einsatzzweck: schnelle Gameplay-Automation, Hooks, Utilities.
- Host: `GregLuaHost`.

```lua
-- Datei: Mods/Scripts/hello.lua
greg.log_info("Hello from Lua")
```

### Rust

- Einsatzzweck: performancekritische Module, FFI-Integration.
- Host: `GregRustHost`.

```rust
// Datei: Mods/Scripts/example.rs
// Hinweis: derzeit FFI-Brücke nutzt native Exports; reine .rs-Datei ist Trigger für Host-Aktivierung.
fn main() {
    // runtime-spezifisch
}
```

### Python

- Einsatzzweck: Prototyping, Automation, Daten-Workflows.
- Host: `GregPythonHost`.

```python
# Datei: Mods/Scripts/hello.py
greg.log_info("Hello from Python")
```

### C# Script

- Einsatzzweck: Skriptbasierte C#-Erweiterungen ohne Plugin-Build.
- Host: `GregCSharpScriptHost`.

```csharp
// Datei: Mods/Scripts/hello.cs
// [UNVERIFIED] Vollständige Script-Ausführung hängt von Roslyn-Runtime ab.
```

### JavaScript / TypeScript

- Einsatzzweck: schnelle Script-Automation, modulare Tools.
- Host: `GregJsHost`.

```javascript
// Datei: Mods/Scripts/hello.js
greg_log("Hello from JS");
```

```ts
// Datei: Mods/Scripts/hello.ts
// Derzeit nur Erkennung; Transpiler-Integration ist notwendig.
```

## Bekannte Einschränkungen

- `GregCSharpScriptHost`: Ausführungslayer ist in `MISSING.md` dokumentiert.
- TypeScript: Erkennung vorhanden, keine integrierte Transpile-Pipeline.
- Python: Wenn `Python.Runtime.dll` fehlt, bleibt Python-Host deaktiviert.

## Weiterführende Seiten

- [MoonSharp Lua Integration](./moonsharp-lua-integration.md)
- [Getting Started](./getting-started.md)
- [Framework Dependencies](./framework-dependencies.md)
