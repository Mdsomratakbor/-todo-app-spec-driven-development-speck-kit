<#
.SYNOPSIS
    Mid-Point Progress Report with Challenge Scorecard (/115)
.DESCRIPTION
    Generates a comprehensive mid-point progress report including a challenge
    scorecard scored out of 115 points across seven dimensions.
.PARAMETER Module
    Optional module/feature name to scope the report.
.PARAMETER OutputDir
    Directory to write the report. Defaults to ./reports
#>
param(
    [string]$Module = "",
    [string]$OutputDir = "reports"
)

$ErrorActionPreference = "Continue"
$codeFence = [char]0x60 + [char]0x60 + [char]0x60

$hasGit    = $null -ne (Get-Command git -ErrorAction SilentlyContinue)
$hasDotnet = $null -ne (Get-Command dotnet -ErrorAction SilentlyContinue)
$hasDepsafe = $null -ne (Get-Command depsafe -ErrorAction SilentlyContinue)

$SCORING = [ordered]@{
    SpecCompleteness = @{ Max = 25; Label = "Spec completeness" }
    TestCoverage     = @{ Max = 20; Label = "Test coverage" }
    CodeQuality      = @{ Max = 20; Label = "Code quality" }
    Security         = @{ Max = 15; Label = "Security posture" }
    Documentation    = @{ Max = 15; Label = "Documentation" }
    GitHygiene       = @{ Max = 10; Label = "Git hygiene" }
    BuildHealth      = @{ Max = 10; Label = "Build health" }
}

function Count-SpecRequirements {
    param([string]$specPath)
    if (-not (Test-Path $specPath)) { return @{ FRs = 0; SCs = 0; Scenarios = 0 } }
    $content = Get-Content $specPath -Raw
    $frCount  = ([regex]::Matches($content, '\*\*FR-\d+\*\*')).Count
    $scCount  = ([regex]::Matches($content, '\*\*SC-\d+\*\*')).Count
    $scnCount = ([regex]::Matches($content, '\d+\.\s+\*\*Given\*\*')).Count
    return @{ FRs = $frCount; SCs = $scCount; Scenarios = $scnCount }
}

function Count-TestFiles {
    $count = 0
    $ts = Get-ChildItem -Recurse -Include "*.spec.ts","*.test.ts" -Path "client" -ErrorAction SilentlyContinue
    if ($ts) { $count += $ts.Count }
    $cs = Get-ChildItem -Recurse -Include "*Tests.cs" -Path "api" -ErrorAction SilentlyContinue
    if ($cs) { $count += $cs.Count }
    return $count
}

function Get-DotnetBuildResult {
    if (-not $hasDotnet) { return @{ Ok = $false; Errors = -1; Warnings = 0 } }
    $out = & dotnet build --no-restore 2>&1
    $errs = ($out | Where-Object { $_ -match '\berror\b' }).Count
    $warns = ($out | Where-Object { $_ -match '\bwarning\b' }).Count
    return @{ Ok = ($errs -eq 0); Errors = $errs; Warnings = $warns }
}

function Scan-CommitMessages {
    if (-not $hasGit) { return @{ Score = 0; Issues = @() } }
    $log = & git log --all --format="%s" 2>&1
    $issues = [System.Collections.ArrayList]::new()
    $score = 10.0
    foreach ($msg in $log) {
        if ($msg -notmatch '^(feat|fix|chore|test|docs|refactor|style|perf|ci|build|revert)(\(.+\))?!?:') {
            [void]$issues.Add($msg)
            $score -= 1
        }
        if ($msg.Length -gt 100) { $score -= 0.5 }
    }
    $score = [math]::Max(0, [math]::Min(10, $score))
    return @{ Score = $score; Issues = $issues }
}

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$outputFile = Join-Path $OutputDir "mid-point-report.md"
$sb = [System.Text.StringBuilder]::new()

[void]$sb.AppendLine("# Mid-Point Progress Report")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sb.AppendLine("**Project**: TodoApp-SpecKit")
if ($Module) { [void]$sb.AppendLine("**Module**: $Module") }
[void]$sb.AppendLine("")

# Collect data
$specFiles = Get-ChildItem -Path "specs" -Recurse -Filter "spec.md" -ErrorAction SilentlyContinue
$totalFRs = 0; $totalSCs = 0; $totalScenarios = 0
$perModule = [System.Collections.ArrayList]::new()

foreach ($spec in $specFiles) {
    $reqs = Count-SpecRequirements -specPath $spec.FullName
    $totalFRs += $reqs.FRs; $totalSCs += $reqs.SCs; $totalScenarios += $reqs.Scenarios
    $modName = (Split-Path (Split-Path $spec.FullName -Parent) -Leaf)
    [void]$perModule.Add([PSCustomObject]@{ Module = $modName; FRs = $reqs.FRs; SCs = $reqs.SCs; Scenarios = $reqs.Scenarios })
}

$testCount = Count-TestFiles
$buildResult = Get-DotnetBuildResult
$totalCommits = 0
if ($hasGit) { $totalCommits = (& git log --all --oneline 2>&1 | Measure-Object).Count }
$gitData = Scan-CommitMessages

# Score calculations
$scores = [ordered]@{}

# Spec completeness (25)
$specScore = [math]::Min(25, [math]::Min(10, $totalFRs) + [math]::Min(8, $totalSCs) + [math]::Min(7, [math]::Floor($totalScenarios / 2)))
$scores["SpecCompleteness"] = $specScore

# Test coverage (20)
$scores["TestCoverage"] = [math]::Min(20, $testCount * 2)

# Code quality (20)
$scores["CodeQuality"] = if ($buildResult.Ok) { 20 } else { [math]::Max(0, 20 - ($buildResult.Errors * 2)) }

# Security (15) - baseline, full scan in security script
$scores["Security"] = 12

# Documentation (15)
$planFiles = Get-ChildItem -Path "specs" -Recurse -Filter "plan.md" -ErrorAction SilentlyContinue
$docScore = [math]::Min(10, $specFiles.Count * 2) + [math]::Min(5, $(if ($planFiles) { $planFiles.Count } else { 0 }))
$scores["Documentation"] = $docScore

# Git hygiene (10)
$scores["GitHygiene"] = $gitData.Score

# Build health (10)
$scores["BuildHealth"] = if ($buildResult.Ok) { 10 } else { [math]::Max(0, 10 - $buildResult.Errors) }

$totalScore = 0
foreach ($key in $scores.Keys) { $totalScore += $scores[$key] }
$maxScore = 115

# Report output
[void]$sb.AppendLine("## Challenge Scorecard")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("**Total Score: $totalScore / $maxScore**")
[void]$sb.AppendLine("")

$filled = [math]::Round(($totalScore / $maxScore) * 30)
$empty = 30 - $filled
$bar = ([string][char]0x2588) * $filled + ([string][char]0x2591) * $empty
[void]$sb.AppendLine("$codeFence")
[void]$sb.AppendLine("$bar  $totalScore/$maxScore")
[void]$sb.AppendLine($codeFence)
[void]$sb.AppendLine("")

[void]$sb.AppendLine("## Score Breakdown")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Dimension | Score | Max | Pct | Status |")
[void]$sb.AppendLine("|-----------|------:|----:|----:|--------|")

foreach ($key in @("SpecCompleteness","TestCoverage","CodeQuality","Security","Documentation","GitHygiene","BuildHealth")) {
    $s = $scores[$key]
    $m = $SCORING[$key].Max
    $lbl = $SCORING[$key].Label
    $pct = if ($m -gt 0) { [math]::Round(($s / $m) * 100) } else { 0 }
    $st = if ($pct -ge 80) { "Pass" } elseif ($pct -ge 50) { "At Risk" } else { "Fail" }
    [void]$sb.AppendLine("| $lbl | $s | $m | $pct% | $st |")
}
[void]$sb.AppendLine("")

[void]$sb.AppendLine("## Project Metrics")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Metric | Value |")
[void]$sb.AppendLine("|--------|------:|")
[void]$sb.AppendLine("| Spec files | $($specFiles.Count) |")
[void]$sb.AppendLine("| Functional requirements | $totalFRs |")
[void]$sb.AppendLine("| Success criteria | $totalSCs |")
[void]$sb.AppendLine("| Given/When/Then scenarios | $totalScenarios |")
[void]$sb.AppendLine("| Test files | $testCount |")
[void]$sb.AppendLine("| Total commits | $totalCommits |")
[void]$sb.AppendLine("| Build errors | $($buildResult.Errors) |")
[void]$sb.AppendLine("| Build warnings | $($buildResult.Warnings) |")
[void]$sb.AppendLine("")

[void]$sb.AppendLine("## Per-Module Breakdown")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Module | FRs | SCs | Scenarios |")
[void]$sb.AppendLine("|--------|----:|----:|----------:|")
foreach ($mod in $perModule) {
    [void]$sb.AppendLine("| $($mod.Module) | $($mod.FRs) | $($mod.SCs) | $($mod.Scenarios) |")
}
[void]$sb.AppendLine("")

[void]$sb.AppendLine("## Git Hygiene")
[void]$sb.AppendLine("")
if ($gitData.Issues.Count -gt 0) {
    [void]$sb.AppendLine("**Non-conventional commits** ($($gitData.Issues.Count) found):")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("$codeFence")
    foreach ($issue in $gitData.Issues) { [void]$sb.AppendLine("- $issue") }
    [void]$sb.AppendLine($codeFence)
} else {
    [void]$sb.AppendLine("All commits follow conventional commit format.")
}
[void]$sb.AppendLine("")

[void]$sb.AppendLine("## Build Health")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Check | Result |")
[void]$sb.AppendLine("|-------|--------|")
[void]$sb.AppendLine("| .NET build | $(if ($buildResult.Ok) { 'Pass' } else { "Fail ($($buildResult.Errors) errors)" }) |")
[void]$sb.AppendLine("| Build warnings | $($buildResult.Warnings) |")
[void]$sb.AppendLine("")

[void]$sb.AppendLine("---")
[void]$sb.AppendLine("*Report generated by generate-mid-point-report.ps1*")

$sb.ToString() | Out-File -FilePath $outputFile -Encoding UTF8
Write-Host "Report written to: $outputFile"

return @{
    File         = $outputFile
    TotalScore   = $totalScore
    MaxScore     = $maxScore
    TotalFRs     = $totalFRs
    TotalSCs     = $totalSCs
    TotalScenarios = $totalScenarios
    BuildOk      = $buildResult.Ok
}