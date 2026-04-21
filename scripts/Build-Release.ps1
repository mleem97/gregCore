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
dotnet build "$repoRoot\gregCore.csproj" -c Release -o "$buildDir"

# 3. Check output
$dllPath = "$buildDir\gregCore.dll"
if (-not (Test-Path $dllPath)) {
    Write-Error "Build failed: gregCore.dll not found at $dllPath"
}

# 4. Package SDK Assets
& "$PSScriptRoot\Package-SdkAssets.ps1"

# 5. Package release
$version = (Get-Content "$repoRoot\VERSION").Trim()
$zipName = "gregCore-v$version.zip"
$zipPath = "$publishDir\$zipName"

Write-Host "Packaging version $version into $zipName..." -ForegroundColor Yellow

# Copy files to a temporary folder for zipping
$tmpDir = "$publishDir\tmp"
if (Test-Path $tmpDir) { Remove-Item -Recurse -Force $tmpDir }
New-Item -ItemType Directory -Path $tmpDir | Out-Null
Copy-Item $dllPath -Destination $tmpDir
Copy-Item "$repoRoot\README.md" -Destination $tmpDir
Copy-Item "$repoRoot\CHANGELOG.md" -Destination $tmpDir
Copy-Item "$repoRoot\assets\greg_hooks.json" -Destination $tmpDir

# Create zip
Compress-Archive -Path "$tmpDir\*" -DestinationPath $zipPath -Force

Remove-Item -Recurse -Force $tmpDir

# Copy SDK packs to publish dir for release workflow
Write-Host "Copying SDK packs to $publishDir..." -ForegroundColor Yellow
Copy-Item -Path "$repoRoot\sdk\packs\*.zip" -Destination $publishDir -ErrorAction SilentlyContinue

Write-Host "--- Successfully packaged gregCore to $zipPath ---" -ForegroundColor Green

