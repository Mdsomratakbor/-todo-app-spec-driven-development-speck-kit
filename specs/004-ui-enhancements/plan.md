# Implementation Plan: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Date**: 2026-07-20 (updated 2026-08-03)
**Status**: Ready for Implementation

---

## Technical Context

| Item | Value |
|------|-------|
| Feature Directory | `specs/004-ui-enhancements/` |
| Spec File | `specs/004-ui-enhancements/spec.md` |
| Implementation Plan | `specs/004-ui-enhancements/plan.md` |
| Data Model | `specs/004-ui-enhancements/data-model.md` |
| Research | `specs/004-ui-enhancements/research.md` |
| Contracts | `specs/004-ui-enhancements/contracts/ui-contracts.md` |
| Quickstart | `specs/004-ui-enhancements/quickstart.md` |
| Branch | `004-ui-enhancements` |

**Platform**: Client-only feature (Angular 21 SPA, Angular Material, Bootstrap). No backend/API changes.

**Language/Framework**: Angular 21 standalone components, TypeScript, Angular Material (MatSnackBar, MatIcon, MatCard), `@angular/animations`.

**Primary Dependencies**: `@angular/material` (snack-bar, card, icon, button, form-field), `@angular/platform-browser/animations` (route transitions).

**Storage**: N/A (no persistence).

**Testing**: Jasmine/Karma via `ng test`; build verification via `ng build`.

**Target Platform**: Modern browsers (Chrome, Edge, Firefox, Safari); `prefers-reduced-motion` media query supported.

**Project Type**: Web SPA frontend.

**Performance Goals**: Animations GPU-friendly (transform/opacity only), 200-300ms transitions, no perceived navigation delay.

**Constraints**: All animations respect `prefers-reduced-motion`; snackbar duration max 8s; no new third-party dependencies.

**Scale/Scope**: 30 tasks across 7 phases; touches global styles, interceptors, shared animations, and 10 components.

---

## Clarifications Integrated (2026-08-03)

From `/speckit.clarify` session (`spec.md` → `## Clarifications`):

1. **Toast durations**: success 4s, warning 6s, error 8s (error persists longest).
2. **Auth endpoint error ownership**: global interceptor suppresses toasts on `/auth/` endpoints; login/register components own all error messaging there.
3. **401 on non-auth URLs**: show "Your session has expired. Please log in again." toast **and** redirect to login.
4. **`prefers-reduced-motion` scope**: all motion disabled — route transitions render static, card hover scale/transform disabled, button spinner rotation disabled (static icon).
5. **Network vs. unexpected messages**: distinct messages — network (status 0) → "A network error occurred. Please check your connection."; 500/unexpected → "An unexpected error occurred. Please try again later."

These decisions are binding for task acceptance criteria.

---

## Constitution Check

| Principle | Status | Notes |
|-----------|--------|-------|
| Principle I: Architecture & Design First | ✅ PASS | Client-only feature; follows existing component-based architecture |
| Principle I: Notification standards | ✅ PASS | Uses MatSnackBar with severity styling per constitution |
| Principle II: Naming & Structure | ✅ PASS | New files under `client/src/app/shared/` kebab-case |
| Principle III: Git workflow | ✅ PASS | Feature branch `004-ui-enhancements`, conventional commits |
| Principle IV: Error handling | ✅ PASS | Centralized interceptor + component-level complement, no silent failures |
| Principle IV: Notifications | ✅ PASS | Severity colors red/amber/green per constitution |
| Principle IV: Validation | N/A | No new forms created; existing validation untouched |
| Principle IV: Testing | ✅ PASS | Unit tests via Jasmine for new services/interceptors where applicable |
| Principle V: Security | ✅ PASS | Auth interceptor 401 redirect preserved; no auth bypass |
| Principle VI: Task lifecycle | ✅ PASS | Tasks follow 13-step lifecycle during implementation |
| Definition of Done | ✅ PASS | Build passes, tests pass, no regressions |

**GATE**: PASS — no violations requiring justification.

---

## Current State Analysis

### Strengths (to preserve)
- `NotificationService` already exists with `success`/`error`/`warning` severity methods
- Auth interceptor already handles 401 redirects on non-auth URLs
- Loading states partially implemented in login/register
- Signal-based reactivity throughout (`signal`, `input`, `output`)
- `fadeSlideIn` CSS animation already used by `EmptyStateComponent`

### Gaps (to address)
- Snackbar CSS classes (`snackbar-success`, `snackbar-error`, `snackbar-warning`) referenced but not defined
- No centralized HTTP error interceptor; error handling duplicated across components
- Components show their own error toasts on non-`/auth/` failures — duplicates the interceptor toast (interaction model refactor in Phase 3)
- No route transition animations on `<router-outlet>`
- Card components (`todo-card`, `category-card`) lack hover/interaction states
- Button loading states inconsistent across login, register, todo-form, category-form
- No `prefers-reduced-motion` handling beyond none

### Implemented State (verified 2026-08-03)
- `styles.scss`: snackbar severity colors + Material Icon pseudo-elements, `fadeSlideIn` under `prefers-reduced-motion: no-preference`
- `error.interceptor.ts`: maps 400/403/404/409/429/500 + network; skips 401 (auth interceptor handles redirect)
- `route.animations.ts`: `routeAnimation` trigger, fade+slide 300ms, leave 200ms
- `app.html`: `[@routeAnimation]` on router-outlet wrapper
- `todo-card`/`category-card`: hover lift + shadow under `no-preference`
- login: spinner "Signing in...", disabled button

### Remaining Work vs. Clarifications
- **C1**: `NotificationService` uses 4s default / 6s warning — update to success 4s / warning 6s / error 8s
- **C2**: `error.interceptor.ts` must suppress toasts on `/auth/` endpoints (currently suppresses only 401 globally)
- **C3**: 401 on non-auth URLs must now also show the session-expired toast (currently silent redirect)
- **C4**: spinner rotation must be disabled under `prefers-reduced-motion` (currently always rotates)
- **C5**: network vs 500 messages already distinct — verify exact strings match spec

---

## Implementation Steps

### Phase 1: Setup
1. Verify Angular project builds and required dependencies present

### Phase 2: Toast/Snackbar Styling (US1)
1. Add global snackbar styles to `styles.scss` (severity colors via `--mdc-snackbar-container-color`, icons via Material Icon pseudo-elements, slide-in animation)
2. Verify all three severity levels have distinct visual styles
3. Update `NotificationService` durations: success 4s, warning 6s, error 8s
4. Ensure slide-in respects `prefers-reduced-motion` (no animation when reduced)

### Phase 3: Global Error Interceptor (US2)
1. Create `error.interceptor.ts` in `shared/interceptors/`
2. Map HTTP status codes to user-friendly messages (per Error Codes table in spec)
3. Skip toasts for requests whose URL includes `/auth/` (components own messaging)
4. On 401 for non-auth URLs: show session-expired toast; auth interceptor performs redirect
5. Preserve network (status 0) and 500 distinct messages
6. Register in `app.config.ts` after `authInterceptor`
7. Apply interaction model (spec §Error Handling Interaction Model): remove component-owned error toasts in todo-list, todo-detail, category-list — interceptor owns non-`/auth/` error toasts; components only reset loading state. profile is an `/auth/`-URL component (auth/me, auth/logout) and retains its own error toasts

### Phase 4: Route Transition Animations (US3)
1. Define `routeAnimation` trigger in `shared/animations/route.animations.ts`
2. Apply to `<router-outlet>` wrapper in `app.html`
3. Respect `prefers-reduced-motion` (render static, no animation)

### Phase 5: Card Interaction Effects (US4)
1. Update `todo-card.component.ts` styles: hover lift, shadow transition, scale
2. Update `category-card.component.ts` styles: same treatment
3. Disable transform under `prefers-reduced-motion`
4. Ensure consistent transition timing (200-300ms ease-out)

### Phase 6: Button Loading States (US5)
1. Ensure all form submit buttons show spinner + disabled state during loading
2. Consistent pattern across login, register, todo-form, category-form
3. Disable spinner rotation under `prefers-reduced-motion` (static icon)

### Phase 7: Polish & Verification
1. Add unit tests for `NotificationService` durations and `errorInterceptor` routing/messages
2. Verify build and run full test suite
3. Update AGENTS.md session summary

---

## Phases & Task Breakdown

See `tasks.md` for full task details (T001-T030).

### Phase 1: Setup
- T001 Verify Angular build (`ng build`)
- T002 [P] Confirm Angular Material + animations dependencies

### Phase 2: User Story 1 — Toast Notifications (US1) 🎯 MVP
- T003 [US1] Add snackbar severity styles + Material Icon pseudo-elements to `client/src/styles.scss`
- T004 [US1] Add snackbar slide-in animation (reduced-motion aware) to `client/src/styles.scss`
- T005 [P] [US1] Add per-severity durations (success 4s, warning 6s, error 8s) to `client/src/app/shared/services/notification.service.ts`
- T006 [US1] Verify US1 acceptance (quickstart Scenario 1)

### Phase 3: User Story 2 — Global Error Handling (US2)
- T007 [US2] Create `client/src/app/shared/interceptors/error.interceptor.ts` (status mapping, network/500 distinct messages)
- T008 [US2] Add `/auth/` suppression + 401 session-expired toast to error interceptor
- T009 [US2] Register error interceptor after auth interceptor in `client/src/app/app.config.ts`
- T010 [US2] Verify US2 acceptance (quickstart Scenario 2)
- T027-T030 [US2] Remove component-owned duplicate error toasts (interaction model) — todo-list, todo-detail, category-list, profile

### Phase 4: User Story 3 — Page Transitions (US3)
- T011 [US3] Define route animation trigger in `client/src/app/shared/animations/route.animations.ts`
- T012 [US3] Apply route animations to `client/src/app/app.html` + `client/src/app/app.ts`
- T013 [US3] Verify US3 acceptance (quickstart Scenario 3)

### Phase 5: User Story 4 — Interactive Cards (US4)
- T014 [P] [US4] Add hover/lift effects to `client/src/app/features/todos/todo-card/todo-card.component.ts`
- T015 [P] [US4] Add hover/lift effects to `client/src/app/features/categories/category-card/category-card.component.ts`
- T016 [US4] Verify US4 acceptance (quickstart Scenario 4)

### Phase 6: User Story 5 — Button Loading States (US5)
- T017-T020 [P] [US5] Add loading spinners to login, register, todo-form, category-form
- T021 [US5] Verify US5 acceptance (quickstart Scenario 5)

### Phase 7: Polish & Cross-Cutting
- T022 [P] Unit tests for `NotificationService` durations
- T023 [P] Unit tests for `errorInterceptor`
- T024 Verify build (`ng build`)
- T025 Run tests (`ng test`)
- T026 Update AGENTS.md session summary

---

## Dependencies

- T001, T002 → US1 implementation (build/deps baseline)
- US2 depends on US1 (error toasts reuse `snackbar-error` + `NotificationService`)
- US3, US4, US5 depend on Setup only (independently parallelizable)
- T003 → T004 sequential in `styles.scss`; T005 [P] parallel (different file)
- T007 → T008 → T009 sequential (same interceptor + registration)
- T027-T030 depend on T007-T009 (interceptor behavior + registration); parallel across 4 files
- T011 → T012 sequential (animation before template)
- T014/T015 parallel; T017-T020 parallel
- T022, T023 [P] parallel test specs
- T024-T026 → all phases complete

---

## Risk Mitigation

| Risk | Severity | Mitigation |
|------|----------|------------|
| Toast spam on rapid failures | Low | Toast replaces previous (MatSnackBar default); acceptable per spec |
| Auth interceptor vs error interceptor ordering | Medium | Register `errorInterceptor` after `authInterceptor`; `authInterceptor` handles redirect, `errorInterceptor` handles toasts |
| Duplicate error messages (component + global) | Medium | `/auth/` URL suppression (C2) prevents duplicates on login/register; interaction-model refactor (T027-T030) removes component error toasts on non-`/auth/` URLs |
| Reduced-motion regression | Low | All animations gated behind `prefers-reduced-motion: no-preference` |
| 401 double handling | Low | Clear ownership: auth interceptor = redirect + clear tokens; error interceptor = session-expired toast (non-auth URLs only) |

---

## Rollback Plan

If a change causes test failures or visual regressions:
1. `git checkout change/004-ui-enhancements -- <file>` to restore individual files
2. For interceptor issues, remove `errorInterceptor` from `withInterceptors` array in `app.config.ts`
3. For animation issues, remove `@routeAnimation` binding from `<router-outlet>`
4. For toast duration issues, revert `NotificationService` duration constants
