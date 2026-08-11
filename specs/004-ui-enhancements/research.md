# Research: Frontend UI Enhancements

**Feature**: 004-ui-enhancements
**Date**: 2026-08-03
**Status**: Complete

---

## 1. Snackbar Severity Styling

**Decision**: Style Angular Material `MatSnackBar` via global CSS classes (`snackbar-success`, `snackbar-error`, `snackbar-warning`) using Material 3 CSS variables (`--mdc-snackbar-container-color`) and Material Icon pseudo-elements.

**Rationale**:
- MatSnackBar in Material 3 exposes `--mdc-snackbar-container-color` for container color; overriding per panel class is the supported customization path
- Panel classes are set in `NotificationService.show()` via `panelClass: ['snackbar-<severity>']`
- Material Icon pseudo-elements (`::before` with `font-family: "Material Icons"`) avoid template changes in service — no DOM coupling
- Severity colors follow constitution: success green (#2e7d32), error red (#c62828), warning amber/orange (#e65100)

**Alternatives Considered**:
- Separate snackbar component per severity: overkill; panel class approach is idiomatic
- Inline styles in service: mixes styling into logic, harder to theme globally
- Icon inside template: would require custom snackbar component; pseudo-element is lighter

---

## 2. Toast Auto-Dismiss Durations

**Decision**: Success 4s, warning 6s, error 8s (clarification Q1).

**Rationale**:
- Errors require longer read time; success is transient feedback
- Warning between success and error
- MatSnackBar default options currently 3000ms in `app.config.ts`; per-call `duration` in `NotificationService` overrides it — service must set explicit per-severity durations

**Alternatives Considered**:
- Uniform duration: fails to prioritize error readability
- Errors only with "Close" (no auto-dismiss): forces interaction, worse UX for non-blocking failures
- Longer than 8s for errors: users perceive as stuck

---

## 3. Global HTTP Error Interceptor

**Decision**: Single `HttpInterceptorFn` (`errorInterceptor`) maps status codes to user-friendly messages and shows an error toast for unhandled failures.

**Rationale**:
- Centralizes error messaging, removing per-component duplication
- `catchError` re-throws the error so component-level `subscribe` error handlers still run (complements, not conflicts)
- Status 0 = network failure (Angular `HttpErrorResponse` convention) → distinct message (clarification Q5)
- 500/unexpected → "An unexpected error occurred. Please try again later."
- Reads `error.error?.detail` from FluentResponse-style envelopes when present, falling back to status-map

**Alternatives Considered**:
- Error handling only in components: inconsistent, easy to forget (the very gap this feature closes)
- Global `ErrorHandler`: catches uncaught errors but not HTTP responses; interceptor is the correct seam for HTTP failures

---

## 4. Auth Endpoint Ownership & 401 Handling

**Decision**: Error interceptor suppresses toasts on `/auth/` endpoints (clarification Q2); 401 on non-auth URLs shows session-expired toast while auth interceptor redirects (clarification Q3).

**Rationale**:
- Login/register already surface inline error messages and their own toasts (e.g., "Invalid email or password."); a global toast would duplicate
- Ownership split: auth interceptor = token clearing + redirect; error interceptor = session-expired toast for non-auth URLs
- 401 on `/auth/` (bad credentials) → no global toast; the login component handles it
- Prevents the "silent failure" US2 targets: expired session now explains itself

**Alternatives Considered**:
- Global toast on 401 everywhere: duplicates login/register messaging and confuses credential errors with session expiry
- Silent redirect (pre-clarification): silent failure, violates US2 intent
- Suppress all 401 toasts: loses session-expired feedback

---

## 5. Route Transition Animations

**Decision**: Shared `routeAnimation` trigger (`trigger('routeAnimation')`) in `shared/animations/route.animations.ts`, applied to the `<router-outlet>` wrapper; fade-in + slide-up 300ms for enter, 200ms fade-out for leave; gated by `prefers-reduced-motion: no-preference`.

**Rationale**:
- Angular animations on `query(':enter')`/`query(':leave')` are the idiomatic route-transition mechanism
- transform/opacity only → GPU-composited, avoids layout thrash (60fps)
- 300ms enter / 200ms leave stays within US3's ~300ms and does not delay perceived navigation
- Media-query guard makes reduced-motion render static (clarification Q4)

**Alternatives Considered**:
- CSS-only transitions on route components: harder to coordinate enter/leave; Angular trigger is declarative
- Per-component animation definitions: inconsistent; shared constant ensures standardization

---

## 6. Card Interaction Effects

**Decision**: CSS hover lift on `todo-card` and `category-card`: `transform: translateY(-2px)` + box-shadow, transition 0.2s ease-out, gated by `prefers-reduced-motion: no-preference`.

**Rationale**:
- MatCard is presentational; CSS transitions in component `styles` are sufficient (no extra libs)
- Small translate + shadow increase gives tactile feedback without layout shift
- 200ms ease-out matches US4 (200-300ms)
- Scale/transform disabled under reduced motion (clarification Q4)

**Alternatives Considered**:
- Material ripple on whole card: requires `MatRipple` directive wiring; hover lift already conveys interactivity; ripple kept on action buttons
- Larger translate (e.g., -4px): too strong, felt like layout jump

---

## 7. Button Loading States

**Decision**: Submit buttons show a `sync` Material Icon with a CSS `spin` animation plus "Saving..."/"Signing in..." text; `disabled` bound to a `loading()` signal; spinner rotation disabled under `prefers-reduced-motion` (static icon).

**Rationale**:
- Signal-based `loading()` flag is consistent with existing signal patterns in the app
- `[disabled]="loading() || form.invalid"` prevents double-submission (US5)
- Text + spinner keeps button width stable and communicates state clearly
- `@media (prefers-reduced-motion: no-preference)` guard stops rotation for reduced-motion users (clarification Q4)

**Alternatives Considered**:
- `mat-progress-spinner` inside button: heavier component, sizing issues inside `mat-button`
- Disabled state without spinner: no progress feedback
- Changing label only: insufficient feedback

---

## 8. Testing & Verification Strategy

**Decision**: Jasmine/Karma unit tests for `errorInterceptor` (status mapping, `/auth/` suppression, network/500 messages) and `NotificationService` durations; build verification via `ng build`; manual quickstart scenarios for visual effects.

**Rationale**:
- `provideHttpClient(withInterceptors([...]))` allows testing interceptors via `HttpClient` with `HttpTestingController`
- CSS/visual behaviors (snackbar colors, card hover, route animation) are verified via build + manual check in quickstart, consistent with prior frontend features
- Existing `.spec.ts` files use Jasmine; follow that convention

**Alternatives Considered**:
- Playwright E2E for every animation: disproportionate for polish feature
- No tests: constitution requires tests alongside changes

---

## Summary

All technical unknowns resolved. No new dependencies. Implementation leverages existing Angular Material primitives and signal patterns; every animation is gated behind `prefers-reduced-motion`, error messaging is centralized with clear auth-endpoint ownership, and all clarified decisions are mapped to concrete tasks in `plan.md`.
