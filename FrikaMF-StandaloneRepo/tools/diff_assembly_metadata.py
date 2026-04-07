#!/usr/bin/env python3
"""
Compare current lib/references state with a saved snapshot to detect game / interop updates.

Typical use after Steam updates:
  1. python tools/refresh_refs.py
  2. python tools/diff_assembly_metadata.py
  3. python tools/diff_assembly_metadata.py --save-snapshot

Snapshots are gitignored under lib/references/.previous/
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import shutil
import sys
from pathlib import Path


def _repo_root() -> Path:
    return Path(__file__).resolve().parent.parent


def _sha256(path: Path) -> str | None:
    if not path.is_file():
        return None
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


def _parse_manifest(manifest: Path) -> dict[str, tuple[int, float]]:
    """path | size | mtime -> map rel path -> (size, mtime)"""
    out: dict[str, tuple[int, float]] = {}
    if not manifest.is_file():
        return out
    for line in manifest.read_text(encoding="utf-8").splitlines():
        line = line.strip()
        if not line or line.startswith("#"):
            continue
        parts = [p.strip() for p in line.split("|")]
        if len(parts) != 3:
            continue
        rel, size_s, mtime_s = parts
        try:
            out[rel] = (int(size_s), float(mtime_s))
        except ValueError:
            continue
    return out


def main() -> None:
    parser = argparse.ArgumentParser(description="Diff Assembly-CSharp / manifest vs previous snapshot.")
    parser.add_argument("--save-snapshot", action="store_true", help="Copy MANIFEST + hashes to .previous/")
    args = parser.parse_args()

    repo = _repo_root()
    melon = repo / "lib" / "references" / "MelonLoader"
    il2 = melon / "Il2CppAssemblies"
    ac = il2 / "Assembly-CSharp.dll"
    manifest = repo / "lib" / "references" / "MANIFEST.txt"
    prev_dir = repo / "lib" / "references" / ".previous"
    snap_manifest = prev_dir / "MANIFEST.txt"
    snap_json = prev_dir / "assembly-csharp.json"

    if not ac.is_file():
        print(f"Error: {ac} not found. Run: python tools/refresh_refs.py", file=sys.stderr)
        sys.exit(1)

    current_hash = _sha256(ac)
    current_meta = _parse_manifest(manifest)

    if args.save_snapshot:
        prev_dir.mkdir(parents=True, exist_ok=True)
        if manifest.is_file():
            shutil.copy2(manifest, snap_manifest)
        payload = {
            "Assembly-CSharp.dll_sha256": current_hash,
            "manifest_lines": len(current_meta),
        }
        snap_json.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")
        print(f"Saved snapshot to {prev_dir}")
        return

    print(f"Assembly-CSharp.dll sha256 (current): {current_hash}")

    if not snap_json.is_file():
        print("No previous snapshot (lib/references/.previous/assembly-csharp.json). Run with --save-snapshot after a known-good sync.")
        sys.exit(0)

    prev = json.loads(snap_json.read_text(encoding="utf-8"))
    prev_hash = prev.get("Assembly-CSharp.dll_sha256")
    if prev_hash and prev_hash != current_hash:
        print(f"CHANGE: Assembly-CSharp.dll hash differs from snapshot.")
        print(f"  previous: {prev_hash}")
        print(f"  current:  {current_hash}")
    else:
        print("Assembly-CSharp.dll hash matches snapshot (or snapshot had no hash).")

    prev_meta = _parse_manifest(snap_manifest)
    if prev_meta and current_meta:
        prev_keys = set(prev_meta)
        cur_keys = set(current_meta)
        added = cur_keys - prev_keys
        removed = prev_keys - cur_keys
        changed = {k for k in prev_keys & cur_keys if prev_meta[k] != current_meta[k]}
        if added:
            print(f"MANIFEST added entries ({len(added)}): {sorted(added)[:20]}{'...' if len(added) > 20 else ''}")
        if removed:
            print(f"MANIFEST removed entries ({len(removed)}): {sorted(removed)[:20]}{'...' if len(removed) > 20 else ''}")
        if changed:
            print(f"MANIFEST size/mtime changed ({len(changed)}): {sorted(changed)[:20]}{'...' if len(changed) > 20 else ''}")
        if not added and not removed and not changed:
            print("MANIFEST.txt content (paths/sizes) matches previous snapshot.")


if __name__ == "__main__":
    main()
