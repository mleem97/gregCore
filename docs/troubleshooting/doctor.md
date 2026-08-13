# Diagnostics and troubleshooting

Audience: players and mod authors.

The current runtime diagnostic baseline is Unity `6000.4.12f1`, MelonLoader `0.7.3`, Il2CppInterop `1.5.1`, with game version `UNKNOWN` until a verified game fingerprint is available. A healthy startup ends with `Framework initialization complete`.

For hook failures, record the hook ID, exception, game fingerprint, and log path. GregCore isolates callback failures; restart the game after changing a mod. Do not copy legacy `FMF.HexLabelMod.dll` or `ModFramework/FMF` artifacts into a new installation. BepInEx is not a supported installation target until its adapter has a runtime verification record.
