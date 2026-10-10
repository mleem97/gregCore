# Diagnostics and troubleshooting

Audience: players and mod authors.

The current runtime diagnostic baseline is Unity `6000.4.12f1`, MelonLoader `0.7.3`, Il2CppInterop `1.5.1`, with game version `UNKNOWN` until a verified game fingerprint is available. A healthy startup ends with `Framework initialization complete`.

For hook failures, record the hook ID, exception, game fingerprint, and log path. GregCore isolates callback failures; restart the game after changing a mod. Do not copy legacy `FMF.HexLabelMod.dll` or `ModFramework/FMF` artifacts into a new installation. BepInEx is not a supported installation target until its adapter has a runtime verification record.

## UNSUPPORTED_GAME_BUILD + broken routes after load

Symptoms: `Doctor: UNSUPPORTED_GAME_BUILD` at boot, `GameApi drift` warning,
customers cannot reach their servers after loading a (vanilla) save, no crash.

Cause: the game build is newer than gregCore's verified fingerprint. Device-ID
rewrites (HwId system) are the most invasive load-time behavior: if the new
build binds save entries to live objects differently than assumed, rewritten
IDs dangle and route evaluation finds no routes.

Behavior: on unsupported/unknown builds, HwId rewrites stay **disabled**
(route-safe passthrough, one loud `[gregCore][HwId] DISABLED ...` warning) —
vanilla data loads untouched, routes work as without gregCore. On supported
builds, IDs are derived **deterministically** (SHA-256 of the legacy ID), so
live objects and save endpoints correlate by construction; see
`docs/modding/hardware-ids.md`.

What to do: update gregCore to a release whose verified fingerprint includes
your game build (or add the fingerprint to a reviewed manifest), then reload
the vanilla save. Never hand-edit save IDs.
