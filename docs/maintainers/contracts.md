# Contract maintenance

Audience: GregCore maintainers.

Edit `framework/greg_hooks.json` only after reviewing the corresponding game member. Keep a stable `id`, canonical `gregMod.*`, `gregExt.*`, or `gregPlugin.*` name, payload schema, threading rule, and status. Preserve `legacy` aliases only as explicitly deprecated migration bridges.

Run:

```bash
python3 scripts/validate_contracts.py
dotnet build gregCore.csproj -c Release -p:CI=true --no-restore
dotnet test tests/gregCore.Tests.csproj -c Release --no-restore
```
