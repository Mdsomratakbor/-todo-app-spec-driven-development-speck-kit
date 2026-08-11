---
description: Gate 3 — Validate that the implementation matches the spec and plan, with passing tests and security review
---

## User Input

```text
$ARGUMENTS
```

## Gate Definition

| Attribute | Value |
|-----------|-------|
| **Gate ID** | GATE-003 |
| **Purpose** | Validate implementation quality — code matches spec/plan, tests pass, security in place, no regression |
| **Reviewer Role** | Developer / Reviewer (or AI acting on their behalf) |
| **Required Before** | Gate 4 (deployment readiness), any PR merge |
| **SLA / Timeline** | < 5 minutes automated review; < 2 hours for full human review |
| **Exit Criteria** | All 6 criteria below pass; all Phase tasks marked `[X]`; build produces 0 errors; test suite passes at >= 80% coverage; no CRITICAL/HIGH security findings |
| **Constitution References** | §IV (Quality & Testing), §V (Security), §VI.8-11 (Implement, Build, Test, Verify DoD), §Delivery Standards (Testing Strategy) |
| **Artifact Checklist** | tasks.md tracked with `[X]` for current phase, build/compile passes 0 errors, unit tests exist for core logic, integration tests cover API endpoints, security measures (auth, validation, rate limiting) implemented per plan, error handling and structured logging in place, no regression in unchanged areas |

**Failure Path**: If Gate 3 fails, the implementation task must be fixed and Gate 3 re-run. Each failure includes a specific remediation suggestion. Merge/PR is blocked until Gate 3 passes.

**Waiver Process**: Waivers for Gate 3 require documented acceptance of risk from Technical Lead. Test coverage < 80% cannot be waived. Security findings cannot be waived without explicit security review.

**Re-review Rules**: Every implementation task completion triggers Gate 3. Full re-review required if any spec/plan contract changes after implementation starts.

## Operating Constraints

- **READ-ONLY**: Do not modify any files unless user explicitly asks
- Inspect the actual codebase, not just documents

## Execution Steps

### 1. Setup

Parse `$ARGUMENTS`: if the first token matches a `XXX-name` pattern (e.g. `002-billing`), treat it as a feature ID and set `FEATURE_DIR = <repo-root>/specs/<feature-id>`. Otherwise run `.specify/scripts/powershell/check-prerequisites.ps1 -Json` from repo root and parse JSON for FEATURE_DIR.

Set:
- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md
- CONSTITUTION = `.specify/memory/constitution.md` (if exists)

If `tasks.md` is missing, STOP and report: "Missing tasks.md — run /speckit.tasks first."

### 2. Load Artifacts

Read from PLAN: file structure, endpoint definitions, data model, validation rules, middleware
Read from SPEC: Functional Requirements, Success Criteria
Read from TASKS: Completed vs pending tasks

### 3. Inspect Codebase

Scan relevant directories (e.g., `api/`, `client/`) based on the file paths in PLAN and TASKS.

### 4. Run Gate 3 Validation Checks

#### 4a. Task Completion

- Are all tasks marked `[X]` in tasks.md matching what's in the codebase?
- No incomplete tasks remain for current phase?

#### 4b. Implementation vs Spec Alignment

- Do implemented endpoints match the API design in plan.md?
- Do entities/fields match the data model?
- Are validation rules (title length, due date range, etc.) enforced in code?
- Are business rules (status transitions, soft-delete, etc.) implemented?

#### 4c. Testing & Validation

**Definition**: "Validation Targets" are the measurable test outcomes defined in the plan's testing strategy (e.g., >= 80% code coverage, 100% pass rate, 0 flaky tests). They are distinct from "Automated Tests" (which are the test implementations themselves). Validation Targets answer "how good is good enough?" while Automated Tests answer "does the code work?"

- Do unit tests exist for core business logic?
- Do integration tests cover API endpoints?
- Are test results clean (no failing tests)?
- Is coverage >= 80% (or target defined in plan)?
- Are "Validation Targets" (coverage %, pass rate) explicitly defined and met?

#### 4d. Security & Input Validation

- Are API endpoints protected with `[Authorize]` or equivalent?
- Is input validation implemented (frontend + backend)?
- Are SQL queries parameterized (no injection risk)?
- Is rate limiting in place?

#### 4e. Error Handling & Logging

- Are ProblemDetails returned for errors?
- Is structured logging implemented with correlation IDs?
- Are all error states from UI-001/UI-002 handled?

#### 4f. Non-Regression

**Definition**: "Non-Regression" is verified by running existing tests (unit, integration, E2E) from prior phases on the unchanged modules and confirming they still pass at the same rate. Scope: all tests in the test project, not just tests for the changed files. Risk level: any test failure in an unchanged area is a regression.

- Do existing features still work (check unchanged areas)?
- Are there no breaking changes to contracts?

### 5. Report

Output:

```
## Gate 3 — Implementation Quality

| # | Criterion | Status | Evidence |
|---|-----------|--------|----------|
| 1 | Task Completion              | ✅ / ❌ | ... |
| 2 | Spec/Plan Alignment          | ✅ / ❌ | ... |
| 3 | Testing & Validation         | ✅ / ❌ | ... |
| 4 | Security & Input Validation  | ✅ / ❌ | ... |
| 5 | Error Handling & Logging     | ✅ / ❌ | ... |
| 6 | Non-Regression               | ✅ / ❌ | ... |

**Result**: ✅ PASS (or ❌ FAIL — N/M criteria failing)
```

### 6. Failure Handling

If any criterion fails:
1. Specific code/file references are provided for each failure
2. Remediation steps are suggested
3. Re-run `/speckit.gate3` after fixes
4. Escalate to Technical Lead if spec/code alignment disputes arise

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt for each FAIL item before editing.
