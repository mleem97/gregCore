# Backward compatibility policy

## Public API

Within the gregCore 1.x line:

- public types are not removed or moved without a forwarding facade;
- existing public methods are not removed or changed incompatibly;
- new optional parameters must not replace existing overloads;
- DTO changes are additive;
- hook IDs remain stable and renamed hooks keep aliases;
- old behavior is marked `[Obsolete]` for at least one minor release before removal in a new major version;
- `eng/PublicApi.Shipped.txt` is append-only.

`AssemblyVersion` remains `1.0.0.0` for binary-compatible 1.x releases. Package, file and informational versions continue to follow semantic versioning.

## Legacy assemblies

Broad `AppDomain.AssemblyResolve` redirects are prohibited. Compatibility assemblies must use one of:

- `TypeForwardedTo` for types that retain binary-compatible signatures;
- explicit facade types that delegate to the new API;
- isolated conversion adapters for legacy DTOs.

Only exact legacy assembly simple names may be redirected. A request for an unknown `gregCore.*` version must fail visibly instead of being silently mapped to an incompatible assembly.

## Hook contracts

A hook contract consists of its stable greg hook ID and payload schema. The underlying game method may change per compatibility profile. Profile-specific candidates map the stable hook ID to the current IL2CPP signature.

Payload fields are additive. A field cannot change type inside the same major version. Removed game capabilities remain registered as unsupported capabilities instead of disappearing from the public API.

## Save data

Every persisted record includes:

- schema identifier;
- schema version;
- producing gregCore version;
- migration history where applicable.

Migrations are forward-only, deterministic and tested from every still-supported schema. Unknown future schemas are never rewritten by an older framework.

## Compatibility tests

Each supported release line should retain fixture mods compiled against previous SDK releases. CI verifies that they load against the current 1.x binaries and that public API baselines do not regress.
