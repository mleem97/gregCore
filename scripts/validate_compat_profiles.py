#!/usr/bin/env python3
"""Validate gregCore compatibility profile files using only the Python stdlib."""

from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path
from typing import Any

SHA256_RE = re.compile(r"^[0-9a-fA-F]{64}$")
REQUIRED_TOP_LEVEL = {
    "schemaVersion",
    "profileId",
    "framework",
    "game",
    "unity",
    "runtime",
    "referenceFiles",
    "features",
}


def load_json(path: Path) -> Any:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as exc:
        raise ValueError(f"{path}: {exc}") from exc


def validate_profile(path: Path) -> tuple[str, list[str]]:
    data = load_json(path)
    errors: list[str] = []
    if not isinstance(data, dict):
        return "", [f"{path}: profile root must be an object"]

    missing = REQUIRED_TOP_LEVEL - data.keys()
    if missing:
        errors.append(f"{path}: missing fields: {', '.join(sorted(missing))}")

    if data.get("schemaVersion") != 2:
        errors.append(f"{path}: schemaVersion must be 2")

    profile_id = data.get("profileId")
    if not isinstance(profile_id, str) or not profile_id.strip():
        errors.append(f"{path}: profileId must be a non-empty string")
        profile_id = ""

    unity = data.get("unity", {})
    if not isinstance(unity, dict) or unity.get("backend") != "IL2CPP":
        errors.append(f"{path}: unity.backend must be IL2CPP")
    if unity.get("exactVersionKnown") is True and not unity.get("version"):
        errors.append(f"{path}: exact Unity profiles require unity.version")

    runtime = data.get("runtime", {})
    if not isinstance(runtime, dict):
        errors.append(f"{path}: runtime must be an object")
    else:
        if runtime.get("loader") not in {"MelonLoader", "BepInEx.IL2CPP"}:
            errors.append(f"{path}: unsupported runtime.loader")
        architectures = runtime.get("architectures", [])
        if not architectures:
            errors.append(f"{path}: at least one runtime architecture is required")

    references = data.get("referenceFiles", [])
    if not isinstance(references, list):
        errors.append(f"{path}: referenceFiles must be an array")
    else:
        seen_paths: set[str] = set()
        for index, reference in enumerate(references):
            label = f"{path}: referenceFiles[{index}]"
            if not isinstance(reference, dict):
                errors.append(f"{label} must be an object")
                continue
            ref_path = reference.get("path")
            if not isinstance(ref_path, str) or not ref_path:
                errors.append(f"{label}.path must be non-empty")
            elif ref_path.lower() in seen_paths:
                errors.append(f"{label}.path is duplicated: {ref_path}")
            else:
                seen_paths.add(ref_path.lower())
            sha256 = reference.get("sha256")
            if sha256 is not None and (not isinstance(sha256, str) or not SHA256_RE.fullmatch(sha256)):
                errors.append(f"{label}.sha256 must be null or 64 hexadecimal characters")
            size = reference.get("size")
            if size is not None and (not isinstance(size, int) or size < 0):
                errors.append(f"{label}.size must be null or a non-negative integer")

    return profile_id, errors


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--compat-root", type=Path, default=Path("compat"))
    args = parser.parse_args()

    compat_root: Path = args.compat_root
    profile_paths = sorted((compat_root / "profiles").glob("*.json"))
    if not profile_paths:
        print("No compatibility profiles found", file=sys.stderr)
        return 1

    errors: list[str] = []
    profile_ids: dict[str, Path] = {}
    for path in profile_paths:
        profile_id, profile_errors = validate_profile(path)
        errors.extend(profile_errors)
        if profile_id:
            if profile_id in profile_ids:
                errors.append(f"duplicate profileId {profile_id}: {profile_ids[profile_id]} and {path}")
            profile_ids[profile_id] = path

    current_path = compat_root / "current.json"
    try:
        current = load_json(current_path)
        profile_ref = current.get("profile") if isinstance(current, dict) else None
        if not isinstance(profile_ref, str) or not profile_ref:
            errors.append(f"{current_path}: profile pointer is required")
        else:
            resolved = (compat_root / profile_ref).resolve()
            if not resolved.is_file():
                errors.append(f"{current_path}: referenced profile does not exist: {profile_ref}")
            if compat_root.resolve() not in resolved.parents:
                errors.append(f"{current_path}: profile pointer escapes compat root")
    except ValueError as exc:
        errors.append(str(exc))

    if errors:
        print("Compatibility validation failed:", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 1

    print(f"Validated {len(profile_paths)} compatibility profile(s).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
