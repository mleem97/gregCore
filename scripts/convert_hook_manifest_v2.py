#!/usr/bin/env python3
"""Convert the legacy game_hooks.json array to the v2 manifest shape."""

from __future__ import annotations

import argparse
import json
from pathlib import Path
from typing import Any

ALIASES = {
    "Void": "System.Void",
    "Boolean": "System.Boolean",
    "Bool": "System.Boolean",
    "Int16": "System.Int16",
    "Int32": "System.Int32",
    "Int": "System.Int32",
    "Int64": "System.Int64",
    "Long": "System.Int64",
    "UInt16": "System.UInt16",
    "UInt32": "System.UInt32",
    "UInt": "System.UInt32",
    "UInt64": "System.UInt64",
    "ULong": "System.UInt64",
    "Single": "System.Single",
    "Float": "System.Single",
    "Double": "System.Double",
    "String": "System.String",
    "Object": "System.Object",
}
HIGH_FREQUENCY = {"Update", "FixedUpdate", "LateUpdate", "OnUpdate"}


def normalize_type(name: str) -> str:
    value = name.strip()
    return ALIASES.get(value, value)


def convert_hook(hook: dict[str, Any]) -> dict[str, Any]:
    namespace = hook.get("Namespace", "")
    class_name = hook.get("ClassName", "")
    method_name = hook.get("MethodName", "")
    group = hook.get("Group", "System")
    type_name = f"{namespace}.{class_name}" if namespace else class_name

    return {
        "id": f"greg.{group}.{method_name}",
        "group": group,
        "required": False,
        "highFrequency": method_name in HIGH_FREQUENCY,
        "captureArguments": True,
        "patchKind": "postfix",
        "candidates": [
            {
                "profiles": [],
                "assembly": "Assembly-CSharp",
                "type": type_name,
                "method": method_name,
                "genericArity": 0,
                "static": None,
                "returnType": normalize_type(hook.get("ReturnType", "System.Void")),
                "parameterTypes": [
                    normalize_type(parameter.get("Type", ""))
                    for parameter in hook.get("Parameters", [])
                ],
            }
        ],
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("input", type=Path)
    parser.add_argument("output", type=Path)
    parser.add_argument("--profile-id", required=True)
    parser.add_argument("--assembly-sha256")
    parser.add_argument("--metadata-sha256")
    args = parser.parse_args()

    legacy = json.loads(args.input.read_text(encoding="utf-8"))
    if not isinstance(legacy, list):
        raise SystemExit("legacy manifest root must be an array")

    hooks = [convert_hook(hook) for hook in legacy]
    hooks.sort(key=lambda hook: (hook["id"], hook["candidates"][0]["type"]))

    manifest = {
        "$schema": "game_hooks.schema.v2.json",
        "schemaVersion": 2,
        "profileId": args.profile_id,
        "generatedFrom": {
            "assemblyCSharpSha256": args.assembly_sha256,
            "metadataSha256": args.metadata_sha256,
            "generatorVersion": "2.0.0",
        },
        "hooks": hooks,
    }
    args.output.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Converted {len(hooks)} hooks to {args.output}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
