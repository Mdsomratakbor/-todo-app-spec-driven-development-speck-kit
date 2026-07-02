---
description: Gate 2 — Validate that the implementation plan (plan.md) is complete, consistent, and traceable to the spec
---

## User Input

```text
$ARGUMENTS
```

## Purpose

Gate 2 validates the **implementation plan** (plan.md) — ensuring architecture, design, data model, API contracts, and all technical decisions are complete, internally consistent, and traceable to the specification. Run after any plan-related task to catch drift early.

## Operating Constraints

- **READ-ONLY**: Do not modify any files unless user explicitly asks
- **Context efficient**: Load only relevant sections

## Execution Steps

### 1. Setup

Parse `$ARGUMENTS`: if the first token matches a `XXX-name` pattern (e.g. `002-billing`), treat it as a feature ID and set `FEATURE_DIR = <repo-root>/specs/<feature-id>`. Otherwise run `.specify/scripts/powershell/check-prerequisites.ps1 -Json` from repo root and parse JSON for FEATURE_DIR.

Set:
- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md

If `plan.md` is missing, STOP and report: "Missing plan.md — run /speckit.plan first."

### 2. Load Artifacts

Read from PLAN:
- Architecture diagram and layer responsibilities
- Solution structure (file tree)
- API design (endpoints, contracts)
- Data model (entities, relationships, constraints)
- UI/UX design (components, flows, states)
- Security design (auth, validation, rate limiting)
- Performance targets and strategy
- Logging & monitoring design
- Testing strategy
- Traceability matrix
- Risk assessment

Read from SPEC (for cross-reference):
- Functional Requirements (FR-###)
- Success Criteria (SC-###)
- Constraints

### 3. Run Gate 2 Validation Checks

#### 3a. Behaviour Completeness (Layer 3)

- Are all behaviour scenarios (happy path, error, boundary, business rule) defined?
- Does each scenario have: ID, preconditions, given/when/then, expected result, success criteria?
- Do scenarios cover all Functional Requirements from spec.md?

#### 3b. Constraints Completeness (Layer 4)

- Are constraints documented for: Security, Validation, Logging, Performance, Error Handling, API Design, DB, UI?
- Does each constraint have: ID, forbidden action, reason, consequence, responsible layer, verification method?

#### 3c. Architecture Definition

- Is architecture documented with component/ layer diagram?
- Are layer responsibilities clearly defined?
- Is dependency flow specified?
- Are data flows documented for key operations?

#### 3d. API & Data Model Completeness

- Are all endpoints defined with methods, paths, request/response DTOs?
- Is validation strategy documented?
- Are all entities defined with fields, types, relationships, constraints?

#### 3e. Cross-Cutting Concerns

- Are security considerations documented?
- Are performance targets specified?
- Is logging & monitoring designed?
- Is testing strategy defined?

#### 3f. Traceability

- Does each requirement trace to a behaviour scenario?
- Does each behaviour scenario trace to planned components?
- Is there no specification drift (scope creep or missing scope)?

#### 3g. Risk Assessment

- Are technical risks identified with mitigations?
- Are dependency risks documented?

### 4. Report

Output:

```
## Gate 2 — Plan Quality

| # | Criterion | Status | Evidence |
|---|-----------|--------|----------|
| 1 | Behaviour Completeness       | ✅ / ❌ | ... |
| 2 | Constraints Completeness     | ✅ / ❌ | ... |
| 3 | Architecture Definition      | ✅ / ❌ | ... |
| 4 | API & Data Model             | ✅ / ❌ | ... |
| 5 | Cross-Cutting Concerns       | ✅ / ❌ | ... |
| 6 | Traceability                 | ✅ / ❌ | ... |
| 7 | Risk Assessment              | ✅ / ❌ | ... |

**Result**: ✅ PASS (or ❌ FAIL — N/M criteria failing)
```

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt for each FAIL item before editing.
