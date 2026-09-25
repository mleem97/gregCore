#!/usr/bin/env python3
"""Validate the portable artifacts collected by the external game smoke test."""
import json
import sys
from pathlib import Path

REQUIRED = ("doctor.json", "fingerprint.json", "hook-install-report.json", "loaded-mod-report.json")

def main() -> int:
    if len(sys.argv) != 2:
        print("usage: validate_release_artifacts.py <artifact-directory>", file=sys.stderr)
        return 2
    root = Path(sys.argv[1])
    missing = [name for name in REQUIRED if not (root / name).is_file()]
    if missing:
        print("missing: " + ", ".join(missing), file=sys.stderr)
        return 1
    doctor = json.loads((root / "doctor.json").read_text(encoding="utf-8"))
    hooks = json.loads((root / "hook-install-report.json").read_text(encoding="utf-8"))
    if doctor.get("Status") == "SELF_TEST_FAILED" or hooks.get("SafeMode") and doctor.get("Status") == "SUPPORTED_GAME_BUILD":
        print("release gate failed: self-test or inconsistent safe mode", file=sys.stderr)
        return 1
    print(f"validated smoke-test artifacts for {doctor.get('Status', 'UNKNOWN')}")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
