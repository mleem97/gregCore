# Branch cleanup — 2026-08-13

The complete remote branch inventory was inspected in ascending author-date
order. Before deletion there were 227 named remote refs including the newly
created integration and promotion branches. The resulting repository keeps
only:

- `main` — published release line;
- `dev` — current development integration line;
- `pre-release` — gate-passed promotion candidate;
- `release/v1.2.1` — existing historical release snapshot;
- `agent/gregcore-integration` — the open consolidated PR #235.

222 obsolete branches were deleted after the open PR audit. They fell into
these classes:

- 97 Sentinel path-traversal variants, including repeated `StartsWith` and
  portrait proposals;
- 86 Bolt rack/server lookup variants, many with repeated CI-only changes;
- 15 Jules branches containing duplicate fixes, exploratory refactors, or
  tests that were not compatible with the current test/reference setup;
- 24 other stale feature, fix, performance, test, and automation branches;
- the broad architecture PR #207, which was reviewed but intentionally not
  merged wholesale because it changes project layout and compatibility scope.

## Implementability decisions

- The strongest Portrait path-validation and `NetworkMap` server lookup
  changes were consolidated in PR #235.
- Lua sandbox branches were not copied blindly: the current `GregIoLuaModule`,
  `LuaModuleLoader`, and `LuaHotReload` already canonicalize paths and enforce
  a separator-aware root boundary. The many variants differ mostly in CI
  churn or duplicate validation.
- The proposed Steam callback queue was not adopted because its unbounded
  queue conflicts with the framework's bounded-work policy; it needs a
  bounded/coalescing design and an integration test first.
- FishNet reflection/RPC proposals were not adopted because they add runtime
  coupling and possible duplicate broadcasts; they need a dedicated
  multiplayer contract review.
- The timezone fallback and isolated test proposals were reviewed but left out
  of the integration commit because they are unrelated to the current
  security/performance scope and some depend on optional runtime assemblies.
  Their commits remain recoverable from the closed PR records.

The deletion was intentionally limited to non-protected, non-release branches;
the GitHub PR records preserve the review history and the release snapshot is
unchanged.
