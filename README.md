# gregCore

**gregCore** — MelonLoader-based modding stack for **Data Center**: translations, Harmony hooks, event handling, a Rust/native bridge, and framework extensions.

[![CI Build](https://github.com/MLeeM97/gregCore/actions/workflows/build.yml/badge.svg)](https://github.com/MLeeM97/gregCore/actions/workflows/build.yml)
[![Latest Release](https://img.shields.io/github/v/release/MLeeM97/gregCore?include_prereleases)](https://github.com/MLeeM97/gregCore/releases)

![Greg Framework API Banner](https://github.com/user-attachments/assets/3e78050a-67e8-4eaa-981e-7fa5cfbc466c)

| | |
|:---|:---|
| **Repository** | [`MLeeM97/gregCore`](https://github.com/MLeeM97/gregCore) |
| **Author** | **MLeeM97** |
| **Target** | Windows x64, .NET 6 |

---

## Installation

1. Lade die neueste `gregCore-vX.X.X.zip` von der [Releases](https://github.com/MLeeM97/gregCore/releases) Seite herunter.
2. Entpacke den Inhalt in dein Spielverzeichnis: `Data Center/`. (Die `gregCore.dll` landet in `Mods/`).
3. Starte das Spiel.

---

## Lokaler Build & Entwicklung

### Voraussetzungen
- **.NET 6 SDK**
- **Data Center** installiert
- **MelonLoader (net6)** im Spiel installiert

### Referenzen vorbereiten
Damit der Compiler die Spiel-Klassen kennt, müssen die Interop-DLLs synchronisiert werden:
```bash
python gregTools/refresh_refs.py
```
Dies kopiert die notwendigen Dateien aus deinem Steam-Ordner nach `gregLib/references/`.

### Build ausführen
Nutze das PowerShell-Skript für einen sauberen Release-Build:
```powershell
./gregScripts/Build-Release.ps1
```
Das fertige Paket liegt danach im Ordner `publish/`.

---

## Continuous Integration (GitHub & Gitea)

Das Projekt nutzt einen einheitlichen Workflow für GitHub Actions und Gitea Actions.

### CI "Grün" bekommen (Echte Builds)
Da Spiel-DLLs nicht im Repo liegen dürfen, überspringt die CI standardmäßig den Build oder schlägt fehl. Um echte Builds in der Cloud zu ermöglichen:

1. Packe deine `gregLib/references/MelonLoader` Dateien in ein verschlüsseltes ZIP.
2. Lade dieses ZIP an einen privaten Ort hoch (z.B. Dropbox, privater Server).
3. Setze das Secret **`REFS_URL`** in deinen Repository-Einstellungen auf den direkten Download-Link.
4. Die CI wird diese Referenzen nun automatisch laden und einen validen Build (inkl. Release-Artefakt bei Tags) erstellen.

---

## Verzeichnisstruktur

| Komponente | Pfad | Beschreibung |
| :--- | :--- | :--- |
| **Core Loader** | `gregModLoader/` | Hauptlogik & Language Bridges. |
| **SDK** | `gregSdk/` | API für Mod-Entwickler. |
| **Scripts** | `gregScripts/` | Automatisierung & Build-Tools. |
| **Addons** | `../gregAddons/` | (Extern) Optionale Plugins & Node.js Tools. |

---

## Lizenz
gregCore wird "as-is" für die Data Center Community bereitgestellt. Autor: MLeeM97.
