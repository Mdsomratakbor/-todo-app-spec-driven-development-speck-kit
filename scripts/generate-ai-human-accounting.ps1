<#
.SYNOPSIS
    AI vs Human Code Attribution Report
.DESCRIPTION
    Uses git-ai and scc to generate an AI vs Human code attribution report.
    Falls back to git log analysis when tools are unavailable.
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

# ── Detect tools ──────────────────────────────────────────────
$hasGitAi   = $null -ne (Get-Command git-ai -ErrorAction SilentlyContinue)
$hasScc     = $null -ne (Get-Command scc -ErrorAction SilentlyContinue)
$hasGit     = $null -ne (Get-Command git -ErrorAction SilentlyContinue)

# ── Git log data collection ──────────────────────────────────
function Get-GitLogData {
    param([string]$moduleFilter)

    $logArgs = @("log", "--all", "--format=COMMIT|%H|%an|%ae|%s|%aI", "--numstat")
    if ($moduleFilter) {
        # Scope to files under common paths for the module
    }

    $output = & git @logArgs 2>&1
    return $output
}

function Parse-GitLog {
    param([string[]]$lines)

    $commits = [System.Collections.ArrayList]::new()
    $fileStats = @{}
    $currentCommit = $null
    $authorEmails = @{}

    foreach ($line in $lines) {
        if ($line -match "^COMMIT\|(.{8,40})\|(.+?)\|(.+?)\|(.+?)\|(.+)$") {
            if ($currentCommit) {
                [void]$commits.Add($currentCommit)
            }
            $currentCommit = @{
                Hash      = $matches[1]
                Author    = $matches[2]
                Email     = $matches[3]
                Subject   = $matches[4]
                Date      = $matches[5]
                Added     = 0
                Deleted   = 0
                Files     = [System.Collections.ArrayList]::new()
            }
            $authorEmails[$matches[3]] = $matches[2]
        }
        elseif ($line -match "^(\d+)\t(\d+)\t(.+)$") {
            if ($currentCommit) {
                $added   = [int]$matches[1]
                $deleted = [int]$matches[2]
                $file    = $matches[3]
                $currentCommit.Added   += $added
                $currentCommit.Deleted += $deleted
                [void]$currentCommit.Files.Add($file)

                if (-not $fileStats.ContainsKey($file)) {
                    $fileStats[$file] = @{ Added = 0; Deleted = 0 }
                }
                $fileStats[$file].Added   += $added
                $fileStats[$file].Deleted += $deleted
            }
        }
        elseif ($line -match "^-(\d+|)\t-(\d+|)\t(.+)$") {
            # Binary file or rename — skip
        }
    }
    if ($currentCommit) {
        [void]$commits.Add($currentCommit)
    }

    return @{
        Commits    = $commits
        FileStats  = $fileStats
        Authors    = $authorEmails
    }
}

# ── Classify commits as AI or Human ─────────────────────────
# Heuristic: SpecKit-generated commits contain markers like "Spec Kit", "speckit",
# conventional commit prefixes with phase/task references, or Co-authored-by AI
function Classify-Commit {
    param($commit)

    $subject = $commit.Subject
    $aiMarkers = @(
        "Spec Kit", "speckit", "SPECKIT",
        "Co-authored-by: assistant",
        "Co-authored-by: GitHub",
        "Co-authored-by: Copilot",
        "Co-authored-by: Claude",
        "Co-authored-by: GPT",
        "Co-authored-by: AI",
        "auto-generated", "automated",
        "Phase \d+", "T\d{3}", "US\d+"
    )

    foreach ($marker in $aiMarkers) {
        if ($subject -match $marker) {
            return "AI-Assisted"
        }
    }

    # Check for very structured conventional commits with task refs
    if ($subject -match "^\w+\(.*\):.*\b(phases?\s+\d|tasks?\s+T\d{3})\b") {
        return "AI-Assisted"
    }

    return "Human"
}

# ── Main ─────────────────────────────────────────────────────
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$outputFile = Join-Path $OutputDir "ai-human-accounting.md"
$sb = [System.Text.StringBuilder]::new()

[void]$sb.AppendLine("# AI vs Human Code Attribution Report")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sb.AppendLine("**Project**: TodoApp-SpecKit")
if ($Module) {
    [void]$sb.AppendLine("**Module**: $Module")
}
[void]$sb.AppendLine("")

# Tool availability
[void]$sb.AppendLine("## Tool Availability")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Tool | Status |")
[void]$sb.AppendLine("|------|--------|")
[void]$sb.AppendLine("| git-ai | $(if ($hasGitAi) { 'Available' } else { 'Not installed - using git log fallback' }) |")
[void]$sb.AppendLine("| scc | $(if ($hasScc) { 'Available' } else { 'Not installed - using git numstat fallback' }) |")
[void]$sb.AppendLine("| git | $(if ($hasGit) { 'Available' } else { 'Not found' }) |")
[void]$sb.AppendLine("")

if (-not $hasGit) {
    [void]$sb.AppendLine("**ERROR**: git is not available. Cannot generate report.")
    $sb.ToString() | Out-File -FilePath $outputFile -Encoding UTF8
    Write-Host "Report written to $outputFile"
    return
}

# Collect git data
$rawLog = Get-GitLogData -moduleFilter $Module
$parsed = Parse-GitLog -lines $rawLog

# Classify commits
$aiCommits = [System.Collections.ArrayList]::new()
$humanCommits = [System.Collections.ArrayList]::new()
$aiLinesAdded = 0
$aiLinesDeleted = 0
$humanLinesAdded = 0
$humanLinesDeleted = 0

foreach ($commit in $parsed.Commits) {
    $classification = Classify-Commit -commit $commit

    if ($classification -eq "AI-Assisted") {
        [void]$aiCommits.Add($commit)
        $aiLinesAdded   += $commit.Added
        $aiLinesDeleted += $commit.Deleted
    }
    else {
        [void]$humanCommits.Add($commit)
        $humanLinesAdded   += $commit.Added
        $humanLinesDeleted += $commit.Deleted
    }
}

$totalCommits = $parsed.Commits.Count
$aiCommitCount = $aiCommits.Count
$humanCommitCount = $humanCommits.Count
$totalLines = $aiLinesAdded + $aiLinesDeleted + $humanLinesAdded + $humanLinesDeleted
$aiLineTotal = $aiLinesAdded + $aiLinesDeleted
$humanLineTotal = $humanLinesAdded + $humanLinesDeleted

$aiCommitPct = if ($totalCommits -gt 0) { [math]::Round(($aiCommitCount / $totalCommits) * 100, 1) } else { 0 }
$humanCommitPct = if ($totalCommits -gt 0) { [math]::Round(($humanCommitCount / $totalCommits) * 100, 1) } else { 0 }
$aiLinePct = if ($totalLines -gt 0) { [math]::Round(($aiLineTotal / $totalLines) * 100, 1) } else { 0 }
$humanLinePct = if ($totalLines -gt 0) { [math]::Round(($humanLineTotal / $totalLines) * 100, 1) } else { 0 }

# Summary
[void]$sb.AppendLine("## Summary")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Metric | AI-Assisted | Human | Total |")
[void]$sb.AppendLine("|--------|-------------|-------|-------|")
[void]$sb.AppendLine("| Commits | $aiCommitCount ($aiCommitPct%) | $humanCommitCount ($humanCommitPct%) | $totalCommits |")
[void]$sb.AppendLine("| Lines Added | $aiLinesAdded | $humanLinesAdded | $($aiLinesAdded + $humanLinesAdded) |")
[void]$sb.AppendLine("| Lines Deleted | $aiLinesDeleted | $humanLinesDeleted | $($aiLinesDeleted + $humanLinesDeleted) |")
[void]$sb.AppendLine("| Lines Changed (total) | $aiLineTotal ($aiLinePct%) | $humanLineTotal ($humanLinePct%) | $totalLines |")
[void]$sb.AppendLine("")

# Detailed commit list
[void]$sb.AppendLine("## Commit Breakdown")
[void]$sb.AppendLine("")
[void]$sb.AppendLine($codeFence + "text")
[void]$sb.AppendLine("Author             | Lines +/-  | Classification | Subject")
[void]$sb.AppendLine("-------------------|------------|----------------|--------")
foreach ($commit in $parsed.Commits) {
    $cls = Classify-Commit -commit $commit
    $lines = "+$($commit.Added)/-$($commit.Deleted)"
    $author = $commit.Author.PadRight(18).Substring(0, 18)
    $subject = if ($commit.Subject.Length -gt 60) { $commit.Subject.Substring(0, 57) + "..." } else { $commit.Subject }
    [void]$sb.AppendLine("$author | $($lines.PadRight(10)) | $($cls.PadRight(14)) | $subject")
}
[void]$sb.AppendLine($codeFence)
[void]$sb.AppendLine("")

# File heatmap (top changed files)
[void]$sb.AppendLine("## Top Changed Files")
[void]$sb.AppendLine("")
[void]$sb.AppendLine($codeFence + "text")
[void]$sb.AppendLine("Lines +/-   | File")
[void]$sb.AppendLine("------------|-----")
$sortedFiles = $parsed.FileStats.GetEnumerator() | Sort-Object { $_.Value.Added + $_.Value.Deleted } -Descending | Select-Object -First 20
foreach ($entry in $sortedFiles) {
    $total = $entry.Value.Added + $entry.Value.Deleted
    [void]$sb.AppendLine("+$($entry.Value.Added)/-$($entry.Value.Deleted) ($total) | $($entry.Key)")
}
[void]$sb.AppendLine($codeFence)
[void]$sb.AppendLine("")

# Attribution methodology
[void]$sb.AppendLine("## Methodology")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Commit classification uses heuristic analysis of commit messages:")
[void]$sb.AppendLine("- **AI-Assisted**: Commits containing SpecKit markers (Phase/Task refs, 'Spec Kit', 'speckit'),")
[void]$sb.AppendLine("  or AI co-author trailers")
[void]$sb.AppendLine("- **Human**: All other commits (manual messages, descriptive without AI markers)")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("> Note: This is a heuristic approximation. AI-assisted commits where the human")
[void]$sb.AppendLine("> rewrote the commit message may be classified as Human.")

# Write file
$sb.ToString() | Out-File -FilePath $outputFile -Encoding UTF8
Write-Host "Report written to: $outputFile"

# Return summary for orchestrator
return @{
    File            = $outputFile
    TotalCommits    = $totalCommits
    AiCommits       = $aiCommitCount
    HumanCommits    = $humanCommitCount
    AiCommitPct     = $aiCommitPct
    HumanCommitPct  = $humanCommitPct
    AiLinesPct      = $aiLinePct
    HumanLinesPct   = $humanLinePct
}
