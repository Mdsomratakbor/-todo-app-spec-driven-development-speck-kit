# Data Model: Loading Indicators for All Operations

**Feature**: 005-add-loading-indicators
**Date**: 2026-08-11

## Scope

Client-only feature. **No database, entity, or persistence model changes.** This document defines the frontend UI-state shapes introduced or affected by the feature.

## Type: SkeletonVariant

Defined in `client/src/app/shared/components/loading/skeleton.component.ts`.

| Variant | Screen | Placeholder shapes mirroring real content |
|---------|--------|--------------------------------------------|
| `todo-list` | `/todos` | Todo-card rows: title line, description line, chip row (priority/category/status), action-button stub |
| `category-list` | `/categories` | Category-card rows: color-dot + name line, todo-count subtitle line, action-button stub |
| `detail` | `/todos/:id` | Back button stub, title bar, body text block, chip row |
| `profile` | `/profile` | Centered profile card: title, field lines (email/role/date), action stub |
| `rows` | generic fallback | Repeated card rows (replaces current `LoadingSpinnerComponent` skeleton branch usage) |

### API

```
variant: input<SkeletonVariant>                 // default 'rows'
count:  input<number>                           // number of placeholder rows (default 3)
label:  input<string>                           // announced text, e.g. 'Loading todos...'
```

Root element: `role="status"` + `aria-live="polite"`; shimmer applied only under `@media (prefers-reduced-motion: no-preference)` (FR-008).

## Type: LoadingState (screen-level signals)

Per data screen (todo-list, category-list, todo-detail, profile):

| Signal | Type | Meaning |
|--------|------|---------|
| `loading` | `WritableSignal<boolean>` | First-load pending, **no data on screen** → render `app-skeleton` |
| `refreshing` | `WritableSignal<boolean>` | Background refresh pending, **data present** → keep content + slim `MatProgressBar` (FR-006) |
| `error` | `WritableSignal<boolean>` | First load failed with no data → render `app-error-state` (retry) (FR-005) |
| `deletingId` | `WritableSignal<string \| undefined>` | Id of item being deleted (lists) → busy state on that row's delete button (FR-004) |

### Invariants

- `loading()` and `refreshing()` are mutually exclusive per request: `refreshing` is set only when the screen already holds data; `loading` only when it holds none.
- `loading()`/`refreshing()` are driven exclusively by the `withLoadingState` operator (T018-T019), never set manually.
- `error()` is set only when `loading()` completes with failure and no data exists.
- `deletingId` is set to the target id on delete start and cleared in both success and error handlers.

## Operator: withLoadingState

Defined in `client/src/app/shared/utils/loading.operator.ts`.

```
withLoadingState<T>(
  loading: WritableSignal<boolean>,
  opts?: { deferMs?: number; minMs?: number },
): OperatorFunction<T, T>
```

| Option | Default | Behavior |
|--------|---------|----------|
| `deferMs` | `200` | If the source emits (value or error) before this delay, the `loading` signal is never touched → no flicker (SC-005) |
| `minMs` | `300` | Once visible, `loading` stays `true` for at least this long (no one-frame flash) |

- On source emission: signal set `false` (respecting `minMs`); downstream value/error propagated unchanged.
- `loading` is set `true` only when `deferMs` elapses while the source is still pending.

## State: Busy Buttons

| Operation | Component | Busy mechanism |
|-----------|-----------|----------------|
| Submit/Save (create/update todo, category) | todo-form, category-form | `saving()` + `[disabled]="saving() \|\| form.invalid"` (existing, standardized) |
| Login / Register | login, register | `loading()` + `[disabled]` (existing, standardized) |
| Delete todo | todo-list (via todo-card) | `deletingId` passed to card; delete button disabled + mini spinner while `deletingId === todo.id` |
| Delete category | category-list (via category-card) | same `deletingId` pattern |
| Logout (toolbar) | app.ts/app.html | busy signal + `[disabled]` + `.btn-spinner` |
| Logout (profile) | profile | busy signal + `[disabled]` + `.btn-spinner` |
| Apply / Clear filters | filter-bar | `[disabled]` bound to in-flight list request |
| Retry | error-state | busy signal + `[disabled]` while retry in flight |

All use the global `.btn-spinner` (`sync` icon + `spin`, rotation disabled under reduced motion) defined in `client/src/styles.scss`.

## Relationships

```
SkeletonComponent ──renders──> layout-shaped placeholder markup (Material CSS vars)
withLoadingState ──drives──> loading()/refreshing() signals in todo-list, category-list, todo-detail, profile
ErrorStateComponent ──emits──> retry() ──re-runs──> load method
data screens ──bind──> aria-busy on container while loading()/refreshing()
todo-list/category-list ──passes──> deletingId ──into──> todo-card/category-card
errorInterceptor (004) ──owns──> error toasts; ErrorStateComponent ──only for──> failed first load with no data
```

## Validation Rules

| Rule | Enforcement Layer |
|------|-------------------|
| Loader appears within 200 ms for pending ops (SC-001) | `withLoadingState` `deferMs = 200` |
| No loader flash for ops completing under threshold (SC-005) | `withLoadingState` defer, never shows if source emits early |
| No skeleton when content already present (US1·A3) | `refreshing()` branch keeps content; skeleton only when no data |
| Duplicate submissions prevented (SC-004) | `[disabled]` + `deletingId` busy states on every wait-point |
| Reduced motion → static indicators (FR-008) | `@media (prefers-reduced-motion: no-preference)` on shimmer/spin/progress-bar |
| Loading announced to AT (FR-009) | `role="status"` + `aria-live="polite"` on loaders; `aria-busy` on containers |
| No stale loading state after navigation (edge case) | `takeUntilDestroyed()` on screen subscriptions |
