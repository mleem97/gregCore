---
title: gregCore Framework Dependencies
description: Vollständige Übersicht über Laufzeit- und Sprachabhängigkeiten sowie On-Demand-Aktivierung.
slug: /framework-dependencies
---

## Dependency-Tabelle

| Abhängigkeit | Version | Typ | Wann aktiv? | Quelle |
|---|---|---|---|---|
| MelonLoader | aktuell | Runtime | immer | GameDir |
| Il2CppInterop | aktuell | Runtime | immer | GameDir |
| 0Harmony | aktuell | Runtime | immer | GameDir |
| MoonSharp | 2.0.0 | NuGet/embedded | nur bei `*.lua` | `gregCore.dll` |
| Rust-Bridge DLL | aktuell | FFI/ABI | nur bei `*.rs`/`*.rmod` | `gregCore.dll` |
| Python-Host | aktuell | Binding | nur bei `*.py` | `gregCore.dll` |
| JS-Runtime | aktuell | Binding | nur bei `*.js`/`*.ts` | `gregCore.dll` |

## Immer vorhanden, aber on-demand aktiviert

`gregCore` enthält die Framework-Bausteine, instanziiert Hosts aber nur bei Bedarf:

- Keine passende Script-Datei → Host wird nicht erstellt.
- Script-Datei vorhanden + Dependency fehlt → Warnung, Host bleibt deaktiviert.

## `gregCore.csproj` (Auszug)

```xml
<ItemGroup>
  <PackageReference Include="Jint" Version="4.1.0" />
  <PackageReference Include="Mono.Cecil" Version="0.11.6" />
  <PackageReference Include="MoonSharp" Version="2.0.0" />
  <PackageReference Include="pythonnet" Version="3.0.3" />
</ItemGroup>
```

## Registry-API

```csharp
// Signatur
public static bool IsActive(Language lang)
```

```csharp
// Signatur
public static IGregLanguageHost GetHost(Language lang)
```

```csharp
// Signatur
public static void ScanAndActivate(string modsScriptsDir)
```

## MISSING.md-Workflow

Wenn eine Sprach-Bridge noch nicht vollständig ist:

1. Stub mit `IGregLanguageHost` implementieren.
2. Lokales `MISSING.md` mit Pflicht-Header pflegen.
3. `MISSING.md` nie committen (`.gitignore`: `MISSING.md`, `**/MISSING.md`).

## Konfliktmarkierung

- ⚠️ WIKI↔CODE CONFLICT: Historische Pfade in älteren Dokumenten können `Plugins/<Lang>` nennen. Der aktuelle Standard ist `Mods/Scripts` für Script-Erkennung.
