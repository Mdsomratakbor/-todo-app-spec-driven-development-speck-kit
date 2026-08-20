# Tasks: Loading Indicators for All Operations

**Input**: Design documents from `/specs/005-add-loading-indicators/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/, quickstart.md

**Tests**: The feature spec does not explicitly request test-first tasks. Unit tests are included in the Polish phase (Phase 6) per research.md §11, following the project's Vitest convention.

**Organization**: Tasks are grouped by phase matching plan.md (which owns the canonical T001-T033 numbering and dependencies).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions
- FR-### references map each task to spec.md requirements (constitution Traceability Rules)

## Path Conventions

- Web app: frontend code under `client/src/` (Angular 21 standalone components)

---

## Phase 1: Setup

**Purpose**: Verify the baseline before any changes

- [X] T001 Verify the Angular project builds cleanly (`ng build`) and the existing test suite passes (`ng test`) in `client/`
- [X] T002 Confirm `MatProgressBar` availability in `@angular/material` v21 and review existing Vitest component-spec conventions in `client/src/app/**/*.spec.ts`

**Checkpoint**: Baseline verified; task implementation can begin.

---

## Phase 2: User Story 1 — Skeleton Loading (US1)

**Goal**: Every data screen shows a skeleton shaped like its real content during initial load, resolving without layout shift (FR-002, FR-003).

**Independent Test**: Load each data screen on a throttled connection (quickstart Scenario 1) — a skeleton matching the real layout appears (not a spinner/blank/text), then resolves into content in the same position (CLS < 0.1). Already-loaded screens show no skeleton.

- [X] T003 [US1] Create `SkeletonComponent` in `client/src/app/shared/components/loading/skeleton.component.ts` — selector `app-skeleton`, inputs `variant` (`todo-list` | `category-list` | `detail` | `profile` | `rows`), `count` (default 3), `label` (announced text); shapes mirror real card/detail markup using Material CSS variables; shimmer gated by `@media (prefers-reduced-motion: no-preference)` (FR-002, FR-003, FR-008)
- [X] T004 [P] [US1] Add `aria-live` label handling to `SkeletonComponent` — root `role="status"` + `aria-live="polite"` announces `label` (e.g., "Loading todos...") (FR-009)
- [X] T005 [US1] Apply `app-skeleton` (todo-list variant) in `client/src/app/features/todos/todo-list/todo-list.component.ts` — replace the generic `<app-loading-spinner [skeleton]="true" ...>` in the `@if (loading())` branch (FR-002)
- [X] T006 [P] [US1] Apply `app-skeleton` (category-list variant) in `client/src/app/features/categories/category-list/category-list.component.ts` — replace the plain `<app-loading-spinner message="Loading categories..." />` in the `@if (loading())` branch (FR-002)
- [X] T007 [P] [US1] Apply `app-skeleton` (detail variant) in `client/src/app/features/todos/todo-detail/todo-detail.component.ts` — replace `<app-loading-spinner message="Loading todo..." />` in the `@if (loading())` branch (FR-002)
- [X] T008 [P] [US1] Apply `app-skeleton` (profile variant) in `client/src/app/features/auth/profile/profile.component.ts` — replace the plain text `@if (loading()) { <p>Loading profile...</p> }` branch (FR-002, FR-009)
- [X] T009 [US1] Verify US1 acceptance via quickstart Scenario 1 — all 4 data screens show layout-mirroring skeletons that resolve without significant layout shift; already-loaded screens show no skeleton

**Checkpoint**: User Story 1 is fully functional and independently testable.

---

## Phase 3: User Story 2 — Busy State on Every Operation (US2)

**Goal**: Every wait-causing operation shows a busy state at the point of action, is disabled against duplicate submission, and resets on success/error (FR-004, FR-005).

**Independent Test**: Trigger each operation (submit/save, delete, logout, filter apply, retry) via quickstart Scenario 2 — the triggering control immediately shows a busy state and cannot be activated twice; it returns to its normal enabled state after completion or failure (with an error notification on failure).

- [X] T010 [US2] Move `.btn-spinner` + `spin` keyframes into `client/src/styles.scss` (gated by `@media (prefers-reduced-motion: no-preference)`); remove the now-duplicated copies from `client/src/app/features/auth/login/login.component.ts`, `client/src/app/features/auth/register/register.component.ts`, `client/src/app/features/todos/todo-form/todo-form.component.ts`, `client/src/app/features/categories/category-form/category-form.component.ts` (FR-010)
- [X] T011 [US2] Add `deletingId` busy state to `client/src/app/features/todos/todo-card/todo-card.component.ts` (add `deleting` input; delete button shows `.btn-spinner` + `[disabled]`) and wire it in `client/src/app/features/todos/todo-list/todo-list.component.ts` (pass `[deleting]="deletingId() === todo.id"`; set/clear on success and error) (FR-004, FR-005)
- [X] T012 [P] [US2] Add `deletingId` busy state to `client/src/app/features/categories/category-card/category-card.component.ts` (add `deleting` input) and wire it in `client/src/app/features/categories/category-list/category-list.component.ts` (FR-004, FR-005)
- [X] T013 [US2] Add busy/disabled state to toolbar Logout in `client/src/app/app.html` and `client/src/app/app.ts` — busy signal + `[disabled]` + `.btn-spinner` while `authService.signOut()` is pending (FR-004)
- [X] T014 [P] [US2] Add busy/disabled state to profile Logout in `client/src/app/features/auth/profile/profile.component.ts` — busy signal + `[disabled]` + `.btn-spinner` while `logout()` is pending (FR-004)
- [X] T015 [US2] Add busy/disabled state to filter Apply/Clear in `client/src/app/features/todos/filter-bar/filter-bar.component.ts` — bind `[disabled]` to the in-flight list request so filters cannot be re-applied mid-load (FR-004)
- [X] T016 [US2] Add busy/disabled state to retry buttons in `client/src/app/shared/components/loading/error-state.component.ts` — `retrying()` + `[disabled]` + `.btn-spinner` (depends on T020) (FR-004, FR-005)
- [X] T017 [US2] Verify US2 acceptance via quickstart Scenario 2 — every operation shows a busy state at the point of action, disables the trigger, and re-enables on completion or failure (no duplicate submissions)

**Checkpoint**: User Story 2 is fully functional and independently testable.

---

## Phase 4: User Story 4 — Flicker-Free & Accessible (US4)

**Goal**: No loader flashes for fast operations, reduced-motion users see static indicators, and assistive technology is announced loading state (FR-007, FR-008, FR-009).

**Independent Test**: Run operations completing in <200 ms (no flash), enable reduced-motion (static indicators), and use a screen reader (loading announced) via quickstart Scenario 4.

- [X] T018 [US4] Create `withLoadingState` in `client/src/app/shared/utils/loading.operator.ts` — `withLoadingState(loading: WritableSignal<boolean>, opts?: { deferMs?: number; minMs?: number }): OperatorFunction<T, T>` with defaults `deferMs = 200`, `minMs = 300`; loading never set if source emits within `deferMs` (FR-007, SC-005); held for at least `minMs` once shown (SC-001); forward value/error unchanged
- [X] T019 [US4] Wire data-screen loads through `withLoadingState` in `todo-list.component.ts`, `category-list.component.ts`, `todo-detail.component.ts`, `profile.component.ts` so fast loads never flash (depends on T018) (FR-007)
- [X] T020 [US4] Create `ErrorStateComponent` in `client/src/app/shared/components/loading/error-state.component.ts` — selector `app-error-state`, inputs `message`, `retryLabel` (default 'Retry'), `retrying` (busy), output `retry`; root `role="status"` (FR-005)
- [X] T021 [US4] Add `aria-busy` bindings to data-screen containers during `loading()`/`refreshing()` in `todo-list.component.ts`, `category-list.component.ts`, `todo-detail.component.ts`, `profile.component.ts`; audit `role="status"`/`aria-live` on all loaders (FR-009)
- [X] T022 [US4] Audit reduced-motion across all loaders — skeleton shimmer (`skeleton.component.ts`), `.btn-spinner` rotation (`styles.scss`), and `MatProgressBar` indeterminate animation render static under `prefers-reduced-motion` (FR-008)
- [X] T023 [US4] Verify US4 acceptance via quickstart Scenario 4 — no flash for ops under 200 ms; static indicators under reduced-motion; loading announced by screen reader; `aria-busy` bound

**Checkpoint**: User Story 4 is fully functional and independently testable.

---

## Phase 5: User Story 3 — Non-Blocking Background Refresh (US3)

**Goal**: Background refreshes (filter/page changes, post-edit reloads) keep existing content visible with a non-blocking indicator instead of re-showing a full skeleton (FR-006).

**Independent Test**: With data loaded, trigger a refresh via quickstart Scenario 3 — existing content stays visible and interactive, a slim indeterminate progress bar appears, and it disappears when the request finishes (or an error toast is shown, content preserved).

- [X] T024 [US3] Split `loading()`/`refreshing()` in `client/src/app/features/todos/todo-list/todo-list.component.ts` — skeleton only when no data (`loading()`); keep content + slim indeterminate `MatProgressBar` when data exists (`refreshing()`) (FR-006; depends on T018/T019)
- [X] T025 [P] [US3] Split `loading()`/`refreshing()` in `client/src/app/features/categories/category-list/category-list.component.ts` — skeleton only when no data; keep content + slim `MatProgressBar` when data exists (FR-006; depends on T018/T019)
- [X] T026 [US3] Add `takeUntilDestroyed()` to data-screen subscriptions in `todo-list.component.ts`, `category-list.component.ts`, `todo-detail.component.ts`, `profile.component.ts` so a pending load never applies its state to a destroyed screen (spec edge case: navigate away mid-request)
- [X] T027 [US3] Integrate `ErrorStateComponent` + retry in `todo-list.component.ts` and `category-list.component.ts` — show `app-error-state` with Retry when a first load fails with no data; on a failed refresh with data present, keep content and rely on the interceptor toast (depends on T020) (FR-005)
- [X] T028 [US3] Verify US3 acceptance via quickstart Scenario 3 — non-blocking indicator during refresh, content preserved, indicator disappears on completion; failed refresh keeps content + toast

**Checkpoint**: User Story 3 is fully functional and independently testable.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Tests, build verification, and documentation for all user stories

- [X] T029 [P] Unit tests for `SkeletonComponent`, `ErrorStateComponent`, and `withLoadingState` — `client/src/app/shared/components/loading/skeleton.component.spec.ts` (each variant renders, `role="status"` + `aria-live="polite"` present, reduced-motion static class), `client/src/app/shared/components/loading/error-state.component.spec.ts` (`retry` emitted on click, `retrying` disables button, message rendered), `client/src/app/shared/utils/loading.operator.spec.ts` (source emitting within `deferMs` never touches `loading`; `minMs` minimum display enforced; error path resets loading; Vitest fake timers)
- [X] T030 [P] Update screen specs for skeleton/refresh/retry states in `client/src/app/features/todos/todo-list/todo-list.component.spec.ts`, `client/src/app/features/categories/category-list/category-list.component.spec.ts`, `client/src/app/features/todos/todo-detail/todo-detail.component.spec.ts`, `client/src/app/features/auth/profile/profile.component.spec.ts` (Vitest)
- [X] T031 Verify production build succeeds (`ng build`) in `client/`
- [X] T032 Run the full test suite (`ng test`) in `client/` and confirm all tests pass
- [X] T033 Update `AGENTS.md` session summary with 005-loading-indicators implementation notes

---

## Dependencies

- T001, T002 → all phases (baseline + conventions)
- US1 (T003-T009) → independent of US2/US3/US4; T004 [P] parallel to T003 (same folder, different file)
- T005-T008 [P] parallel across 4 screens; T009 after all
- US2 (T010-T017): T010 before T013-T015 (global style reuse); T011/T012 [P] parallel; T016 depends on T020 (retry component); T017 after
- US4 (T018-T023): T018 → T019 (operator before wiring); T020 → T016 (retry button); T022 after T010/T018; T023 after
- US3 (T024-T028): T024/T025 [P] parallel; T026 independent; T027 depends on T020 (error component); T028 after
- T029, T030 [P] parallel test specs; T031-T033 → all phases complete

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: User Story 1 (skeleton system)
3. **STOP and VALIDATE**: Load each data screen on a throttled connection — skeletons mirror layout and resolve without shift (quickstart Scenario 1)
4. Deploy/demo if ready

### Incremental Delivery

1. Setup → shared loader infrastructure ready
2. Add US1 (skeleton loading) → test independently → Deploy/Demo (MVP)
3. Add US2 (busy states) → test independently → Deploy/Demo
4. Add US4 (flicker-free + accessible) → test independently → Deploy/Demo
5. Add US3 (background refresh) → test independently → Deploy/Demo
6. Polish: tests, build, test run, AGENTS.md

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup together
2. Once Setup is done:
   - Developer A: US1 (skeletons)
   - Developer B: US2 (busy states)
   - Developer C: US4 then US3 (flicker/a11y + refresh, sequential on shared screens)
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story is independently completable and testable via its quickstart Scenario
- Commit after each task or logical group (conventional commits, constitution Principle III)
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
- Verification tasks (T009, T017, T023, T028) run the matching quickstart Scenario and confirm acceptance criteria
