<#
.SYNOPSIS
    Security Scan Report - detect-secrets, dotnet vulnerabilities, depsafe
.DESCRIPTION
    Runs multiple security scanning tools and generates a consolidated report.
    Falls back to manual checks when tools are unavailable.
.PARAMETER Module
    Optional module/feature name.
.PARAMETER OutputDir
    Directory to write the report. Defaults to ./reports
#>
param(
    [string]$Module = "",
    [string]$OutputDir = "reports"
)

$ErrorActionPreference = "Continue"
$codeFence = [char]0x60 + [char]0x60 + [char]0x60

$hasDotnet   = $null -ne (Get-Command dotnet -ErrorAction SilentlyContinue)
$hasDepsafe  = $null -ne (Get-Command depsafe -ErrorAction SilentlyContinue)
$hasDetectPy = $null -ne (Get-Command python -ErrorAction SilentlyContinue)
$hasDetect   = $false
if ($hasDetectPy) {
    $ver = & python -m detect_secrets --version 2>&1
    $hasDetect = ($LASTEXITCODE -eq 0)
}

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$outputFile = Join-Path $OutputDir "security-scan.md"
$sb = [System.Text.StringBuilder]::new()

[void]$sb.AppendLine("# Security Scan Report")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sb.AppendLine("**Project**: TodoApp-SpecKit")
if ($Module) { [void]$sb.AppendLine("**Module**: $Module") }
[void]$sb.AppendLine("")

# Tool availability
[void]$sb.AppendLine("## Tool Availability")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Tool | Status | Version |")
[void]$sb.AppendLine("|------|--------|---------|")
[void]$sb.AppendLine("| detect-secrets | $(if ($hasDetect) { 'Available' } else { 'Not available' }) | $(if ($hasDetect) { & python -m detect_secrets --version 2>&1 } else { 'N/A' }) |")
[void]$sb.AppendLine("| dotnet | $(if ($hasDotnet) { 'Available' } else { 'Not available' }) | $(if ($hasDotnet) { & dotnet --version 2>&1 } else { 'N/A' }) |")
[void]$sb.AppendLine("| depsafe | $(if ($hasDepsafe) { 'Available' } else { 'Not available' }) | $(if ($hasDepsafe) { & depsafe --version 2>&1 } else { 'N/A' }) |")
[void]$sb.AppendLine("")

# ── 1. Secret Scanning ──────────────────────────────────────
[void]$sb.AppendLine("## 1. Secret Scanning")
[void]$sb.AppendLine("")

$secretsFound = 0
$secretDetails = @()

if ($hasDetect) {
    [void]$sb.AppendLine("**Tool**: detect-secrets (Python)")
    [void]$sb.AppendLine("")
    $scanOut = & python -m detect_secrets scan 2>&1
    if ($scanOut) {
        $parsed = $scanOut | ConvertFrom-Json -ErrorAction SilentlyContinue
        if ($parsed) {
            foreach ($file in $parsed.results.PSObject.Properties) {
                foreach ($finding in $file.Value) {
                    $secretsFound++
                    $secretDetails += [PSCustomObject]@{
                        File    = $file.Name
                        Type    = $finding.type
                        Line    = $finding.line_number
                    }
                }
            }
        }
    }
    [void]$sb.AppendLine("Findings: $secretsFound")
} else {
    [void]$sb.AppendLine("**Tool**: Manual regex scan (detect-secrets not available)")
    [void]$sb.AppendLine("")

    # Manual scan for common secret patterns
    $patterns = @(
        @{ Pattern = "(?i)(password|passwd|pwd)\s*[:=]\s*[`"`'][^\s`"`']{8,}"; Name = "Hardcoded password" },
        @{ Pattern = "(?i)(api[_-]?key|apikey)\s*[:=]\s*[`"`'][A-Za-z0-9]{16,}"; Name = "API key" },
        @{ Pattern = "(?i)(secret|token)\s*[:=]\s*[`"`'][A-Za-z0-9]{16,}"; Name = "Secret/token" },
        @{ Pattern = "-----BEGIN\s+(RSA\s+)?PRIVATE\s+KEY-----"; Name = "Private key" },
        @{ Pattern = "(?i)(aws[_-]?access[_-]?key[_-]?id|AKIA)[A-Z0-9]{16}"; Name = "AWS key" },
        @{ Pattern = "(?i)connectionstring.*(?:password|pwd)\s*=\s*[^;]+"; Name = "Connection string with password" }
    )

    $scannedFiles = Get-ChildItem -Recurse -Include "*.cs","*.ts","*.js","*.json","*.yaml","*.yml","*.env","*.config","*.xml" -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '(node_modules|\.git|bin|obj|dist)' }

    foreach ($file in $scannedFiles) {
        $fileContent = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $fileContent) { continue }

        foreach ($p in $patterns) {
            $matches = [regex]::Matches($fileContent, $p.Pattern)
            foreach ($m in $matches) {
                $lineNum = ($fileContent.Substring(0, $m.Index) -split "`n").Count
                $secretsFound++
                $secretDetails += [PSCustomObject]@{
                    File = $file.FullName.Replace($PWD.Path + "\", "")
                    Type = $p.Name
                    Line = $lineNum
                }
            }
        }
    }

    [void]$sb.AppendLine("Files scanned: $($scannedFiles.Count)")
    [void]$sb.AppendLine("Findings: $secretsFound")
}

[void]$sb.AppendLine("")

if ($secretDetails.Count -gt 0) {
    [void]$sb.AppendLine("### Findings")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("| # | File | Type | Line |")
    [void]$sb.AppendLine("|---|------|------|-----:|")
    $idx = 0
    foreach ($d in $secretDetails) {
        $idx++
        [void]$sb.AppendLine("| $idx | $($d.File) | $($d.Type) | $($d.Line) |")
    }
    [void]$sb.AppendLine("")
} else {
    [void]$sb.AppendLine("> No secrets detected.")
    [void]$sb.AppendLine("")
}

# ── 2. NuGet Vulnerability Scan ─────────────────────────────
[void]$sb.AppendLine("## 2. NuGet Package Vulnerabilities")
[void]$sb.AppendLine("")

$nugetVulns = 0
$nugetDetails = @()

if ($hasDotnet) {
    # Scan all .csproj files
    $csprojFiles = Get-ChildItem -Recurse -Filter "*.csproj" -Path "api" -ErrorAction SilentlyContinue

    foreach ($csproj in $csprojFiles) {
        $projDir = $csproj.DirectoryName
        $vulnOutput = & dotnet list $csproj.FullName package --vulnerable 2>&1
        $inVuln = $false
        $currentPkg = ""

        foreach ($line in $vulnOutput) {
            if ($line -match '>\s+(\S+)\s+(\S+)') {
                $currentPkg = $matches[1]
            }
            if ($line -match '(Critical|High|Moderate|Low)') {
                $severity = $matches[1]
                $nugetVulns++
                $nugetDetails += [PSCustomObject]@{
                    Project  = $csproj.BaseName
                    Package  = $currentPkg
                    Severity = $severity
                    Info     = $line.Trim()
                }
            }
        }
    }

    [void]$sb.AppendLine("Projects scanned: $($csprojFiles.Count)")
    [void]$sb.AppendLine("Vulnerable packages: $nugetVulns")
} else {
    [void]$sb.AppendLine("> dotnet CLI not available. Skipping NuGet vulnerability scan.")
}

[void]$sb.AppendLine("")

if ($nugetDetails.Count -gt 0) {
    [void]$sb.AppendLine("### Vulnerable Packages")
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("| # | Project | Package | Severity |")
    [void]$sb.AppendLine("|---|---------|---------|----------|")
    $idx = 0
    foreach ($d in $nugetDetails) {
        $idx++
        [void]$sb.AppendLine("| $idx | $($d.Project) | $($d.Package) | $($d.Severity) |")
    }
    [void]$sb.AppendLine("")
} else {
    [void]$sb.AppendLine("> No known NuGet vulnerabilities found.")
    [void]$sb.AppendLine("")
}

# ── 3. Dependency Safety (depsafe) ──────────────────────────
[void]$sb.AppendLine("## 3. Dependency Safety (depsafe)")
[void]$sb.AppendLine("")

if ($hasDepsafe) {
    Push-Location "api"
    $depsafeOut = & depsafe scan 2>&1
    $exitCode = $LASTEXITCODE
    Pop-Location

    [void]$sb.AppendLine("$codeFence")
    [void]$sb.AppendLine(($depsafeOut -join "`n"))
    [void]$sb.AppendLine($codeFence)
    [void]$sb.AppendLine("")
    [void]$sb.AppendLine("**Exit code**: $exitCode")
} else {
    [void]$sb.AppendLine("> depsafe not available. Skipping dependency safety scan.")
}
[void]$sb.AppendLine("")

# ── 4. Angular/Client Dependencies ──────────────────────────
[void]$sb.AppendLine("## 4. Client-Side Dependency Audit")
[void]$sb.AppendLine("")

$npmAuditVulns = 0
if (Test-Path "client/node_modules") {
    Push-Location "client"
    $npmOut = & npm audit --json 2>&1
    $npmExit = $LASTEXITCODE
    Pop-Location

    if ($npmOut) {
        $npmJson = $npmOut -join "`n" | ConvertFrom-Json -ErrorAction SilentlyContinue
        if ($npmJson.metadata -and $npmJson.metadata.vulnerabilities) {
            $vulns = $npmJson.metadata.vulnerabilities
            $npmAuditVulns = ($vulns.critical + $vulns.high + $vulns.moderate + $vulns.low)
            [void]$sb.AppendLine("| Severity | Count |")
            [void]$sb.AppendLine("|----------|------:|")
            [void]$sb.AppendLine("| Critical | $($vulns.critical) |")
            [void]$sb.AppendLine("| High | $($vulns.high) |")
            [void]$sb.AppendLine("| Moderate | $($vulns.moderate) |")
            [void]$sb.AppendLine("| Low | $($vulns.low) |")
            [void]$sb.AppendLine("| **Total** | **$npmAuditVulns** |")
        } else {
            [void]$sb.AppendLine("> npm audit returned no structured vulnerability data.")
        }
    } else {
        [void]$sb.AppendLine("> npm audit returned no output.")
    }
} else {
    [void]$sb.AppendLine("> client/node_modules not installed. Run `npm install` first.")
}
[void]$sb.AppendLine("")

# ── Summary ──────────────────────────────────────────────────
$totalFindings = $secretsFound + $nugetVulns + $npmAuditVulns
$riskLevel = if ($totalFindings -eq 0) { "LOW" } elseif ($totalFindings -le 5) { "MEDIUM" } else { "HIGH" }

[void]$sb.AppendLine("## Security Summary")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Category | Findings |")
[void]$sb.AppendLine("|----------|----------|")
[void]$sb.AppendLine("| Secrets | $secretsFound |")
[void]$sb.AppendLine("| NuGet vulnerabilities | $nugetVulns |")
[void]$sb.AppendLine("| npm vulnerabilities | $npmAuditVulns |")
[void]$sb.AppendLine("| **Total** | **$totalFindings** |")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("**Overall Risk Level**: $riskLevel")
[void]$sb.AppendLine("")

[void]$sb.AppendLine("---")
[void]$sb.AppendLine("*Report generated by run-security-scan.ps1*")

$sb.ToString() | Out-File -FilePath $outputFile -Encoding UTF8
Write-Host "Report written to: $outputFile"

return @{
    File             = $outputFile
    SecretsFound     = $secretsFound
    NuGetVulns       = $nugetVulns
    NpmVulns         = $npmAuditVulns
    TotalFindings    = $totalFindings
    RiskLevel        = $riskLevel
}