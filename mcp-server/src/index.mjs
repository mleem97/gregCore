/**
 * gregFramework / gregCore — MCP server (Model Context Protocol).
 * Stdio (IDE) or HTTP Streamable MCP + optional static Docusaurus (Docker).
 *
 * Tools: docs search/read, hook registries (greg + legacy greg), CONTRIBUTING, repo overview.
 */
import { randomUUID } from 'node:crypto';
import fs from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { glob } from 'glob';
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import { StreamableHTTPServerTransport } from '@modelcontextprotocol/sdk/server/streamableHttp.js';
import { createMcpExpressApp } from '@modelcontextprotocol/sdk/server/express.js';
import { isInitializeRequest } from '@modelcontextprotocol/sdk/types.js';
import express from 'express';
import * as z from 'zod/v4';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const GREG_INSTRUCTIONS = `You assist with **gregFramework** modding for Data Center (MelonLoader / IL2CPP).
Use the MCP tools before inventing APIs: read docs, search hooks, load registries.
Hook catalog: canonical ids use **greg.<DOMAIN>.<Class>.<Method>.<Signature>** (see greg_hooks.json). Legacy greg/greg names may still appear in older docs — prefer greg.* in new work.
Prefer small, reviewable changes; respect MelonLoader and IL2CPP constraints.`;

const REPO_OVERVIEW = `## gregCore / gregFramework (split-repo layout)

| Path (under gregCore repo) | Role |
|----------------------------|------|
| framework/ | MelonLoader runtime (FrikaMF.csproj, Core, HookBinder, …) |
| framework/gregFramework/ | greg.* hook registry (greg_hooks.json), Core helpers |
| plugins/ | greg.Plugin.* extensions |
| mods/ | Gameplay mods |
| Templates/ | Scaffolds |
| docs/ | Markdown consumed by this MCP (often mirrored in gregWiki) |
| mcp-server/ | This Node MCP server |
| scripts/ | Hook generation (parse_merged_code.py), build helpers |

Multi-repo workspace: sibling folders \`gregWiki/\`, \`gregMod.*\`, \`gregExt.*\` live next to \`gregCore/\` under \`gregFramework/\`.`;

function envStr(...keys) {
  for (const k of keys) {
    if (process.env[k]) return process.env[k];
  }
  return null;
}

function parseArgs(argv) {
  const out = {
    http: false,
    stdio: false,
    host: envStr('GREG_MCP_HOST', 'GREG_MCP_HOST') ?? '0.0.0.0',
    port: Number(envStr('GREG_MCP_PORT', 'GREG_MCP_PORT') ?? 3000),
    staticDir: envStr('GREG_MCP_STATIC', 'GREG_MCP_STATIC') ?? null,
    dataRoot: envStr('GREG_MCP_DATA_ROOT', 'GREG_MCP_DATA_ROOT') ?? null
  };
  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];
    if (a === '--http') out.http = true;
    else if (a === '--stdio') out.stdio = true;
    else if (a === '--host' && argv[i + 1]) out.host = argv[++i];
    else if (a === '--port' && argv[i + 1]) out.port = Number(argv[++i]);
    else if (a === '--static' && argv[i + 1]) out.staticDir = argv[++i];
    else if (a === '--data-root' && argv[i + 1]) out.dataRoot = argv[++i];
  }
  if (!out.http && !out.stdio) out.http = true;
  return out;
}

function defaultDataRoot() {
  const fromEnv = envStr('GREG_MCP_DATA_ROOT', 'GREG_MCP_DATA_ROOT');
  if (fromEnv) return path.resolve(fromEnv);
  return path.resolve(__dirname, '..', '..');
}

function docsDir(dataRoot) {
  return path.join(dataRoot, 'docs');
}

function contributingPath(dataRoot) {
  return path.join(dataRoot, 'CONTRIBUTING.md');
}

/** Try paths in order; return first existing file. */
async function tryAccess(paths) {
  for (const p of paths) {
    try {
      await fs.access(p);
      return p;
    } catch {
      /* continue */
    }
  }
  return null;
}

/** Canonical greg hook list (generated from MergedCode.md). */
async function resolveGregHooksJsonPath(dataRoot) {
  const candidates = [
    path.join(dataRoot, 'framework', 'gregFramework', 'greg_hooks.json'),
    path.join(dataRoot, 'gregFramework', 'greg_hooks.json'),
    path.join(dataRoot, 'greg_hooks.json')
  ];
  return tryAccess(candidates);
}

/** Legacy declarative list (greg_hooks.json) if still present. */
async function resolveLegacygregHooksJsonPath(dataRoot) {
  const candidates = [
    path.join(dataRoot, 'greg_hooks.json'),
    path.join(dataRoot, 'FrikaModFramework', 'greg_hooks.json')
  ];
  return tryAccess(candidates);
}

function safeDocPath(dataRoot, rel) {
  const normalized = path.normalize(rel).replace(/^(\.\.(\/|\\|$))+/, '');
  const full = path.resolve(docsDir(dataRoot), normalized);
  const root = path.resolve(docsDir(dataRoot));
  if (!full.startsWith(root)) {
    throw new Error('Invalid path: path traversal');
  }
  return full;
}

function registerTools(mcpServer, dataRoot) {
  mcpServer.registerTool(
    'greg_search_docs',
    {
      title: 'Search documentation (Markdown)',
      description:
        'Volltext-Suche (Teilstring, case-insensitive) in allen **docs/**/*.md**-Dateien unter `dataRoot`. ' +
        'Liefert Pfade relativ zu `docs/` plus kurze Snippets (~280 Zeichen) um den Treffer. ' +
        'Nutzen: Themen finden, bevor du eine ganze Datei mit `greg_read_doc` lädst.',
      inputSchema: {
        query: z.string().min(1).max(256).describe('Suchbegriff (z. B. HookBinder, MelonLoader, Harmony)'),
        limit: z.number().int().min(1).max(25).optional().describe('Max. Trefferzeilen (Standard 8)')
      }
    },
    async ({ query, limit }) => {
      const max = limit ?? 8;
      const root = docsDir(dataRoot);
      let files;
      try {
        files = await glob('**/*.md', { cwd: root, nodir: true, posix: true });
      } catch (e) {
        return {
          content: [{ type: 'text', text: `docs folder missing or unreadable: ${root}\n${e}` }],
          isError: true
        };
      }
      const q = query.toLowerCase();
      const hits = [];
      for (const rel of files) {
        if (hits.length >= max * 4) break;
        const full = path.join(root, rel);
        const text = await fs.readFile(full, 'utf8');
        const lower = text.toLowerCase();
        if (!lower.includes(q)) continue;
        const idx = lower.indexOf(q);
        const start = Math.max(0, idx - 120);
        const snippet = text.slice(start, start + 280).replace(/\s+/g, ' ');
        hits.push({ path: rel.replace(/\\/g, '/'), snippet });
        if (hits.length >= max) break;
      }
      return {
        content: [
          {
            type: 'text',
            text:
              hits.length === 0
                ? `No matches for "${query}" in docs.`
                : JSON.stringify({ query, count: hits.length, hits }, null, 2)
          }
        ]
      };
    }
  );

  mcpServer.registerTool(
    'greg_read_doc',
    {
      title: 'Read one documentation file',
      description:
        'Liest **eine** Markdown-Datei unter `docs/` vollständig als Text. ' +
        'Pfad relativ zu `docs/` mit Schrägstrich (z. B. `reference/mcp-server.md`). ' +
        'Kein Path-Traversal: `..` wird verworfen.',
      inputSchema: {
        path: z.string().min(1).max(512).describe('Relativer Pfad unter docs/, z. B. intro.md')
      }
    },
    async ({ path: rel }) => {
      try {
        const full = safeDocPath(dataRoot, rel);
        const text = await fs.readFile(full, 'utf8');
        return {
          content: [{ type: 'text', text }]
        };
      } catch (e) {
        return {
          content: [{ type: 'text', text: String(e) }],
          isError: true
        };
      }
    }
  );

  mcpServer.registerTool(
    'greg_hook_registry',
    {
      title: 'greg_hooks.json (vollständiger Katalog)',
      description:
        'Gibt die **komplette** `greg_hooks.json` zurück (Version 2, generiert aus MergedCode.md): alle öffentlichen ' +
        'Patch-Ziele mit Namen `greg.<DOMAIN>.<Class>.<Method>.<Sig>`, Strategie, `hotLoop`, `friendlyAlias`, usw. ' +
        'Nutzen: exakte Hook-IDs für Mods / Dispatcher; bei großen Dateien nur in kleinen Chunks im Client verarbeiten.',
      inputSchema: z.object({})
    },
    async () => {
      try {
        const p = await resolveGregHooksJsonPath(dataRoot);
        if (!p) {
          return {
            content: [
              {
                type: 'text',
                text: `No greg_hooks.json found under dataRoot=${dataRoot}. Expected framework/gregFramework/greg_hooks.json or greg_hooks.json.`
              }
            ],
            isError: true
          };
        }
        const text = await fs.readFile(p, 'utf8');
        return { content: [{ type: 'text', text }] };
      } catch (e) {
        return {
          content: [{ type: 'text', text: `Could not read greg_hooks.json: ${e}` }],
          isError: true
        };
      }
    }
  );

  mcpServer.registerTool(
    'greg_hook_search',
    {
      title: 'greg_hooks.json durchsuchen (ohne volle Datei)',
      description:
        'Filtert Einträge in **greg_hooks.json** nach Teilstring in `name`, `description`, `patchTarget`, `methodName`, `className`. ' +
        'Ideal, um passende `greg.*`-Hooks zu finden, ohne die ganze JSON-Datei in den Kontext zu laden.',
      inputSchema: {
        query: z.string().min(1).max(256).describe('Teilstring, case-insensitive'),
        limit: z.number().int().min(1).max(50).optional().describe('Max. Treffer (Standard 15)')
      }
    },
    async ({ query, limit }) => {
      const max = limit ?? 15;
      try {
        const p = await resolveGregHooksJsonPath(dataRoot);
        if (!p) {
          return {
            content: [{ type: 'text', text: `greg_hooks.json not found under ${dataRoot}` }],
            isError: true
          };
        }
        const raw = JSON.parse(await fs.readFile(p, 'utf8'));
        const hooks = Array.isArray(raw.hooks) ? raw.hooks : [];
        const q = query.toLowerCase();
        const out = [];
        for (const h of hooks) {
          const blob = JSON.stringify(h).toLowerCase();
          if (!blob.includes(q)) continue;
          out.push(h);
          if (out.length >= max) break;
        }
        return {
          content: [
            {
              type: 'text',
              text: JSON.stringify(
                { query, matched: out.length, totalHooks: hooks.length, hooks: out },
                null,
                2
              )
            }
          ]
        };
      } catch (e) {
        return {
          content: [{ type: 'text', text: String(e) }],
          isError: true
        };
      }
    }
  );

  mcpServer.registerTool(
    'greg_hook_stats',
    {
      title: 'greg_hooks.json — Kurzstatistik',
      description:
        'Liest nur Metadaten aus **greg_hooks.json** (`version`, `description`, `stats`, `generationOptions`) ohne die `hooks`-Liste. ' +
        'Schneller Check nach Regenerierung (parse_merged_code.py).',
      inputSchema: z.object({})
    },
    async () => {
      try {
        const p = await resolveGregHooksJsonPath(dataRoot);
        if (!p) {
          return { content: [{ type: 'text', text: `greg_hooks.json not found under ${dataRoot}` }], isError: true };
        }
        const raw = JSON.parse(await fs.readFile(p, 'utf8'));
        const summary = {
          version: raw.version,
          description: raw.description,
          generatedFrom: raw.generatedFrom,
          generationOptions: raw.generationOptions,
          stats: raw.stats,
          hookFile: p
        };
        return { content: [{ type: 'text', text: JSON.stringify(summary, null, 2) }] };
      } catch (e) {
        return { content: [{ type: 'text', text: String(e) }], isError: true };
      }
    }
  );

  mcpServer.registerTool(
    'greg_hook_registry',
    {
      title: 'greg_hooks.json (Legacy-Katalog)',
      description:
        'Liefert **greg_hooks.json** falls im `dataRoot` vorhanden (flach oder unter `FrikaModFramework/`). ' +
        'Ältere deklarative greg/greg-Hook-Liste; für neue Arbeit **greg_hook_registry** / **greg_hook_search** bevorzugen.',
      inputSchema: z.object({})
    },
    async () => {
      try {
        const p = await resolveLegacygregHooksJsonPath(dataRoot);
        if (!p) {
          return {
            content: [
              {
                type: 'text',
                text: `No greg_hooks.json under dataRoot=${dataRoot}. Use greg_hook_registry if you only have greg_hooks.json.`
              }
            ],
            isError: true
          };
        }
        const text = await fs.readFile(p, 'utf8');
        return { content: [{ type: 'text', text }] };
      } catch (e) {
        return {
          content: [{ type: 'text', text: `Could not read legacy hook registry: ${e}` }],
          isError: true
        };
      }
    }
  );

  mcpServer.registerTool(
    'greg_read_contributing',
    {
      title: 'CONTRIBUTING.md',
      description:
        'Liest **CONTRIBUTING.md** im `dataRoot` (Konventionen, Branching, Reviews). ' +
        'Pfad: `<dataRoot>/CONTRIBUTING.md`.',
      inputSchema: z.object({})
    },
    async () => {
      try {
        const text = await fs.readFile(contributingPath(dataRoot), 'utf8');
        return { content: [{ type: 'text', text }] };
      } catch (e) {
        return {
          content: [{ type: 'text', text: String(e) }],
          isError: true
        };
      }
    }
  );

  mcpServer.registerTool(
    'greg_repo_layout',
    {
      title: 'Repo-Überblick (statisch)',
      description:
        'Kurze Markdown-Tabelle zu **gregCore**-Top-Level-Pfaden — **kein** Dateisystem-Scan. ' +
        'Schneller Orientierungspunkt für LLM, bevor gezielt Docs oder Hooks geladen werden.',
      inputSchema: z.object({})
    },
    async () => ({
      content: [{ type: 'text', text: REPO_OVERVIEW }]
    })
  );

  mcpServer.registerTool(
    'greg_list_doc_paths',
    {
      title: 'Alle Markdown-Pfade unter docs/',
      description:
        'Listet **relativ zu docs/** alle `*.md`-Dateien (alphabetisch/glob-Reihenfolge), begrenzt durch `limit`. ' +
        'Discovery, wenn `greg_search_docs` zu unpräzise ist.',
      inputSchema: {
        limit: z.number().int().min(1).max(500).optional().describe('Max. Pfade (Standard 200)')
      }
    },
    async ({ limit }) => {
      const max = limit ?? 200;
      const root = docsDir(dataRoot);
      const files = await glob('**/*.md', { cwd: root, nodir: true, posix: true });
      const slice = files.slice(0, max);
      return {
        content: [
          {
            type: 'text',
            text: JSON.stringify(
              { count: files.length, returned: slice.length, paths: slice },
              null,
              2
            )
          }
        ]
      };
    }
  );

  mcpServer.registerResource(
    'greg-repo-overview-md',
    'greg://repo/overview',
    { mimeType: 'text/markdown', description: 'Statischer gregCore/gregFramework-Überblick (wie greg_repo_layout)' },
    async () => ({
      contents: [{ uri: 'greg://repo/overview', text: REPO_OVERVIEW }]
    })
  );

  mcpServer.registerResource(
    'greg-repo-overview-md',
    'greg://repo/overview',
    { mimeType: 'text/markdown', description: 'Alias: gleicher Inhalt wie greg://repo/overview' },
    async () => ({
      contents: [{ uri: 'greg://repo/overview', text: REPO_OVERVIEW }]
    })
  );

  mcpServer.registerPrompt(
    'greg_modding_context',
    {
      description:
        'Einstiegsprompt: gregFramework-Modding + optionaler Fokus (hooks, templates, plugins, workshop). ' +
        'Lädt keine Dateien — kombiniere mit Tools für echte Quellen.',
      argsSchema: {
        topic: z.string().optional().describe('Optional: hooks | templates | plugins | workshop')
      }
    },
    async ({ topic }) => ({
      messages: [
        {
          role: 'user',
          content: {
            type: 'text',
            text: `${GREG_INSTRUCTIONS}\n\n${REPO_OVERVIEW}\n\nOptional focus: ${topic ?? '(none)'}.\nUse greg_hook_search / greg_hook_registry and docs tools before coding.`
          }
        }
      ]
    })
  );
}

function creategregMcpServer(dataRoot) {
  const mcpServer = new McpServer(
    { name: 'greg-mcp-modding', version: '1.1.0' },
    { instructions: GREG_INSTRUCTIONS }
  );
  registerTools(mcpServer, dataRoot);
  return mcpServer;
}

function attachStreamableMcp(app, dataRoot) {
  const transports = {};

  app.post('/mcp', async (req, res) => {
    try {
      const sessionId = req.headers['mcp-session-id'];
      let transport;
      if (sessionId && transports[sessionId]) {
        transport = transports[sessionId];
      } else if (!sessionId && isInitializeRequest(req.body)) {
        transport = new StreamableHTTPServerTransport({
          sessionIdGenerator: () => randomUUID(),
          onsessioninitialized: sid => {
            transports[sid] = transport;
          }
        });
        const server = creategregMcpServer(dataRoot);
        await server.connect(transport);
        await transport.handleRequest(req, res, req.body);
        return;
      } else {
        res.status(400).json({
          jsonrpc: '2.0',
          error: { code: -32000, message: 'Bad Request: No valid session ID or initialize payload' },
          id: null
        });
        return;
      }
      await transport.handleRequest(req, res, req.body);
    } catch (error) {
      console.error('MCP POST error:', error);
      if (!res.headersSent) {
        res.status(500).json({
          jsonrpc: '2.0',
          error: { code: -32603, message: String(error) },
          id: null
        });
      }
    }
  });

  app.get('/mcp', async (req, res) => {
    const sessionId = req.headers['mcp-session-id'];
    if (!sessionId || !transports[sessionId]) {
      res.status(400).send('Invalid or missing mcp-session-id');
      return;
    }
    const transport = transports[sessionId];
    await transport.handleRequest(req, res);
  });
}

async function main() {
  const args = parseArgs(process.argv);
  const dataRoot = path.resolve(args.dataRoot ?? defaultDataRoot());

  if (args.stdio) {
    const mcpServer = creategregMcpServer(dataRoot);
    const transport = new StdioServerTransport();
    await mcpServer.connect(transport);
    return;
  }

  const app = createMcpExpressApp({ host: args.host === '0.0.0.0' ? '0.0.0.0' : args.host });
  app.get('/health', (_req, res) => {
    res.json({ ok: true, service: 'greg-mcp', version: '1.1.0', dataRoot });
  });

  attachStreamableMcp(app, dataRoot);

  if (args.staticDir) {
    const staticRoot = path.resolve(args.staticDir);
    app.use(express.static(staticRoot));
  }

  app.listen(args.port, args.host, () => {
    console.error(
      `[greg-mcp] listening http://${args.host}:${args.port}  (MCP: POST/GET /mcp , health: GET /health)`
    );
    if (args.staticDir) {
      console.error(`[greg-mcp] static: ${path.resolve(args.staticDir)}`);
    }
  });

  process.on('SIGINT', () => process.exit(0));
}

main().catch(err => {
  console.error(err);
  process.exit(1);
});
