# Feature Specification: Frontend UI Enhancements

**Feature Branch**: `004-ui-enhancements`

**Created**: 2026-07-20

**Status**: Ready for Implementation (2026-08-03, quality gates passed)

**Input**: User description: "add new feature for frontend design improvement and make the UI more interactive and add toaster message for the error and success message"

---

## Context

### Feature Purpose

Improve the TodoApp frontend user experience through visual polish, interactive feedback, and better error/success communication. The existing app functions correctly but lacks visual polish: snackbar notifications have no styling, there is no centralized error handling, page transitions are instant without animation, and card components lack hover/interaction states.

### Business Objective

Create a polished, professional-grade user interface that provides clear feedback for all user actions, smooth transitions between views, and delightful interaction states. This improves user trust, reduces confusion during errors, and makes the application feel responsive and modern.

### Scope

- Styled snackbar/toast notifications with severity-specific colors and icons
- Centralized HTTP error interceptor with automatic toast on failures
- Page transition animations (fade + slide)
- Card hover/active/interaction effects
- Button loading states with spinners
- Global UI polish and consistency improvements

### Out of Scope

- Dark mode toggle (future enhancement)
- Drag-and-drop interactions
- Real-time updates / WebSocket
- Mobile-responsive redesign
- New feature pages or functionality

---

## Clarifications

### Session 2026-08-03

- Q: What should the auto-dismiss duration be for toast notifications per severity? → A: Success 4s, warning 6s, error 8s (error persists longest).
- Q: How should duplicate toasts be avoided on auth endpoints (login/register)? → A: Global interceptor suppresses toasts on `/auth/` endpoints; auth components own all error messaging there.
- Q: Should a 401 on a non-auth URL show a toast or only redirect silently? → A: Show "Your session has expired. Please log in again." toast and redirect to login.
- Q: What scope should `prefers-reduced-motion` cover? → A: All motion: route transitions render static, card hover scale/transform disabled, button spinner rotation disabled.
- Q: Should network and unexpected/500 errors share one message or use distinct messages? → A: Distinct: network → "A network error occurred. Please check your connection."; 500/unexpected → "An unexpected error occurred. Please try again later."

---

## Design Tokens

Single source of truth for visual values referenced by the user stories. Colors follow the constitution severity palette (green/amber/red).

| Token | Value | Used by |
|---|---|---|
| `--snackbar-success-bg` | `#2e7d32` | US1 success toast container color |
| `--snackbar-error-bg` | `#c62828` | US1 error toast container color |
| `--snackbar-warning-bg` | `#e65100` | US1 warning toast container color |
| Success icon | `check_circle` (Material Icons) | US1 success toast |
| Error icon | `error` (Material Icons) | US1 error toast |
| Warning icon | `warning` (Material Icons) | US1 warning toast |
| Toast slide-in | `translateX(100% → 0)`, 300ms `ease-out` | US1 |
| Card hover transform | `translateY(-2px)` | US4 |
| Card hover shadow | `0 4px 12px rgba(0,0,0,0.15)` | US4 |
| Card transition | `transform 0.2s ease-out, box-shadow 0.2s ease-out` | US4 |
| Route enter | fade 0→1 + `translateY(12px → 0)`, 300ms `ease-out` | US3 |
| Route leave | fade 1→0 + `translateY(0 → -4px)`, 200ms `ease-out` | US3 |
| Button spinner | `sync` Material Icon, 18px, `spin` 1s linear infinite | US5 |

## Error Handling Interaction Model

Defines ownership so components complement — never conflict with — the global interceptor (FR-002).

| Actor | Responsibility |
|---|---|
| `errorInterceptor` | Shows an error toast for every non-401 HTTP failure on non-`/auth/` URLs. **Suppresses** toasts on `/auth/` URLs. Always re-throws the error so component `subscribe` error handlers still execute. |
| `authInterceptor` | Owns the 401 redirect for non-`/auth/` URLs (clears tokens, navigates to `/login`). Registered before `errorInterceptor` in the chain. |
| Auth components (login, register, profile) | Own **all** error messaging on `/auth/` URLs (inline form errors and/or their own toast). The interceptor shows no toast for these requests. |
| Feature components (todo-list, todo-detail, category-list) | Must **not** show their own error toasts for HTTP failures on non-`/auth/` URLs (the interceptor already toasts). Their error handlers only reset loading state. Success toasts remain component-owned (the interceptor fires only on errors). |

Consequences:
- No duplicate toasts for the same failure.
- Failures are never silent (interceptor always toasts, except `/auth/` URLs where the component owns messaging).
- On `/auth/` URLs the interceptor still re-throws; the component decides the message.
- An HTTP response body `detail` field takes precedence over the status-code map; when absent, the status map applies.

## Toast Display Contract

| Property | Value |
|---|---|
| Position | Bottom-right (`horizontalPosition: 'end'`, `verticalPosition: 'bottom'`) |
| Stacking | Single visible toast — a new toast replaces the current one (MatSnackBar default) |
| Max-width | 480px; text wraps; container scales down on narrow viewports |
| Easing | Slide-in `translateX(100% → 0)` over 300ms `ease-out` |
| Dismiss | MatSnackBar "Close" action button or auto-dismiss after the per-severity duration |
| Duration | success 4s, warning 6s, error 8s |
| Route navigation | Toasts persist across route changes (rendered in the global overlay) |
| Form interaction | Auto-dismiss proceeds regardless of focus or typing |
| Accessibility | MatSnackBar renders inside a `polite` live region — toast text is announced to screen readers; the "Close" button is keyboard-accessible |
| Forced-colors / high-contrast | Color overrides are dropped; Material's default high-contrast snackbar rendering is preserved |

## Behavioral & Non-Functional Decisions

- **Network detection**: a network failure is detected by Angular `HttpErrorResponse.status === 0`.
- **Same-route navigation**: does not re-trigger the route animation (no-op).
- **Reduced-motion mid-session**: if `prefers-reduced-motion` is toggled while an animation is playing, the current animation completes; subsequent triggers render static (clarification Q4).
- **Dynamic card content**: if card content changes while hovered, the transition re-evaluates against the new layout; no special handling required.
- **Network recovery mid-request**: an already-shown toast stays for its duration; a successful retry is a new request and fires no error toast.
- **Performance**: animations use only `transform`/`opacity` (GPU-composited, ~60fps, no layout thrash).
- **Touch devices**: hover is unavailable; cards rely on existing tap navigation and the action-button ripple for feedback.
- **Browser support**: modern evergreen browsers (latest Chrome, Edge, Firefox, Safari) with CSS transitions and `prefers-reduced-motion` support.

## Requirement ID Scheme

Each user story maps to a functional requirement (FR-xxx) for traceability.

| FR | User Story | Title | Plan Phase |
|---|---|---|---|
| FR-001 | US1 | Toast Notifications | Phase 2 |
| FR-002 | US2 | Global Error Handling | Phase 3 |
| FR-003 | US3 | Page Transitions | Phase 4 |
| FR-004 | US4 | Interactive Cards | Phase 5 |
| FR-005 | US5 | Button Loading States | Phase 6 |

---

## User Stories

### US1: Toast Notifications [P1 — MVP] (FR-001)

> As a user, I want to see clearly styled success and error messages with icons so that I can quickly understand the outcome of my actions.

**Acceptance Criteria**:
- Success toasts show a green background (`#2e7d32`) with a `check_circle` icon
- Error toasts show a red background (`#c62828`) with an `error` icon
- Warning toasts show an amber background (`#e65100`) with a `warning` icon
- Toasts slide in from the right (`translateX(100% → 0)`, 300ms `ease-out`)
- Positioned bottom-right (`horizontalPosition: 'end'`, `verticalPosition: 'bottom'`); max-width 480px; a new toast replaces the current one
- Dismissible via the MatSnackBar "Close" action button or auto-dismiss after duration
- Auto-dismiss duration: success 4s, warning 6s, error 8s (error persists longest)
- Toasts persist across route changes and auto-dismiss regardless of form focus

### US2: Global Error Handling [P2] (FR-002)

> As a user, I want to see a toast notification whenever an HTTP request fails, even if the component forgets to handle errors, so that I never encounter a silent failure.

**Acceptance Criteria**:
- All HTTP errors (4xx, 5xx, network errors) on non-`/auth/` URLs show a toast notification
- 401 on non-auth URLs shows "Your session has expired. Please log in again." toast and redirects to login (auth interceptor owns redirect; error interceptor owns the toast)
- Global interceptor suppresses toasts on `/auth/` endpoints; auth components own all error messaging there
- Component-level error handling complements, rather than conflicts with, the global handler (see Error Handling Interaction Model)
- Network errors (status 0) show "A network error occurred. Please check your connection."
- Unexpected/500 errors show "An unexpected error occurred. Please try again later."
- Server `error.error?.detail` takes precedence over the status-code map; when absent the status map applies

### US3: Page Transitions [P3] (FR-003)

> As a user, I want smooth animated transitions when navigating between pages so that the app feels fluid and responsive.

**Acceptance Criteria**:
- Route changes trigger a fade-in + slide-up animation: enter fade 0→1 + `translateY(12px → 0)` over 300ms `ease-out`; leave fade 1→0 + `translateY(0 → -4px)` over 200ms `ease-out`
- Navigating to the same route is a no-op (animation not re-triggered)
- The animation is subtle and does not delay perceived navigation speed
- Animations respect the user's `prefers-reduced-motion` setting; route transitions render static (no animation) when reduced motion is enabled
- If reduced motion is toggled mid-animation, the current animation completes; subsequent transitions render static

### US4: Interactive Cards [P4] (FR-004)

> As a user, I want cards to respond visually when I hover or click so that the interface feels tactile and interactive.

**Acceptance Criteria**:
- Todo cards and category cards lift on hover: `transform: translateY(-2px)` + shadow `0 4px 12px rgba(0,0,0,0.15)`, transition `0.2s ease-out` (identical values for both card types)
- Click/tap feedback uses the Material ripple on the card's action buttons (no full-card ripple)
- Transitions are smooth (`0.2s ease-out`)
- Card hover transform is disabled when `prefers-reduced-motion` is enabled (lift/shadow may remain static)
- On touch devices hover does not apply; cards still navigate on tap via existing click handling

### US5: Button Loading States [P5] (FR-005)

> As a user, I want to see a loading indicator on submit buttons while the request is in progress so that I know the system is working and don't double-submit.

**Acceptance Criteria**:
- Submit buttons show a `sync` Material Icon spinner (18px, `spin` 1s linear infinite) plus "Signing in..." (login) / "Registering..." (register) / "Saving..." (todo, category) text while loading
- Buttons are disabled during loading via the HTML `disabled` attribute (`[disabled]="loading() || form.invalid"`) to prevent double-submission
- On request failure, the loading signal resets, the button re-enables, and the error toast comes from the error interceptor (or the auth component on `/auth/` URLs)
- Spinner rotation is disabled (static icon shown) when `prefers-reduced-motion` is enabled
- Loading state is managed with an Angular `signal()`, consistent with existing patterns
- Existing behavior already partially implemented (login/register show spinners); ensure consistency across all forms

---

## Error Codes

| HTTP Status | Error Code | Meaning | Toast Message |
|---|---|---|---|
| 400 | VALIDATION_ERROR | Validation failed | Server `error.error?.detail` when present; otherwise "Please check your input and try again." |
| 401 | UNAUTHORIZED | Not authenticated | "Your session has expired. Please log in again." (non-auth URLs) |
| 403 | FORBIDDEN | Insufficient permissions | "You don't have permission to perform this action." |
| 404 | NOT_FOUND | Resource not found | "The requested resource was not found." |
| 409 | CONFLICT | Resource conflict | Message from server |
| 429 | RATE_LIMIT_EXCEEDED | Too many requests | "Too many requests. Please try again later." |
| 0 | NETWORK_ERROR | Network failure | "A network error occurred. Please check your connection." |
| 500 | INTERNAL_ERROR | Server error | "An unexpected error occurred. Please try again later." |
