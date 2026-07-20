# Task List: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Generated**: 2026-07-20
**Total Tasks**: 15
**User Stories**: 5 (US1-US5)
**Platform**: Client (Angular 21)

---

## Phase 1: Toast/Snackbar Styling (US1)

- [ ] T001 Add snackbar severity styles to `client/src/styles.scss`
- [ ] T002 Add snackbar slide-in animation in `client/src/styles.scss`

---

## Phase 2: Global Error Interceptor (US2)

- [ ] T003 Create `client/src/app/shared/interceptors/error.interceptor.ts`
- [ ] T004 Register error interceptor in `client/src/app/app.config.ts`

---

## Phase 3: Route Transition Animations (US3)

- [ ] T005 Create `client/src/app/shared/animations/route.animations.ts`
- [ ] T006 Apply route animations to `client/src/app/app.html`

---

## Phase 4: Card Interaction Effects (US4)

- [ ] T007 Add hover/lift effects to `client/src/app/features/todos/todo-card/todo-card.component.ts`
- [ ] T008 Add hover/lift effects to `client/src/app/features/categories/category-card/category-card.component.ts`

---

## Phase 5: Button Loading States (US5)

- [ ] T009 Add loading spinner to login form in `client/src/app/features/auth/login/login.component.ts`
- [ ] T010 Add loading spinner to register form in `client/src/app/features/auth/register/register.component.ts`
- [ ] T011 Add loading spinner to todo-form in `client/src/app/features/todos/todo-form/todo-form.component.ts`
- [ ] T012 Add loading spinner to category-form in `client/src/app/features/categories/category-form/category-form.component.ts`

---

## Phase 6: Polish & Verification

- [ ] T013 Verify build (`npm run build` or `ng build`)
- [ ] T014 Run tests (`npm test`)
- [ ] T015 Update AGENTS.md

---

## Dependencies

```
Phase 1 (Toast Styling)
    │
    ▼
Phase 2 (Error Interceptor) ──→ Phase 3 (Route Animations) ──→ Phase 4 (Card Effects) ──→ Phase 5 (Button States)
                                                                                                    │
                                                                                                    ▼
                                                                                            Phase 6 (Polish & Verify)
```

**Parallel Opportunities**: Phases 3, 4, and 5 are independent and can be done in any order after Phase 1.

---

## Task Count Summary

| Phase | Tasks | User Story | Platform |
|-------|-------|------------|----------|
| Phase 1: Toast/Snackbar Styling | 2 | US1 | Client |
| Phase 2: Global Error Interceptor | 2 | US2 | Client |
| Phase 3: Route Transition Animations | 2 | US3 | Client |
| Phase 4: Card Interaction Effects | 2 | US4 | Client |
| Phase 5: Button Loading States | 4 | US5 | Client |
| Phase 6: Polish & Verification | 3 | — | Client |
| **Total** | **15** | **5** | **Client** |
