#!/usr/bin/env python3
"""Validate legacy and v2 gregCore hook manifests."""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path
from typing import Any


def fail(errors: list[str], message: str) -> None:
    errors.append(message)


def validate_v2(data: dict[str, Any], path: Path) -> list[str]:
    errors: list[str] = []
    if data.get("schemaVersion") != 2:
        fail(errors, f"{path}: schemaVersion must be 2")
    if not data.get("profileId"):
        fail(errors, f"{path}: profileId is required")

    hooks = data.get("hooks")
    if not isinstance(hooks, list):
        return [f"{path}: hooks must be an array"]

    ids: set[str] = set()
    signatures: dict[tuple[Any, ...], str] = {}
    for hook_index, hook in enumerate(hooks):
        label = f"{path}: hooks[{hook_index}]"
        if not isinstance(hook, dict):
            fail(errors, f"{label} must be an object")
            continue

        hook_id = hook.get("id")
        if not isinstance(hook_id, str) or not hook_id:
            fail(errors, f"{label}.id is required")
            hook_id = f"<invalid-{hook_index}>"
        elif hook_id in ids:
            fail(errors, f"{label}.id is duplicated: {hook_id}")
        ids.add(hook_id)

        if hook.get("patchKind") not in {"prefix", "postfix"}:
            fail(errors, f"{label}.patchKind must be prefix or postfix")

        candidates = hook.get("candidates")
        if not isinstance(candidates, list) or not candidates:
            fail(errors, f"{label}.candidates must contain at least one candidate")
            continue

        for candidate_index, candidate in enumerate(candidates):
            candidate_label = f"{label}.candidates[{candidate_index}]"
            if not isinstance(candidate, dict):
                fail(errors, f"{candidate_label} must be an object")
                continue
            for key in ("assembly", "type", "method", "genericArity", "returnType", "parameterTypes"):
                if key not in candidate:
                    fail(errors, f"{candidate_label}.{key} is required")
            if candidate.get("static") not in (True, False, None):
                fail(errors, f"{candidate_label}.static must be true, false or null")
            parameter_types = candidate.get("parameterTypes")
            if not isinstance(parameter_types, list) or not all(isinstance(item, str) and item for item in parameter_types):
                fail(errors, f"{candidate_label}.parameterTypes must contain non-empty strings")
                parameter_types = []

            signature = (
                candidate.get("assembly"),
                candidate.get("type"),
                candidate.get("method"),
                candidate.get("genericArity"),
                candidate.get("static"),
                tuple(parameter_types),
                hook.get("patchKind"),
            )
            previous = signatures.get(signature)
            if previous and previous == hook_id:
                fail(errors, f"{candidate_label}: duplicate candidate signature in {hook_id}")
            signatures[signature] = hook_id

    return errors


def validate_legacy(data: list[Any], path: Path) -> list[str]:
    errors: list[str] = []
    for index, hook in enumerate(data):
        label = f"{path}: hooks[{index}]"
        if not isinstance(hook, dict):
            fail(errors, f"{label} must be an object")
            continue
        for key in ("Group", "ClassName", "MethodName", "Parameters"):
            if key not in hook:
                fail(errors, f"{label}.{key} is required")
        parameters = hook.get("Parameters")
        if not isinstance(parameters, list):
            fail(errors, f"{label}.Parameters must be an array")
    return errors


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("paths", nargs="+", type=Path)
    args = parser.parse_args()

    errors: list[str] = []
    for path in args.paths:
        try:
            data = json.loads(path.read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError) as exc:
            errors.append(f"{path}: {exc}")
            continue

        if isinstance(data, dict):
            errors.extend(validate_v2(data, path))
        elif isinstance(data, list):
            errors.extend(validate_legacy(data, path))
        else:
            errors.append(f"{path}: root must be an object or array")

    if errors:
        print("Hook manifest validation failed:", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 1

    print(f"Validated {len(args.paths)} hook manifest(s).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
