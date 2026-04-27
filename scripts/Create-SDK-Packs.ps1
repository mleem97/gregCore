#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Erstellt SDK-Packs für alle unterstützten Sprachen (Lua, JS, Python, Go, Rust).
    Jedes Pack wird als ZIP für Workshop-Upload vorbereitet.

.DESCRIPTION
    Kopiert Beispiel-Mods + Dokumentation in sdk/packs/{Sprache}/ und erstellt ZIPs.
    Wird von Deploy-Release-ToWorkshop.ps1 aufgerufen.

.PARAMETER OutputDir
    Zielverzeichnis für die erstellten ZIP-Packs. Default: sdk/packs
#>

param(
    [string]$OutputDir = (Join-Path $PSScriptRoot "..\sdk\packs")
)

$ErrorActionPreference = "Stop"
$RootDir = Split-Path $PSScriptRoot -Parent
$ExamplesDir = Join-Path $RootDir "examples"

Write-Host "=== gregCore SDK Pack Builder ===" -ForegroundColor Cyan
Write-Host "Root: $RootDir"
Write-Host "Examples: $ExamplesDir"
Write-Host "Output: $OutputDir"
Write-Host ""

# Ensure output dir exists
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
}

$sdks = @(
    @{ Name = "Lua";    Dir = "Lua";    Desc = "MoonSharp Lua SDK" },
    @{ Name = "Js";     Dir = "Js";     Desc = "Jint JavaScript SDK" },
    @{ Name = "Python"; Dir = "Python"; Desc = "pythonnet Python SDK" },
    @{ Name = "Go";     Dir = "Go";     Desc = "CGo FFI SDK" },
    @{ Name = "Rust";   Dir = "Rust";   Desc = "Rust FFI SDK" }
)

$packedCount = 0

foreach ($sdk in $sdks) {
    $srcDir = Join-Path $ExamplesDir $sdk.Dir
    
    if (!(Test-Path $srcDir)) {
        Write-Host "  [SKIP] $($sdk.Name) – Kein Beispiel unter $srcDir" -ForegroundColor Yellow
        continue
    }

    $packDir = Join-Path $OutputDir "gregCore-SDK-$($sdk.Name)"
    $zipFile = Join-Path $OutputDir "gregCore-SDK-$($sdk.Name).zip"

    # Clean previous pack
    if (Test-Path $packDir) { Remove-Item -Recurse -Force $packDir }
    if (Test-Path $zipFile) { Remove-Item -Force $zipFile }

    # Create pack structure
    New-Item -ItemType Directory -Force -Path $packDir | Out-Null

    # Copy example files
    Copy-Item -Path "$srcDir\*" -Destination $packDir -Recurse -Force

    # Create manifest
    $manifest = @{
        name        = "gregCore-SDK-$($sdk.Name)"
        version     = "1.1.0"
        description = $sdk.Desc
        language    = $sdk.Name
        requires    = "gregCore@1.1.0"
    } | ConvertTo-Json -Depth 3
    
    $manifest | Set-Content -Path (Join-Path $packDir "manifest.json") -Encoding UTF8

    # Create README
    $readme = @"
# gregCore $($sdk.Name) SDK

$($sdk.Desc) für Data Center Modding.

## Installation

1. Kopiere den Inhalt dieses Ordners nach ``Data Center/Mods/$($sdk.Name)Mods/``
2. Stelle sicher, dass gregCore v1.1.0+ installiert ist
3. Starte Data Center mit MelonLoader

## API

Siehe gregCore Wiki: https://gregframework.eu/wiki/sdk/$($sdk.Name.ToLower())

## Unterstützung

- Discord: #modding-sdk
- GitHub Issues: gregCore/issues
"@
    $readme | Set-Content -Path (Join-Path $packDir "README.md") -Encoding UTF8

    # Create ZIP
    Compress-Archive -Path "$packDir\*" -DestinationPath $zipFile -Force
    
    $zipSize = (Get-Item $zipFile).Length / 1KB
    Write-Host "  [OK] $($sdk.Name) → $zipFile ($([math]::Round($zipSize, 1)) KB)" -ForegroundColor Green
    $packedCount++
}

Write-Host ""
Write-Host "=== $packedCount SDK-Packs erstellt in $OutputDir ===" -ForegroundColor Green
