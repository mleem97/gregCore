# Build-Release.ps1
# Builds gregCore and packages it for release.

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path "$PSScriptRoot\.."
$buildDir = "$repoRoot\bin\Release"
$publishDir = "$repoRoot\publish"

Write-Host "--- gregCore Build & Package Script ---" -ForegroundColor Cyan

# 1. Clean up
if (Test-Path $publishDir) { Remove-Item -Recurse -Force $publishDir }
New-Item -ItemType Directory -Path $publishDir | Out-Null

# 2. Build
Write-Host "Building gregCore (Release)..." -ForegroundColor Yellow
dotnet build "$repoRoot\gregCore.csproj" -c Release

# 3. Check output
$dllPath = "$repoRoot\bin\Release\net6.0\gregCore.dll"
if (-not (Test-Path $dllPath)) {
    Write-Error "Build failed: gregCore.dll not found at $dllPath"
}

# 4. Package
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dllPath).FileVersion
$zipName = "gregCore-v$version.zip"
$zipPath = "$publishDir\$zipName"

Write-Host "Packaging version $version into $zipName..." -ForegroundColor Yellow

# Copy files to a temporary folder for zipping
$tmpDir = "$publishDir\tmp"
New-Item -ItemType Directory -Path $tmpDir | Out-Null
Copy-Item $dllPath -Destination $tmpDir
Copy-Item "$repoRoot\README.md" -Destination $tmpDir
Copy-Item "$repoRoot\CHANGELOG.md" -Destination $tmpDir
Copy-Item "$repoRoot\gregFramework\greg_hooks.json" -Destination $tmpDir

# Create zip
Compress-Archive -Path "$tmpDir\*" -DestinationPath $zipPath -Force

Remove-Item -Recurse -Force $tmpDir

Write-Host "--- Successfully packaged gregCore to $zipPath ---" -ForegroundColor Green
