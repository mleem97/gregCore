#!/usr/bin/env pwsh

$ErrorActionPreference = "Stop"

$ProjFile = Join-Path $PSScriptRoot "gregCore.csproj"
$OutDir = Join-Path $PSScriptRoot "publish_out"

Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore $ProjFile
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed."
    exit 1
}

Write-Host "Building and publishing gregCore and dependencies..." -ForegroundColor Cyan
dotnet publish $ProjFile --configuration Release -o $OutDir
if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed."
    exit 1
}

# Try to find Data Center
$GameDir = "C:\Program Files (x86)\Steam\steamapps\common\Data Center"
if (!(Test-Path $GameDir)) {
    Write-Host "Data Center directory not found at default location. Skipping deploy." -ForegroundColor Yellow
} else {
    $ModsDir = Join-Path $GameDir "Mods"
    $UserLibsDir = Join-Path $GameDir "UserLibs"
    
    if (!(Test-Path $ModsDir)) {
        New-Item -ItemType Directory -Force -Path $ModsDir | Out-Null
    }
    if (!(Test-Path $UserLibsDir)) {
        New-Item -ItemType Directory -Force -Path $UserLibsDir | Out-Null
    }
    
    $SourceDll = Join-Path $OutDir "gregCore.dll"
    if (Test-Path $SourceDll) {
        Write-Host "Deploying gregCore.dll to $ModsDir..." -ForegroundColor Cyan
        Copy-Item -Path $SourceDll -Destination $ModsDir -Force
        
        Write-Host "Deploying dependencies to $UserLibsDir..." -ForegroundColor Cyan
        $Deps = @(
            "Acornima.dll",
            "Jint.dll",
            "LiteDB.dll",
            "MoonSharp.Interpreter.dll",
            "Newtonsoft.Json.dll",
            "Python.Runtime.dll"
        )
        
        foreach ($dep in $Deps) {
            $depPath = Join-Path $OutDir $dep
            if (Test-Path $depPath) {
                Copy-Item -Path $depPath -Destination $UserLibsDir -Force
                Write-Host "  -> $dep" -ForegroundColor Gray
            } else {
                Write-Host "  Warning: Dependency $dep not found in $OutDir" -ForegroundColor Yellow
            }
        }
        
        Write-Host "Deployment successful." -ForegroundColor Green
    } else {
        Write-Error "Built DLL not found at $SourceDll"
        exit 1
    }
}

Write-Host "Build pipeline completed successfully." -ForegroundColor Green
exit 0
