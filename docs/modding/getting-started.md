# Getting started with GregCore

Audience: C# and Lua mod authors.

Status: `STATIC_CONTRACT`; runtime verification requires the Data Center installation.

Compatibility target: Unity `6000.4.12f1`, MelonLoader `0.7.3`, Il2CppInterop `1.5.1`.

Install GregCore's MelonLoader artifact into the game's `Mods` directory. On the first start, inspect the MelonLoader log for `Framework initialization complete`. GregCore reports its diagnostics in the same log and does not claim runtime support for an unknown game build.

Expected files:

- `Data Center/Mods/gregCore.dll`
- `Data Center/Mods/game_hooks.json`
- `Data Center/Mods/framework/greg_hooks.json`

If startup reports `UNSUPPORTED_GAME_BUILD`, keep the diagnostic report and do not enable hooks manually. The report must be reviewed against the committed manifest before updating the game.
