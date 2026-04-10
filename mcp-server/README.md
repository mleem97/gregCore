# gregCore MCP server (`@frikamf/mcp-modding`)

**Model Context Protocol**-Server für Assistenten (Cursor, Claude Code, …): liest **Doku**, **Hook-Kataloge** und **CONTRIBUTING** aus einem konfigurierbaren `dataRoot`.

## Was der Server macht

| Modus | Transport | Typischer Einsatz |
|-------|-------------|-------------------|
| **stdio** | stdin/stdout | Cursor / IDE: ein Prozess pro Session |
| **HTTP** (Standard) | Streamable MCP `POST/GET /mcp` | Docker: Wiki-Static + MCP auf einem Port |

Healthcheck: **`GET /health`** → `{ ok, service, version, dataRoot }`.

## Tools (API für das LLM)

| Tool | Wirkung |
|------|---------|
| **`greg_search_docs`** | Teilstringsuche in `docs/**/*.md`; liefert Pfade + Snippets. |
| **`greg_read_doc`** | Eine Markdown-Datei unter `docs/` vollständig lesen (Pfad relativ, kein `..`). |
| **`greg_list_doc_paths`** | Alle `.md`-Pfade unter `docs/` (Discovery, limitierbar). |
| **`greg_hook_registry`** | **Komplette** `greg_hooks.json` (greg.*-Katalog, Version 2). |
| **`greg_hook_search`** | Gefilterte Hook-Einträge nach Query (ohne die ganze JSON zu laden). |
| **`greg_hook_stats`** | Nur Metadaten (`version`, `stats`, `generationOptions`, …). |
| **`greg_hook_registry`** | Legacy **`greg_hooks.json`**, falls noch vorhanden. |
| **`greg_read_contributing`** | `CONTRIBUTING.md` im `dataRoot`. |
| **`greg_repo_layout`** | Statischer Markdown-Überblick über `gregCore`-Struktur. |

**Ressourcen:** `greg://repo/overview` (und Alias `greg://repo/overview`) — gleicher Text wie `greg_repo_layout`.

**Prompt:** `greg_modding_context` — Kurzkontext + optional `topic`; kombinieren mit den Tools für echte Quellen.

## `dataRoot` — was muss liegen wo?

Der Server erwartet unter **`dataRoot`** (oder per `--data-root` / Env):

- **`docs/`** — Markdown (für Search/Read/List)
- **`CONTRIBUTING.md`**
- **`greg_hooks.json`** — bevorzugt an einem dieser Pfade:
  - `framework/gregFramework/greg_hooks.json` (typisch bei `dataRoot` = `gregCore/`)
  - `greg_hooks.json` (flach)

Optional (Legacy):

- **`greg_hooks.json`** oder **`FrikaModFramework/greg_hooks.json`**

**Beispiel** (stdio, aus `gregCore/mcp-server/`):

```bash
npm install
node src/index.mjs --stdio --data-root ..
```

`--data-root ..` zeigt auf **`gregCore/`** (übergeordnet von `mcp-server/`).

## Umgebungsvariablen

| Variable | Bedeutung |
|----------|-----------|
| **`GREG_MCP_DATA_ROOT`** (oder `GREG_MCP_DATA_ROOT`) | Wurzelverzeichnis für Docs + Hooks |
| **`GREG_MCP_HOST`** / **`GREG_MCP_PORT`** (oder `GREG_MCP_*`) | HTTP-Bindung (Default `0.0.0.0:3000`) |
| **`GREG_MCP_STATIC`** (oder `GREG_MCP_STATIC`) | Optional: statisches Verzeichnis (z. B. Docusaurus `build/`) |

## Docker

Siehe gregWiki: [MCP server](https://github.com/mleem97/gregWiki/blob/main/docs/reference/mcp-server.md) — `docker compose up docs-mcp` o. ä.
