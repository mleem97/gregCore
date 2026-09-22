# Changelog — gregCore

Format: [Keep a Changelog](https://keepachangelog.com/de/1.0.0/). Version: siehe [`VERSION`](VERSION).

## [Unreleased]

### Added

- Tasten-HUD (`GregHudRegistry`/`GregHud`): Mods melden Hotkeys an, Anzeige als Tastenleiste am rechten Rand (HUD-Layer).
- Zentrales Mod-Hub (`GregModHub`, Taste F1): listet registrierte Mods/Menues mit Oeffnen-/Schliessen-Knoepfen (Opener via `GregMenuRegistry.RegisterOpener`/`TryOpen`/`Snapshot`, Klick-Routing mit EventSystem-Fallback).
- Einheitliches Open-Source-Layout (README, Docs, Badges) nach gregCore-Vorbild.
- Codacy-Analyse-Konfiguration (`.codacy/`, CLI v2 wie gregModmanager): `lizard`, `opengrep`, `pylint`, `trivy` für C#/Python/Shell; lokal ohne CI-Doppel-Lauf, `GH_TOKEN`/`CODACY_API_TOKEN` nur als Forgejo-Secrets.

## [#] — 2026-09-22

- Initialer standardisierter Stand.
