# Versioning and branch policy

## Source of truth

`main` always represents the newest tested gregCore release candidate for the profile referenced by `compat/current.json`. It must not claim support for a Unity or game build that has not passed profile validation and an in-game smoke test.

Compatibility is identified by the complete tuple:

```text
framework version
+ game version/build
+ Unity version
+ IL2CPP metadata/reference fingerprint
+ loader version
+ Il2CppInterop version
+ platform/architecture
```

Unity version alone is not a sufficient compatibility key.

## Branches

### Current development

```text
main
```

### Framework work

```text
feature/<topic>
fix/<topic>
refactor/<topic>
```

### Maintained compatibility lines

```text
compat/u<unity>/game-<game-version>/gc-<major>.<minor>.x
```

Example:

```text
compat/u6000.5/game-1.0.50.15/gc-1.2.x
```

The branch advances only with compatible patch releases for that exact profile line.

### Exact archive branches

```text
archive/u<unity>/game-<game-version>/gc-<framework-version>
```

Example:

```text
archive/u6000.5/game-1.0.50.15/gc-1.2.1
```

Archive branches are immutable and protected after creation. They exist because the repository policy requires every exact framework/profile combination to remain independently addressable. Tags remain the canonical immutable release identity.

## Tags

```text
u<unity>-game<game-version>-gc<framework-version>
```

Example:

```text
u6000.5-game1.0.50.15-gc1.2.1
```

## Release process

1. Update code, `VERSION`, project metadata and changelog in a release pull request.
2. Validate the selected compatibility profile and hook manifest.
3. Run CI on Windows and Linux.
4. Run the profile's game smoke test.
5. Trigger `gregCore Release` manually with the exact version and profile path.
6. The workflow creates the tag, updates the maintenance branch and creates the immutable archive branch.

Normal pushes never bump versions, create tags, publish releases or create branches.

## Backports

Backports are cherry-picked from `main` into the relevant `compat/...` branch. A backport must not replace the compatibility profile or introduce APIs that require a newer Unity/game runtime. Each backport receives a new patch release and exact archive branch.
