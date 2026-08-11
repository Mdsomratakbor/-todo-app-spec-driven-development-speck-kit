# UI Contracts: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Date**: 2026-08-03

These are the frontend interface contracts introduced or affected by this feature. There are no backend API changes.

## Contract 1: NotificationService API

File: `client/src/app/shared/services/notification.service.ts`

| Member | Signature | Behavior |
|--------|-----------|----------|
| `show` | `show(message: string, severity?: NotificationSeverity): void` | Opens snackbar with `panelClass: ['snackbar-{severity}']` and Close action |
| `success` | `success(message: string): void` | `show(message, 'success')`; 4s duration |
| `error` | `error(message: string): void` | `show(message, 'error')`; 8s duration |
| `warning` | `warning(message: string): void` | `show(message, 'warning')`; 6s duration |

**Contract rules**:
- Panel class string is always `snackbar-{success|error|warning}` — global styles in `styles.scss` depend on this exact naming
- Severity maps to duration: success 4s, warning 6s, error 8s (clarification Q1)
- Dismissible via MatSnackBar "Close" action button or auto-dismiss after duration
- Toast display contract (spec §Toast Display Contract): positioned bottom-right (`horizontalPosition: 'end'`, `verticalPosition: 'bottom'`); single visible toast — a new toast replaces the current one; max-width 480px; persists across route changes; auto-dismiss proceeds regardless of form focus

## Contract 2: errorInterceptor

File: `client/src/app/shared/interceptors/error.interceptor.ts`

| Behavior | Rule |
|----------|------|
| Status → message | 400, 403, 404, 409, 429, 500 mapped; status 0 = network error (spec §Behavioral & Non-Functional Decisions); server `detail` takes precedence; fallback = "Please check your input and try again." for 400 |
| `/auth/` URLs | No toast shown — auth components own error messaging (clarification Q2) |
| 401 non-auth | Show "Your session has expired. Please log in again." toast; do NOT clear tokens or redirect (auth interceptor owns redirect) |
| Propagation | Always re-throws `throwError` so component error handlers still run |
| Network | "A network error occurred. Please check your connection." |
| 500/unexpected | "An unexpected error occurred. Please try again later." (clarification Q5) |

**Interaction model** (spec §Error Handling Interaction Model): feature components (todo-list, todo-detail, category-list, profile) must NOT show their own error toasts for non-`/auth/` HTTP failures — the interceptor owns those; component error handlers only reset loading state. Auth components keep their own error messaging. Success toasts remain component-owned.

## Contract 3: Interceptor Ordering

File: `client/src/app/app.config.ts`

```ts
provideHttpClient(
  withFetch(),
  withInterceptors([authInterceptor, errorInterceptor]),
)
```

- `authInterceptor` must be registered before `errorInterceptor` (requests flow auth → error)
- Order guarantees: auth interceptor sees the 401 first for redirect/cleanup; error interceptor then shows the session-expired toast for non-auth URLs

## Contract 4: Route Animation Trigger

File: `client/src/app/shared/animations/route.animations.ts`

| Property | Value |
|----------|-------|
| Trigger name | `routeAnimation` |
| Enter | fade 0→1, translateY 12px→0, 300ms ease-out |
| Leave | fade 1→0, translateY 0→-4px, 200ms ease-out |
| Reduced motion | Animation disabled — components render statically (clarification Q4) |

Applied in `app.html`:

```html
<div [@routeAnimation]="getRouteAnimation()">
  <router-outlet />
</div>
```

## Contract 5: Snackbar CSS Classes

File: `client/src/styles.scss`

| Class | Container color | Icon (::before) |
|-------|-----------------|-----------------|
| `.snackbar-success` | `--mdc-snackbar-container-color: #2e7d32` | `check_circle` |
| `.snackbar-error` | `--mdc-snackbar-container-color: #c62828` | `error` |
| `.snackbar-warning` | `--mdc-snackbar-container-color: #e65100` | `warning` |

Slide-in / fade animation gated by `@media (prefers-reduced-motion: no-preference)`.

## Contract 6: Card Hover Effects

Files: `client/src/app/features/todos/todo-card/todo-card.component.ts`, `client/src/app/features/categories/category-card/category-card.component.ts`

| Property | Value |
|----------|-------|
| Hover transform | `translateY(-2px)` |
| Hover shadow | `0 4px 12px rgba(0,0,0,0.15)` |
| Transition | `transform 0.2s ease-out, box-shadow 0.2s ease-out` |
| Reduced motion | Transform/scale disabled; static (clarification Q4) |

## Contract 7: Button Loading States

Files: login, register, todo-form, category-form components

| Property | Value |
|----------|-------|
| Spinner | `MatIcon` `fontIcon="sync"`, 18px + CSS `spin` 1s linear infinite |
| Label | "Saving..." (todo/category), "Signing in..." (login), "Registering..." (register) |
| Disabled | `[disabled]="loading() \|\| form.invalid"` |
| Reduced motion | Spinner rotation disabled — static icon (clarification Q4) |
| Reset on error | `loading.set(false)` in error callback (button re-enables) |
