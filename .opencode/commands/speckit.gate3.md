---
description: Gate 3 — Validate that the implementation matches the spec and plan, with passing tests and security review
---

## User Input

```text
$ARGUMENTS
```

## Purpose

Gate 3 validates **implementation quality** — ensuring the codebase matches the spec/plan, tests pass, security measures are in place, and there's no regression. Run after any implementation task to catch quality issues early.

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

- Do unit tests exist for core business logic?
- Do integration tests cover API endpoints?
- Are test results clean (no failing tests)?
- Is coverage >= 80% (or target defined in plan)?

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

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt for each FAIL item before editing.
