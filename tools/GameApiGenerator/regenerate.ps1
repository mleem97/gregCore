# Regeneriert src/gregCore.GameApi/Generated aus der Dummy-Assembly-CSharp.dll.
# Aufruf aus dem Repo-Root:  tools/GameApiGenerator/regenerate.ps1 [-AssemblyPath ...]
param([string]$AssemblyPath = "")
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
if ([string]::IsNullOrWhiteSpace($AssemblyPath)) { $AssemblyPath = Join-Path $root "references/Assembly-CSharp.dll" }
dotnet run --project (Join-Path $root "tools/GameApiGenerator/GameApiGenerator.csproj") -c Release -- `
  $AssemblyPath (Join-Path $root "references") (Join-Path $root "src/gregCore.GameApi")
