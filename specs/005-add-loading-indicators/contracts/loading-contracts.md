# UI Contracts: Loading Indicators for All Operations

**Feature**: 005-add-loading-indicators
**Date**: 2026-08-11

These are the frontend interface contracts introduced or affected by this feature. There are no backend/API changes.

## Contract 1: SkeletonComponent

File: `client/src/app/shared/components/loading/skeleton.component.ts`

| Member | Signature | Behavior |
|--------|-----------|----------|
| Selector | `app-skeleton` | Standalone component |
| `variant` | `input<SkeletonVariant>` (default `'rows'`) | `todo-list` \| `category-list` \| `detail` \| `profile` \| `rows`; shapes mirror real layout |
| `count` | `input<number>` (default `3`) | Number of placeholder rows/units |
| `label` | `input<string>` | Announced text via live region (e.g., "Loading todos...") |

**Contract rules**:
- Root element has `role="status"` + `aria-live="polite"`; screen readers announce `label` on entry (FR-009)
- Shimmer animation applied only under `@media (prefers-reduced-motion: no-preference)`; static blocks under reduced motion (FR-008)
- Placeholder shapes sized via Material CSS variables (`--mat-sys-*`) to match real content and avoid layout shift (FR-003)
- Renders content that occupies the same container/position as the resolved content — never shifts the layout on resolution

## Contract 2: withLoadingState operator

File: `client/src/app/shared/utils/loading.operator.ts`

```
withLoadingState<T>(
  loading: WritableSignal<boolean>,
  opts?: { deferMs?: number; minMs?: number },
): OperatorFunction<T, T>
```

| Rule | Value |
|------|-------|
| Default `deferMs` | `200` — source completing sooner never touches `loading` (no flicker, SC-005) |
| Default `minMs` | `300` — once shown, `loading` stays `true` for at least this long |
| Pending at `deferMs` | `loading.set(true)` — indicator appears within 200 ms (SC-001) |
| Emission (next/error/complete) | `loading` cleared respecting `minMs`; source event forwarded unchanged |
| Ownership | Screens must NOT set `loading()`/`refreshing()` manually; the operator is the single writer |

Applied as `observable.pipe(withLoadingState(this.loading))` before `.subscribe()`.

## Contract 3: ErrorStateComponent

File: `client/src/app/shared/components/loading/error-state.component.ts`

| Member | Signature | Behavior |
|--------|-----------|----------|
| Selector | `app-error-state` | Standalone component |
| `message` | `input<string>` | Failure text shown to the user |
| `retryLabel` | `input<string>` (default `'Retry'`) | Retry button label |
| `retry` | `output<void>()` | Emitted when Retry is clicked |
| `retrying` | `input<boolean>` | While true, Retry button shows `.btn-spinner` + `[disabled]` |

**Contract rules**:
- Shown **only** when a first load fails with no data present (FR-005 "retry where appropriate")
- When data is already present and a refresh fails: keep content, rely on `errorInterceptor` toast — no `ErrorStateComponent`
- Root has `role="status"` for AT announcement

## Contract 4: Screen loading-state contract

Applied to: todo-list, category-list, todo-detail, profile

| State | Signal | Rendered UI |
|-------|--------|-------------|
| First load pending (no data) | `loading() === true` | `app-skeleton` (variant for that screen) |
| Refresh pending (data present) | `refreshing() === true` | Existing content kept + slim indeterminate `MatProgressBar` above content (FR-006) |
| Loaded | `loading() === false && error() === false` | Real content |
| First-load failure (no data) | `error() === true` | `app-error-state` with Retry |
| Cached/already-loaded navigation | — | No skeleton; content renders immediately (US1·A3) |

**Container a11y**: data-screen container binds `aria-busy` while `loading()` or `refreshing()` is true (FR-009).

**Cancellation**: all screen subscriptions use `takeUntilDestroyed()` — a pending load never mutates a destroyed screen (spec edge case).

## Contract 5: Busy-state contract (point-of-action)

| Operation | Component | Rule |
|-----------|-----------|------|
| Submit/save | todo-form, category-form | `saving()` + `[disabled]="saving() \|\| form.invalid"`; `.btn-spinner` + "Saving..." (existing) |
| Login/Register | login, register | `loading()` + `[disabled]`; `.btn-spinner` + label (existing) |
| Delete todo | todo-list ↔ todo-card | Parent `deletingId`; card delete button disabled + mini `.btn-spinner` while `deletingId() === todo().id` |
| Delete category | category-list ↔ category-card | Same `deletingId` pattern |
| Logout | app.ts/app.html toolbar + profile | busy signal + `[disabled]` + `.btn-spinner` |
| Apply/Clear filters | filter-bar | `[disabled]` while a list request is in flight |
| Retry | error-state | `retrying()` + `[disabled]` + `.btn-spinner` |

**Contract rules**:
- Every user-triggered operation with a network wait shows a busy state at the point of action and disables the trigger (FR-004)
- Busy states clear in success **and** error handlers (control re-enables; FR-005)
- All spinner visuals use the single global `.btn-spinner` style (FR-010)

## Contract 6: Global loader styles

File: `client/src/styles.scss`

| Rule | Value |
|------|-------|
| `.btn-spinner` | inline-flex icon, 18px, margin-right 4px; `spin` keyframes 1s linear infinite — **only** under `@media (prefers-reduced-motion: no-preference)` |
| Shimmer (skeleton) | `background-position` gradient animation, 1.5s infinite — **only** under `prefers-reduced-motion: no-preference` |
| Progress-bar (refresh) | `MatProgressBar` indeterminate animation disabled under reduced motion |
| Per-component duplicates | Removed; single source in `styles.scss` |

## Contract 7: Feature boundaries

- **No backend/API/contract changes** (spec assumption): request latency is unchanged; loaders reflect existing latency
- **No new dependencies**: `MatProgressBar`, `MatProgressSpinner`, `MatIcon`, `MatButton` all from `@angular/material` v21.2.14
- **Existing routing transitions unchanged** (spec assumption)
- **Error messaging ownership**: toasts stay in `errorInterceptor` (004); this feature only adds the retry surface and busy states — no duplicate toasts
