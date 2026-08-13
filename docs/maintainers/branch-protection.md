# GregCore branch and release policy

This repository uses a deliberately one-way promotion flow:

```text
feature/fix/integration -> dev -> pre-release -> main
                                      |
                                      +-> release/vX.Y.Z (immutable snapshot)
```

## Branch responsibilities

| Branch | Meaning | Allowed incoming pull requests | Version form |
| --- | --- | --- | --- |
| `dev` | Current development integration line | Feature, fix, security, performance, and integration branches | `X.Y.Z-dev.N` |
| `pre-release` | The latest `dev` state whose gates passed | `dev` only | `X.Y.Z-rc.N` while frozen, otherwise the promoted dev version |
| `main` | Current published release | `pre-release` only | Stable `X.Y.Z` |
| `release/vX.Y.Z` | Historical release snapshot | None | Stable `X.Y.Z` |

`main` is not a development branch. A change reaches it only through a reviewed
pull request whose base is `main` and whose head is `pre-release`. A direct push,
feature-to-main PR, and auto-version-bump commit are all forbidden.

## Required protection rules

The GitHub ruleset/branch protection configuration must apply to `dev`,
`pre-release`, `main`, and `release/v*`:

- pull requests are required; direct pushes are disabled;
- required status checks are `policy/branch-flow`, `contracts`, `tests`,
  `build-windows`, `build-linux`, and `docs`;
- at least one approving review is required, with stale approvals dismissed;
- conversation resolution is required;
- force pushes and branch deletion are disabled;
- administrators are included in enforcement;
- linear history is preferred; squash merge is the repository default;
- `release/v*` is locked after creation and has no merge target;
- only the release workflow may create `release/v*` branches.

The `branch-policy.yml` check enforces the source-branch direction because
ordinary branch protection does not express “only this exact source branch” on
its own. The workflow is therefore a required check, not advisory documentation.

## Promotion procedure

1. Work is reviewed and merged into `dev`. Every development build uses a
   monotonically increasing `X.Y.Z-dev.N` version.
2. A maintainer opens `dev -> pre-release`. CI must pass on the exact merge
   commit. This is the only route into `pre-release`.
3. When the candidate is accepted, the version is changed in a separate,
   reviewed commit to the stable `X.Y.Z` value and the changelog entry is
   complete. That commit is promoted through `pre-release -> main`.
4. The `Release` workflow runs from `main`, creates `release/vX.Y.Z` exactly
   once, creates tag `vX.Y.Z`, and publishes the release assets. Re-running it
   never rewrites an existing branch or tag.
5. A released branch is retained as an audit snapshot. Fixes go to a new
   development line and are never merged back into an old release branch.

## Emergency handling

An emergency fix still follows `feature/fix -> dev -> pre-release -> main`.
If GitHub service failure requires an administrator bypass, the maintainer must
record the reason, approving reviewer, commit, and resulting release in the
changelog and release notes. The release snapshot remains immutable.

## SemVer contract

GregCore follows Semantic Versioning 2.0.0 and Keep a Changelog. `dev` and
`pre-release` identifiers are prerelease metadata, not separate numeric release
versions. The current development line after `v1.2.1` is `1.2.2-dev.0`; it is
more meaningful than inventing a lower `0.x` line after a published `1.x`
release. A stable `1.2.2` is created only when the candidate is ready.
