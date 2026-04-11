# greg-hook-scanner

Reads [`gregFramework/greg_hooks.json`](../../gregFramework/greg_hooks.json) and regenerates the Docusaurus page [`docs/wiki/framework/greg-hooks.mdx`](../../docs/wiki/framework/greg-hooks.mdx).

## Setup

```bash
cd tools/greg-hook-scanner
npm install
npm run build
npm run hooks:docs
```

Future: language scanners under `src/scanners/`, merge into registry, codegen for bindings.
