# Copies a published WorkshopUploader build into the game folder under WorkshopUploader/.
# Usage:
#   pwsh -File scripts/Copy-WorkshopUploaderToGame.ps1 -PublishDir "WorkshopUploader\bin\Release\net6.0-windows\win-x64\publish" -GameDir "C:\Program Files (x86)\Steam\steamapps\common\Data Center"

param(
    [Parameter(Mandatory = $true)]
    [string] $PublishDir,
    [Parameter(Mandatory = $true)]
    [string] $GameDir
)

$dest = Join-Path $GameDir "WorkshopUploader"
if (-not (Test-Path -LiteralPath $PublishDir)) {
    throw "Publish directory not found: $PublishDir"
}
New-Item -ItemType Directory -Path $dest -Force | Out-Null
Copy-Item -Path (Join-Path $PublishDir "*") -Destination $dest -Recurse -Force
Write-Host "Copied to $dest"
