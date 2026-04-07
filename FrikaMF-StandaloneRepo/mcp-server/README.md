# @frikamf/mcp-modding

MCP server for **FrikaMF** modding: search/read `docs/`, load `fmf_hooks.json`, `CONTRIBUTING.md`, and repo layout hints.

## Run

- **HTTP + static Docusaurus (Docker):** use the repository root `Dockerfile` / `docker compose up docs-mcp` — see `docs/reference/mcp-server.md`.
- **Stdio (local IDE):**

```bash
npm install
node src/index.mjs --stdio --data-root ..
```

## Environment

| Variable | Meaning |
|----------|---------|
| `FMF_MCP_DATA_ROOT` | Root containing `docs/`, `CONTRIBUTING.md`, and `fmf_hooks.json` or `FrikaModFramework/fmf_hooks.json` |
| `FMF_MCP_HOST` / `FMF_MCP_PORT` | HTTP bind (default `0.0.0.0:3000`) |
| `FMF_MCP_STATIC` | Path to Docusaurus `build/` for static hosting |
