---
description: Gate 1 — Validate that the feature specification (spec.md) is complete, clear, and unambiguous
---

## User Input

```text
$ARGUMENTS
```

## Gate Definition

| Attribute | Value |
|-----------|-------|
| **Gate ID** | GATE-001 |
| **Purpose** | Validate the quality of the specification itself (spec.md) |
| **Reviewer Role** | Technical Lead / Architect (or AI acting on their behalf) |
| **Required Before** | Gate 2 (planning), any implementation task |
| **SLA / Timeline** | < 5 minutes automated review; < 1 hour for human sign-off |
| **Exit Criteria** | All 5 criteria below pass; no `[NEEDS CLARIFICATION]` or `TODO` markers remain in spec.md |
| **Constitution References** | §I (Architecture First — spec must precede implementation), §VI.2 (Verify Specification), §VI.7 (Check Specification Drift) |
| **Artifact Checklist** | spec.md exists, all FR/NFR/BR sections present, user stories defined, edge cases documented, acceptance criteria measurable |

**Failure Path**: If Gate 1 fails, spec.md must be revised and Gate 1 re-run. Failures are logged with specific criterion references. Feature implementation cannot begin until Gate 1 passes.

**Waiver Process**: Gate 1 may be waived only by explicit Technical Lead approval with documented rationale. Waivers expire after 7 days.

**Re-review Rules**: If spec.md is amended after Gate 1 passes, the changed sections trigger automatic Gate 1 re-run. Full re-review required if > 30% of spec content changes.

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

Each criterion is evaluated as pass (`✅ PASS`) or fail (`❌ FAIL`) with supporting evidence:

#### 3a. Context Completeness

**Definition**: "Context" refers to the project/feature scope documentation — business objective, user need, feature boundaries, assumptions, dependencies, and external system touchpoints.

- Is the feature scope clearly defined with boundaries?
- Are the business objective and user need stated?
- Are assumptions and dependencies documented?
- Are external system touchpoints identified?

#### 3b. Contract Clarity

**Definition**: "Contracts" are defined API contracts (endpoints, request/response DTOs), data contracts (entity schemas, field types, constraints), and interface contracts (service boundaries, repository interfaces).

- Are API contracts, data contracts, or interface contracts defined?
- Are input/output formats specified?
- Are validation rules documented?
- Are error codes or error scenarios defined?

#### 3c. Baseline Metrics

**Definition**: "Baseline Metrics" are quantified performance and quality targets established at specification time. They are a **one-time establishment** of expected thresholds (e.g., "P95 < 500ms"), not an ongoing monitoring set — though they serve as input to ongoing monitoring configuration. Metrics must be expressed as concrete numbers, not vague terms.

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

### 5. Failure Handling

If any criterion fails:
1. Each failing criterion includes specific evidence and suggested remediation
2. The user receives clear guidance on what spec.md sections need revision
3. Re-run `/speckit.gate1` after spec.md changes
4. Escalate to Technical Lead if criteria disputes arise

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt the user for each FAIL item:
"Would you like me to fix criterion #N? (yes/no)"

**Only edit files if user explicitly answers "yes" to a specific fix.**
