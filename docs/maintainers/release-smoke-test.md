# Release smoke test

The real Data Center installation is external to this repository. Run the
following gate on Windows x64 and Linux/Proton using the verified versions:

- Unity `6000.4.12f1`
- MelonLoader `0.7.3`
- Il2CppInterop `1.5.1`

Install GregCore, the C# template mod, and the Lua manifest mod. Verify startup,
scene changes, lifecycle events, config access, a main-thread action, Lua reload,
C# unload, save/load, shutdown, and restart. After each run archive:
`doctor.json`, `MelonLoader.log`, `gregCore.log`, `fingerprint.json`,
`hook-install-report.json`, and `loaded-mod-report.json`.

Validate the archive with:

```bash
python3 scripts/validate_release_artifacts.py path/to/archive
```

The gate passes only when both example mods load, reload/unload removes their
subscriptions, no unexplained Critical/High errors remain, and an unknown build
starts in safe mode without enabling risk-bearing hooks. An external game run is
required; repository CI cannot claim this gate passed without the archived logs.
