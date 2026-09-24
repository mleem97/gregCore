#!/bin/bash
# gregCore Build + Deploy - dual profile.
#
#   ./build.sh              Release-Build (normales Logging, Version 1.2.3)
#   ./build.sh --dev        Dev-Build (Debug, erweitertes [Dev]-Logging via
#                           #if DEBUG, Version 1.2.3-dev.0)
#   ./build.sh --deploy     gregCore.dll in den Spielordner kopieren (Mods/)
#   ./build.sh --both       Release UND Dev bauen
#
# Artefakte landen in Releases/: gregCore.dll (Release), gregCore.dev.dll (Dev).
# Nur EINE Variante darf gleichzeitig im Mods/-Ordner liegen (gleicher
# MelonInfo-Name "gregCore" -> Konflikt). --deploy schreibt deshalb immer die
# gewaehlte Variante als gregCore.dll in Mods/ und entfernt die andere.
#
# Flavor-Ausgabe (MelonInfo) steuert BuildInfo via Compile-Profil:
#   Release -> 1.2.3        (kein [Dev]-Logging)
#   Debug   -> 1.2.3-dev.0  ([Dev]-Logging aktiv)
set -e

GAME_DIR="${DATACENTER_HOME:-$HOME/.local/share/Steam/steamapps/common/Data Center}"
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIG="Release"
DEPLOY=0
BOTH=0

for arg in "$@"; do
  case "$arg" in
    --dev) CONFIG="Debug" ;;
    --release) CONFIG="Release" ;;
    --both) BOTH=1 ;;
    --deploy) DEPLOY=1 ;;
    Debug|Release) CONFIG="$arg" ;;
    *) ;;
  esac
done

mkdir -p "$HERE/Releases"

build_one() {  # $1 = Release|Debug
  local cfg="$1" flavor ver dst
  echo -e "\e[36m== gregCore [$cfg] ==\e[0m"
  if [ "$cfg" = "Debug" ]; then flavor="DEV"; ver="1.2.3-dev.0"; dst="gregCore.dev.dll"; else flavor="RELEASE"; ver="1.2.3"; dst="gregCore.dll"; fi
  dotnet restore "$HERE/gregCore.csproj" >/dev/null
  dotnet build "$HERE/gregCore.csproj" --configuration "$cfg" --no-restore --nologo -v q
  cp -p "$HERE/bin/$cfg/net6.0/gregCore.dll" "$HERE/Releases/$dst"
  LAST_DST="$dst"
  echo -e "\e[32mBuilt $dst ($flavor $ver).\e[0m"
}

if [ "$BOTH" -eq 1 ]; then
  build_one Release
  build_one Debug
else
  build_one "$CONFIG"
fi

if [ "$DEPLOY" -eq 1 ]; then
  if [ ! -d "$GAME_DIR" ]; then
    echo -e "\e[33mSpielverzeichnis '$GAME_DIR' nicht gefunden. DATACENTER_HOME setzen.\e[0m"
    exit 3
  fi
  MODS_DIR="$GAME_DIR/Mods"
  mkdir -p "$MODS_DIR"
  rm -f "$MODS_DIR/gregCore.dev.dll"
  # Bei --both ist die Deploy-Variante mehrdeutig -> Release (stabil) deployen.
  DEPLOY_SRC="$LAST_DST"
  if [ "$BOTH" -eq 1 ] && [ "$DEPLOY_SRC" != "gregCore.dll" ]; then
    DEPLOY_SRC="gregCore.dll"
    echo -e "\e[33m--both --deploy: Deploye Release-Variante; Dev liegt als Releases/gregCore.dev.dll bereit.\e[0m"
  fi
  cp -p "$HERE/Releases/$DEPLOY_SRC" "$MODS_DIR/gregCore.dll"
  echo -e "\e[32mDeployed Releases/$DEPLOY_SRC -> $MODS_DIR/gregCore.dll\e[0m"
fi

echo -e "\e[32mBuild pipeline completed successfully.\e[0m"
exit 0