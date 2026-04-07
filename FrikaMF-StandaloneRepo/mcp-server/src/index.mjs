/**
 * FrikaMF modding MCP server — stdio (local) or HTTP Streamable MCP + static Docusaurus (Docker).
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

const FMF_INSTRUCTIONS = `You help with FrikaMF (Freaky Modding Framework) modding for the community game stack in this monorepo.
Use the tools to read official docs, hook naming, and the hook registry before inventing APIs.
Conventions: Harmony patches, event hooks named FMF.<Domain>.<Event> (see CONTRIBUTING / fmf-hook-naming).
Prefer small, reviewable changes; respect MelonLoader / IL2CPP constraints.`;

const REPO_OVERVIEW = `## FrikaMF monorepo (high level)

| Path | Role |
|------|------|
| framework/ | MelonLoader runtime (FrikaMF.csproj, Main.cs, hooks) |
| mods/ | Gameplay mods (FMF.Mod.*) |
| plugins/ | FFM.Plugin.* |
| templates/ | Scaffolds for new mods/plugins |
| docs/ | Docusaurus markdown (source of truth for this MCP) |
| wiki/ | Docusaurus app (build output served in Docker) |
| tools/ | Hook catalog, scripts |
| FrikaModFramework/fmf_hooks.json | Declarative hook registry |`;

function parseArgs(argv) {
  const out = {
    http: false,
    stdio: false,
    host: process.env.FMF_MCP_HOST ?? '0.0.0.0',
    port: Number(process.env.FMF_MCP_PORT ?? 3000),
    staticDir: process.env.FMF_MCP_STATIC ?? null,
    dataRoot: process.env.FMF_MCP_DATA_ROOT ?? null
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
  if (process.env.FMF_MCP_DATA_ROOT) return path.resolve(process.env.FMF_MCP_DATA_ROOT);
  const repoRoot = path.resolve(__dirname, '..', '..');
  return repoRoot;
}

function docsDir(dataRoot) {
  return path.join(dataRoot, 'docs');
}

function contributingPath(dataRoot) {
  return path.join(dataRoot, 'CONTRIBUTING.md');
}

async function resolveHooksJsonPath(dataRoot) {
  const flat = path.join(dataRoot, 'fmf_hooks.json');
  try {
    await fs.access(flat);
    return flat;
  } catch {
    const nested = path.join(dataRoot, 'FrikaModFramework', 'fmf_hooks.json');
    await fs.access(nested);
    return nested;
  }
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

function registerFmfTools(mcpServer, dataRoot) {
  mcpServer.registerTool(
    'fmf_search_docs',
    {
      title: 'Search docs',
      description:
        'Search Markdown under docs/ by substring (case-insensitive). Returns paths and short snippets.',
      inputSchema: {
        query: z.string().min(1).max(256).describe('Search string'),
        limit: z.number().int().min(1).max(25).optional().describe('Max results (default 8)')
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
    'fmf_read_doc',
    {
      title: 'Read doc file',
      description: 'Read a Markdown file under docs/ (use forward slashes, e.g. intro.md or reference/fmf-hook-naming.md).',
      inputSchema: {
        path: z.string().min(1).max(512).describe('Path relative to docs/')
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
    'fmf_hook_registry',
    {
      title: 'Hook registry JSON',
      description: 'Return fmf_hooks.json (declarative FMF hook list).',
      inputSchema: z.object({})
    },
    async () => {
      try {
        const p = await resolveHooksJsonPath(dataRoot);
        const text = await fs.readFile(p, 'utf8');
        return { content: [{ type: 'text', text }] };
      } catch (e) {
        return {
          content: [{ type: 'text', text: `Could not read hook registry: ${e}` }],
          isError: true
        };
      }
    }
  );

  mcpServer.registerTool(
    'fmf_read_contributing',
    {
      title: 'CONTRIBUTING.md',
      description: 'Read the repository CONTRIBUTING.md (conventions, workflow).',
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
    'fmf_repo_layout',
    {
      title: 'Repo layout overview',
      description: 'Short Markdown overview of top-level folders (no disk read).',
      inputSchema: z.object({})
    },
    async () => ({
      content: [{ type: 'text', text: REPO_OVERVIEW }]
    })
  );

  mcpServer.registerTool(
    'fmf_list_doc_paths',
    {
      title: 'List doc paths',
      description: 'List Markdown paths under docs/ (for discovery).',
      inputSchema: {
        limit: z.number().int().min(1).max(500).optional().describe('Max paths (default 200)')
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
    'fmf-repo-overview-md',
    'fmf://repo/overview',
    { mimeType: 'text/markdown', description: 'Monorepo layout (same as fmf_repo_layout tool)' },
    async () => ({
      contents: [{ uri: 'fmf://repo/overview', text: REPO_OVERVIEW }]
    })
  );

  mcpServer.registerPrompt(
    'fmf_modding_context',
    {
      description:
        'Starter context for FrikaMF modding: pass an optional topic (hooks, templates, plugins, workshop).',
      argsSchema: {
        topic: z.string().optional().describe('Optional focus area')
      }
    },
    async ({ topic }) => ({
      messages: [
        {
          role: 'user',
          content: {
            type: 'text',
            text: `${FMF_INSTRUCTIONS}\n\n${REPO_OVERVIEW}\n\nOptional focus: ${topic ?? '(none)'}.\nUse MCP tools to pull exact docs and fmf_hooks.json before coding.`
          }
        }
      ]
    })
  );
}

function createFmfMcpServer(dataRoot) {
  const mcpServer = new McpServer(
    { name: 'frika-mf-modding', version: '1.0.0' },
    { instructions: FMF_INSTRUCTIONS }
  );
  registerFmfTools(mcpServer, dataRoot);
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
        const server = createFmfMcpServer(dataRoot);
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
    const mcpServer = createFmfMcpServer(dataRoot);
    const transport = new StdioServerTransport();
    await mcpServer.connect(transport);
    return;
  }

  const app = createMcpExpressApp({ host: args.host === '0.0.0.0' ? '0.0.0.0' : args.host });
  app.get('/health', (_req, res) => {
    res.json({ ok: true, service: 'fmf-mcp', dataRoot });
  });

  attachStreamableMcp(app, dataRoot);

  if (args.staticDir) {
    const staticRoot = path.resolve(args.staticDir);
    app.use(express.static(staticRoot));
  }

  app.listen(args.port, args.host, () => {
    console.error(
      `[fmf-mcp] listening http://${args.host}:${args.port}  (MCP: POST/GET /mcp , health: GET /health)`
    );
    if (args.staticDir) {
      console.error(`[fmf-mcp] static Docusaurus: ${path.resolve(args.staticDir)}`);
    }
  });

  process.on('SIGINT', () => process.exit(0));
}

main().catch(err => {
  console.error(err);
  process.exit(1);
});
