# gregCore Wiki Site (Astro Starlight)

Static docs site built from `../.wiki/` (single source of truth — edit the
markdown there, never the generated `src/content/docs/`).

## Local dev

```bash
cd wiki-site
npm ci
npm run dev      # imports .wiki, serves at http://localhost:4321
```

## How it works

- `npm run sync` (also part of `dev`/`build`) runs
  `scripts/import-wiki.mjs`: copies `../.wiki/*.md`, skips `_Sidebar.md`,
  maps `Home.md → index`, lowercases filenames for stable slugs,
  prepends `title:` frontmatter from the first H1, and rewrites
  `[[Wiki Links]]` to relative links (warns on dangling targets).
- Sidebar in `astro.config.mjs` mirrors `.wiki/_Sidebar.md` — update both
  when adding pages.

## Container (Portainer)

Build context must be the **repo root** (the Dockerfile needs `.wiki/`):

```bash
# local check with podman/docker:
podman build -f wiki-site/Dockerfile -t gregwiki:latest .
podman run -d --name gregwiki -p 8088:80 gregwiki:latest
curl -s -o /dev/null -w '%{http_code}\n' http://localhost:8088/
```

Portainer → Stacks → Add stack → Repository:
repository URL = this repo, compose path = `wiki-site/docker-compose.yml`.
`WIKI_PORT` env sets the host port (default 8088). Traefik labels for a
public hostname are prepared (commented) in the compose file.
