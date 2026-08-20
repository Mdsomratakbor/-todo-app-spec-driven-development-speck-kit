<#
.SYNOPSIS
    Orchestrator - runs all Gate deliverable report generators
.DESCRIPTION
    Runs all 4 report scripts sequentially and displays a summary table.
.PARAMETER Module
    Module/feature name to scope reports. Auto-detected from specs/ if not provided.
.PARAMETER OutputDir
    Directory to write reports. Defaults to ./reports
#>
param(
    [string]$Module = "",
    [string]$OutputDir = "reports"
)

$ErrorActionPreference = "Continue"
$codeFence = [char]0x60 + [char]0x60 + [char]0x60
$scriptDir = $PSScriptRoot

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  Gate Deliverable Report Generator" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Auto-detect module if not provided
if (-not $Module) {
    $specDirs = Get-ChildItem -Path "specs" -Directory -ErrorAction SilentlyContinue | Where-Object { Test-Path (Join-Path $_.FullName "spec.md") }
    if ($specDirs) {
        $latest = $specDirs | Sort-Object Name -Descending | Select-Object -First 1
        $Module = $latest.Name
        Write-Host "Auto-detected module: $Module" -ForegroundColor Yellow
    } else {
        $Module = "TodoApp-SpecKit"
        Write-Host "No spec modules found, using project name: $Module" -ForegroundColor Yellow
    }
}

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$results = @{}
$startTime = Get-Date

# ── 1. AI-Human Accounting ──────────────────────────────────
Write-Host ""
Write-Host "[1/4] AI-Human Accounting Report..." -ForegroundColor Green
try {
    $r = & "$scriptDir\generate-ai-human-accounting.ps1" -Module $Module -OutputDir $OutputDir
    $results["ai-human"] = $r
    Write-Host "  -> Complete" -ForegroundColor DarkGreen
} catch {
    Write-Host "  -> ERROR: $($_.Exception.Message)" -ForegroundColor Red
    $results["ai-human"] = @{ Error = $_.Exception.Message }
}

# ── 2. Mid-Point Report ─────────────────────────────────────
Write-Host ""
Write-Host "[2/4] Mid-Point Report..." -ForegroundColor Green
try {
    $r = & "$scriptDir\generate-mid-point-report.ps1" -Module $Module -OutputDir $OutputDir
    $results["midpoint"] = $r
    Write-Host "  -> Complete (Score: $($r.TotalScore)/$($r.MaxScore))" -ForegroundColor DarkGreen
} catch {
    Write-Host "  -> ERROR: $($_.Exception.Message)" -ForegroundColor Red
    $results["midpoint"] = @{ Error = $_.Exception.Message }
}

# ── 3. Non-Regression Plan ──────────────────────────────────
Write-Host ""
Write-Host "[3/4] Non-Regression Plan..." -ForegroundColor Green
try {
    $r = & "$scriptDir\generate-non-regression-plan.ps1" -Module $Module -OutputDir $OutputDir
    $results["nonregression"] = $r
    Write-Host "  -> Complete (FRs: $($r.FRCount), SCs: $($r.SCCount), Scenarios: $($r.Scenarios))" -ForegroundColor DarkGreen
} catch {
    Write-Host "  -> ERROR: $($_.Exception.Message)" -ForegroundColor Red
    $results["nonregression"] = @{ Error = $_.Exception.Message }
}

# ── 4. Security Scan ────────────────────────────────────────
Write-Host ""
Write-Host "[4/4] Security Scan..." -ForegroundColor Green
try {
    $r = & "$scriptDir\run-security-scan.ps1" -Module $Module -OutputDir $OutputDir
    $results["security"] = $r
    Write-Host "  -> Complete (Findings: $($r.TotalFindings), Risk: $($r.RiskLevel))" -ForegroundColor DarkGreen
} catch {
    Write-Host "  -> ERROR: $($_.Exception.Message)" -ForegroundColor Red
    $results["security"] = @{ Error = $_.Exception.Message }
}

$endTime = Get-Date
$elapsed = $endTime - $startTime

# ── Summary Table ────────────────────────────────────────────
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  SUMMARY" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Build summary as a structured output
$summarySb = [System.Text.StringBuilder]::new()
[void]$summarySb.AppendLine("# Gate Deliverables - Summary")
[void]$summarySb.AppendLine("")
[void]$summarySb.AppendLine("**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$summarySb.AppendLine("**Module**: $Module")
[void]$summarySb.AppendLine("**Elapsed**: $([math]::Round($elapsed.TotalSeconds, 1))s")
[void]$summarySb.AppendLine("")

[void]$summarySb.AppendLine("## Results Overview")
[void]$summarySb.AppendLine("")
[void]$summarySb.AppendLine("| Report | Status | Key Metrics |")
[void]$summarySb.AppendLine("|--------|--------|-------------|")

# AI-Human
if ($results["ai-human"] -and -not $results["ai-human"].Error) {
    $ah = $results["ai-human"]
    [void]$summarySb.AppendLine("| AI-Human Accounting | OK | $($ah.TotalCommits) commits, AI: $($ah.AiCommitPct)%, Human: $($ah.HumanCommitPct)% |")
} else {
    [void]$summarySb.AppendLine("| AI-Human Accounting | FAIL | $($results['ai-human'].Error) |")
}

# Mid-Point
if ($results["midpoint"] -and -not $results["midpoint"].Error) {
    $mp = $results["midpoint"]
    $grade = if ($mp.TotalScore -ge 90) { "A" } elseif ($mp.TotalScore -ge 75) { "B" } elseif ($mp.TotalScore -ge 60) { "C" } else { "D" }
    [void]$summarySb.AppendLine("| Mid-Point Report | OK | Score: $($mp.TotalScore)/$($mp.MaxScore) (Grade: $grade) |")
} else {
    [void]$summarySb.AppendLine("| Mid-Point Report | FAIL | $($results['midpoint'].Error) |")
}

# Non-Regression
if ($results["nonregression"] -and -not $results["nonregression"].Error) {
    $nr = $results["nonregression"]
    [void]$summarySb.AppendLine("| Non-Regression Plan | OK | $($nr.FRCount) FRs, $($nr.SCCount) SCs, $($nr.Scenarios) scenarios |")
} else {
    [void]$summarySb.AppendLine("| Non-Regression Plan | FAIL | $($results['nonregression'].Error) |")
}

# Security
if ($results["security"] -and -not $results["security"].Error) {
    $sec = $results["security"]
    $riskColor = switch ($sec.RiskLevel) { "LOW" { "GREEN" } "MEDIUM" { "YELLOW" } default { "RED" } }
    [void]$summarySb.AppendLine("| Security Scan | OK | Secrets: $($sec.SecretsFound), NuGet: $($sec.NuGetVulns), npm: $($sec.NpmVulns) - Risk: $($sec.RiskLevel) |")
} else {
    [void]$summarySb.AppendLine("| Security Scan | FAIL | $($results['security'].Error) |")
}

[void]$summarySb.AppendLine("")

# Action items
[void]$summarySb.AppendLine("## Action Items")
[void]$summarySb.AppendLine("")

$actionItems = [System.Collections.ArrayList]::new()

if ($results["security"] -and -not $results["security"].Error) {
    $sec = $results["security"]
    if ($sec.SecretsFound -gt 0) {
        [void]$actionItems.Add("URGENT: Review and remediate $($sec.SecretsFound) secret(s) found in codebase")
    }
    if ($sec.NuGetVulns -gt 0) {
        [void]$actionItems.Add("HIGH: Update $($sec.NuGetVulns) vulnerable NuGet package(s)")
    }
    if ($sec.NpmVulns -gt 0) {
        [void]$actionItems.Add("MEDIUM: Run `npm audit fix` to resolve $($sec.NpmVulns) npm vulnerability(ies)")
    }
}

if ($results["midpoint"] -and -not $results["midpoint"].Error) {
    $mp = $results["midpoint"]
    if ($mp.TotalScore -lt 90) {
        [void]$actionItems.Add("Review scorecard gaps - current score $($mp.TotalScore)/$($mp.MaxScore)")
    }
    if (-not $mp.BuildOk) {
        [void]$actionItems.Add("CRITICAL: Fix build errors before proceeding")
    }
}

if ($actionItems.Count -eq 0) {
    [void]$summarySb.AppendLine("No critical action items. Project is in good shape.")
} else {
    foreach ($item in $actionItems) {
        [void]$summarySb.AppendLine("- $item")
    }
}

[void]$summarySb.AppendLine("")
[void]$summarySb.AppendLine("## Generated Files")
[void]$summarySb.AppendLine("")
[void]$summarySb.AppendLine("| Report | Path |")
[void]$summarySb.AppendLine("|--------|------|")
[void]$summarySb.AppendLine("| AI-Human Accounting | reports/ai-human-accounting.md |")
[void]$summarySb.AppendLine("| Mid-Point Report | reports/mid-point-report.md |")
[void]$summarySb.AppendLine("| Non-Regression Plan | reports/non-regression-plan.md |")
[void]$summarySb.AppendLine("| Security Scan | reports/security-scan.md |")
[void]$summarySb.AppendLine("| **Summary** | reports/gate-summary.md |")

$summaryFile = Join-Path $OutputDir "gate-summary.md"
$summarySb.ToString() | Out-File -FilePath $summaryFile -Encoding UTF8

# Display summary in console
Write-Host ($summarySb.ToString()) -ForegroundColor White
Write-Host ""
Write-Host "All reports written to: $OutputDir/" -ForegroundColor Cyan
Write-Host "Summary: $summaryFile" -ForegroundColor Cyan
Write-Host ""

return $results