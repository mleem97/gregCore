#!/bin/bash
set -e

echo -e "\e[36mRestoring NuGet packages...\e[0m"
dotnet restore src/gregCore.csproj

echo -e "\e[36mBuilding and packing gregCore.dll via ILRepack...\e[0m"
dotnet build src/gregCore.csproj --configuration Release

# Game directory for Linux / Proton
GAME_DIR="$HOME/.local/share/Steam/steamapps/common/Data Center"

if [ -d "$GAME_DIR" ]; then
    MODS_DIR="$GAME_DIR/Mods"
    mkdir -p "$MODS_DIR"
    
    SOURCE_DLL="src/bin/Release/net6.0/gregCore.dll"
    if [ -f "$SOURCE_DLL" ]; then
        echo -e "\e[36mDeploying to $MODS_DIR...\e[0m"
        cp "$SOURCE_DLL" "$MODS_DIR/"
        echo -e "\e[32mDeployment successful.\e[0m"
    else
        echo -e "\e[31mBuilt DLL not found at $SOURCE_DLL\e[0m"
        exit 1
    fi
else
    echo -e "\e[33mData Center directory not found. Skipping deploy.\e[0m"
fi

echo -e "\e[32mBuild pipeline completed successfully.\e[0m"
exit 0
