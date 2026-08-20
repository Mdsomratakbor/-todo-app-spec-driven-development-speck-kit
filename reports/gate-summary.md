# Gate Deliverables - Summary

**Generated**: 2026-08-19 14:42:12
**Module**: 005-add-loading-indicators
**Elapsed**: 3.9s

## Results Overview

| Report | Status | Key Metrics |
|--------|--------|-------------|
| AI-Human Accounting | OK | 20 commits, AI: 40%, Human: 60% |
| Mid-Point Report | OK | Score: 101/115 (Grade: A) |
| Non-Regression Plan | FAIL | At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\generate-non-regression-plan.ps1:213 char:14
+ [foreach ($fr in $allFRs) {
+              ~
Missing closing ')' in expression.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\generate-non-regression-plan.ps1:213 char:1
+ [foreach ($fr in $allFRs) {
+ ~~~~~~~~~~~~~
Unexpected attribute 'foreach'.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\generate-non-regression-plan.ps1:213 char:15
+ [foreach ($fr in $allFRs) {
+               ~~
Unexpected token 'in' in expression or statement.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\generate-non-regression-plan.ps1:213 char:25
+ [foreach ($fr in $allFRs) {
+                         ~
Unexpected token ')' in expression or statement.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\generate-non-regression-plan.ps1:215 char:2
+ }]
+  ~
Unexpected token ']' in expression or statement.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\generate-non-regression-plan.ps1:213 char:11
+ [foreach ($fr in $allFRs) {
+           ~~~
Attribute argument must be a constant or a script block. |
| Security Scan | FAIL | At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:86 char:11
+         @{ Pattern = '(?i)(password|passwd|pwd)\s*[:=]\s*["\x27][^\s" …
+           ~
Missing closing '}' in statement block or type definition.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:86 char:72
+ … Pattern = '(?i)(password|passwd|pwd)\s*[:=]\s*["\x27][^\s"';]{8,}'; N …
+                                                               ~
Unexpected token ']' in expression or statement.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:86 char:76
+ … tern = '(?i)(password|passwd|pwd)\s*[:=]\s*["\x27][^\s"';]{8,}'; Name …
+                                                                ~
Missing expression after ','.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:88 char:73
+ … Pattern = '(?i)(secret|token)\s*[:=]\s*["\x27][A-Za-z0-9]{16,}'; Name …
+                                                                ~
Missing expression after ','.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:90 char:64
+ …       @{ Pattern = '(?i)(aws[_-]?access[_-]?key[_-]?id|AKIA)[A-Z0-9]{ …
+                                                                ~
Array index expression is missing or not valid.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:158 char:59
+             if ($line -match '(Critical|High|Moderate|Low)') {
+                                                           ~~~~
The string is missing the terminator: '.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:278 char:3
+ }
+   ~
Missing closing ')' in subexpression.

At D:\personal-projects\researchs\TodoApp-SpecKit\scripts\run-security-scan.ps1:80 char:8
+ } else {
+        ~
Missing closing '}' in statement block or type definition. |

## Action Items

- CRITICAL: Fix build errors before proceeding

## Generated Files

| Report | Path |
|--------|------|
| AI-Human Accounting | reports/ai-human-accounting.md |
| Mid-Point Report | reports/mid-point-report.md |
| Non-Regression Plan | reports/non-regression-plan.md |
| Security Scan | reports/security-scan.md |
| **Summary** | reports/gate-summary.md |

