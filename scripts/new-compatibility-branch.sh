#!/usr/bin/env bash
set -euo pipefail

usage() {
  echo "Usage: $0 <profile.json> <framework-version> [start-point] [--push]" >&2
}

[[ $# -ge 2 ]] || { usage; exit 2; }
PROFILE=$1
VERSION=$2
START_POINT=${3:-HEAD}
PUSH=false
[[ ${4:-} == "--push" || ${3:-} == "--push" ]] && PUSH=true

[[ $VERSION =~ ^[0-9]+\.[0-9]+\.[0-9]+([+-][0-9A-Za-z.-]+)?$ ]] || {
  echo "Invalid semantic version: $VERSION" >&2
  exit 2
}

readarray -t VALUES < <(python3 - "$PROFILE" <<'PY'
import json, sys
profile = json.load(open(sys.argv[1], encoding='utf-8'))
print(profile['profileId'])
print(profile['unity']['version'])
print(profile['game']['version'])
PY
)
PROFILE_ID=${VALUES[0]}
UNITY=${VALUES[1]}
GAME=${VALUES[2]}
IFS=. read -r MAJOR MINOR _ <<< "$VERSION"

MAINTENANCE="compat/u${UNITY}/game-${GAME}/gc-${MAJOR}.${MINOR}.x"
ARCHIVE="archive/u${UNITY}/game-${GAME}/gc-${VERSION}"
TAG="u${UNITY}-game${GAME}-gc${VERSION}"

git rev-parse --verify "$START_POINT" >/dev/null
if git show-ref --verify --quiet "refs/heads/$ARCHIVE"; then
  echo "Immutable archive branch already exists: $ARCHIVE" >&2
  exit 1
fi
if git show-ref --verify --quiet "refs/tags/$TAG"; then
  echo "Release tag already exists: $TAG" >&2
  exit 1
fi

if ! git show-ref --verify --quiet "refs/heads/$MAINTENANCE"; then
  git branch "$MAINTENANCE" "$START_POINT"
fi
git branch "$ARCHIVE" "$START_POINT"
git tag -a "$TAG" "$START_POINT" -m "gregCore $VERSION for $PROFILE_ID"

if $PUSH; then
  git push origin "$MAINTENANCE" "$ARCHIVE" "$TAG"
fi

printf 'Profile: %s\nMaintenance: %s\nArchive: %s\nTag: %s\n' \
  "$PROFILE_ID" "$MAINTENANCE" "$ARCHIVE" "$TAG"
