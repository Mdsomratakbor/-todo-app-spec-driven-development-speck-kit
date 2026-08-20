<#
.SYNOPSIS
    Non-Regression Plan - parses spec.md for FR/SC requirements and scenarios
.DESCRIPTION
    Extracts all functional requirements (FR-NNN), success criteria (SC-NNN),
    and Given/When/Then test scenarios from spec.md files. Generates a
    non-regression test plan matrix.
.PARAMETER Module
    Optional module/feature name. If empty, scans all specs.
.PARAMETER OutputDir
    Directory to write the report. Defaults to ./reports
#>
param(
    [string]$Module = "",
    [string]$OutputDir = "reports"
)

$ErrorActionPreference = "Continue"
$codeFence = [char]0x60 + [char]0x60 + [char]0x60

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$outputFile = Join-Path $OutputDir "non-regression-plan.md"
$sb = [System.Text.StringBuilder]::new()

function Parse-SpecFile {
    param([string]$path)

    $content = Get-Content $path -Raw
    $specName = "Unknown"
    if ($content -match '#\s+Feature Specification:\s*(.+)') { $specName = $matches[1].Trim() }

    # Extract FRs
    $frs = [System.Collections.ArrayList]::new()
    $frMatches = [regex]::Matches($content, '\*\*(FR-\d+)\*\*:\s*(.+?)(?:\n|$)')
    foreach ($m in $frMatches) {
        [void]$frs.Add([PSCustomObject]@{
            Id   = $m.Groups[1].Value
            Text = $m.Groups[2].Value.Trim()
        })
    }

    # Extract SCs
    $scs = [System.Collections.ArrayList]::new()
    $scMatches = [regex]::Matches($content, '\*\*(SC-\d+)\*\*:\s*(.+?)(?:\n|$)')
    foreach ($m in $scMatches) {
        [void]$scs.Add([PSCustomObject]@{
            Id   = $m.Groups[1].Value
            Text = $m.Groups[2].Value.Trim()
        })
    }

    # Extract Given/When/Then scenarios
    $scenarios = [System.Collections.ArrayList]::new()
    $scenarioMatches = [regex]::Matches($content, '(\d+)\.\s+\*\*Given\*\*\s+(.+?),\s*\*\*When\*\*\s+(.+?),\s*\*\*Then\*\*\s+(.+?)(?:\n|$)')
    foreach ($m in $scenarioMatches) {
        [void]$scenarios.Add([PSCustomObject]@{
            Number = [int]$m.Groups[1].Value
            Given  = $m.Groups[2].Value.Trim()
            When   = $m.Groups[3].Value.Trim()
            Then   = $m.Groups[4].Value.Trim()
        })
    }

    # Extract User Stories
    $userStories = [System.Collections.ArrayList]::new()
    $usMatches = [regex]::Matches($content, '###\s+User Story\s+(\d+)\s*[-\u2013]\s*(.+?)\s*\((.+?)\)')
    foreach ($m in $usMatches) {
        [void]$userStories.Add([PSCustomObject]@{
            Number   = [int]$m.Groups[1].Value
            Title    = $m.Groups[2].Value.Trim()
            Priority = $m.Groups[3].Value.Trim()
        })
    }

    return @{
        SpecName    = $specName
        FRs         = $frs
        SCs         = $scs
        Scenarios   = $scenarios
        UserStories = $userStories
    }
}

# Discover spec files
$specDir = "specs"
$specFilter = if ($Module) { "*/$Module/spec.md" } else { "*/spec.md" }
$specFiles = Get-ChildItem -Path $specDir -Recurse -Filter "spec.md" -ErrorAction SilentlyContinue

if ($Module) {
    $specFiles = $specFiles | Where-Object { $_.DirectoryName -match [regex]::Escape($Module) }
}

$allFRs = [System.Collections.ArrayList]::new()
$allSCs = [System.Collections.ArrayList]::new()
$allScenarios = [System.Collections.ArrayList]::new()
$allUSs = [System.Collections.ArrayList]::new()
$allParsed = [System.Collections.ArrayList]::new()

foreach ($spec in $specFiles) {
    $parsed = Parse-SpecFile -path $spec.FullName
    [void]$allParsed.Add($parsed)

    foreach ($fr in $parsed.FRs) {
        [void]$allFRs.Add([PSCustomObject]@{
            Module = (Split-Path (Split-Path $spec.FullName -Parent) -Leaf)
            Id     = $fr.Id
            Text   = $fr.Text
        })
    }
    foreach ($sc in $parsed.SCs) {
        [void]$allSCs.Add([PSCustomObject]@{
            Module = (Split-Path (Split-Path $spec.FullName -Parent) -Leaf)
            Id     = $sc.Id
            Text   = $sc.Text
        })
    }
    foreach ($scn in $parsed.Scenarios) {
        [void]$allScenarios.Add([PSCustomObject]@{
            Module = (Split-Path (Split-Path $spec.FullName -Parent) -Leaf)
            Number = $scn.Number
            Given  = $scn.Given
            When   = $scn.When
            Then   = $scn.Then
        })
    }
    foreach ($us in $parsed.UserStories) {
        [void]$allUSs.Add([PSCustomObject]@{
            Module   = (Split-Path (Split-Path $spec.FullName -Parent) -Leaf)
            Number   = $us.Number
            Title    = $us.Title
            Priority = $us.Priority
        })
    }
}

# ── Generate Report ──────────────────────────────────────────
[void]$sb.AppendLine("# Non-Regression Test Plan")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sb.AppendLine("**Project**: TodoApp-SpecKit")
if ($Module) { [void]$sb.AppendLine("**Module**: $Module") }
[void]$sb.AppendLine("")

# Summary
[void]$sb.AppendLine("## Summary")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Metric | Count |")
[void]$sb.AppendLine("|--------|------:|")
[void]$sb.AppendLine("| Specs parsed | $($specFiles.Count) |")
[void]$sb.AppendLine("| User stories | $($allUSs.Count) |")
[void]$sb.AppendLine("| Functional requirements (FR) | $($allFRs.Count) |")
[void]$sb.AppendLine("| Success criteria (SC) | $($allSCs.Count) |")
[void]$sb.AppendLine("| Given/When/Then scenarios | $($allScenarios.Count) |")
[void]$sb.AppendLine("")

# User Stories
[void]$sb.AppendLine("## User Stories")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| # | Module | Title | Priority |")
[void]$sb.AppendLine("|---|--------|-------|----------|")
foreach ($us in $allUSs) {
    [void]$sb.AppendLine("| US-$($us.Number) | $($us.Module) | $($us.Title) | $($us.Priority) |")
}
[void]$sb.AppendLine("")

# Functional Requirements
[void]$sb.AppendLine("## Functional Requirements")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| ID | Module | Requirement |")
[void]$sb.AppendLine("|----|--------|-------------|")
foreach ($fr in $allFRs) {
    $text = if ($fr.Text.Length -gt 80) { $fr.Text.Substring(0, 77) + "..." } else { $fr.Text }
    [void]$sb.AppendLine("| $($fr.Id) | $($fr.Module) | $text |")
}
[void]$sb.AppendLine("")

# Success Criteria
[void]$sb.AppendLine("## Success Criteria")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| ID | Module | Criterion |")
[void]$sb.AppendLine("|----|--------|-----------|")
foreach ($sc in $allSCs) {
    $text = if ($sc.Text.Length -gt 80) { $sc.Text.Substring(0, 77) + "..." } else { $sc.Text }
    [void]$sb.AppendLine("| $($sc.Id) | $($sc.Module) | $text |")
}
[void]$sb.AppendLine("")

# Test Scenario Matrix
[void]$sb.AppendLine("## Test Scenario Matrix")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Each scenario maps to a non-regression test case.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| # | Module | Given | When | Then | Test Case ID |")
[void]$sb.AppendLine("|---|--------|-------|------|------|--------------|")
$idx = 0
foreach ($scn in $allScenarios) {
    $idx++
    $tcId = "TC-$($scn.Module.Substring(0, [math]::Min(3, $scn.Module.Length)).ToUpper())-$($idx.ToString('D3'))"
    $given = if ($scn.Given.Length -gt 30) { $scn.Given.Substring(0, 27) + "..." } else { $scn.Given }
    $when  = if ($scn.When.Length -gt 30) { $scn.When.Substring(0, 27) + "..." } else { $scn.When }
    $then  = if ($scn.Then.Length -gt 30) { $scn.Then.Substring(0, 27) + "..." } else { $scn.Then }
    [void]$sb.AppendLine("| $idx | $($scn.Module) | $given | $when | $then | $tcId |")
}
[void]$sb.AppendLine("")

# Traceability Matrix
[void]$sb.AppendLine("## Traceability: FR to Test Scenarios")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("$codeFence")
foreach ($fr in $allFRs) {
    $frText = $fr.Text
    if ($frText.Length -gt 60) { $frText = $frText.Substring(0, 57) + "..." }
    [void]$sb.AppendLine("$($fr.Id): $frText")
}
[void]$sb.AppendLine("")
$scnTotal = ($allScenarios | Measure-Object).Count
[void]$sb.AppendLine("Covered by: $scnTotal Given/When/Then scenarios")
[void]$sb.AppendLine($codeFence)
[void]$sb.AppendLine("")

[void]$sb.AppendLine("---")
[void]$sb.AppendLine("*Report generated by generate-non-regression-plan.ps1*")

$sb.ToString() | Out-File -FilePath $outputFile -Encoding UTF8
Write-Host "Report written to: $outputFile"

return @{
    File       = $outputFile
    FRCount    = $allFRs.Count
    SCCount    = $allSCs.Count
    Scenarios  = $allScenarios.Count
    USCount    = $allUSs.Count
    SpecsCount = $specFiles.Count
}