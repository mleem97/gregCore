# API coverage

Coverage has two separate denominators:

1. The complete static inventory in `game_hooks.json` and the assembly scanner output.
2. The explicitly reviewed, modding-relevant members represented by `framework/greg_hooks.json`.

Only the second denominator is eligible for a 100% support claim. Unknown game fingerprints are reported as unsupported; they are not silently treated as compatible. The scanner records the assembly version and SHA-256 fingerprint so a coverage diff can be reproduced for a new build.

Run from the repository root:

```bash
python3 scripts/validate_contracts.py
python3 ../docs/audit/generate_coverage.py
```

Expected artifacts are the validation summary and `docs/audit/coverage-matrix.csv`.
