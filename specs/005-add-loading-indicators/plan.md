# Implementation Plan: Loading Indicators for All Operations

**Feature**: 005-add-loading-indicators
**Date**: 2026-08-11
**Status**: Ready for Implementation
**Branch**: `005-loading-indicators`

---

## Summary

Frontend-only feature that makes loading feedback visible, consistent, and accessible across every user-facing operation in the Angular SPA:

- **Skeleton placeholders** (Facebook-style) shaped like the real content on every data screen (todo list, categories, todo detail, profile) — replacing the current mix of generic spinners, skeleton cards, and plain text.
- **Busy state on every user-triggered operation** (submit, save, delete, logout, filter apply, retry) with duplicate-submission protection.
- **Flicker-free** loading for fast operations via a deferred/min-display-time loading operator.
- **Non-blocking** indicator for background refreshes (filter/page changes, reloads after edits) that keeps existing content visible.
- **Accessible** and reduced-motion aware: `aria-live` announcements, `aria-busy`, shimmer disabled under `prefers-reduced-motion`.

No backend/API or database changes.

## Technical Context

| Item | Value |
|------|-------|
| Feature Directory | `specs/005-add-loading-indicators/` |
| Spec File | `specs/005-add-loading-indicators/spec.md` |
| Implementation Plan | `specs/005-add-loading-indicators/plan.md` |
| Data Model | `specs/005-add-loading-indicators/data-model.md` |
| Research | `specs/005-add-loading-indicators/research.md` |
| Contracts | `specs/005-add-loading-indicators/contracts/loading-contracts.md` |
| Quickstart | `specs/005-add-loading-indicators/quickstart.md` |
| Branch | `005-loading-indicators` |

**Platform**: Client-only feature (Angular 21 SPA, Angular Material, Bootstrap). No backend/API changes.

**Language/Framework**: Angular 21 standalone components, TypeScript, Angular Material (`MatProgressSpinner`, `MatProgressBar`, `MatIcon`, `MatButton`), RxJS.

**Primary Dependencies**: `@angular/material` (progress-spinner, progress-bar, icon, button, card) — all already installed (v21.2.14). **No new third-party dependencies.**

**Storage**: N/A (no persistence; pure UI state).

**Testing**: Vitest via `ng test` (`@angular/build:unit-test`, `vitest/globals` — project migrated from Jasmine in 004); build verification via `ng build`.

**Target Platform**: Modern browsers (Chrome, Edge, Firefox, Safari); `prefers-reduced-motion` media query supported; assistive technologies (screen readers).

**Performance Goals**: Indicator appears within 200 ms for slow ops (SC-001); no loader flash for ops completing under the 200 ms defer threshold (SC-005); shimmer uses transform/opacity only for GPU compositing; no layout shift when skeletons resolve (SC-003).

**Constraints**: Frontend-only; no new dependencies; all motion gated behind `prefers-reduced-motion: no-preference`; consistent behavior across all screens (FR-010); existing routing transitions unchanged (spec assumption).

**Scale/Scope**: ~30 tasks across 6 phases; touches 2 shared loading components, 1 RxJS utility, global styles, and 11 components.

---

## Clarifications & Design Decisions (from research.md)

These decisions are binding for task acceptance criteria.

1. **Skeleton architecture**: dedicated `SkeletonComponent` (selector `app-skeleton`) with layout variants (`todo-list`, `category-list`, `detail`, `profile`, `rows`) rather than overloading `LoadingSpinnerComponent`. The existing spinner keeps its role for point-of-action feedback.
2. **Flicker suppression**: shared RxJS operator `withLoadingState(loading, { deferMs, minMs })` — `deferMs = 200` (no loader shown if the op finishes earlier → SC-005), `minMs = 300` (once visible, stays at least 300 ms → no flash-for-one-frame). SC-001's "within 200 ms" is satisfied because any op still running at 200 ms shows the indicator.
3. **Non-blocking refresh split**: data screens distinguish `loading()` (no data yet → full skeleton) from `refreshing()` (data already present → keep content, show a slim indeterminate `MatProgressBar` above the list, UI remains interactive). Filter/page changes and post-edit reloads are background refreshes.
4. **Reduced motion**: shimmer/pulse/rotation all gated behind `@media (prefers-reduced-motion: no-preference)`; static (non-animated) skeletons and static spinner icons otherwise.
5. **Accessibility**: every loader root has `role="status"` + `aria-live="polite"` with an announced label; data-screen containers bind `aria-busy` during load. Screen readers announce "Loading {screen}..." then the resolved content.
6. **Busy states**: single global `.btn-spinner` style + `spin` keyframes in `styles.scss` (currently duplicated per component); delete actions use a `deletingId` signal passed into cards to disable the delete button and show a mini spinner; retry buttons reuse the same busy pattern.
7. **Error + retry**: error toasts remain the `errorInterceptor`'s job (from 004). New shared `ErrorStateComponent` with a Retry button shows only when a first load fails with no data; if a refresh fails with data present, keep the data and rely on the interceptor toast (no disruptive retry surface).
8. **Parallel requests**: the primary data request drives the skeleton; secondary lookups (filter-bar options, form category dropdowns) are non-blocking and populate when ready (documented edge-case handling).
9. **Cancellation on navigation**: subscriptions in data screens use `takeUntilDestroyed()` so a pending load never applies its loading state to a destroyed screen (documented edge-case handling).
10. **Long-running ops**: no artificial timeout; indicator persists with clear messaging until completion.

---

## Constitution Check

| Principle | Status | Notes |
|-----------|--------|-------|
| Principle I: Architecture & Design First | ✅ PASS | Client-only; follows existing component-based architecture; shared presentational components |
| Principle I: Informative UI | ✅ PASS | Loading/empty/error states for every user action — the core of this feature |
| Principle I: Notification standards | ✅ PASS | Reuses `NotificationService` + `errorInterceptor` for error/success messaging |
| Principle II: Naming & Structure | ✅ PASS | New files under `client/src/app/shared/components/loading/` and `shared/utils/` in kebab-case |
| Principle III: Git workflow | ✅ PASS | Feature branch `005-loading-indicators`, conventional commits |
| Principle IV: Notifications | ✅ PASS | Error toasts centralized in `errorInterceptor` (004); retry surfaces non-duplicative |
| Principle IV: Validation | N/A | No new forms or input validation introduced |
| Principle IV: Testing | ✅ PASS | Vitest unit tests for new components/operator and updated screen specs |
| Principle V: Security | ✅ PASS | No auth changes; `takeUntilDestroyed()` prevents state leakage across routes |
| Principle V: Accessibility | ✅ PASS | FR-008 (reduced motion), FR-009 (`aria-live`/`aria-busy` announcements) |
| Principle VI: Task lifecycle | ✅ PASS | Every task follows the 13-step lifecycle during implementation |
| Definition of Done | ✅ PASS | Build passes, tests pass, no regressions, a11y/reduced-motion validated |

**GATE**: PASS — no violations requiring justification. No complexity-tracking table needed.

---

## Current State Analysis

### Strengths (to preserve)
- `LoadingSpinnerComponent` exists with a shimmer-based skeleton branch (`skeleton` input) and `role="status" aria-live="polite"` root
- todo-list already shows a skeleton during initial load; login/register/todo-form/category-form already have button spinners (004) with reduced-motion gating
- `errorInterceptor` centralizes error toasts; `NotificationService` exists with severity styling (004)
- Signal-based state throughout (`signal`, `input`, `output`)
- `MatProgressSpinner` and `MatProgressBar` both available in `@angular/material` v21

### Gaps (to address)
- **Inconsistent skeletons**: todo-list uses a generic 3-card skeleton; category-list and todo-detail use plain spinners; profile uses raw text "Loading profile..." — none mirror the real layout (violates FR-002/FR-003/SC-002)
- **No flicker suppression**: every request toggles the loader immediately, so fast operations flash (violates FR-007/SC-005)
- **Background refreshes block the whole screen**: filter/page changes and reloads re-render the full skeleton instead of keeping content with a non-blocking indicator (violates FR-006)
- **Missing busy states**: delete buttons (todo-card, category-card, todo-list, todo-detail, category-list), toolbar logout, profile logout, and filter Apply/Clear have no busy/disabled state (violates FR-004/SC-004)
- **No retry affordance** on failed first loads (FR-005 "retry where appropriate")
- **`.btn-spinner` styles duplicated** across 4 components; shimmer animation not gated behind `prefers-reduced-motion` (violates FR-008)
- **No `aria-busy`** on data containers; profile loader not announced by a status region (FR-009 partial)
- **Subscriptions not cancelled** on navigation (edge case: loading state leaking across screens)

### Implemented State (verified 2026-08-11)
- `LoadingSpinnerComponent` (`shared/components/loading-spinner/`) with `skeleton` + `message` + `diameter` inputs; shimmer keyframes always animated (not reduced-motion aware)
- todo-list: `@if (loading()) { <app-loading-spinner [skeleton]="true" message="Loading todos..." /> }`
- todo-detail: `<app-loading-spinner message="Loading todo..." />` (spinner, not skeleton)
- category-list: `<app-loading-spinner message="Loading categories..." />` (spinner, not skeleton)
- profile: `@if (loading()) { <p>Loading profile...</p> }` (plain text)
- Button spinners (`sync` icon + `spin`) in login, register, todo-form, category-form — all with their own copy of `.btn-spinner` styles
- `errorInterceptor` shows error toasts for non-`/auth/` failures; component error handlers only reset loading state
- No screens use `refreshing`-vs-`loading` distinction; all reloads show full loader

### Remaining Work vs. Spec
- **US1** (skeleton data screens): shared `SkeletonComponent` + apply to todo-list, category-list, todo-detail, profile
- **US2** (busy state on every operation): global `.btn-spinner`, busy delete/logout/filter/retry buttons, duplicate-submission protection
- **US3** (background refresh): `loading` vs `refreshing` split + slim `MatProgressBar`
- **US4** (accessible + flicker-free): `withLoadingState` operator, reduced-motion gating, `aria-live`/`aria-busy`
- Error + retry surface for failed first loads (FR-005)

---

## Implementation Steps

### Phase 1: Setup
1. Verify Angular build (`ng build`) and test baseline (`ng test`) before changes
2. Confirm `MatProgressBar` import path and existing test conventions (Vitest)

### Phase 2: Skeleton System (US1)
1. Create `SkeletonComponent` (`app-skeleton`) with layout variants mirroring real content (todo list card rows, category rows, detail layout, profile card) — shimmer gated by `prefers-reduced-motion: no-preference`
2. Apply `app-skeleton` to todo-list (replace generic skeleton), category-list (replace spinner), todo-detail (replace spinner), profile (replace text)
3. Verify skeletons resolve into content without layout shift (SC-003)

### Phase 3: Busy States for Every Operation (US2)
1. Move `.btn-spinner` + `spin` keyframes into `styles.scss` as global styles; remove per-component duplicates
2. Add busy/disabled state to delete actions (todo-card, category-card via `deletingId` input), toolbar logout, profile logout, filter Apply/Clear, retry buttons
3. Ensure duplicate-submission prevention everywhere a wait occurs (FR-004/SC-004)

### Phase 4: Flicker-Free & Accessible Loaders (US4)
1. Create `withLoadingState` RxJS operator (`deferMs` 200, `minMs` 300) and wire data-screen loads through it
2. Add `aria-busy` bindings to data-screen containers; confirm every loader root announces via `role="status"`/`aria-live="polite"`
3. Audit reduced-motion: shimmer, spinner rotation, progress-bar animation all static under `prefers-reduced-motion`

### Phase 5: Non-Blocking Background Refresh (US3)
1. Split `loading()` vs `refreshing()` in data screens; keep content and show slim `MatProgressBar` on refresh
2. Add `takeUntilDestroyed()` to data-screen subscriptions (cancel on navigation)
3. Add error + retry surface (`ErrorStateComponent`) for failed first loads

### Phase 6: Polish & Verification
1. Unit tests for `SkeletonComponent`, `ErrorStateComponent`, `withLoadingState` operator; update screen specs
2. Verify build and run full test suite
3. Update AGENTS.md session summary

---

## Phases & Task Breakdown

See `tasks.md` for full task details.

### Phase 1: Setup
- T001 Verify Angular build (`ng build`) and test baseline (`ng test`)
- T002 Confirm `MatProgressBar` import path + Vitest conventions for component specs

### Phase 2: User Story 1 — Skeleton Loading (US1)
- T003 Create `client/src/app/shared/components/loading/skeleton.component.ts` with layout variants + reduced-motion-aware shimmer
- T004 [P] Add `aria-live` label handling to skeleton component (announce "Loading {screen}")
- T005 Apply `app-skeleton` (todo-list variant) to `todo-list.component.ts`
- T006 [P] Apply `app-skeleton` (category-list variant) to `category-list.component.ts`
- T007 [P] Apply `app-skeleton` (detail variant) to `todo-detail.component.ts`
- T008 [P] Apply `app-skeleton` (profile variant) to `profile.component.ts`
- T009 Verify US1 acceptance (quickstart Scenario 1; layout mirroring, no layout shift)

### Phase 3: User Story 2 — Busy State on Every Operation (US2)
- T010 Move `.btn-spinner` + `spin` keyframes to `client/src/styles.scss`; remove per-component duplicates (login, register, todo-form, category-form)
- T011 Add `deletingId` busy state to `todo-card` + `todo-list` (disable delete + mini spinner)
- T012 [P] Add `deletingId` busy state to `category-card` + `category-list`
- T013 Add busy/disabled state to toolbar Logout (`app.html`/`app.ts`)
- T014 [P] Add busy/disabled state to profile Logout button
- T015 Add busy/disabled state to filter-bar Apply/Clear
- T016 Add busy/disabled state to retry buttons (depends on T020)
- T017 Verify US2 acceptance (quickstart Scenario 2; no duplicate submissions)

### Phase 4: User Story 4 — Flicker-Free & Accessible (US4)
- T018 Create `client/src/app/shared/utils/loading.operator.ts` (`withLoadingState`, deferMs 200, minMs 300)
- T019 Wire data-screen loads through `withLoadingState` (todo-list, category-list, todo-detail, profile)
- T020 Create `client/src/app/shared/components/loading/error-state.component.ts` (message + Retry, busy-aware)
- T021 Add `aria-busy` bindings to data-screen containers; audit `role="status"`/`aria-live` on all loaders
- T022 Audit reduced-motion: shimmer, spinner rotation, progress-bar static under `prefers-reduced-motion`
- T023 Verify US4 acceptance (quickstart Scenario 4)

### Phase 5: User Story 3 — Non-Blocking Background Refresh (US3)
- T024 Split `loading()` vs `refreshing()` in todo-list (keep content + slim `MatProgressBar` on refresh)
- T025 [P] Split `loading()` vs `refreshing()` in category-list
- T026 Add `takeUntilDestroyed()` to data-screen subscriptions (cancel on navigation)
- T027 Wire error + retry state: show `ErrorStateComponent` on failed first load, keep data on failed refresh
- T028 Verify US3 acceptance (quickstart Scenario 3)

### Phase 6: Polish & Cross-Cutting
- T029 [P] Unit tests: `SkeletonComponent` variants + reduced-motion, `ErrorStateComponent`, `withLoadingState` operator
- T030 [P] Update screen specs (todo-list, category-list, todo-detail, profile) for skeleton/refresh/retry states
- T031 Verify build (`ng build`)
- T032 Run tests (`ng test`)
- T033 Update AGENTS.md session summary

---

## Dependencies

- T001, T002 → all phases (baseline + conventions)
- US1 (T003-T009) → independent of US2/US4/US5; T004 [P] parallel to T003 (same folder, different file)
- T005-T008 [P] parallel across 4 screens; T009 after all
- US2 (T010-T017): T010 before T013-T015 (global style reuse); T011/T012 [P] parallel; T016 depends on T020 (retry component); T017 after
- US4 (T018-T023): T018 → T019 (operator before wiring); T020 → T016 (retry button); T022 after T010/T018; T023 after
- US3 (T024-T028): T024/T025 [P] parallel; T026 independent; T027 depends on T020 (error component); T028 after
- T029, T030 [P] parallel test specs; T031-T033 → all phases complete

---

## Risk Mitigation

| Risk | Severity | Mitigation |
|------|----------|------------|
| Skeleton layout drift vs. real content (layout shift) | Medium | Variants mirror actual card/detail markup; verify per screen via quickstart Scenario 1 + visual review |
| Loader flashes on fast local ops | Medium | `withLoadingState` defers 200 ms and enforces 300 ms minimum; screen capture verification (SC-005) |
| Delete spam / double-submission | Medium | `deletingId` disables the button at the point of action; form buttons already `[disabled]`-bound |
| Refresh masking content | Medium | `refreshing()` keeps content visible; only the slim progress bar is added; verified in quickstart Scenario 3 |
| Reduced-motion regression | Low | All animation gated behind `prefers-reduced-motion: no-preference`; a11y audit task (T022) |
| A11y double-announcement (status region + aria-busy) | Low | `aria-live` label on loader + `aria-busy` on container complement each other; verified by screen-reader review |
| Component styles drift (`.btn-spinner` de-duplication) | Low | Centralize in `styles.scss`; remove per-component copies in T010 |
| Loading state applied to destroyed screen | Low | `takeUntilDestroyed()` on all data-screen subscriptions (T026) |

---

## Rollback Plan

If a change causes test failures or visual regressions:
1. `git checkout 005-loading-indicators -- <file>` to restore individual files
2. For skeleton issues, revert a screen to its prior loader (`app-loading-spinner`) by reverting T005-T008 changes
3. For flicker issues, revert `withLoadingState` wiring in T019 (fall back to immediate `loading.set`)
4. For refresh issues, remove the `refreshing()` branch and return to `@if (loading())` skeleton-only
5. For busy-state issues, remove the `deletingId`/disabled bindings on the affected button
6. Global `.btn-spinner` is additive in `styles.scss`; deleting component copies is safe to revert individually
