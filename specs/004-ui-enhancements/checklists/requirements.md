# Requirements Checklist: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Created**: 2026-07-20

---

## Toast/Snackbar Styling (US1)

- [ ] CHK001 Success toast has green background
- [ ] CHK002 Error toast has red background
- [ ] CHK003 Warning toast has amber background
- [ ] CHK004 Toasts slide in from the right
- [ ] CHK005 Toasts are dismissible via Close button
- [ ] CHK006 Error toast shows a warning icon
- [ ] CHK007 Success toast shows a checkmark icon

## Global Error Interceptor (US2)

- [ ] CHK008 All HTTP errors show a toast notification
- [ ] CHK009 401 on non-auth URLs redirects to login (existing behavior preserved)
- [ ] CHK010 Network errors show "A network error occurred"
- [ ] CHK011 500 errors show "An unexpected error occurred"
- [ ] CHK012 Component error handling still works alongside global handler

## Route Transition Animations (US3)

- [ ] CHK013 Page transitions have fade-in + slide-up animation
- [ ] CHK014 Animation duration is ~300ms
- [ ] CHK015 Animation respects `prefers-reduced-motion`

## Interactive Cards (US4)

- [ ] CHK016 Todo cards have hover lift effect
- [ ] CHK017 Category cards have hover lift effect
- [ ] CHK018 Shadow increases smoothly on hover
- [ ] CHK019 Transitions are 200-300ms ease-out

## Button Loading States (US5)

- [ ] CHK020 Login button shows spinner when loading
- [ ] CHK021 Register button shows spinner when loading
- [ ] CHK022 Todo form button shows spinner when loading
- [ ] CHK023 Category form button shows spinner when loading
- [ ] CHK024 Buttons are disabled during loading
- [ ] CHK025 Double-submission is prevented during loading
