# Open PR audit — 2026-08-13

The open pull requests were inspected oldest to newest. The audit found two
repeating proposal families, not 28 independent features:

- **Sentinel:** path traversal checks for `CustomEmployeeManager.SetPortrait`.
  The selected implementation validates logical IDs, rejects separators and
  rooted paths, and canonicalizes the final path inside `ModAssets`.
- **Bolt:** replacement of repeated `FindObjectsOfType<Server>()` calls with
  `NetworkMap` collections. The selected implementation uses the authoritative
  server/broken-server registries and does not allocate a global scene scan in
  the hot API path.

PR #207 is a broad architecture migration. It was reviewed separately and is
not merged wholesale because it changes the project layout and release model;
compatible ideas are documented in `docs/codebase/` and the branch policy. It
must not be mixed into the smaller security/performance integration without a
separate compatibility review.

| PR | Created (UTC) | Branch | Decision |
| ---: | --- | --- | --- |
| #207 | 2026-07-28 | `refactor/il2cpp-version-neutral` | Close as superseded; retain architecture ideas for a separate reviewed migration. |
| #208 | 2026-07-29 | `sentinel/fix-setportrait-traversal-*` | Close; duplicate Sentinel variant. |
| #209 | 2026-07-29 | `sentinel/path-traversal-setportrait-*` | Close; duplicate Sentinel variant. |
| #210 | 2026-07-30 | `sentinel/fix-setportrait-path-traversal-*` | Close; duplicate Sentinel variant. |
| #211 | 2026-07-31 | `perf/lua-server-module-*` | Close; superseded by the selected NetworkMap implementation. |
| #212 | 2026-07-31 | `sentinel/fix-path-traversal-setportrait-*` | Close; duplicate Sentinel variant. |
| #213 | 2026-08-01 | `bolt/optimize-lua-server-lookup-*` | Close; superseded by the selected NetworkMap implementation. |
| #214 | 2026-08-02 | `sentinel/fix-path-traversal-employee-manager-*` | Close; duplicate Sentinel variant. |
| #215 | 2026-08-03 | `sentinel/fix-setportrait-path-traversal-*` | Close; duplicate Sentinel variant. |
| #216 | 2026-08-03 | `bolt-optimize-findobjects-*` | Close; broader variant superseded by the selected implementation. |
| #217 | 2026-08-03 | `sentinel-fix-path-traversal-*` | Close; duplicate Sentinel variant. |
| #218 | 2026-08-04 | `bolt/optimize-lua-server-module-*` | Close; superseded by the selected NetworkMap implementation. |
| #219 | 2026-08-04 | `sentinel/fix-path-traversal-portrait-*` | Close; duplicate and includes unrelated changes. |
| #220 | 2026-08-05 | `bolt-optimize-server-lookups-*` | Close; superseded by the selected NetworkMap implementation. |
| #221 | 2026-08-05 | `perf/optimize-findobjectsoftype-*` | Close; empty effective diff against current main. |
| #222 | 2026-08-05 | `sentinel/fix-custom-employee-manager-*` | Close; duplicate Sentinel variant. |
| #223 | 2026-08-07 | `sentinel/fix-path-traversal-setportrait-*` | Close; duplicate Sentinel variant. |
| #224 | 2026-08-07 | `bolt-optimize-lua-server-queries-*` | Close; superseded by the selected NetworkMap implementation. |
| #225 | 2026-08-08 | `sentinel-security-pathtraversal-*` | Close; duplicate Sentinel variant. |
| #226 | 2026-08-08 | `bolt-optimize-luaserver-api-*` | Close; superseded by the selected NetworkMap implementation. |
| #227 | 2026-08-09 | `sentinel/fix-path-traversal-portrait-*` | Close; duplicate Sentinel variant. |
| #228 | 2026-08-09 | `bolt/optimize-lua-server-api-*` | Close; superseded by the selected NetworkMap implementation. |
| #229 | 2026-08-09 | `sentinel-fix-setportrait-traversal-*` | Close; duplicate Sentinel variant. |
| #230 | 2026-08-10 | `bolt-optimize-findobjectsoftype-*` | Close; superseded by the selected NetworkMap implementation. |
| #231 | 2026-08-11 | `sentinel/fix-path-traversal-setportrait-*` | Close; duplicate Sentinel variant. |
| #232 | 2026-08-11 | `bolt-lua-server-opt-*` | Close; superseded by the selected NetworkMap implementation. |
| #233 | 2026-08-12 | `sentinel/fix-path-traversal-setportrait-*` | Close; duplicate Sentinel variant. |
| #234 | 2026-08-12 | `bolt/optimize-lua-server-module-*` | Close; superseded by the selected NetworkMap implementation. |

## Cleanup rule

After the integration PR is opened, each listed PR is closed with a link to
this audit. Its source branch is deleted only after GitHub confirms the PR is
closed and only when it is not `main`, `dev`, `pre-release`, or `release/*`.
The existing `release/v1.2.1` branch is preserved as a historical snapshot.
