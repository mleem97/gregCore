---
active: true
iteration: 1
max_iterations: 0
completion_promise: "DONE"
started_at: "2026-04-11T12:00:00Z"
---

Ich migriere mein Modding-Framework von einem alten Legacy-Stand ("FrikaModFramework") zu meinem neuen GregFramework. Du bist ein Senior DevOps & Technical Writer. Deine Aufgabe ist es, das aktuelle Verzeichnis radikal aufzuräumen, zu bereinigen und in eine neue Docusaurus-Struktur zu überführen.

Phase 1: Flatten & Clean (Vorbereitung)

Verschiebe alle Dateien aus allen Unterordnern (außer .git) direkt in das Wurzelverzeichnis (./).

Lösche danach alle nun leeren Unterordner.

WICHTIG: Scanne JEDE Datei (Markdown, MDX, JSON, TXT). Ersetze alle Vorkommen der folgenden Begriffe (case-insensitive) durch "GregFramework":

Frika / FrikaMod
FrikaModFramework
FMF / FFM
Luminescent Design System -> GregFramework Design System

Phase 2: Struktur-Migration (Neuordnung)
Erstelle die folgende Ordnerstruktur und verschiebe die passenden Legacy-Dateien hinein (umbenennen falls angegeben):

📂 01_getting-started/
intro.md (Basis: intro.md)
quickstart.md (Basis: getting-started.md)
architecture.md (Basis: architecture.md / system-architecture-principles.md)

📂 02_development/
📂 concepts/
hooks-and-events.md (Basis: greg-hooks-and-events.md)
modding-basics.md (Basis: developers.md)

📂 api-reference/
hooks-catalog.md (Basis: fmf-hooks-catalog.md oder greg-hooks-catalog.md)
naming-conventions.md (Basis: fmf-hook-naming.md)

📂 hooks-library/
KONSOLIDIERUNG: Suche alle Dateien, die mit NETWORK-, EMPLOYEE-, GAMEPLAY-, SYSTEM-, SERVER-, PLAYER-, RACK-, CUSTOMER- und UI- beginnen.
Erstelle pro Präfix EINE große Datei (z.B. network-hooks.md). Kopiere den Inhalt aller kleinen Einzeldateien unter passenden Überschriften in diese Sammeldatei. Lösche danach die Einzeldateien.

📂 03_design-system/
visual-philosophy.md (Basis: luminescent-design-system.md)

📂 04_community/
contributing.md (Basis: docusaurus-workflow.md)
mod-index.md (Konvertiere mod-index.json in eine saubere Markdown-Tabelle)

📂 05_legal/
sponsors.md (Basis: SPONSORS.md)
privacy.md / terms.md

📂 _internal/ (Alles was nicht ins Wiki soll)
Verschiebe hierhin: README.md, tree.md, IDEA_BACKLOG.md, repo-inventory.md, monorepo-target-layout.md.

Phase 3: Docusaurus Metadata (Refinement)
Erstelle in jedem Hauptordner eine _category_.json Datei mit dem passenden Label (z.B. label: "🛠️ Development") und setze die position entsprechend der Nummerierung.

Füge in jede Markdown-Datei einen sauberen Frontmatter Block am Anfang ein:
---
title: [Generiere Titel basierend auf Dateiname/Inhalt]
sidebar_label: [Kurzer Name]
---
Prüfe interne Links: Wenn eine Datei auf eine alte Struktur verlinkt (z.B. [Arch](../framework/architecture.md)), korrigiere den Link auf die neue flache Struktur.

Phase 4: Cleanup
Lösche alle verbleibenden Dateien im Root, die nicht in die neue Struktur einsortiert wurden (außer Konfigurationsdateien).

Ziel: Ein sauberer, logischer Dokumentations-Baum, der sich wie aus einem Guss liest und keine Spuren des alten Namens mehr enthält.