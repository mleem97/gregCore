#!/usr/bin/env pwsh

$ErrorActionPreference = "Stop"

Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore src/gregCore.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed."
    exit 1
}

Write-Host "Building and packing gregCore.dll via ILRepack..." -ForegroundColor Cyan
dotnet build src/gregCore.csproj --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed."
    exit 1
}

# Try to find Data Center
$GameDir = "C:\Program Files (x86)\Steam\steamapps\common\Data Center"
if (!(Test-Path $GameDir)) {
    Write-Host "Data Center directory not found at default location. Skipping deploy." -ForegroundColor Yellow
} else {
    $ModsDir = Join-Path $GameDir "Mods"
    if (!(Test-Path $ModsDir)) {
        New-Item -ItemType Directory -Force -Path $ModsDir | Out-Null
    }
    
    $SourceDll = "src/bin/Release/net6.0/gregCore.dll"
    if (Test-Path $SourceDll) {
        Write-Host "Deploying to $ModsDir..." -ForegroundColor Cyan
        Copy-Item -Path $SourceDll -Destination $ModsDir -Force
        Write-Host "Deployment successful." -ForegroundColor Green
    } else {
        Write-Error "Built DLL not found at $SourceDll"
        exit 1
    }
}

Write-Host "Build pipeline completed successfully." -ForegroundColor Green
exit 0
