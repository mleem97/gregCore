#!/bin/bash
# Regeneriert src/gregCore.GameApi/Generated aus der Dummy-Assembly-CSharp.dll.
# Aufruf aus dem Repo-Root:  tools/GameApiGenerator/regenerate.sh [Pfad-zu-Assembly-CSharp.dll]
set -e
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
ASM="${1:-$ROOT/references/Assembly-CSharp.dll}"
dotnet run --project "$ROOT/tools/GameApiGenerator/GameApiGenerator.csproj" -c Release -- \
  "$ASM" "$ROOT/references" "$ROOT/src/gregCore.GameApi"
