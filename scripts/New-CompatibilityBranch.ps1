#!/usr/bin/env pwsh
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory = $true)]
    [string]$Profile,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+(?:[-+][0-9A-Za-z.-]+)?$')]
    [string]$FrameworkVersion,

    [string]$StartPoint = 'HEAD',
    [switch]$Push
)

$ErrorActionPreference = 'Stop'
$profileData = Get-Content -Raw -LiteralPath $Profile | ConvertFrom-Json
$unity = [string]$profileData.unity.version
$game = [string]$profileData.game.version
if ([string]::IsNullOrWhiteSpace($unity) -or [string]::IsNullOrWhiteSpace($game)) {
    throw 'Profile must contain unity.version and game.version.'
}

$parts = $FrameworkVersion.Split('.')
$line = "$($parts[0]).$($parts[1]).x"
$maintenance = "compat/u$unity/game-$game/gc-$line"
$archive = "archive/u$unity/game-$game/gc-$FrameworkVersion"
$tag = "u$unity-game$game-gc$FrameworkVersion"

& git rev-parse --verify $StartPoint | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Unknown start point: $StartPoint" }

foreach ($ref in @($maintenance, $archive)) {
    & git show-ref --verify --quiet "refs/heads/$ref"
    if ($LASTEXITCODE -eq 0) {
        if ($ref -eq $archive) { throw "Immutable archive branch already exists: $archive" }
        Write-Host "Maintenance branch already exists: $maintenance"
        continue
    }

    if ($PSCmdlet.ShouldProcess($ref, "Create from $StartPoint")) {
        & git branch $ref $StartPoint
        if ($LASTEXITCODE -ne 0) { throw "Failed to create $ref" }
    }
}

& git show-ref --verify --quiet "refs/tags/$tag"
if ($LASTEXITCODE -eq 0) { throw "Release tag already exists: $tag" }
if ($PSCmdlet.ShouldProcess($tag, "Create annotated tag from $StartPoint")) {
    & git tag -a $tag $StartPoint -m "gregCore $FrameworkVersion for $($profileData.profileId)"
    if ($LASTEXITCODE -ne 0) { throw "Failed to create tag $tag" }
}

if ($Push) {
    & git push origin $maintenance $archive $tag
    if ($LASTEXITCODE -ne 0) { throw 'Push failed.' }
}

[pscustomobject]@{
    Profile = $profileData.profileId
    MaintenanceBranch = $maintenance
    ArchiveBranch = $archive
    Tag = $tag
}
