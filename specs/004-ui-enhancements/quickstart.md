# Quickstart: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Date**: 2026-08-03

## Prerequisites

- Node.js (Angular 21 compatible) and npm
- Clone of TodoApp repo with `client/` Angular workspace
- Backend API running (for auth flows and HTTP-error scenarios), or network-isolated mode for interceptor tests

## Setup

```bash
# 1. Install dependencies
cd client
npm install

# 2. Run the dev server
npm start
```

Open `http://localhost:4200`. The backend (if running) is expected at its configured proxy/URL.

## Validation Scenarios

### Scenario 1: Toast severity styling (US1)

1. Log in successfully → success toast (green background, `check_circle` icon, auto-dismisses ~4s)
2. Trigger a failed save (e.g., submit todo with server-side conflict) → error toast (red background, `error` icon, auto-dismisses ~8s)
3. Trigger a warning (if a flow surfaces one) → amber toast (`warning` icon, ~6s)
4. Click "Close" on a toast → dismisses immediately

Expected: all three severities are visually distinct with matching Material Icons and correct durations.

### Scenario 2: Global error handling (US2)

1. With backend stopped, attempt any list action → network error toast "A network error occurred. Please check your connection."
2. With backend running, request a non-existent resource (e.g., delete an already-deleted todo) → mapped status toast
3. Navigate to `/todos` while unauthenticated with an expired token → session-expired toast + redirect to login
4. Enter wrong credentials on login → login page shows inline/its own error message; **no duplicate global toast** (auth endpoint ownership)

### Scenario 3: Route transitions (US3)

1. Navigate between pages (e.g., Todos → Categories → Login) → fade-in + slide-up (~300ms)
2. Enable "prefers-reduced-motion" at OS/browser level → transitions render instantly/static, no animation

### Scenario 4: Card interactions (US4)

1. Hover a todo card and a category card → card lifts 2px with increased shadow, smooth 200ms transition
2. Click an action button → ripple-like feedback on the button
3. With reduced motion enabled → hover transform disabled (no lift/scale)

### Scenario 5: Button loading states (US5)

1. Click Login/Register submit → button shows spinning `sync` icon + "Signing in...", disabled
2. Click Todo/Category form submit → "Saving...", disabled, no double-submit possible
3. On request failure → spinner stops, button re-enables, error toast shown
4. With reduced motion enabled → static icon (no rotation)

## Run Tests

```bash
cd client
ng test            # unit tests (Jasmine/Karma)
ng build           # production build verification
```

Targeted: any new interceptor/service specs use Jasmine conventions in `client/src/app/**/*.spec.ts`.

## Contracts & Design

- UI contracts: [contracts/ui-contracts.md](contracts/ui-contracts.md)
- Frontend state/type shapes: [data-model.md](data-model.md)
- Full plan: [plan.md](plan.md)
