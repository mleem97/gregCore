# GregCoverageScanner

Scans a Data Center installation without loading Unity or executing game code.
All inputs are sorted and all output collections are sorted, so identical input
files produce byte-identical artifacts.

```bash
dotnet run --project tools/GregCoverageScanner -- \
  --game-root /path/to/DataCenter \
  --output coverage/build-<fingerprint>
```

The output contains `fingerprint.json`, `assembly-inventory.json`,
`modding-manifest.json`, `coverage.csv`, and `coverage-diff.json`. Scanner
discoveries have status `review`; only explicitly reviewed `implemented` hooks
may be copied into `framework/greg_hooks.json`.
