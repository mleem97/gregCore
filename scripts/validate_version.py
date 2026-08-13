#!/usr/bin/env python3
"""Validate the SemVer contract used by GregCore CI and release automation."""

from __future__ import annotations

import re
import sys


SEMVER = re.compile(
    r"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)"
    r"(?:-((?:0|[1-9]\d*|[0-9A-Za-z-]*[A-Za-z-][0-9A-Za-z-]*)(?:\.(?:0|[1-9]\d*|[0-9A-Za-z-]*[A-Za-z-][0-9A-Za-z-]*))*))?"
    r"(?:\+[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?$"
)


def main() -> int:
    if len(sys.argv) != 2 or not SEMVER.fullmatch(sys.argv[1]):
        print("invalid SemVer; expected MAJOR.MINOR.PATCH[-prerelease][+build]", file=sys.stderr)
        return 2
    print(f"valid SemVer: {sys.argv[1]}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
