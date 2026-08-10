# Tasks: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Generated**: 2026-08-03 (updated 2026-08-03)
**Total Tasks**: 30
**User Stories**: 5 (US1-US5)
**Platform**: Client (Angular 21, Angular Material)

**Input**: Design documents from `specs/004-ui-enhancements/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/, quickstart.md

**Tests**: Included for new/modified services and interceptors (Jasmine/Karma per constitution). Tests are written per story where applicable.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Verify the client project baseline and confirm required dependencies before any UI work begins.

- [X] T001 Verify Angular project builds successfully (`ng build`) in `client/`
- [X] T002 [P] Confirm `@angular/material` (snack-bar, card, icon, button, form-field), `@angular/platform-browser/animations`, and Material Icons font are present in `client/package.json` and `client/src/index.html`

---

## Phase 2: User Story 1 - Toast Notifications (US1) 🎯 MVP

**Goal**: Deliver clearly styled severity toasts with icons, slide-in animation, and per-severity auto-dismiss durations.

**Independent Test**: Log in → success toast (green, `check_circle`, ~4s); trigger a failed save → error toast (red, `error`, ~8s); trigger a warning → amber toast (`warning`, ~6s). Click "Close" dismisses immediately. All per quickstart.md Scenario 1.

### Implementation for User Story 1

- [X] T003 [US1] Add snackbar severity container styles (`snackbar-success` #2e7d32, `snackbar-error` #c62828, `snackbar-warning` #e65100) plus Material Icon pseudo-element (`::before` with `font-family: "Material Icons"`) for each severity to `client/src/styles.scss`
- [X] T004 [US1] Add snackbar slide-in animation in `client/src/styles.scss`, gated by `@media (prefers-reduced-motion: no-preference)`
- [X] T005 [P] [US1] Add per-severity auto-dismiss durations (success 4s, warning 6s, error 8s) and severity-based panel class selection to `client/src/app/shared/services/notification.service.ts`
- [X] T006 [US1] Verify US1 acceptance criteria (colors, icons, slide-in, Close button, durations) via `specs/004-ui-enhancements/quickstart.md` Scenario 1

**Checkpoint**: User Story 1 fully functional — toast styling is the shared foundation for US2 error toasts.

---

## Phase 3: User Story 2 - Global Error Handling (US2)

**Goal**: Centralized HTTP error interceptor that toasts every unhandled failure with correct routing rules.

**Independent Test**: Stop the backend → any list action shows network toast "A network error occurred. Please check your connection."; request a non-existent resource → mapped status toast; expired token on a non-auth URL → session-expired toast + redirect to login; bad credentials on login → inline/login-page message with NO duplicate global toast. Per quickstart.md Scenario 2.

### Implementation for User Story 2

- [X] T007 [US2] Create `errorInterceptor` with status→message map (400/403/404/409/429/500), server `error.error?.detail` precedence, distinct network (status 0) message "A network error occurred. Please check your connection." and fallback "An unexpected error occurred. Please try again later." in `client/src/app/shared/interceptors/error.interceptor.ts`
- [X] T008 [US2] Add `/auth/` URL suppression (no global toast; components own messaging) and 401-on-non-auth session-expired toast "Your session has expired. Please log in again." (re-throw, do not redirect — auth interceptor owns redirect) to `client/src/app/shared/interceptors/error.interceptor.ts`
- [X] T009 [US2] Register `errorInterceptor` after `authInterceptor` in `withInterceptors([...])` in `client/src/app/app.config.ts`
- [X] T010 [US2] Verify US2 acceptance criteria (all statuses toast, `/auth/` suppression, 401 toast + redirect, distinct messages) via quickstart.md Scenario 2

### Interaction Model Refactor (duplicate-toast elimination)

**Goal**: Apply the Error Handling Interaction Model (spec §Error Handling Interaction Model) — feature components stop showing their own error toasts for non-`/auth/` HTTP failures; the interceptor owns those toasts.

- [X] T027 [US2] Remove component-owned error toasts for non-`/auth/` failures in `client/src/app/features/todos/todo-list/todo-list.component.ts` (keep success toasts and `loading` reset; error toast comes from the interceptor)
- [X] T028 [US2] Remove component-owned error toasts for non-`/auth/` failures in `client/src/app/features/todos/todo-detail/todo-detail.component.ts` (keep `loading` reset)
- [X] T029 [US2] Remove component-owned error toasts for non-`/auth/` failures in `client/src/app/features/categories/category-list/category-list.component.ts` (keep success toasts and `loading` reset)
- [X] T030 [US2] Verify `client/src/app/features/auth/profile/profile.component.ts` error toasts are **retained** — its endpoints (`/api/v1/auth/me`, `/api/v1/auth/logout`) are `/auth/` URLs where the interceptor suppresses toasts, so the component owns error messaging (no duplicate)

**Checkpoint**: User Story 2 independently functional — no duplicate toasts, no silent failures.

---

## Phase 4: User Story 3 - Page Transitions (US3)

**Goal**: Smooth fade + slide route transitions that respect `prefers-reduced-motion`.

**Independent Test**: Navigate Todos → Categories → Login; each transition fades in + slides up (~300ms). Enable OS-level reduced motion → transitions render static instantly. Per quickstart.md Scenario 3.

### Implementation for User Story 3

- [X] T011 [US3] Create `routeAnimation` trigger (enter: fade 0→1 + translateY 12px→0, 300ms ease-out; leave: 200ms ease-out) in `client/src/app/shared/animations/route.animations.ts`
- [X] T012 [US3] Apply `@routeAnimation` binding to the `<router-outlet>` wrapper in `client/src/app/app.html` and register `animations: [routeAnimation]` in `client/src/app/app.ts`, gated by `prefers-reduced-motion` (render static when reduced)
- [X] T013 [US3] Verify US3 acceptance criteria (~300ms fade+slide, no perceived delay, reduced-motion static) via `specs/004-ui-enhancements/quickstart.md` Scenario 3

**Checkpoint**: User Story 3 independently functional.

---

## Phase 5: User Story 4 - Interactive Cards (US4)

**Goal**: Hover lift/scale feedback on todo and category cards with reduced-motion awareness.

**Independent Test**: Hover a todo card and a category card → 2px lift + increased shadow, 0.2s ease-out; click action button → ripple. With reduced motion → no transform. Per quickstart.md Scenario 4.

### Implementation for User Story 4

- [X] T014 [P] [US4] Add hover lift (`translateY(-2px)`) + shadow (`0 4px 12px rgba(0,0,0,0.15)`) with `transition: transform 0.2s ease-out, box-shadow 0.2s ease-out`, gated by `prefers-reduced-motion: no-preference`, to `client/src/app/features/todos/todo-card/todo-card.component.ts`
- [X] T015 [P] [US4] Add hover lift (`translateY(-2px)`) + shadow (`0 4px 12px rgba(0,0,0,0.15)`) with `transition: transform 0.2s ease-out, box-shadow 0.2s ease-out`, gated by `prefers-reduced-motion: no-preference`, to `client/src/app/features/categories/category-card/category-card.component.ts`
- [X] T016 [US4] Verify US4 acceptance criteria (both card types lift, smooth 200-300ms, reduced-motion static) via `specs/004-ui-enhancements/quickstart.md` Scenario 4

**Checkpoint**: User Story 4 independently functional.

---

## Phase 6: User Story 5 - Button Loading States (US5)

**Goal**: Consistent spinner + disabled state on all form submit buttons, with reduced-motion static icon.

**Independent Test**: Submit login/register → "Signing in..." with spinning `sync` icon, button disabled; submit todo/category forms → "Saving...", disabled; on failure spinner stops and button re-enables. With reduced motion → static icon. Per quickstart.md Scenario 5.

### Implementation for User Story 5

- [X] T017 [P] [US5] Add loading spinner (`MatIcon fontIcon="sync"` + CSS `spin`) with "Signing in..." label and `[disabled]="loading() || form.invalid"` to `client/src/app/features/auth/login/login.component.ts`
- [X] T018 [P] [US5] Add loading spinner (`MatIcon fontIcon="sync"` + CSS `spin`) with "Registering..." label and `[disabled]="loading() || form.invalid"` to `client/src/app/features/auth/register/register.component.ts`
- [X] T019 [P] [US5] Add loading spinner (`MatIcon fontIcon="sync"` + CSS `spin`) with "Saving..." label and `[disabled]="saving() || form.invalid"` to `client/src/app/features/todos/todo-form/todo-form.component.ts`
- [X] T020 [P] [US5] Add loading spinner (`MatIcon fontIcon="sync"` + CSS `spin`) with "Saving..." label and `[disabled]="saving() || form.invalid"` to `client/src/app/features/categories/category-form/category-form.component.ts`
- [X] T021 [US5] Verify US5 acceptance criteria (spinner + label on all four forms, disabled during load, no double-submit, reduced-motion static icon) via `specs/004-ui-enhancements/quickstart.md` Scenario 5

**Checkpoint**: User Story 5 independently functional.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Test coverage, build/test verification, and documentation updates across all stories.

### Tests (Jasmine/Karma)

- [X] T022 [P] Unit test `NotificationService` per-severity durations (success 4s, warning 6s, error 8s) and panel classes in `client/src/app/shared/services/notification.service.spec.ts`
- [X] T023 [P] Unit test `errorInterceptor` status→message mapping, `/auth/` URL suppression, 401 session-expired toast for non-auth URLs, and distinct network/500 messages in `client/src/app/shared/interceptors/error.interceptor.spec.ts`

### Verification & Documentation

- [X] T024 Verify production build succeeds (`ng build`) in `client/`
- [X] T025 Run unit tests (`ng test`) in `client/` and confirm no regressions
- [X] T026 Update `AGENTS.md` session summary for `004-ui-enhancements` (plan reference already points to `specs/004-ui-enhancements/plan.md`)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **US1 (Phase 2)**: Depends on Setup completion; delivers shared toast styling foundation
- **US2 (Phase 3)**: Depends on US1 (error toasts reuse `snackbar-error` styles + `NotificationService`); T027-T030 (duplicate-toast removal) depend on T007-T009 (interceptor behavior + registration)
- **US3 (Phase 4)**: Depends on Setup only
- **US4 (Phase 5)**: Depends on Setup only
- **US5 (Phase 6)**: Depends on Setup only
- **Polish (Phase 7)**: Depends on all user stories

### User Story Dependencies

- **User Story 1 (P1, MVP)**: No dependencies on other stories — blocks US2
- **User Story 2 (P2)**: Requires US1 toast styling; independently testable
- **User Story 3 (P3)**: Independent of other stories
- **User Story 4 (P4)**: Independent of other stories
- **User Story 5 (P5)**: Independent of other stories

### Within Each User Story

- Implementation tasks before verification task
- Same-file tasks are sequential (e.g., T003 → T004 in `styles.scss`)
- Verification task confirms story completeness before moving on

### Parallel Opportunities

- T002 [P] runs in parallel with T001
- T005 [P] (notification.service.ts) runs in parallel with T003/T004 (styles.scss)
- T014 [P] and T015 [P] (todo-card / category-card) run in parallel
- T017-T020 [P] (login, register, todo-form, category-form) all run in parallel
- T022 [P] and T023 [P] (test specs) run in parallel
- US3, US4, US5 can proceed in parallel after Setup (US2 additionally needs US1)

---

## Parallel Example: Button Loading States (US5)

```bash
# Launch all four form updates together (different files, no dependencies):
Task: "Add loading spinner to client/src/app/features/auth/login/login.component.ts"
Task: "Add loading spinner to client/src/app/features/auth/register/register.component.ts"
Task: "Add loading spinner to client/src/app/features/todos/todo-form/todo-form.component.ts"
Task: "Add loading spinner to client/src/app/features/categories/category-form/category-form.component.ts"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: User Story 1 (Toast Notifications) — shared styling foundation
3. **STOP and VALIDATE**: Verify US1 via quickstart.md Scenario 1
4. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup → foundation verified
2. Add US1 (toast styling) → test independently → MVP
3. Add US2 (error interceptor) → test independently → deploy/demo
4. Add US3 (page transitions) → test independently → deploy/demo
5. Add US4 (card effects) → test independently → deploy/demo
6. Add US5 (button states) → test independently → deploy/demo
7. Add Polish (tests + verification)

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup together
2. Once Setup is done:
   - Developer A: US1 → then US2 (US2 depends on US1)
   - Developer B: US3
   - Developer C: US4
   - Developer D: US5
3. Stories integrate independently; US2 lands after US1

---

## Task Count Summary

| Phase | Tasks | User Story | Count |
|-------|-------|------------|-------|
| Phase 1: Setup | T001-T002 | — | 2 |
| Phase 2: Toast Notifications | T003-T006 | US1 | 4 |
| Phase 3: Global Error Handling | T007-T010, T027-T030 | US2 | 8 |
| Phase 4: Page Transitions | T011-T013 | US3 | 3 |
| Phase 5: Interactive Cards | T014-T016 | US4 | 3 |
| Phase 6: Button Loading States | T017-T021 | US5 | 5 |
| Phase 7: Polish & Verification | T022-T026 | — | 5 |
| **Total** | **T001-T030** | **5** | **30** |

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story independently completable and testable via quickstart.md scenarios
- Commit after each task or logical group
- All animations gated behind `prefers-reduced-motion: no-preference` (clarification Q4)
- Toast durations fixed: success 4s, warning 6s, error 8s (clarification Q1)
- Error interceptor suppresses `/auth/` toasts (clarification Q2) and shows session-expired toast on non-auth 401 (clarification Q3)
- Distinct network vs. unexpected messages (clarification Q5)
