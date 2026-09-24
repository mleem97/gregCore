# GregCore Testing

The test project is `tests/gregCore.Tests.csproj`. Current tests cover event dispatch and isolation, dependency ordering and failures, plugin persistent IDs, diagnostics, resource disposal, and rack/cable patch behavior.

The release build was verified with temporary output directories and no game deployment. The final local run built `gregCore.dll` in Release and executed 26 xUnit tests successfully using .NET roll-forward to the installed runtime.

[TODO] A real Data Center in-game smoke test with archived logs and runtime artifacts remains external work.

## Evidence

- `tests/gregCore.Tests.csproj`
- `tests/Events/`
- `tests/Core/`
- `tests/Infrastructure/`
- `tests/Patches/`
- `tests/PublicApi/`
- `docs/maintainers/release-smoke-test.md`

