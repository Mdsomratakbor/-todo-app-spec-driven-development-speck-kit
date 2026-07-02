---
description: Gate 1 — Validate that the feature specification (spec.md) is complete, clear, and unambiguous
---

## User Input

```text
$ARGUMENTS
```

## Purpose

Gate 1 validates the **quality of the specification itself** (spec.md) — ensuring requirements are complete, clear, consistent, and measurable before implementation proceeds. Runs in seconds after a task completes to catch spec-quality drift early.

## Operating Constraints

- **READ-ONLY**: Do not modify any files unless user explicitly asks
- **Context efficient**: Load only the relevant parts of spec.md

## Execution Steps

### 1. Setup

Parse `$ARGUMENTS`: if the first token matches a `XXX-name` pattern (e.g. `002-billing`), treat it as a feature ID and set `FEATURE_DIR = <repo-root>/specs/<feature-id>`. Otherwise run `.specify/scripts/powershell/check-prerequisites.ps1 -Json` from repo root and parse JSON for FEATURE_DIR.

Set:
- SPEC = FEATURE_DIR/spec.md

If `spec.md` is missing, STOP and report: "Missing spec.md — run /speckit.specify first."

### 2. Load Artifacts

Read from SPEC:
- Overview/Context section
- Functional Requirements (FR-###)
- Success Criteria (SC-###)
- User Stories and acceptance criteria
- Edge cases (if present)
- Non-functional requirements
- Dependencies and assumptions

### 3. Run Gate 1 Validation Checks

Evaluate each criterion and produce a pass (`✅ PASS`) or fail (`❌ FAIL`) with evidence:

#### 3a. Context Completeness

- Is the feature scope clearly defined with boundaries?
- Are the business objective and user need stated?
- Are assumptions and dependencies documented?
- Are external system touchpoints identified?

#### 3b. Contract Clarity

- Are API contracts, data contracts, or interface contracts defined?
- Are input/output formats specified?
- Are validation rules documented?
- Are error codes or error scenarios defined?

#### 3c. Baseline Metrics

- Are performance targets quantified (not vague terms like "fast")?
- Are success criteria measurable and objective?
- Are quality targets (availability, response time) specified?

#### 3d. Requirements Quality

- Are all Functional Requirements uniquely identified (FR-###)?
- Are requirements unambiguous and specific?
- Are requirements testable?
- No unresolved `[NEEDS CLARIFICATION]` or `TODO` markers remain?

#### 3e. Coverage

- Are all user scenarios (happy path, error, edge case) covered?
- Are non-functional requirements addressed (security, performance, UX)?

### 4. Report

Output a compact markdown table:

```
## Gate 1 — Specification Quality

| # | Criterion | Status | Evidence |
|---|-----------|--------|----------|
| 1 | Context Completeness    | ✅ / ❌ | ... |
| 2 | Contract Clarity        | ✅ / ❌ | ... |
| 3 | Baseline Metrics        | ✅ / ❌ | ... |
| 4 | Requirements Quality    | ✅ / ❌ | ... |
| 5 | Scenario Coverage       | ✅ / ❌ | ... |

**Result**: ✅ PASS (or ❌ FAIL — N/M criteria failing)
```

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt the user for each FAIL item:
"Would you like me to fix criterion #N? (yes/no)"

**Only edit files if user explicitly answers "yes" to a specific fix.**
