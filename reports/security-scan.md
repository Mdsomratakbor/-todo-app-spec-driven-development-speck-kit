# Security Scan Report

**Generated**: 2026-08-19 15:00:06
**Project**: TodoApp-SpecKit
**Module**: 005-add-loading-indicators

## Tool Availability

| Tool | Status | Version |
|------|--------|---------|
| detect-secrets | Not available | N/A |
| dotnet | Available | 10.0.300 |
| depsafe | Available | 1.5.1+d08ba8dc563ef5cf46f63169a1b72c1da9a58d82 |

## 1. Secret Scanning

**Tool**: Manual regex scan (detect-secrets not available)

Files scanned: 301
Findings: 27

### Findings

| # | File | Type | Line |
|---|------|------|-----:|
| 1 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 22 |
| 2 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 44 |
| 3 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 59 |
| 4 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 88 |
| 5 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 95 |
| 6 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 114 |
| 7 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 129 |
| 8 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 136 |
| 9 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 151 |
| 10 | api\tests\TodoApp.IntegrationTests\Api\AuthEndpointsTests.cs | Hardcoded password | 209 |
| 11 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Login\LoginCommandHandlerTests.cs | Hardcoded password | 37 |
| 12 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Login\LoginCommandHandlerTests.cs | Hardcoded password | 73 |
| 13 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Login\LoginCommandHandlerTests.cs | Hardcoded password | 97 |
| 14 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Login\LoginCommandHandlerTests.cs | Hardcoded password | 113 |
| 15 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Login\LoginCommandValidatorTests.cs | Hardcoded password | 15 |
| 16 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Login\LoginCommandValidatorTests.cs | Hardcoded password | 29 |
| 17 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Login\LoginCommandValidatorTests.cs | Hardcoded password | 44 |
| 18 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandHandlerTests.cs | Hardcoded password | 37 |
| 19 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandHandlerTests.cs | Hardcoded password | 67 |
| 20 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandHandlerTests.cs | Hardcoded password | 84 |
| 21 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandHandlerTests.cs | Hardcoded password | 109 |
| 22 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandValidatorTests.cs | Hardcoded password | 15 |
| 23 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandValidatorTests.cs | Hardcoded password | 29 |
| 24 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandValidatorTests.cs | Hardcoded password | 44 |
| 25 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandValidatorTests.cs | Hardcoded password | 89 |
| 26 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandValidatorTests.cs | Hardcoded password | 104 |
| 27 | api\tests\TodoApp.UnitTests\Application\Auth\Commands\Register\RegisterCommandValidatorTests.cs | Hardcoded password | 119 |

## 2. NuGet Package Vulnerabilities

Projects scanned: 6
Vulnerable packages: 9

### Vulnerable Packages

| # | Project | Package | Severity |
|---|---------|---------|----------|
| 1 | TodoApp.Api |  | high |
| 2 | TodoApp.Api |  | low |
| 3 | TodoApp.Application |  | low |
| 4 | TodoApp.Domain |  | low |
| 5 | TodoApp.Infrastructure |  | low |
| 6 | TodoApp.IntegrationTests |  | high |
| 7 | TodoApp.IntegrationTests |  | high |
| 8 | TodoApp.IntegrationTests |  | low |
| 9 | TodoApp.UnitTests |  | low |

## 3. Dependency Safety (depsafe)

```
'scan' was not matched. Did you mean one of the following?
sbom
Required command was not provided.
Unrecognized command or argument 'scan'.

Description:
  DepSafe - Dependency safety and compliance for .NET and npm projects

Usage:
  DepSafe [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  analyze <path>     Analyze package health for a project or solution [default: .]
  check <package>    Check health of a single package
  sbom <path>        Generate Software Bill of Materials (SBOM) [default: .]
  vex <path>         Generate VEX (Vulnerability Exploitability eXchange) document [default: .]
  cra-report <path>  Generate comprehensive CRA compliance report [default: .]
  licenses <path>    Analyze license compatibility of dependencies [default: .]
  badge <path>       Generate shields.io badges for README [default: .]
  typosquat <path>   Check dependencies for potential typosquatting attacks [default: .]

```

**Exit code**: 1

## 4. Client-Side Dependency Audit

| Severity | Count |
|----------|------:|
| Critical | 0 |
| High | 14 |
| Moderate | 4 |
| Low | 1 |
| **Total** | **19** |

## Security Summary

| Category | Findings |
|----------|----------|
| Secrets | 27 |
| NuGet vulnerabilities | 9 |
| npm vulnerabilities | 19 |
| **Total** | **55** |

**Overall Risk Level**: HIGH

---
*Report generated by run-security-scan.ps1*

