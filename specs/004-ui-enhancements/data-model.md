# Data Model: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Date**: 2026-08-03

## Scope

Client-only feature. **No database, entity, or persistence model changes.** This document defines the frontend data/state shapes introduced or affected by the feature.

## Type: NotificationSeverity

Defined in `client/src/app/shared/services/notification.service.ts`.

| Value | Visual Style | Toast Duration | Icon |
|-------|--------------|----------------|------|
| `success` | Green container (#2e7d32) | 4s | `check_circle` |
| `warning` | Amber container (#e65100) | 6s | `warning` |
| `error` | Red container (#c62828) | 8s | `error` |

### API

```
show(message: string, severity: NotificationSeverity = 'success'): void
success(message: string): void
error(message: string): void
warning(message: string): void
```

Panel class emitted: `snackbar-{severity}` (used by global styles).

## Type: HTTP Error Message Map

Defined in `client/src/app/shared/interceptors/error.interceptor.ts`.

| Key (HTTP status) | Message |
|-------------------|---------|
| `400` | "Please check your input and try again." |
| `403` | "You don't have permission to perform this action." |
| `404` | "The requested resource was not found." |
| `409` | "A conflict occurred. Please try again." |
| `429` | "Too many requests. Please try again later." |
| `500` | "An unexpected error occurred. Please try again later." |
| `0` (network) | "A network error occurred. Please check your connection." |
| any other | "An unexpected error occurred. Please try again later." |

### Precedence

1. `error.error?.detail` (server-provided message, e.g., FluentResponse envelope) — used when present
2. Status map above
3. Network vs. fallback message

### Routing Rules (clarifications Q2/Q3)

- URL includes `/auth/` → no global toast (component owns messaging)
- Status `401` + non-auth URL → session-expired toast + auth interceptor redirects

## State: Loading Flag (per form)

| Form | Signal | Disabled Expression |
|------|--------|---------------------|
| login | `loading = signal(false)` | `[disabled]="loading() \|\| loginForm.invalid"` |
| register | `loading = signal(false)` | `[disabled]="loading() \|\| registerForm.invalid"` |
| todo-form | `saving = signal(false)` | `[disabled]="saving() \|\| form.invalid"` |
| category-form | `saving = signal(false)` | `[disabled]="saving() \|\| form.invalid"` |

Button label while loading: "Saving..." (todo/category forms), "Signing in..." (login/register).

## Relationships

```
NotificationService ──uses──> MatSnackBar (panelClass: snackbar-{severity})
errorInterceptor ──uses──> NotificationService (error toasts)
authInterceptor ──uses──> Router (401 redirect on non-auth URLs)
app.html ──applies──> routeAnimation trigger (reduced-motion aware)
todo-card / category-card ──CSS──> hover lift/scale (reduced-motion aware)
```

## Validation Rules

| Rule | Enforcement Layer |
|------|-------------------|
| Toast duration: success 4s, warning 6s, error 8s | `NotificationService` constants |
| `/auth/` endpoints: no global toast | `errorInterceptor` URL check |
| Non-auth 401: session-expired toast + redirect | `errorInterceptor` + `authInterceptor` |
| Reduced motion: no animation (route, card transform, spinner rotation) | CSS `@media (prefers-reduced-motion: no-preference)` + animation guard |
| Double-submission prevention | `[disabled]` bound to loading/saving signal |
