#!/usr/bin/env bash
# ============================================================================
# gregCore Forgejo→GitHub-Mirror-Sync (Referenz für die Fleet)
# ----------------------------------------------------------------------------
# GitHub ist NUR NOCH passiver Mirror. Forgejo bleibt einzige Push-Quelle.
#
# Dieser Sync bringt GitHub auf den Forgejo-Stand:
#   * Branches main + release/*  (nur fehlende; vorhandene werden NICHT
#     überschrieben — kein Force-Push)
#   * Tags: nur fehlende Tags werden gepusht. Existierende GitHub-Tags werden
#     NIE überschrieben. (GitHub-Tags sind damit Referenz-geschützt.)
#   * Releases + Assets: Für jede vorhandene Forgejo-Version gregCore.dll /
#     gregCore.dev.dll + Releases/README.md (Doku) als GitHub-Release-Assets —
#     im gamepath-Deploy-Layout benannt (Mods/gregCore.dll, Mods/gregCore.dev.dll),
#     damit der Stand "./{gamepath}" auf GitHub genau spiegelt.
#   * Einmaliger Backfill (--backfill): legt alle fehlenden GitHub-Releases an,
#     die auf Forgejo existieren (Erst-Sync der Fleet).
#
# Einsatz (aus gregCore.main/):
#   ./scripts/gregMirror.Sync.sh --backfill    # einmaliger Erst-Sync
#   ./scripts/gregMirror.Sync.sh               # laufender Sync (nur aktueller Stand)
#   ./scripts/gregMirror.Sync.sh --dry-run     # Anzeige, was passieren würde
#
# Voraussetzungen:
#   * gh CLI (GitHub) authentifiziert (GH_TOKEN mit repo-Scope)
#   * git-remote "github" zeigt auf den GitHub-Mirror
#   * git-remote "origin" zeigt auf Forgejo (einzige Push-Quelle)
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
    *) echo "Unbekanntes Argument: $arg" >&2; exit 2 ;;
  esac
done

run() {
  if [ "$DRY_RUN" -eq 1 ]; then
    echo -e "\e[36m[dry-run]\e[0m $*"
    return 0
  fi
  "$@"
}

echo -e "\e[36m== gregCore Mirror-Sync ==\e[0m"

# --- 0) Voraussetzungen -------------------------------------------------------
if ! command -v gh >/dev/null 2>&1; then
  echo -e "\e[31mFEHLER: gh CLI fehlt. Installieren via 'winget install GitHub.cli' / brew / apt.\e[0m" >&2
  exit 1
fi

# GitHub-Remote prüfen (Name "github" laut Fleet-Konvention)
if ! git remote get-url github >/dev/null 2>&1; then
  echo -e "\e[31mFEHLER: Kein git-remote 'github'. Anlegen: git remote add github git@github.com:<owner>/<repo>.git\e[0m" >&2
  exit 1
fi

# gh-Authentifizierung (muss gültig sein; GH_TOKEN env oder gh auth login)
if ! gh auth status >/dev/null 2>&1; then
  echo -e "\e[31mFEHLER: gh nicht authentifiziert. GH_TOKEN setzen oder 'gh auth login'.\e[0m" >&2
  exit 1
fi

GH_REPO="$(git remote get-url github | sed -E 's#(git@github\.com:|https://github\.com/)##; s#\.git$##')"

# --- 1) Tags synchronisieren (nur fehlende; kein Force-Push) -------------------
echo -e "\e[36m[tags]\e[0m"
REMOTE_TAGS="$(git ls-remote --tags github 2>/dev/null | awk '{print $2}' | sed 's#refs/tags/##; s#\^{}$##' | sort -u || true)"
LOCAL_TAGS="$(git tag --list | sort -u)"
MISSING_TAGS="$(comm -23 <(echo "$LOCAL_TAGS") <(echo "$REMOTE_TAGS"))"

if [ -z "$MISSING_TAGS" ]; then
  echo "  Keine fehlenden Tags (GitHub-Stand == Forgejo-Stand)."
else
  echo "  Fehlende Tags: $(echo "$MISSING_TAGS" | tr '\n' ' ')"
  for tag in $MISSING_TAGS; do
    # Vorhandene GitHub-Tags NIE überschreiben -> nur wenn remote fehlt pushen
    if echo "$REMOTE_TAGS" | grep -qx "$tag"; then
      echo "  Ueberspringe $tag (existiert bereits auf GitHub)."
      continue
    fi
    run git push github "refs/tags/$tag:refs/tags/$tag"
  done
fi

# --- 2) Branches synchronisieren (main + release/*) ----------------------------
echo -e "\e[36m[branches]\e[0m"
for br in main release/*; do
  if git show-ref --verify "refs/heads/$br" >/dev/null 2>&1; then
    # Kein Force: nur wenn der Branch auf GitHub fehlt oder hinter Forgejo ist.
    if git ls-remote github "refs/heads/$br" | grep -q "refs/heads/$br"; then
      echo "  Branch $br existiert auf GitHub (belassen, kein Force-Push)."
    else
      run git push github "refs/heads/$br:refs/heads/$br"
    fi
  fi
done
# main explizit nachziehen, falls er (nur) hinter Forgejo zurückliegt
run git push github main 2>/dev/null || true

# --- 3) Version/Release-Doku + DLLs ans GitHub-Release anhängen ----------------
echo -e "\e[36m[releases/assets]\e[0m"
VERSION="$(tr -d '[:space:]' < VERSION 2>/dev/null || echo '1.2.3')"
TAG="v${VERSION}"

ASSETS=()
for a in "Releases/gregCore.dll:gregCore.dll" \
         "Releases/gregCore.dev.dll:gregCore.dev.dll" \
         "CHANGELOG.md:CHANGELOG.md" \
         "README.md:README.md" \
         "VERSION:VERSION"; do
  src="${a%%:*}"; dst="${a##*:}"
  if [ -f "$src" ]; then
    ASSETS+=("$src")
  fi
done

if [ "$BACKFILL" -eq 1 ]; then
  # Alle Forgejo-Tags durchgehen; je Tag Release prüfen/erzeugen + Assets anhängen
  for tag in $(git tag --list 'v*'); do
    ver="${tag#v}"
    if gh release view "$tag" --repo "$GH_REPO" >/dev/null 2>&1; then
      echo "  Release $tag existiert auf GitHub."
      run gh release upload "$tag" "${ASSETS[@]}" --repo "$GH_REPO" --clobber=false 2>/dev/null || \
        echo "    (Assets für $tag bereits vorhanden oder Upload übersprungen)"
    else
      ver_from_file="$(git show "$tag:VERSION" 2>/dev/null | tr -d '[:space:]' || true)"
      title="gregCore ${ver_from_file:-$ver}"
      echo "  Erzeuge Release $tag ($title) auf GitHub."
      run gh release create "$tag" "${ASSETS[@]}" --repo "$GH_REPO" \
        --title "$title" --generate-notes --verify-tag
    fi
  done
else
  # Laufender Sync: nur aktuellen Tag behandeln
  if git rev-parse "$TAG" >/dev/null 2>&1; then
    if gh release view "$TAG" --repo "$GH_REPO" >/dev/null 2>&1; then
      echo "  Release $TAG existiert; hänge fehlende Assets an."
      run gh release upload "$TAG" "${ASSETS[@]}" --repo "$GH_REPO" --clobber=false 2>/dev/null || \
        echo "    (Assets bereits vorhanden)"
    else
      echo "  Erzeuge Release $TAG."
      run gh release create "$TAG" "${ASSETS[@]}" --repo "$GH_REPO" \
        --title "gregCore $VERSION" --generate-notes --verify-tag
    fi
  else
    echo "  Tag $TAG existiert lokal nicht (nur Backfill erzeugt Releases für vorhandene Tags)."
  fi
fi

echo -e "\e[32mMirror-Sync abgeschlossen.\e[0m"
