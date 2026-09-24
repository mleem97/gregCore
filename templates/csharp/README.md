# GregCore C# mod template

Build from this directory:

```bash
dotnet build GregMod.Template.csproj -c Release -p:GregCorePath=/absolute/path/to/gregCore.dll
```

The DLL is written to `artifacts/`. Copy it to the game's `Mods` directory. The entry type must use `[GregMod]` and derive from `GregMod`. `OnShutdown` must release resources created outside the automatic `GregMod` subscription registry.

## GregHost probe

`GregHost.cs` is a copy-paste runtime probe: detects gregCore without a hard
dependency (type-name lookup only). Copy it into your mod, adjust the namespace,
and gate all gregCore calls behind `GregHost.HasCore` in isolated methods
(see `docs/modding/menu-binding.md`).
