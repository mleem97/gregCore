#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Creates SDK packs for all supported languages (Lua, JS, Python, Go, Rust).
    Each pack is prepared as a ZIP for workshop upload.

.DESCRIPTION
    Copies example mods + documentation into sdk/packs/{language}/ and creates ZIPs.
    Called by Deploy-Release-ToWorkshop.ps1.

.PARAMETER OutputDir
    Target directory for the created ZIP packs. Default: sdk/packs
#>

param(
    [string]$OutputDir = (Join-Path $PSScriptRoot "..\sdk\packs")
)

$ErrorActionPreference = "Stop"
$RootDir = Split-Path $PSScriptRoot -Parent
$ExamplesDir = Join-Path $RootDir "examples"

Write-Information "=== gregCore SDK Pack Builder ==="
Write-Information "Root: $RootDir"
Write-Information "Examples: $ExamplesDir"
Write-Information "Output: $OutputDir"
Write-Information ""

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
        Write-Information "  [SKIP] $($sdk.Name) – No example under $srcDir"
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

$($sdk.Desc) for Data Center modding.

## Installation

1. Copy the contents of this folder to ``Data Center/Mods/$($sdk.Name)Mods/``
2. Make sure gregCore v1.1.0+ is installed
3. Start Data Center with MelonLoader

## API

See gregCore wiki: https://gregframework.eu/wiki/sdk/$($sdk.Name.ToLower())

## Support

- Discord: #modding-sdk
- GitHub Issues: gregCore/issues
"@
    $readme | Set-Content -Path (Join-Path $packDir "README.md") -Encoding UTF8

    # Create ZIP
    Compress-Archive -Path "$packDir\*" -DestinationPath $zipFile -Force
    
    $zipSize = (Get-Item $zipFile).Length / 1KB
    Write-Information "  [OK] $($sdk.Name) → $zipFile ($([math]::Round($zipSize, 1)) KB)"
    $packedCount++
}

Write-Information ""
Write-Information "=== $packedCount SDK packs created in $OutputDir ==="
