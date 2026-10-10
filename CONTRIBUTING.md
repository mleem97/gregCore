# Contributing — gregCore

Repo: [https://github.com/mleem97/gregCore](https://github.com/mleem97/gregCore) · License: Apache-2.0 · Code of Conduct: [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).

## Workflow

1. Briefly describe the issue or idea (what/why).
2. Branch off current `main`: `feat/<shortname>`, `fix/<shortname>`, `docs/<shortname>`.
3. Small, reviewable commits (Conventional Commits).
4. Before the PR: build + test (see [QUICKSTART.md](QUICKSTART.md)), update docs (`README.md`, `docs/`) and `CHANGELOG.md` (Unreleased).
5. PR with description, screenshots/logs for UI/behavior changes.

## Rules

- No secrets, no binaries unless necessary (then via Releases, not into the repo).
- Do not commit generated artifacts (`bin/`, `obj/`, `dist/`, `node_modules/`, `.next/` …).
- Do NOT report security topics as issues, report them via [SECURITY.md](SECURITY.md) instead.
