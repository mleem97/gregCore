// import-wiki.mjs — copies ../.wiki/*.md into src/content/docs/ for the build.
// - Home.md            -> index.md
// - _Sidebar.md        -> skipped (sidebar lives in astro.config.mjs)
// - all other pages    -> lowercased filenames (stable slugs)
// - prepends `title:` frontmatter from the first `# H1` (Starlight requires it)
// - rewrites [[Wiki Links]] to relative markdown links, validated against
//   the page list (warns on dangling targets instead of failing)
// Run: `node scripts/import-wiki.mjs` (also via `npm run sync` / `npm run build`).
/* global process, console */ // Node.js runtime globals
import { readdirSync, readFileSync, writeFileSync, mkdirSync, rmSync } from 'node:fs';
import { join, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

const here = dirname(fileURLToPath(import.meta.url));
const siteRoot = join(here, '..');
const wikiSrc = process.env.WIKI_SRC ?? join(siteRoot, '..', '.wiki');
const outDir = join(siteRoot, 'src', 'content', 'docs');

const slugOf = (name) =>
  name.trim().toLowerCase().replace(/\s+/g, '-');

rmSync(outDir, { recursive: true, force: true });
mkdirSync(outDir, { recursive: true });

const files = readdirSync(wikiSrc).filter((f) => f.endsWith('.md'));
const known = new Set(files.map((f) => slugOf(f.replace(/\.md$/, ''))));
known.add('home');

let count = 0;
let warnings = 0;
for (const file of files) {
  if (file === '_Sidebar.md') continue;
  const raw = readFileSync(join(wikiSrc, file), 'utf8');
  const lines = raw.split('\n');
  const h1 = lines.find((l) => l.startsWith('# '));
  const title = (h1 ?? `# ${file.replace(/\.md$/, '')}`).replace(/^# /, '').trim();
  const body = raw
    .replace(/^---\n[\s\S]*?\n---\n/, '') // strip existing frontmatter if any
    .replace(/\[\[([^\]]+)\]\]/g, (_, inner) => {
      const [targetRaw, labelRaw] = inner.split('|').map((s) => s.trim());
      const slug = slugOf(targetRaw);
      const label = labelRaw || targetRaw;
      const href = slug === 'home' ? './' : `./${slug}/`;
      if (!known.has(slug)) {
        console.warn(`[wiki] dangling link [[${inner}]] in ${file}`);
        warnings++;
      }
      return `[${label}](${href})`;
    });
  const out = file === 'Home.md' ? 'index.md' : file.toLowerCase();
  writeFileSync(join(outDir, out), `---\ntitle: ${JSON.stringify(title)}\n---\n${body}`);
  count++;
}
console.log(`[wiki] imported ${count} pages from ${wikiSrc} (${warnings} warnings)`);
