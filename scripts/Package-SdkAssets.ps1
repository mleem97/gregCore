# Package-SdkAssets.ps1
# Creates SDK release assets for all supported languages.

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path "$PSScriptRoot\..\.."
$coreRoot = "$repoRoot\gregCore"
$packsDir = "$coreRoot\sdk\packs"

Write-Host "Creating SDK packs in $packsDir" -ForegroundColor Cyan

if (Test-Path $packsDir) {
    Remove-Item -Recurse -Force $packsDir
}
New-Item -ItemType Directory -Path $packsDir | Out-Null

# 1. Package greg-rust-sdk
$rustSdkDir = "$repoRoot\greg-rust-sdk"
if (Test-Path $rustSdkDir) {
    Write-Host "Packaging Rust SDK..."
    Compress-Archive -Path "$rustSdkDir\*" -DestinationPath "$packsDir\greg-rust-sdk.zip" -Force
}

# 2. Package other languages from examples
$examplesDir = "$coreRoot\examples"
if (Test-Path $examplesDir) {
    $langs = @("Go", "Js", "Lua", "Python", "Rust")
    foreach ($lang in $langs) {
        $langDir = "$examplesDir\$lang"
        if (Test-Path $langDir) {
            Write-Host "Packaging $lang SDK/Example..."
            $destZip = "$packsDir\greg-${lang}-sdk.zip".ToLower()
            Compress-Archive -Path "$langDir\*" -DestinationPath $destZip -Force
        }
    }
}

Write-Host "SDK packs created successfully!" -ForegroundColor Green
