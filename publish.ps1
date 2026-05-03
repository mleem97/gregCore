#Requires -Version 7.2
<#
.SYNOPSIS
    Publish script for gregCore — bundles the DLL and its NuGet dependencies.
.DESCRIPTION
    Runs dotnet publish, strips unwanted runtime assemblies, and copies
    the result into the Releases/ folder for distribution.
#>
$ErrorActionPreference = "Stop"

$projectPath = Join-Path $PSScriptRoot "gregCore.csproj"
$releaseDir  = Join-Path $PSScriptRoot "Releases"
$tempDir     = Join-Path $PSScriptRoot "bin" "PublishTemp"

# 1. Clean temp
if (Test-Path $tempDir) {
    Remove-Item -Recurse -Force $tempDir
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

# 2. Publish
Write-Host "[Publish] Building gregCore (Release)..."
dotnet publish $projectPath -c Release --self-contained false -o $tempDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed." }

# 3. Strip assemblies that must NOT ship (see CleanupAfterBuild in .csproj)
$unwanted = @("Acornima.dll", "Python.Runtime.dll")
foreach ($file in $unwanted) {
    $path = Join-Path $tempDir $file
    if (Test-Path $path) {
        Remove-Item $path -Force
        Write-Host "[Publish] Stripped $file"
    }
}

# 4. Ensure Releases dir exists
if (-not (Test-Path $releaseDir)) {
    New-Item -ItemType Directory -Path $releaseDir | Out-Null
}

# 5. Copy runtime assemblies
$shipped = 0
Get-ChildItem -Path $tempDir -Filter "*.dll" | ForEach-Object {
    Copy-Item $_.FullName -Destination $releaseDir -Force
    Write-Host "[Publish] $($_.Name) -> Releases"
    $shipped++
}

# 6. Cleanup temp
Remove-Item -Recurse -Force $tempDir

Write-Host "[Publish] Done. $shipped assembly(s) ready in $releaseDir"
