# Dot-source, then call: Resolve-WorkshopUploaderMonorepoDir $RepoRoot
Set-StrictMode -Version Latest
function Resolve-WorkshopUploaderMonorepoDir {
    param(
        [Parameter(Mandatory)]
        [string]$RepoRoot
    )
    foreach ($name in @('workshopuploader', 'WorkshopUploader')) {
        $dir = Join-Path $RepoRoot $name
        $proj = Join-Path $dir 'WorkshopUploader.csproj'
        if (-not (Test-Path -LiteralPath $proj)) {
            continue
        }
        return (Get-Item -LiteralPath $dir).Name
    }
    throw "WorkshopUploader.csproj not found under workshopuploader\ or WorkshopUploader\ (repo root: $RepoRoot)"
}
