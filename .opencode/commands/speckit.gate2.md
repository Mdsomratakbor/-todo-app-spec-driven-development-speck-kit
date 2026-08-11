---
description: Gate 2 — Validate that the implementation plan (plan.md) is complete, consistent, and traceable to the spec
---

## User Input

```text
$ARGUMENTS
```

## Gate Definition

| Attribute | Value |
|-----------|-------|
| **Gate ID** | GATE-002 |
| **Purpose** | Validate the implementation plan (plan.md) — architecture, design, data model, API contracts, technical decisions |
| **Reviewer Role** | Technical Lead / Architect (or AI acting on their behalf) |
| **Required Before** | Gate 3 (implementation), any code generation task |
| **SLA / Timeline** | < 5 minutes automated review; < 2 hours for human sign-off |
| **Exit Criteria** | All 7 criteria below pass; no architectural contradictions; every FR-### from spec.md traces to at least one plan component |
| **Constitution References** | §I (Architecture & Design First), §VI.4 (Verify Plan), §VI.7 (Check Specification Drift), §VI.B (Traceability Rules) |
| **Artifact Checklist** | plan.md exists, architecture diagram present, solution structure documented, API endpoints defined, data model specified, UI/UX flow described, security design covered, testing strategy included, traceability matrix populated, risk assessment documented |

**Failure Path**: If Gate 2 fails, plan.md must be revised and Gate 2 re-run. Failures are logged with specific criterion references. Code generation cannot begin until Gate 2 passes.

**Waiver Process**: Gate 2 may be waived for trivial plan changes (typos, formatting) with documented rationale. All substantive architecture/design changes require full Gate 2 review.

**Re-review Rules**: If plan.md is amended after Gate 2 passes, the changed sections trigger automatic Gate 2 re-run. Any change to architecture, API contracts, or data model requires full re-review.

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

**Definition**: "Behaviour" refers to Layer 3 of the implementation plan — the behaviour specifications that describe user-system interactions as scenario tables (BH-###). Each scenario must be specific enough that a reviewer can trace it to code: it must name endpoints, entities, field values, and expected HTTP results.

- Are all behaviour scenarios (happy path, error, boundary, business rule) defined?
- Does each scenario have: ID, preconditions, given/when/then, expected result, success criteria?
- Do scenarios cover all Functional Requirements from spec.md?

#### 3b. Constraints Completeness (Layer 4)

- Are constraints documented for: Security, Validation, Logging, Performance, Error Handling, API Design, DB, UI?
- Does each constraint have: ID, forbidden action, reason, consequence, responsible layer, verification method?

#### 3c. Architecture Definition

- Is architecture documented with component / layer diagram?
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

**Definition**: "Specification Drift" is detected when:
- A plan component exists with no corresponding FR/NFR/BR in spec.md
- A spec.md requirement has no corresponding component in plan.md
- The architecture decision differs from what spec.md implies

Detection is done by cross-referencing the FR-to-component mapping in the plan's traceability matrix against the actual spec.md content.

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

### 5. Failure Handling

If any criterion fails:
1. Each failing criterion includes specific evidence from spec.md vs plan.md comparison
2. Guidance on what needs revision is provided
3. Re-run `/speckit.gate2` after plan.md changes
4. Escalate to Technical Lead if spec/plan alignment disputes arise

If `$ARGUMENTS` contains `--fix` or `--repair`, prompt for each FAIL item before editing.
