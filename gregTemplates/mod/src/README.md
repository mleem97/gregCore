Place your mod sources here.

Suggested layout:

- `csharp/` for managed .NET mods (`.csproj` + C# source)
- `lua/` for Lua script mods (bridge discovery/runtime host)
- `typescript/` for TS sources (transpile to JS for runtime)
- `rust/` for native Rust mods (compiled library for `Mods/RustMods`)

Example stubs are included in each folder.

For a complete cross-language example, see `Templates/mod/sysadmin/`.
