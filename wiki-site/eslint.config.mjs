// Minimal ESLint flat config for wiki-site tooling.
// Declares Node.js globals for scripts (import-wiki.mjs runs under node).
export default [
  {
    files: ['scripts/**/*.mjs', '*.mjs', 'astro.config.mjs'],
    languageOptions: {
      ecmaVersion: 2022,
      sourceType: 'module',
      globals: {
        process: 'readonly',
        console: 'readonly',
      },
    },
  },
];
