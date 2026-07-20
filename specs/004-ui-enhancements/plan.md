# Implementation Plan: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Date**: 2026-07-20
**Status**: Ready for Implementation

---

## Technical Context

| Item | Value |
|------|-------|
| Feature Directory | `specs/004-ui-enhancements/` |
| Spec File | `specs/004-ui-enhancements/spec.md` |
| Implementation Plan | `specs/004-ui-enhancements/plan.md` |
| Task List | `specs/004-ui-enhancements/tasks.md` |
| Branch | `004-ui-enhancements` |

---

## Constitution Check

| Principle | Status | Notes |
|-----------|--------|-------|
| Principle I: Clean Architecture | ✅ PASS | Client-only feature; no API changes |
| Principle II: CQRS | ✅ PASS | Not applicable (no new backend commands/queries) |
| Principle III: RESTful API | ✅ PASS | Not applicable |
| Principle IV: Angular Patterns | ✅ PASS | Follows existing standalone component + inject patterns |
| Principle V: Separation of Concerns | ✅ PASS | Error interceptor separates HTTP error handling from component logic |

---

## Current State Analysis

### Strengths (to preserve)
- NotificationService already exists with severity levels
- Auth interceptor already handles 401 redirects
- Loading states partially implemented in login/register
- Animation (fadeSlideIn) already used in EmptyStateComponent
- Signal-based reactivity throughout

### Gaps (to address)
- Snackbar CSS classes (`snackbar-success`, `snackbar-error`, `snackbar-warning`) are referenced but not defined anywhere
- No centralized HTTP error interceptor; error handling duplicated across components
- No route transition animations
- Card components lack hover/interaction states
- Button loading states inconsistent across forms
- No `prefers-reduced-motion` support

---

## Implementation Steps

### Phase 1: Toast/Snackbar Styling
1. Add global snackbar styles to `styles.scss` (severity colors, icons via pseudo-elements, slide-in animation)
2. Verify all three severity levels have distinct visual styles

### Phase 2: Global Error Interceptor
1. Create `error.interceptor.ts` in `shared/interceptors/`
2. Map HTTP status codes to user-friendly messages
3. Show toast notifications for unhandled errors
4. Preserve existing 401 redirect behavior in auth interceptor
5. Register in `app.config.ts`

### Phase 3: Route Transition Animations
1. Define animation trigger in a shared constants/animation file
2. Apply to `<router-outlet>` in `app.html` via route animation trigger
3. Respect `prefers-reduced-motion`

### Phase 4: Card Interaction Effects
1. Update `todo-card.component.ts` styles: hover lift, shadow transition
2. Update `category-card.component.ts` styles: hover lift, shadow transition
3. Ensure consistent transition timing

### Phase 5: Button Loading States
1. Ensure all form submit buttons show spinner + disabled state during loading
2. Consistent pattern across login, register, todo-form, category-form

---

## Phases & Task Breakdown

### Phase 1: Toast/Snackbar Styling
- T001 Add snackbar severity styles to `client/src/styles.scss`
- T002 Add snackbar slide-in animation definition

### Phase 2: Global Error Interceptor
- T003 Create `client/src/app/shared/interceptors/error.interceptor.ts`
- T004 Register error interceptor in `client/src/app/app.config.ts`

### Phase 3: Route Transition Animations
- T005 Define route animation constants in `client/src/app/shared/animations/route.animations.ts`
- T006 Apply route animations to app component template

### Phase 4: Card Interaction Effects
- T007 Add hover/lift effects to TodoCardComponent
- T008 Add hover/lift effects to CategoryCardComponent

### Phase 5: Button Loading States
- T009 Add loading spinner to login form submit button
- T010 Add loading spinner to register form submit button
- T011 Add loading spinner to todo-form submit button
- T012 Add loading spinner to category-form submit button

### Phase 6: Polish & Verification
- T013 Verify build (`npm run build` or `ng build`)
- T014 Run tests (`npm test`)
- T015 Update AGENTS.md

---

## Rollback Plan

If a change causes test failures or visual regressions:
1. `git checkout change/004-ui-enhancements -- <file>` to restore individual files
2. For interceptor issues, remove `errorInterceptor` from `withInterceptors` array in `app.config.ts`
3. For animation issues, remove `@routeAnimation` binding from `<router-outlet>`
