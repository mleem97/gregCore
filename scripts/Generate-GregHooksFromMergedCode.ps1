#requires -Version 7.2
param(
    [string]$MergedCodePath = (Join-Path (Split-Path $PSScriptRoot -Parent) "MergedCode.md"),
    [string]$OutJson = (Join-Path $PSScriptRoot ".." "framework" "gregFramework" "greg_hooks.json")
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $MergedCodePath)) {
    Write-Error "MergedCode.md not found: $MergedCodePath (expected at gregFramework repo root)"
}

$py = Join-Path $PSScriptRoot "parse_merged_code.py"
& python $py --merged $MergedCodePath --out $OutJson @args
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "OK: $OutJson"

