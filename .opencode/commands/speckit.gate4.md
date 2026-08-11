---
description: Gate 4 — Validate deployment readiness: documentation, traceability, and release criteria
---

## User Input

```text
$ARGUMENTS
```

## Gate Definition

| Attribute | Value |
|-----------|-------|
| **Gate ID** | GATE-004 |
| **Purpose** | Validate deployment readiness — fully documented, traceable from requirement to code, meets release criteria |
| **Reviewer Role** | Technical Lead / Project Manager (or AI acting on their behalf) |
| **Required Before** | Production deployment, PR merge to main |
| **SLA / Timeline** | < 5 minutes automated review; < 1 hour for human sign-off |
| **Exit Criteria** | All 5 criteria below pass; no open CRITICAL/HIGH issues; all checklist items in FEATURE_DIR/checklists/ marked `[X]`; Definition of Done satisfied per constitution §Delivery Standards |
| **Constitution References** | §VI.B (Traceability Rules), §Delivery Standards (Definition of Done, Testing Strategy), §Governance (Compliance Review) |
| **Artifact Checklist** | Feature fully implemented (all tasks `[X]`), all prior gates (1-3) pass, traceability chain complete (requirement → scenario → task → code → test), documentation updated (README, API docs, env setup), deployment process documented, migration scripts reversible, rollback strategy defined, configuration externalized, checklist items complete |

**Failure Path**: If Gate 4 fails, the specific gap must be closed and Gate 4 re-run. Deployment is blocked until Gate 4 passes. Production release cannot proceed without passing Gate 4.

**Waiver Process**: Gate 4 waivers require documented acceptance from Technical Lead and Project Manager. Documentation gaps may be waived for internal releases. Traceability gaps cannot be waived.

**Re-review Rules**: Full Gate 4 re-review required before every production deployment. If the feature branch is rebased or merged from main, Gate 4 must be re-run.

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

**Alignment with Constitution §VI.B**: The traceability chain must follow the exact mapping defined in the constitution:
```
Requirement ID (FR, NFR, BR) → Behaviour Scenario (BH-ID) → Constraint (C-ID) → Task (T-ID) → Code (file paths) → Test (test names)
```

- For each FR-### in spec: can you trace to a behaviour scenario → plan component → task → code → test?
- Is the traceability matrix in plan.md complete and accurate?
- Does every task in tasks.md map to a requirement or plan decision?
- Does the traceability chain match the constitution's required format (Principle VI.B)?

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

### 5. Failure Handling

If any criterion fails:
1. Specific gap location and suggested remediation are provided
2. Re-run `/speckit.gate4` after fixes
3. Escalate to Technical Lead and/or Project Manager for sign-off disputes

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt for each FAIL item before editing.

### 6. Next Actions

On ✅ PASS:
- "Feature is deployment-ready. Recommended next step: create PR / merge to main."

On ❌ FAIL:
- List each failing criterion with a concrete remediation command or edit suggestion.
