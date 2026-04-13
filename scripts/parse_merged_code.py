#!/usr/bin/env python3
"""
Parse gregFramework/MergedCode.md (merged Il2CppInterop C#) → greg_hooks.json.

Default mode **full-surface**: list every patchable public method with a *unique* greg id
(`greg.<DOMAIN>.<Class>.<Method>.<SignatureKey>`), not only “interesting” events.

Run:
  python gregCore/scripts/parse_merged_code.py

Flags:
  --no-hot-loops     omit Update / FixedUpdate / LateUpdate / OnGUI (smaller JSON)
  --no-properties    omit get_* / set_* accessors
"""
from __future__ import annotations

import argparse
import hashlib
import json
import re
from pathlib import Path
from typing import Iterable

FENCE_OPEN = "```csharp"
FENCE_CLOSE = "```"

# Il2Cpp: method body starts with `{` (possibly after `where` clause).
METHOD_RE = re.compile(
    r"^\s*public\s+(?:unsafe\s+)?(?:static\s+)?(?:abstract\s+)?(?:virtual\s+)?(?:override\s+)?(?:new\s+)?(?:sealed\s+)?"
    r"([\w.<>\[\],\s]+?)\s+(\w+)\s*\(([^)]*)\)"
    r"(?:\s*where[^{;]+)?\s*\{",
    re.MULTILINE,
)

CLASS_DECL_RE = re.compile(
    r"\bpublic\s+(?:unsafe\s+)?(?:static\s+)?(?:sealed\s+)?(?:abstract\s+)?(?:partial\s+)?(class|struct)\s+(\w+)\b",
)

SKIP_FILE_STEM = re.compile(
    r"d__\d+|b__\d+|NativeMethodInfoPtr|NativeFieldInfoPtr|TypeHandle|__JobReflection|__UnmanagedPostProcessor",
    re.I,
)

# Inherited noise — listing thousands of identical System.Object overrides adds little control.
SKIP_OBJECT_METHODS = frozenset(
    {
        "GetHashCode",
        "Equals",
        "ToString",
        "GetType",
        "Finalize",
        "MemberwiseClone",
        "ReferenceEquals",
    }
)

HOT_LOOP_METHODS = frozenset({"Update", "FixedUpdate", "LateUpdate", "OnGUI"})


def iter_cs_blocks(text: str) -> Iterable[tuple[str, str]]:
    i = 0
    n = len(text)
    while i < n:
        h = text.find("### ", i)
        if h == -1:
            return
        line_end = text.find("\n", h)
        if line_end == -1:
            return
        header = text[h + 4 : line_end].strip()
        rest = line_end + 1
        if not text.startswith(FENCE_OPEN, rest):
            i = rest
            continue
        nl = text.find("\n", rest)
        if nl == -1:
            return
        start = nl + 1
        end = text.find(FENCE_CLOSE, start)
        if end == -1:
            return
        yield header, text[start:end]
        i = end + len(FENCE_CLOSE)


def stem_from_header(header: str) -> str:
    return Path(header).stem or "Unknown"


def split_code_by_class(code: str) -> list[tuple[str, str]]:
    """Associate method regions with the nearest preceding public class/struct declaration."""
    matches = list(CLASS_DECL_RE.finditer(code))
    if not matches:
        return [("Unknown", code)]
    out: list[tuple[str, str]] = []
    for i, m in enumerate(matches):
        name = m.group(2)
        start = m.start()
        end = matches[i + 1].start() if i + 1 < len(matches) else len(code)
        out.append((name, code[start:end]))
    return out


def domain_for_class(class_name: str) -> str:
    c = class_name
    patterns = [
        (("Player", "PlayerData", "PlayerManager", "ObjectInHand"), "PLAYER"),
        (("Technician", "TechnicianManager", "Employee", "Staff"), "EMPLOYEE"),
        (("Customer", "Contract", "Client", "SLA"), "CUSTOMER"),
        (("Server", "Hardware", "Component"), "SERVER"),
        (("Rack", "RackSlot", "RackUnit"), "RACK"),
        (("Network", "Switch", "Cable", "Packet", "Port", "SFP"), "NETWORK"),
        (("Power", "UPS", "PDU", "Grid", "Energy"), "POWER"),
        (("Job", "Task", "Objective", "Quest", "Mission"), "GAMEPLAY"),
        (("UI", "Menu", "Screen", "Panel", "Overlay", "HUD", "Notification", "Tutorial"), "UI"),
        (("GameManager", "SaveManager", "LoadManager", "SceneManager"), "SYSTEM"),
    ]
    for keys, dom in patterns:
        for k in keys:
            if k.lower() in c.lower():
                return dom
    return "SYSTEM"


def normalize_signature(param_list: str) -> str:
    """Strip names, keep types; collapse whitespace for stable ids."""
    if not param_list or not param_list.strip():
        return "Void"
    parts = []
    for raw in param_list.split(","):
        chunk = raw.strip()
        if not chunk:
            continue
        # drop default values
        if "=" in chunk:
            chunk = chunk.split("=")[0].strip()
        # last token is often the param name — Il2Cpp uses typed params like `float _x`
        tokens = chunk.replace("\t", " ").split()
        if len(tokens) >= 2:
            type_part = " ".join(tokens[:-1])
        else:
            type_part = chunk
        parts.append(type_part.replace(" ", ""))
    return "_".join(parts) if parts else "Void"


def signature_key(param_list: str) -> str:
    norm = normalize_signature(param_list)
    if len(norm) <= 96:
        return re.sub(r"[^a-zA-Z0-9_]", "_", norm)
    h = hashlib.sha256(norm.encode("utf-8")).hexdigest()[:20]
    return f"Sig_{h}"


def friendly_alias(method: str) -> str | None:
    """Optional human label; not used as primary hook id."""
    m = {
        "UpdateCoin": "MoneyChanged",
        "UpdateReputation": "ReputationChanged",
        "UpdateXP": "XpChanged",
    }
    return m.get(method)


def should_emit(
    method: str,
    *,
    include_hot_loops: bool,
    include_properties: bool,
) -> bool:
    if method in SKIP_OBJECT_METHODS:
        return False
    if method in (".ctor", "cctor") or method.startswith("_ctor") or method == "_ctor":
        return False
    if "NativeMethodInfoPtr" in method or "NativeFieldInfoPtr" in method:
        return False
    if not include_hot_loops and method in HOT_LOOP_METHODS:
        return False
    if not include_properties and (method.startswith("get_") or method.startswith("set_")):
        return False
    if method.startswith("_") and not method.startswith("get_") and not method.startswith("set_"):
        return False
    return True


def build_hooks_for_class_region(
    class_name: str,
    region_code: str,
    *,
    include_hot_loops: bool,
    include_properties: bool,
) -> list[dict]:
    dom = domain_for_class(class_name)
    hooks: list[dict] = []
    for m in METHOD_RE.finditer(region_code):
        ret = m.group(1).strip()
        name = m.group(2).strip()
        params = m.group(3).strip()
        if not name:
            continue
        if not should_emit(
            name,
            include_hot_loops=include_hot_loops,
            include_properties=include_properties,
        ):
            continue

        sig = signature_key(params)
        # Full steerability: domain + declaring type + method + signature (overloads never merged).
        hook_name = f"greg.{dom}.{class_name}.{name}.{sig}"

        patch_target = f"Il2Cpp.{class_name}::{name}({params})"
        strategy = "Postfix"
        if name.startswith("Can") or name.startswith("Try") or (name.startswith("Check") and "bool" in ret.lower()):
            strategy = "Prefix+Postfix"

        alias = friendly_alias(name)
        entry: dict = {
            "name": hook_name,
            "legacy": None,
            "patchTarget": patch_target,
            "strategy": strategy,
            "description": f"Public surface: {class_name}.{name}",
            "payloadSchema": {
                "returnType": ret.split()[-1] if ret else "void",
                "parameters": params or "",
            },
            "hotLoop": name in HOT_LOOP_METHODS,
            "className": class_name,
            "methodName": name,
        }
        if alias:
            entry["friendlyAlias"] = f"greg.{dom}.{alias}"
        hooks.append(entry)
    return hooks


def build_hooks_for_file(stem: str, code: str, *, include_hot_loops: bool, include_properties: bool) -> list[dict]:
    if SKIP_FILE_STEM.search(stem):
        return []
    all_hooks: list[dict] = []
    for class_name, region in split_code_by_class(code):
        # Prefer declared class name; for single-region files stem may differ from first inner class — use class_name.
        all_hooks.extend(
            build_hooks_for_class_region(
                class_name,
                region,
                include_hot_loops=include_hot_loops,
                include_properties=include_properties,
            )
        )
    # Fallback: if split failed to find methods, try whole file under stem
    if not all_hooks and not SKIP_FILE_STEM.search(stem):
        all_hooks.extend(
            build_hooks_for_class_region(
                stem,
                code,
                include_hot_loops=include_hot_loops,
                include_properties=include_properties,
            )
        )
    return all_hooks


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument(
        "--merged",
        type=Path,
        default=Path(__file__).resolve().parents[2] / "MergedCode.md",
    )
    ap.add_argument(
        "--out",
        type=Path,
        default=Path(__file__).resolve().parents[1] / "framework" / "gregFramework" / "greg_hooks.json",
    )
    ap.add_argument("--max-hooks", type=int, default=0)
    ap.add_argument(
        "--no-hot-loops",
        action="store_true",
        help="Exclude Update/FixedUpdate/LateUpdate/OnGUI from catalog",
    )
    ap.add_argument(
        "--no-properties",
        action="store_true",
        help="Exclude get_*/set_* property accessors",
    )
    args = ap.parse_args()

    text = args.merged.read_text(encoding="utf-8", errors="replace")
    include_hot = not args.no_hot_loops
    include_prop = not args.no_properties

    all_hooks: list[dict] = []
    files = 0
    for header, code in iter_cs_blocks(text):
        files += 1
        stem = stem_from_header(header)
        all_hooks.extend(
            build_hooks_for_file(stem, code, include_hot_loops=include_hot, include_properties=include_prop)
        )

    # De-dup exact same hook name only (should not happen if signatures differ)
    seen: dict[str, dict] = {}
    for h in all_hooks:
        key = h["name"]
        if key not in seen:
            seen[key] = h
    all_hooks = list(seen.values())

    if args.max_hooks > 0:
        all_hooks = all_hooks[: args.max_hooks]

    doc = {
        "version": 2,
        "description": "Full public-method surface from MergedCode.md — unique id per overload. "
        "Mods subscribe by exact `name`; use `friendlyAlias` where present for legacy docs.",
        "generatedFrom": str(args.merged.resolve()),
        "generationOptions": {
            "includeHotLoops": include_hot,
            "includePropertyAccessors": include_prop,
        },
        "legacyPrefixes": [],
        "hooks": all_hooks,
        "stats": {
            "sourceFiles": files,
            "hookCount": len(all_hooks),
            "withHotLoop": sum(1 for h in all_hooks if h.get("hotLoop")),
        },
    }
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(doc, indent=2), encoding="utf-8")
    print(f"Wrote {len(all_hooks)} hooks from {files} files -> {args.out}")


if __name__ == "__main__":
    main()

