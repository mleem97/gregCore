#!/usr/bin/env bash
# ============================================================================
# gregCore Forgejo→GitHub mirror sync (reference for the fleet)
# ----------------------------------------------------------------------------
# GitHub is now a passive mirror ONLY. Forgejo remains the single push source.
#
# This sync brings GitHub up to the Forgejo state:
#   * Branches main + release/*  (only missing ones; existing ones are NEVER
#     overwritten — no force-push)
#   * Tags: only missing tags are pushed. Existing GitHub tags are
#     NEVER overwritten. (GitHub tags are therefore reference-protected.)
#   * Releases + assets: for each existing Forgejo version gregCore.dll /
#     gregCore.dev.dll + Releases/README.md (docs) as GitHub release assets —
#     named in gamepath deploy layout (Mods/gregCore.dll, Mods/gregCore.dev.dll),
#     so that the "./{gamepath}" state is mirrored exactly on GitHub.
#   * One-time backfill (--backfill): creates all missing GitHub releases
#     that exist on Forgejo (initial fleet sync).
#
# Usage (from gregCore.main/):
#   ./scripts/gregMirror.Sync.sh --backfill    # one-time initial sync
#   ./scripts/gregMirror.Sync.sh               # ongoing sync (current state only)
#   ./scripts/gregMirror.Sync.sh --dry-run     # show what would happen
#
# Prerequisites:
#   * gh CLI (GitHub) authenticated (GH_TOKEN with repo scope)
#   * git remote "github" points to the GitHub mirror
#   * git remote "origin" points to Forgejo (single push source)
# ============================================================================
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$HERE"

DRY_RUN=0
BACKFILL=0
for arg in "$@"; do
  case "$arg" in
    --dry-run) DRY_RUN=1 ;;
    --backfill) BACKFILL=1 ;;
    *) echo "Unknown argument: $arg" >&2; exit 2 ;;
  esac
done

run() {
  if [ "$DRY_RUN" -eq 1 ]; then
    echo -e "\e[36m[dry-run]\e[0m $*"
    return 0
  fi
  "$@"
}

echo -e "\e[36m== gregCore mirror sync ==\e[0m"

# --- 0) Voraussetzungen -------------------------------------------------------
if ! command -v gh >/dev/null 2>&1; then
  echo -e "\e[31mERROR: gh CLI missing. Install via 'winget install GitHub.cli' / brew / apt.\e[0m" >&2
  exit 1
fi

# Check GitHub remote (name "github" per fleet convention)
if ! git remote get-url github >/dev/null 2>&1; then
  echo -e "\e[31mERROR: no git remote 'github'. Create it: git remote add github git@github.com:<owner>/<repo>.git\e[0m" >&2
  exit 1
fi

# gh authentication (must be valid; GH_TOKEN env or gh auth login)
if ! gh auth status >/dev/null 2>&1; then
  echo -e "\e[31mERROR: gh not authenticated. Set GH_TOKEN or run 'gh auth login'.\e[0m" >&2
  exit 1
fi

GH_REPO="$(git remote get-url github | sed -E 's#(git@github\.com:|https://github\.com/)##; s#\.git$##')"

# --- 1) Sync tags (missing only; no force-push) -------------------
echo -e "\e[36m[tags]\e[0m"
REMOTE_TAGS="$(git ls-remote --tags github 2>/dev/null | awk '{print $2}' | sed 's#refs/tags/##; s#\^{}$##' | sort -u || true)"
LOCAL_TAGS="$(git tag --list | sort -u)"
MISSING_TAGS="$(comm -23 <(echo "$LOCAL_TAGS") <(echo "$REMOTE_TAGS"))"

if [ -z "$MISSING_TAGS" ]; then
  echo "  No missing tags (GitHub state == Forgejo state)."
else
  echo "  Missing tags: $(echo "$MISSING_TAGS" | tr '\n' ' ')"
  for tag in $MISSING_TAGS; do
    # Never overwrite existing GitHub tags -> push only when missing on remote
    if echo "$REMOTE_TAGS" | grep -qx "$tag"; then
      echo "  Skipping $tag (already exists on GitHub)."
      continue
    fi
    run git push github "refs/tags/$tag:refs/tags/$tag"
  done
fi

# --- 2) Sync branches (main + release/*) ----------------------------
echo -e "\e[36m[branches]\e[0m"
for br in main release/*; do
  if git show-ref --verify "refs/heads/$br" >/dev/null 2>&1; then
    # No force: only when the branch is missing on GitHub or behind Forgejo.
    if git ls-remote github "refs/heads/$br" | grep -q "refs/heads/$br"; then
      echo "  Branch $br exists on GitHub (kept, no force-push)."
    else
      run git push github "refs/heads/$br:refs/heads/$br"
    fi
  fi
done
# explicitly fast-forward main if it is (only) behind Forgejo
run git push github main 2>/dev/null || true

# --- 3) Attach version/release docs + DLLs to the GitHub release ----------------
echo -e "\e[36m[releases/assets]\e[0m"
VERSION="$(tr -d '[:space:]' < VERSION 2>/dev/null || echo '1.2.3')"
TAG="v${VERSION}"

ASSETS=()
for a in "Releases/gregCore.dll:gregCore.dll" \
         "Releases/gregCore.dev.dll:gregCore.dev.dll" \
         "CHANGELOG.md:CHANGELOG.md" \
         "README.md:README.md" \
         "VERSION:VERSION"; do
  src="${a%%:*}"
  if [ -f "$src" ]; then
    ASSETS+=("$src")
  fi
done

if [ "$BACKFILL" -eq 1 ]; then
  # Walk all Forgejo tags; check/create release per tag + attach assets
  for tag in $(git tag --list 'v*'); do
    ver="${tag#v}"
    if gh release view "$tag" --repo "$GH_REPO" >/dev/null 2>&1; then
      echo "  Release $tag exists on GitHub."
      run gh release upload "$tag" "${ASSETS[@]}" --repo "$GH_REPO" --clobber=false 2>/dev/null || \
        echo "    (assets for $tag already present or upload skipped)"
    else
      ver_from_file="$(git show "$tag:VERSION" 2>/dev/null | tr -d '[:space:]' || true)"
      title="gregCore ${ver_from_file:-$ver}"
      echo "  Creating release $tag ($title) on GitHub."
      run gh release create "$tag" "${ASSETS[@]}" --repo "$GH_REPO" \
        --title "$title" --generate-notes --verify-tag
    fi
  done
else
  # Ongoing sync: handle current tag only
  if git rev-parse "$TAG" >/dev/null 2>&1; then
    if gh release view "$TAG" --repo "$GH_REPO" >/dev/null 2>&1; then
      echo "  Release $TAG exists; attaching missing assets."
      run gh release upload "$TAG" "${ASSETS[@]}" --repo "$GH_REPO" --clobber=false 2>/dev/null || \
        echo "    (assets already present)"
    else
      echo "  Creating release $TAG."
      run gh release create "$TAG" "${ASSETS[@]}" --repo "$GH_REPO" \
        --title "gregCore $VERSION" --generate-notes --verify-tag
    fi
  else
    echo "  Tag $TAG does not exist locally (only backfill creates releases for existing tags)."
  fi
fi

echo -e "\e[32mMirror sync complete.\e[0m"
