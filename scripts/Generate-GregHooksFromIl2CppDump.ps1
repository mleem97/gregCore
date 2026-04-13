# Generates greg_hooks.json + gregCore/framework/harmony/*.cs from Il2CppInterop C# sources
# (stand-in when repo root MergedCode.md is absent). Re-run after game / interop updates.

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$il2cppDir = Join-Path $repoRoot 'gregReferences\il2cpp-unpack\Assembly-CSharp\Il2Cpp'
$harmonyPatches = Join-Path $repoRoot 'gregCore\framework\ModLoader\HarmonyPatches.cs'
$outJsonRoot = Join-Path $repoRoot 'greg_hooks.json'
$outJsonFramework = Join-Path $repoRoot 'gregCore\framework\greg_hooks.json'
$outHooksDir = Join-Path $repoRoot 'gregCore\framework\harmony'

if (-not (Test-Path $il2cppDir)) { throw "Missing Il2Cpp sources: $il2cppDir" }
New-Item -ItemType Directory -Force -Path $outHooksDir | Out-Null

function Get-HarmonyExclusions([string]$path) {
    $set = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    if (-not (Test-Path $path)) { return $set }
    $text = Get-Content $path -Raw
    foreach ($m in [regex]::Matches($text, 'typeof\((\w+)\)\s*,\s*nameof\(\w+\.(\w+)\)')) {
        [void]$set.Add("$($m.Groups[1].Value)|$($m.Groups[2].Value)")
    }
    foreach ($m in [regex]::Matches($text, 'typeof\((\w+)\)\s*,\s*"(\w+)"')) {
        [void]$set.Add("$($m.Groups[1].Value)|$($m.Groups[2].Value)")
    }
    return $set
}

$excluded = Get-HarmonyExclusions $harmonyPatches

# Curated game surface for stable compile (full Il2Cpp tree needs extra Unity/asm refs).
$gameHookClasses = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
@(
    'Player', 'PlayerManager', 'PlayerHit', 'PlayerData',
    'Server', 'MainGameManager', 'ComputerShop', 'HRSystem', 'SaveSystem', 'CustomerBase',
    'CablePositions', 'CableLink', 'Rack', 'NetworkMap', 'BalanceSheet', 'MainMenu',
    'TimeController', 'TechnicianManager', 'Technician', 'Objectives',
    'PacketSpawnerSystem', 'NetworkSwitch', 'SFPModule', 'SFPBox', 'PatchPanel'
) | ForEach-Object { [void]$gameHookClasses.Add($_) }

function Test-SkipTypeName([string]$n) {
    if ($n -match 'd__\d+$') { return $true }
    if ($n -match 'b__\d+$') { return $true }
    if ($n -eq 'TypeHandle') { return $true }
    if ($n.StartsWith('__')) { return $true }
    if ($n.StartsWith('_PrivateImplementationDetails_')) { return $true }
    return $false
}

function Get-GregDomain([string]$className) {
    $c = $className
    if ($c -match '^(Player|PlayerData|PlayerManager|ObjectInHand)') { return 'Player' }
    if ($c -match '^(Technician|Employee|Staff|HRSystem)') { return 'Employee' }
    if ($c -match '^(Customer|Contract|Client|SLA)') { return 'Customer' }
    if ($c -match '^(Server|Hardware|Component)') { return 'Server' }
    if ($c -match '^(Rack|RackSlot|RackUnit)') { return 'Rack' }
    if ($c -match '^(Network|Switch|Cable|Packet|Port|SFP)') { return 'Network' }
    if ($c -match '^(Power|UPS|PDU|Grid|Energy)') { return 'Power' }
    if ($c -match '^(Job|Task|Objective|Quest|Mission)') { return 'Gameplay' }
    if ($c -match '^(UI|Menu|Screen|Panel|Overlay|HUD|Notification|Tutorial|Pause|Loading|MainMenu|Settings|BalanceSheet|Chat|Tooltip|KeyHint|Rebind)') { return 'Ui' }
    if ($c -match '^(GameManager|MainGameManager|SaveManager|LoadManager|SceneManager|SaveData|ModLoader|SteamManager|TimeController|AudioManager|Waypoint)') { return 'System' }
    return 'System'
}

function Get-SemanticAction([string]$className, [string]$methodName) {
    $key = "$className|$methodName"
    $map = @{
        'Player|UpdateCoin'                 = 'MoneyChanged'
        'Player|UpdateReputation'         = 'ReputationChanged'
        'Player|UpdateXP'                 = 'XpChanged'
        'Player|WarpPlayer'               = 'Warped'
        'Player|DropAllItems'             = 'DroppedAllItems'
        'Player|LoadPlayer'               = 'Loaded'
        'Player|Start'                    = 'ComponentInitialized'
        'TechnicianManager|AddTechnician' = 'Hired'
        'TechnicianManager|FireTechnician' = 'Fired'
        'TechnicianManager|SendTechnician' = 'Dispatched'
        'TechnicianManager|EnqueueDispatch' = 'JobQueued'
        'TechnicianManager|Awake'         = 'ComponentInitialized'
        'TechnicianManager|RestoreJobQueue' = 'JobQueueLoaded'
        'TechnicianManager|RequestNextJob' = 'NextJobRequested'
        'PacketSpawnerSystem|SpawnPacket' = 'PacketSpawned'
    }
    if ($map.ContainsKey($key)) { return $map[$key] }

    $m = $methodName
    if ($m -eq 'Awake' -or $m -eq 'Start' -or $m -eq 'OnEnable') { return 'ComponentInitialized' }
    if ($m -eq 'OnDisable') { return 'ComponentDisabled' }
    if ($m.StartsWith('Add')) { return ($m.Substring(3) + 'Added') }
    if ($m.StartsWith('Remove')) { return ($m.Substring(6) + 'Removed') }
    if ($m.StartsWith('Update')) { return ($m.Substring(6) + 'Changed') }
    if ($m.StartsWith('Spawn')) { return ($m.Substring(5) + 'Spawned') }
    if ($m.StartsWith('Fire')) { return ($m.Substring(4) + 'Fired') }
    if ($m.StartsWith('Hire')) { return ($m.Substring(4) + 'Hired') }
    if ($m.StartsWith('Buy')) { return ($m.Substring(3) + 'Purchased') }
    if ($m.StartsWith('Sell')) { return ($m.Substring(4) + 'Sold') }
    if ($m.StartsWith('Place')) { return ($m.Substring(5) + 'Placed') }
    if ($m.StartsWith('Load')) { return ($m.Substring(4) + 'Loaded') }
    if ($m.StartsWith('Save')) { return ($m.Substring(4) + 'Saved') }
    if ($m.StartsWith('Warp')) { return ($m.Substring(4) + 'Warped') }
    if ($m.StartsWith('Repair')) { return ($m.Substring(6) + 'Repaired') }
    if ($m.StartsWith('Install')) { return ($m.Substring(7) + 'Installed') }
    if ($m.StartsWith('Break')) { return ($m.Substring(5) + 'Broken') }
    if ($m.StartsWith('Send')) { return ($m.Substring(4) + 'Dispatched') }
    if ($m.StartsWith('Enqueue')) { return ($m.Substring(8) + 'JobQueued') }
    if ($m.StartsWith('Set')) { return ($m.Substring(3) + 'Set') }
    if ($m.StartsWith('Drop')) { return ($m.Substring(4) + 'Dropped') }
    return $m
}

function Get-HookStrategy([string]$methodName, [string]$returnType) {
    if ($methodName -in @('Update', 'FixedUpdate', 'LateUpdate')) { return 'None' }
    if ($methodName -eq 'OnUpdate') { return 'None' }
    # Emit Postfix only: generated Prefix stubs were non-functional and broke Harmony expectations.
    return 'Postfix'
}

function Test-SkipClass([string]$className, [string]$baseClause) {
    if ($className -in @('ModLoader', 'UnitySourceGeneratedAssemblyMonoScriptTypes_v1')) { return $true }
    if ($baseClause -match 'SystemBase') { return $true }
    return $false
}

function Test-SkipInteropSignature([string]$returnType, [string]$argList) {
    $blob = "$returnType $argList"
    if ($blob -match 'EntityCommandBuffer|SystemState|BlobArray|ComponentLookup|BufferLookup') { return $true }
    if ($blob -match '\bEntity\b') { return $true }
    if ($blob -match 'RaycastHit|TextMeshProUGUI|Il2CppStructArray|HashSet<') { return $true }
    return $false
}

function Normalize-HookParamType([string]$pt) {
    $t = $pt.Trim()
    if ($t.StartsWith('Il2CppSystem.Collections.Generic.')) { return $t }
    $t = $t -replace '(?<![\w.])List<', 'Il2CppSystem.Collections.Generic.List<'
    $t = $t -replace '(?<![\w.])Dictionary<', 'Il2CppSystem.Collections.Generic.Dictionary<'
    return $t
}

function Test-ShouldEmitHook([string]$methodName, [string]$returnType) {
    if ($methodName -in @('Update', 'FixedUpdate', 'LateUpdate', 'OnUpdate')) { return $false }
    if ($methodName -eq '.ctor' -or $methodName.StartsWith('op_')) { return $false }
    if ($methodName.Contains('__') -or $methodName.Contains('codegen') -or $methodName.Contains('MethodInternalStatic')) { return $false }
    if ($methodName.StartsWith('get_') -or $methodName.StartsWith('set_')) { return $false }
    $rt = $returnType.Trim()
    if ($rt -eq 'IEnumerator' -or $rt.StartsWith('IEnumerator')) { return $false }
    if ($methodName.StartsWith('Get') -and $rt -ne 'void') {
        if ($methodName -in @('GetQueuedJobs', 'GetActiveJobs')) { return $false }
    }
    return $true
}

function Split-ArgSegments([string]$argListText) {
    $segments = [System.Collections.Generic.List[string]]::new()
    if ([string]::IsNullOrWhiteSpace($argListText)) { return $segments }
    $depth = 0
    $sb = [System.Text.StringBuilder]::new()
    for ($i = 0; $i -lt $argListText.Length; $i++) {
        $c = $argListText[$i]
        if ($c -eq '<') { $depth++ }
        elseif ($c -eq '>') { $depth-- }
        elseif ($c -eq ',' -and $depth -eq 0) {
            [void]$segments.Add($sb.ToString().Trim())
            [void]$sb.Clear()
            continue
        }
        [void]$sb.Append($c)
    }
    if ($sb.Length -gt 0) { [void]$segments.Add($sb.ToString().Trim()) }
    return $segments
}

function Get-Il2CppPatchSignature([string]$className, [string]$methodName, [string]$argListText) {
    if ([string]::IsNullOrWhiteSpace($argListText)) { return "Il2Cpp.$className::$methodName()" }
    $parts = Split-ArgSegments $argListText
    $types = foreach ($p in $parts) {
        $t = ($p -replace '\s*=\s*[^,)]+$', '').Trim()
        if ($t.Length -eq 0) { continue }
        $t = $t -replace '^\s*(ref|out|in|readonly)\s+', ''
        $sp = $t -split '\s+'
        if ($sp.Length -lt 2) { continue }
        ($sp[0..($sp.Length - 2)] -join ' ').Trim()
    }
    return "Il2Cpp.$className::$methodName($($types -join ', '))"
}

function Split-MethodLine([string]$line, [ref]$isStatic) {
    $isStatic.Value = $false
    $m = [regex]::Match($line, '^\t{2}public unsafe static (?<sig>.+?)\s*\((?<args>[^\)]*)\)\s*$')
    if ($m.Success) { $isStatic.Value = $true }
    else {
        $m = [regex]::Match($line, '^\t{2}public unsafe (?<sig>.+?)\s*\((?<args>[^\)]*)\)\s*$')
    }
    if (-not $m.Success) { return $null }
    $sig = $m.Groups['sig'].Value.Trim()
    if ($sig.StartsWith('static ')) { $sig = $sig.Substring(7).Trim() }
    $argList = $m.Groups['args'].Value.Trim()
    $idx = $sig.LastIndexOf(' ')
    if ($idx -lt 0) { return $null }
    $ret = $sig.Substring(0, $idx).Trim()
    $name = $sig.Substring($idx + 1).Trim()
    return [pscustomobject]@{ ReturnType = $ret; Name = $name; ArgList = $argList }
}

function Build-HarmonyParamDecl([string]$className, [bool]$isStatic, [string]$argList, [bool]$includeResultRef) {
    $plist = @()
    if (-not $isStatic) { $plist += "$className __instance" }
    if ($includeResultRef) { $plist += 'ref bool __result' }
    if (-not [string]::IsNullOrWhiteSpace($argList)) {
        foreach ($segment in (Split-ArgSegments $argList)) {
            $t = ($segment -replace '\s*=\s*[^,)]+$', '').Trim()
            if ($t.Length -eq 0) { continue }
            $tokens = @($t -split '\s+' | Where-Object { $_ -notin @('ref', 'out', 'in', 'readonly') })
            if ($tokens.Length -lt 2) { continue }
            $pn = $tokens[-1]
            $pt = Normalize-HookParamType (($tokens[0..($tokens.Length - 2)] -join ' '))
            $plist += "$pt $pn"
        }
    }
    return ($plist -join ', ')
}

function Build-EmitAnonymousBody([string]$className, [bool]$isStatic) {
    if ($className -eq 'Player' -and -not $isStatic) {
        return @'
                    money = __instance.money,
                    reputation = __instance.reputation,
                    xp = __instance.xp,
'@
    }
    if (-not $isStatic) {
        return '                    instance = __instance,'
    }
    return '                    type = typeof(' + $className + '),'
}

$hooks = [System.Collections.Generic.List[object]]::new()
$byDomain = @{}

Get-ChildItem $il2cppDir -Filter '*.cs' -File | ForEach-Object {
    $lines = Get-Content $_.FullName
    $overloadFirst = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $currentClass = $null
    $currentBase = ''
    $classRegex = [regex]'^\tpublic (sealed )?class (?<name>[A-Za-z0-9_]+)\s*:\s*(?<base>[^\r\n{]+)'

    foreach ($line in $lines) {
        $cm = $classRegex.Match($line)
        if ($cm.Success) {
            $cn = $cm.Groups['name'].Value
            $cb = $cm.Groups['base'].Value.Trim()
            if (Test-SkipTypeName $cn) { $currentClass = $null; $currentBase = ''; continue }
            if (Test-SkipClass $cn $cb) { $currentClass = $null; $currentBase = ''; continue }
            if (-not $gameHookClasses.Contains($cn)) { $currentClass = $null; $currentBase = ''; continue }
            $currentClass = $cn
            $currentBase = $cb
            continue
        }
        if ($null -eq $currentClass) { continue }

        $staticFlag = $false
        $parsed = Split-MethodLine $line ([ref]$staticFlag)
        if ($null -eq $parsed) { continue }

        $ret = $parsed.ReturnType
        $mn = $parsed.Name
        $argList = $parsed.ArgList

        if (-not (Test-ShouldEmitHook $mn $ret)) { continue }
        if (Test-SkipInteropSignature $ret $argList) { continue }
        if ($excluded.Contains("$currentClass|$mn")) { continue }

        $ovKey = "$currentClass|$mn"
        if ($overloadFirst.Contains($ovKey)) { continue }
        [void]$overloadFirst.Add($ovKey)

        $domain = Get-GregDomain $currentClass
        $action = Get-SemanticAction $currentClass $mn
        $strategy = Get-HookStrategy $mn $ret
        if ($strategy -eq 'None') { continue }

        $hookName = "greg.$($domain.ToUpperInvariant()).$action"
        $patchTarget = Get-Il2CppPatchSignature $currentClass $mn $argList

        $payload = [ordered]@{ }
        if ($currentClass -eq 'Player' -and $mn -in @('UpdateCoin', 'UpdateReputation', 'UpdateXP', 'WarpPlayer', 'DropAllItems', 'LoadPlayer', 'Start')) {
            if ($mn -eq 'UpdateCoin') { $payload['coinChangeAmount'] = 'float'; $payload['withoutSound'] = 'bool'; $payload['newBalance'] = 'float'; $payload['accepted'] = 'bool' }
            elseif ($mn -eq 'UpdateReputation') { $payload['amount'] = 'float'; $payload['reputation'] = 'float' }
            elseif ($mn -eq 'UpdateXP') { $payload['amount'] = 'float'; $payload['xp'] = 'float'; $payload['accepted'] = 'bool' }
            elseif ($mn -eq 'WarpPlayer') { $payload['position'] = 'Vector3'; $payload['rotation'] = 'Quaternion' }
            elseif ($mn -eq 'LoadPlayer') { $payload['data'] = 'PlayerData' }
            else { $payload['money'] = 'float'; $payload['reputation'] = 'float'; $payload['xp'] = 'float' }
        }
        elseif ($currentClass -eq 'TechnicianManager') {
            if ($mn -eq 'AddTechnician') { $payload['technician'] = 'Technician' }
            elseif ($mn -eq 'FireTechnician') { $payload['technicianID'] = 'int' }
            elseif ($mn -eq 'SendTechnician') { $payload['networkSwitch'] = 'NetworkSwitch'; $payload['server'] = 'Server' }
            elseif ($mn -eq 'EnqueueDispatch') { $payload['job'] = 'TechnicianManager.RepairJob' }
            elseif ($mn -eq 'RestoreJobQueue') { $payload['savedJobs'] = 'List<RepairJobSaveData>' }
            elseif ($mn -eq 'RequestNextJob') { $payload['technician'] = 'Technician' }
            elseif ($mn -eq 'IsDeviceAlreadyAssigned') { $payload['networkSwitch'] = 'NetworkSwitch'; $payload['server'] = 'Server'; $payload['assigned'] = 'bool' }
            elseif ($mn -eq 'Awake') { $payload['queuedJobCount'] = 'int' }
        }
        elseif ($currentClass -eq 'PacketSpawnerSystem' -and $mn -eq 'SpawnPacket') {
            $payload['ecb'] = 'EntityCommandBuffer'
            $payload['spawner'] = 'PacketSpawnerComponent'
            $payload['spawnerIndex'] = 'int'
            $payload['waypoints'] = 'BlobArray<float3>'
        }
        else {
            $payload['method'] = 'string'
        }

        [void]$hooks.Add([ordered]@{
                name          = $hookName
                legacy        = $null
                patchTarget   = $patchTarget
                strategy      = $(if ($strategy -eq 'PrefixPostfix') { 'Prefix+Postfix' } else { 'Postfix' })
                description   = "Auto-generated from Il2Cpp unpack: $currentClass.$mn"
                payloadSchema = $payload
            })

        if (-not $byDomain.ContainsKey($domain)) { $byDomain[$domain] = [System.Collections.Generic.List[object]]::new() }
        [void]$byDomain[$domain].Add([ordered]@{
                Class     = $currentClass
                Method    = $mn
                Ret       = $ret
                Args      = $argList
                Action    = $action
                Strategy  = $strategy
                HookName  = $hookName
                IsStatic  = $staticFlag
            })
    }
}

$seen = @{}
$unique = [System.Collections.Generic.List[object]]::new()
foreach ($h in $hooks) {
    $k = [string]$h.patchTarget
    if ($seen.ContainsKey($k)) { continue }
    $seen[$k] = $true
    [void]$unique.Add($h)
}

$doc = [ordered]@{
    version        = 2
    description    = 'Canonical greg hook registry. Schema: greg.<DOMAIN>.<Action>. Generated from Il2Cpp C# unpack; regenerate with gregCore/scripts/Generate-GregHooksFromIl2CppDump.ps1 when MergedCode.md / interop changes.'
    generatedFrom  = 'gregReferences/il2cpp-unpack/Assembly-CSharp/Il2Cpp/*.cs'
    legacyPrefixes = @()
    hooks          = $unique
}

$json = $doc | ConvertTo-Json -Depth 20
[System.IO.File]::WriteAllText($outJsonRoot, $json, [System.Text.UTF8Encoding]::new($false))
[System.IO.File]::WriteAllText($outJsonFramework, $json, [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $($unique.Count) hooks to $outJsonRoot"

$domainEnumMap = @{
    Player    = 'Player'; Employee = 'Employee'; Customer = 'Customer'; Server = 'Server'
    Rack      = 'Rack'; Network = 'Network'; Power = 'Power'; Gameplay = 'Gameplay'; Ui = 'Ui'; System = 'System'
}

foreach ($kv in $byDomain.GetEnumerator()) {
    $d = $kv.Key
    $items = $kv.Value
    if ($items.Count -eq 0) { continue }

    $gregDomain = $domainEnumMap[$d]
    $className = "Greg$($d)Hooks"
    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine('using System;')
    [void]$sb.AppendLine('using HarmonyLib;')
    [void]$sb.AppendLine('using gregFramework.Core;')
    [void]$sb.AppendLine('using Il2Cpp;')
    [void]$sb.AppendLine('using Il2CppSystem.Collections.Generic;')
    [void]$sb.AppendLine('using Il2CppInterop.Runtime.InteropTypes.Arrays;')
    [void]$sb.AppendLine('using MelonLoader;')
    [void]$sb.AppendLine('using UnityEngine;')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('namespace gregFramework.Hooks;')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('/// <summary>')
    [void]$sb.AppendLine("/// Harmony hooks for domain $d (generated from Il2Cpp unpack).")
    [void]$sb.AppendLine('/// </summary>')
    [void]$sb.AppendLine('[HarmonyPatch]')
    [void]$sb.AppendLine("internal static class $className")
    [void]$sb.AppendLine('{')

    foreach ($it in $items) {
        $cn = $it.Class
        $mn = $it.Method
        $argList = $it.Args
        $action = $it.Action
        $strat = $it.Strategy
        $isSt = [bool]$it.IsStatic
        $handler = "On$cn$mn"

        [void]$sb.AppendLine("    // $cn.$mn")
        [void]$sb.AppendLine("    [HarmonyPatch(typeof($cn), nameof($cn.$mn))]")

        if ($strat -eq 'PrefixPostfix') {
            $pfxParams = Build-HarmonyParamDecl $cn $isSt $argList $true
            $postParams = Build-HarmonyParamDecl $cn $isSt $argList $true
            [void]$sb.AppendLine('    [HarmonyPrefix]')
            [void]$sb.AppendLine("    private static void ${handler}_Prefix($pfxParams)")
            [void]$sb.AppendLine('    {')
            [void]$sb.AppendLine('        // Prefix slot for cancellable bool flows; return false from a conditional transpiler if you skip the original.')
            [void]$sb.AppendLine('    }')
            [void]$sb.AppendLine('')
            [void]$sb.AppendLine('    [HarmonyPostfix]')
            [void]$sb.AppendLine("    private static void ${handler}_Postfix($postParams)")
        }
        else {
            $postParams = Build-HarmonyParamDecl $cn $isSt $argList $false
            [void]$sb.AppendLine('    [HarmonyPostfix]')
            [void]$sb.AppendLine("    private static void $handler($postParams)")
        }

        $emitBody = Build-EmitAnonymousBody $cn $isSt
        [void]$sb.AppendLine('    {')
        [void]$sb.AppendLine('        try')
        [void]$sb.AppendLine('        {')
        [void]$sb.AppendLine('            GregEventDispatcher.Emit(')
        [void]$sb.AppendLine("                GregHookName.Create(GregDomain.$gregDomain, `"$action`"),")
        [void]$sb.AppendLine('                new')
        [void]$sb.AppendLine('                {')
        [void]$sb.AppendLine($emitBody)
        [void]$sb.AppendLine('                });')
        [void]$sb.AppendLine('        }')
        [void]$sb.AppendLine('        catch (System.Exception ex)')
        [void]$sb.AppendLine('        {')
        [void]$sb.AppendLine('            MelonLogger.Warning($"[gregCore] Hook ' + $handler + ' failed: {ex.Message}");')
        [void]$sb.AppendLine('        }')
        [void]$sb.AppendLine('    }')
        [void]$sb.AppendLine('')
    }

    [void]$sb.AppendLine('}')
    $outFile = Join-Path $outHooksDir "Greg$($d)Hooks.cs"
    [System.IO.File]::WriteAllText($outFile, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
    Write-Host "Wrote $outFile ($($items.Count) patches)"
}

# Power domain stub if absent
$powerFile = Join-Path $outHooksDir 'GregPowerHooks.cs'
if (-not $byDomain.ContainsKey('Power') -or $byDomain['Power'].Count -eq 0) {
    $stub = @'
using HarmonyLib;

namespace gregFramework.Hooks;

/// <summary>
/// Reserved for Power / UPS / PDU hooks once matching Il2Cpp surface is classified.
/// </summary>
[HarmonyPatch]
internal static class GregPowerHooks
{
}
'@
    [System.IO.File]::WriteAllText($powerFile, $stub, [System.Text.UTF8Encoding]::new($false))
    Write-Host "Wrote stub $powerFile"
}

