# Quality Gates Checklist: Todo Management System

**Purpose**: Validate the completeness, clarity, consistency, and measurability of the four Quality Gate definitions (Gate 1–4)
**Created**: 2026-06-30
**Feature**: [spec.md](../../specs/001-todo-management/spec.md)

**Note**: This checklist tests the **Quality Gate requirements themselves** — whether they are well-defined, unambiguous, and ready for adoption. It does not test implementation compliance.

---

## Requirement Completeness

- [ ] CHK001 Are the review criteria for each Gate item (e.g., "Context", "Contract", "Behaviour") explicitly documented with specific expectations? [Completeness, Gap]
- [ ] CHK002 Is the process defined for what happens when a Gate review identifies issues or failures? [Completeness, Gap]
- [ ] CHK003 Are the roles or personas responsible for performing each Gate review specified? [Completeness, Gap]
- [ ] CHK004 Is the escalation or remediation path defined when a Gate is not passed? [Completeness, Gap]
- [ ] CHK005 Are exit criteria defined for each Gate — what constitutes "passing" the review? [Completeness, Gap]

---

## Requirement Clarity

- [ ] CHK006 Is the term "Context" in Gate 1 defined with specific artifacts or document references? [Clarity, Gate 1]
- [ ] CHK007 Is "Contract" in Gate 1 unambiguously scoped (API contracts, data contracts, interface contracts)? [Clarity, Gate 1]
- [ ] CHK008 Are "Baseline Metrics" in Gate 1 quantified with specific measurement targets? [Clarity, Gate 1]
- [ ] CHK009 Is "Validation Targets" in Gate 3 defined with measurable success criteria? [Clarity, Gate 3]
- [ ] CHK010 Is "Non-Regression" in Gate 3 clearly scoped (which tests, which coverage threshold, which risk level?)? [Clarity, Gate 3]
- [ ] CHK011 Are the "Behaviour" review criteria in Gate 2 specific enough to guide a reviewer? [Clarity, Gate 2]
- [ ] CHK012 Is "Specification Drift" in Gate 2 defined with concrete detection criteria? [Clarity, Gate 2]

---

## Requirement Consistency

- [ ] CHK013 Does "Traceability" in Gate 4 align with the Traceability Rules defined in Constitution Principle VI.B? [Consistency, Gate 4, Constitution §VI.B]
- [ ] CHK014 Do the Gate 2 items ("Behaviour", "Constraints", "Architecture") map consistently to the Layer 3 and Layer 4 definitions in the implementation plan? [Consistency, Gate 2, Plan §Layer 3-4]
- [ ] CHK015 Are "Automated Tests" (Gate 3) requirements consistent with the testing strategy in the Delivery Standards? [Consistency, Gate 3, Constitution §Delivery Standards]
- [ ] CHK016 Is the Gate ordering consistent with the project workflow (cannot reach Gate 3 without passing Gate 2)? [Consistency]

---

## Acceptance Criteria Quality

- [ ] CHK017 Can each Gate's "Required before" condition be objectively verified? [Measurability, Gate 1-4]
- [ ] CHK018 Is there a documented sign-off or approval mechanism for each Gate transition? [Measurability, Gap]
- [ ] CHK019 Are the "Deployment" review criteria in Gate 4 specific enough to verify before production release? [Measurability, Gate 4]

---

## Scenario Coverage

- [ ] CHK020 Is a Gate failure/rejection scenario defined for each of the four Gates? [Coverage, Exception Flow]
- [ ] CHK021 Is there a defined process for bypassing or waiving a Gate under exceptional circumstances? [Coverage, Gap]
- [ ] CHK022 Are requirements defined for Gate re-review when changes are made after a Gate is passed? [Coverage, Gap]
- [ ] CHK023 Is the timeline or cadence for each Gate specified (how long should review take, SLA)? [Coverage, Gap]

---

## Non-Functional Requirements Coverage

- [ ] CHK024 Are security review criteria explicitly listed or referenced in Gate 3? [Coverage, Gate 3 "Security"]
- [ ] CHK025 Are logging requirements review criteria specified in Gate 3? [Coverage, Gate 3 "Logging"]
- [ ] CHK026 Is documentation completeness verifiable under Gate 4 "Documentation"? [Coverage, Gate 4]

---

## Ambiguities & Gaps

- [ ] CHK027 Is the relationship between the 13-step Task Lifecycle (Principle VI) and the 4 Quality Gates documented? [Ambiguity, Constitution §VI vs Gates]
- [ ] CHK028 Are "Validation Targets" distinct from "Automated Tests" or overlapping concepts? Clarify scope boundaries. [Ambiguity, Gate 3]
- [ ] CHK029 Is "Baseline Metrics" a one-time establishment or an ongoing monitoring threshold? [Ambiguity, Gate 1]
- [ ] CHK030 Is there a defined artifact or deliverable checklist for each Gate that a reviewer can inspect? [Gap]

---

## Dependencies & Assumptions

- [ ] CHK031 Are assumptions about the availability of reviewers or subject matter experts documented? [Assumption, Gap]
- [ ] CHK032 Are dependencies on external tools or systems (CI pipeline, test infrastructure) for Gate verification documented? [Dependency, Gap]
- [ ] CHK033 Is the assumption that all Gates are mandatory documented, with conditions for exception? [Assumption, Gap]

---

## Traceability

- [ ] CHK034 Does each Gate item trace to a constitution principle or Delivery Standard? [Traceability]
- [ ] CHK035 Is a Requirement ID scheme established for the Gate items themselves? [Traceability, Gap]
