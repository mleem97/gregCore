#!/usr/bin/env python3
"""Populate reference sizes and SHA-256 hashes in a compatibility profile."""

from __future__ import annotations

import argparse
import hashlib
import json
import sys
from pathlib import Path
from typing import Any


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def resolve_reference(name: str, roots: list[Path]) -> Path | None:
    relative = Path(name)
    candidates = [root / relative for root in roots]
    candidates.extend(root / relative.name for root in roots)
    for candidate in candidates:
        if candidate.is_file():
            return candidate.resolve()
    return None


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("profile", type=Path)
    parser.add_argument("--root", action="append", type=Path, required=True,
                        help="Reference search root; may be specified multiple times")
    parser.add_argument("--output", type=Path)
    parser.add_argument("--allow-missing-optional", action="store_true")
    args = parser.parse_args()

    profile: dict[str, Any] = json.loads(args.profile.read_text(encoding="utf-8"))
    roots = [root.resolve() for root in args.root]
    errors: list[str] = []

    for reference in profile.get("referenceFiles", []):
        path = resolve_reference(reference["path"], roots)
        if path is None:
            if reference.get("required") or not args.allow_missing_optional:
                errors.append(f"reference not found: {reference['path']}")
            continue
        reference["size"] = path.stat().st_size
        reference["sha256"] = sha256(path)

    if errors:
        for error in errors:
            print(error, file=sys.stderr)
        return 1

    output = args.output or args.profile
    output.write_text(json.dumps(profile, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Wrote verified profile: {output}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
