# Quickstart: Loading Indicators for All Operations

**Feature**: 005-add-loading-indicators
**Date**: 2026-08-11

## Prerequisites

- Node.js (Angular 21 compatible) and npm
- Clone of TodoApp repo with `client/` Angular workspace
- Backend API running (required for realistic latency; a slow connection or DevTools throttling is useful to observe skeletons)

## Setup

```bash
# 1. Install dependencies
cd client
npm install

# 2. Run the dev server
npm start
```

Open `http://localhost:4200`. To observe loading states reliably, use DevTools → Network → throttling (e.g., "Slow 3G") or add a small artificial latency to the backend proxy.

## Validation Scenarios

### Scenario 1: Facebook-style skeleton loading on data screens (US1)

1. With throttling on, navigate to `/todos`, `/categories`, `/todos/:id`, `/profile`
2. Each screen shows a skeleton shaped like its real content (todo card rows, category rows, detail card, profile card) — not a blank page, spinner, or plain text
3. When the request completes, skeletons resolve into the real content **in the same positions** (no significant layout shift)
4. Navigate back to a screen whose data was already loaded → content appears immediately, **no skeleton** (cached/already-loaded rule)

### Scenario 2: Busy state on every user-triggered operation (US2)

1. Submit a todo/category form → button shows `.btn-spinner` + "Saving...", disabled; double-click cannot submit twice
2. Delete a todo/category → only that row's delete button shows a busy state and is disabled until the request settles
3. Click toolbar Logout or profile Logout → button busy + disabled during the request
4. Click filter Apply/Clear while a list request is in flight → disabled until it completes
5. Fail any operation (stop the backend) → busy state is removed, control re-enabled, error toast shown (interceptor)

### Scenario 3: Non-blocking background refresh (US3)

1. Load `/todos` (content visible), then change a filter or go to page 2
2. Existing content stays visible and interactive; a slim indeterminate progress bar appears above the list — no full-screen skeleton
3. When the request finishes, the bar disappears and the updated list is shown (or an error toast if it failed, content preserved)

### Scenario 4: Accessible and flicker-free loaders (US4)

1. Trigger a fast operation that completes in <200 ms (e.g., reload with no throttling) → **no loader flash** (verified by screen capture or review)
2. Trigger a load that takes >200 ms → indicator appears within 200 ms and stays visible for at least 300 ms
3. Enable "prefers-reduced-motion" at OS/browser level → shimmer/rotation/progress-bar animation is static; skeletons render as static blocks
4. Use a screen reader → "Loading {screen}..." is announced when a load starts; containers are `aria-busy`; content is announced when resolved

### Scenario 5: Error + retry (FR-005)

1. With the backend stopped, navigate to `/todos` with no data cached → an error state with a **Retry** button appears (plus the interceptor toast)
2. Start the backend and click Retry → button busy + disabled; on success the list loads; on failure the error state returns

## Run Tests

```bash
cd client
ng test      # unit tests (Vitest)
ng build     # production build verification
```

Targeted specs: `shared/components/loading/skeleton.component.spec.ts`, `shared/components/loading/error-state.component.spec.ts`, `shared/utils/loading.operator.spec.ts`, plus updated `todo-list`/`category-list`/`todo-detail`/`profile` screen specs.

## Contracts & Design

- UI contracts: [contracts/loading-contracts.md](contracts/loading-contracts.md)
- Frontend state/type shapes: [data-model.md](data-model.md)
- Research decisions: [research.md](research.md)
- Full plan: [plan.md](plan.md)
