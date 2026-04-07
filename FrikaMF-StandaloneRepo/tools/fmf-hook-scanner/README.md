# fmf-hook-scanner

Reads [`FrikaModFramework/fmf_hooks.json`](../../FrikaModFramework/fmf_hooks.json) and regenerates the Docusaurus page [`docs/wiki/framework/fmf-hooks.mdx`](../../docs/wiki/framework/fmf-hooks.mdx).

## Setup

```bash
cd tools/fmf-hook-scanner
npm install
npm run build
npm run hooks:docs
```

Future: language scanners under `src/scanners/`, merge into registry, codegen for bindings.
