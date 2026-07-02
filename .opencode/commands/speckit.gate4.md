---
description: Gate 4 — Validate deployment readiness: documentation, traceability, and release criteria
---

## User Input

```text
$ARGUMENTS
```

## Purpose

Gate 4 validates **deployment readiness** — ensuring the feature is fully documented, traceable from requirement to code, and meets all release criteria before production deployment. Run before marking a feature complete or opening a PR.

## Operating Constraints

- **READ-ONLY**: Do not modify any files unless user explicitly asks
- Check that all prior gates (1-3) would pass if run

## Execution Steps

### 1. Setup

Parse `$ARGUMENTS`: if the first token matches a `XXX-name` pattern (e.g. `002-billing`), treat it as a feature ID and set `FEATURE_DIR = <repo-root>/specs/<feature-id>`. Otherwise run `.specify/scripts/powershell/check-prerequisites.ps1 -Json` from repo root and parse JSON for FEATURE_DIR.

Set:
- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md
- CONSTITUTION = `.specify/memory/constitution.md` (if exists)

If any required file is missing, STOP and report which prerequisite command to run.

### 2. Load Artifacts

Read from PLAN: Testing strategy, deployment considerations
Read from SPEC: All requirements, success criteria
Read from TASKS: All tasks (should all be `[X]`)
Read from CONSTITUTION: Delivery Standards, Principle VI.B (Traceability Rules)

### 3. Run Gate 4 Validation Checks

#### 3a. All Prior Gates Pass

- Gate 1 (spec quality): All spec items clear, no `[NEEDS CLARIFICATION]`
- Gate 2 (plan quality): All plan sections complete, traceable
- Gate 3 (implementation quality): All tasks done, tests pass, security in place

(Quick-check each — do not re-run full gates, just verify preconditions.)

#### 3b. Traceability Chain Complete

- For each FR-### in spec: can you trace to a behaviour scenario → plan component → task → code → test?
- Is the traceability matrix in plan.md complete and accurate?
- Does every task in tasks.md map to a requirement or plan decision?

#### 3c. Documentation Completeness

- Is `README.md` or equivalent updated?
- Are API docs accessible (Scalar/OpenAPI)?
- Are environment/setup instructions current?
- Are known limitations documented?

#### 3d. Deployment Criteria

- Are all migration scripts reversible (if applicable)?
- Is the deployment process documented?
- Are configuration values externalized (not hardcoded)?
- Are environment variables documented?
- Is there a rollback strategy?

#### 3e. Quality Gates Sign-off

- Are all checklist items in `FEATURE_DIR/checklists/` marked `[X]`?
- Are there any open CRITICAL/HIGH severity issues?
- Is the feature complete against the Definition of Done?

### 4. Report

Output:

```
## Gate 4 — Deployment Readiness

| # | Criterion | Status | Evidence |
|---|-----------|--------|----------|
| 1 | Prior Gates Pass             | ✅ / ❌ | ... |
| 2 | Traceability Chain           | ✅ / ❌ | ... |
| 3 | Documentation Completeness   | ✅ / ❌ | ... |
| 4 | Deployment Criteria          | ✅ / ❌ | ... |
| 5 | Quality Gates Sign-off       | ✅ / ❌ | ... |

**Result**: ✅ PASS — Ready for deployment
          (or ❌ FAIL — N/M criteria failing)
```

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt for each FAIL item before editing.

### 5. Next Actions

On ✅ PASS:
- "Feature is deployment-ready. Recommended next step: create PR / merge to main."

On ❌ FAIL:
- List each failing criterion with a concrete remediation command or edit suggestion.
