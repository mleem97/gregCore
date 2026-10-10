#!/usr/bin/env python3
"""Validate committed GregCore API and hook-contract invariants."""
from __future__ import annotations

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST = ROOT / "framework" / "greg_hooks.json"


def main() -> None:
    manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    assert manifest["manifestVersion"] >= 1
    assert manifest["schemaVersion"]
    assert manifest.get("unityVersion") == "6000.4.12f1"
    assert manifest.get("melonLoaderVersion") == "0.7.3"
    assert manifest.get("il2cppInteropVersion") == "1.5.1"
    assert manifest["gameBuild"] == "UNKNOWN" or re.fullmatch(r"[A-Za-z0-9._-]+", manifest["gameBuild"])

    ids: set[str] = set()
    names: set[str] = set()
    for hook in manifest.get("hooks", []):
        hook_id = hook["id"]
        name = hook["name"]
        assert hook_id not in ids, f"duplicate hook id: {hook_id}"
        assert name not in names, f"duplicate hook name: {name}"
        assert re.fullmatch(r"greg(?:Mod|Ext|Plugin)\.[a-z][A-Za-z0-9]*(?:\.[a-z][A-Za-z0-9]*)+", name), name
        assert hook.get("threading") in {"main-thread", "any-thread"}
        assert isinstance(hook.get("payloadSchema", {}), dict)
        assert hook.get("status") in {"implemented", "review", "deprecated"}
        for field in ("assembly", "namespace", "type", "member", "signature", "domain", "risk", "approvalReason"):
            assert hook.get(field), f"missing manifest field: {field}"
        assert hook.get("supportedLanguages", []) and set(hook["supportedLanguages"]).issubset({"CSharp", "Lua"})
        assert not name.startswith("greg.")
        if hook.get("legacy"):
            assert hook["legacy"].startswith("greg.")
        ids.add(hook_id)
        names.add(name)

    print(f"validated {len(ids)} canonical hooks from manifest v{manifest['manifestVersion']}")


if __name__ == "__main__":
    main()
