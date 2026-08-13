# Research: Loading Indicators for All Operations

**Feature**: 005-add-loading-indicators
**Date**: 2026-08-11
**Status**: Complete

All technical unknowns resolved from the live codebase (`client/src/app/**`). No new dependencies.

---

## 1. Skeleton Architecture (US1)

**Decision**: New dedicated `SkeletonComponent` (selector `app-skeleton`) under `client/src/app/shared/components/loading/` with layout variants that mirror real content: `todo-list` (card rows with title/description/chip lines), `category-list` (row cards with color-dot line), `detail` (title bar + body blocks), `profile` (centered card with field lines), and `rows` (generic fallback). `LoadingSpinnerComponent` stays for spinner-style point-of-action feedback.

**Rationale**:
- FR-003 requires skeletons to mirror position/size/shape of real content; a per-screen variant renders shapes that match the actual card/detail markup (todo-card, category-card, mat-card detail, profile card)
- Existing `LoadingSpinnerComponent` already has a skeleton branch, but it renders generic card shapes and is duplicated logic; a dedicated component keeps single responsibility and enables per-variant markup
- MatCard/Material CSS variables (`--mat-sys-surface`, `--mat-sys-outline-variant`) give placeholder shapes the same footprint as real content, minimizing layout shift (SC-003)
- Shimmer via `background-position` animation on gradient is the lightweight standard; gated by `prefers-reduced-motion: no-preference` (FR-008)

**Alternatives Considered**:
- Overload `LoadingSpinnerComponent` with a `variant` input: mixes spinner and skeleton concerns; fewer files but higher complexity per component (rejected — violates single responsibility)
- Use `ngx-skeleton-loader` third-party library: new dependency, no theming control over Material variables (rejected — no new dependencies constraint)
- Plain spinners on all screens (current state): does not meet "Facebook-style" requirement (rejected)

---

## 2. Flicker-Free Loading (FR-007 / SC-005)

**Decision**: Shared RxJS operator `withLoadingState(loading, { deferMs = 200, minMs = 300 })` in `client/src/app/shared/utils/loading.operator.ts`, applied via `pipe()` before `subscribe()` in data-screen load methods.

**Rationale**:
- Deferring the `loading` signal for 200 ms means ops that complete faster never display a loader (no one-frame flash) — satisfies SC-005 and the spec edge case "request completes so fast the loader would only flash for a frame"
- SC-001 requires an indicator within 200 ms of initiation; an op still pending at 200 ms gets the loader exactly then — both targets hold simultaneously (this resolves the apparent 200 ms vs 300 ms tension in the spec: defer 200 ms < assumption threshold 300 ms)
- `minMs = 300` enforces a minimum visible duration once shown, avoiding flicker for fast-but-not-instant responses
- Operator form composes with the existing signal + `.subscribe()` pattern used across the app; unit-testable with `TestScheduler`/fake timers (Vitest)

**Alternatives Considered**:
- Component-level `setTimeout` guards in each screen: duplicated logic, easy to get wrong, untestable in isolation (rejected)
- HTTP interceptor that delays/forwards loading state: leaks presentational concern into transport layer (rejected)
- Only minimum-display-time without defer: still flashes for sub-frame completions (rejected)

---

## 3. Non-Blocking Background Refresh (US3 / FR-006)

**Decision**: Data screens track two signals — `loading()` (no data yet → full skeleton) and `refreshing()` (data already present → keep content visible and show a slim indeterminate `MatProgressBar` above the list). Filter changes, page changes, and post-edit reloads are treated as background refreshes.

**Rationale**:
- Keeps the real content on screen during background ops so the user stays oriented (FR-006 non-blocking)
- `MatProgressBar` (indeterminate) is a native Material component already available (`@angular/material/progress-bar` v21.2.14) — no new dependency; it is a visually lightweight non-blocking indicator
- The skeleton only appears when there is no data to show (initial navigation to a screen), matching "cached or already-loaded content → no skeleton" (US1 acceptance 3)
- Distinguishing first-load from refresh also removes the current jarring re-skeleton on every filter/page change

**Alternatives Considered**:
- Keep full skeleton on every request (current behavior): blocks context and causes layout churn (rejected)
- Top-of-page overlay with spinner for all requests: heavier than needed; progress bar is subtler and Material-native (rejected)
- Silent refresh with no indicator: violates FR-006 ("user knows new data is on the way") (rejected)

---

## 4. Reduced Motion (FR-008)

**Decision**: All loader motion — skeleton shimmer, spinner rotation, progress-bar movement — gated behind `@media (prefers-reduced-motion: no-preference)`. Under reduced motion: static skeleton blocks, static spinner icon (no `spin`), and `MatProgressBar` rendered as a static determinate track or with animation disabled.

**Rationale**:
- Matches the established pattern from 004 (route animations, card hover, button spinners already gated)
- CSS media-query gating is a pure-styling solution; no JS feature detection or Angular animation-state plumbing needed
- Consistent with WCAG 2.3.3 and the constitution's accessibility emphasis

**Alternatives Considered**:
- Angular `@angular/animations` trigger to disable animation at runtime: heavier; CSS media query is the idiomatic reduced-motion seam (rejected)

---

## 5. Accessibility Announcements (FR-009)

**Decision**: Every loader root keeps `role="status"` + `aria-live="polite"` with an announced label ("Loading {screen}..."). Data-screen containers bind `aria-busy` while a request is pending. The new `SkeletonComponent`/`ErrorStateComponent` follow the same convention as the existing `LoadingSpinnerComponent` (already has `role="status" aria-live="polite"`).

**Rationale**:
- `role="status"` + `aria-live="polite"` is the supported way to announce non-intrusive async state changes to screen readers; "Loading..." is read, then the resolved content is read
- `aria-busy="true"` on the container tells AT not to announce intermediate updates while content settles (reduces announcement noise)
- Following the existing `LoadingSpinnerComponent` pattern guarantees consistency (FR-010) and reuses a proven convention

**Alternatives Considered**:
- `role="alert"`/`aria-live="assertive"`: interrupts the user for every load — too aggressive for routine loading (rejected)
- Visually-hidden text only with no live region: silent for AT (rejected — this is exactly what FR-009 forbids)

---

## 6. Busy States for Every Operation (US2 / FR-004)

**Decision**: Global `.btn-spinner` style + `spin` keyframes move to `client/src/styles.scss` (deduplicating the 4 per-component copies). Busy-state coverage is extended to: delete icon-buttons (todo-card, category-card) via a `deletingId` input signal from the parent list; toolbar Logout and profile Logout (spinner + `[disabled]`); filter-bar Apply/Clear (`[disabled]` bound to in-flight request); and retry buttons (same pattern).

**Rationale**:
- Centralizing spinner styles satisfies FR-010 (consistent styling/behavior) and removes duplication (constitution: code reuse before duplication)
- `deletingId` (a signal holding the id of the item being deleted) lets the parent own the network call while the card disables only that row's delete button — the point-of-action feedback FR-004 demands
- Every submit/save button already implements `[disabled]="saving() || form.invalid"` (004); extending the pattern to all remaining wait-points completes SC-004 (0 duplicate-created records)
- Buttons remain `[disabled]` during flight, so double-clicks cannot re-fire the same operation

**Alternatives Considered**:
- `mat-progress-spinner` inside every button: sizing issues within `mat-button`; the `sync` icon + text pattern is established (rejected)
- Per-component inline loader directives: overkill; plain disabled + spinner covers the requirement (rejected)

---

## 7. Error + Retry Surface (FR-005)

**Decision**: Error toasts stay owned by `errorInterceptor` (004). New shared `ErrorStateComponent` (icon + message + Retry button, `role="status"`) renders only when a **first load fails with no data**. If a background refresh fails while data is present, content is kept and the interceptor toast is the only signal.

**Rationale**:
- FR-005 requires the indicator be removed, the control re-enabled, and "an error notification MUST be shown with retry where appropriate" — retry is appropriate when there is nothing on screen to recover from
- When data is already visible, a full-screen error would destroy context for a transient failure; the toast suffices and the user can re-trigger via existing controls
- Reuses the interceptor for messaging (no duplication), and `ErrorStateComponent` gives a tangible retry path consistent with the empty-state component pattern

**Alternatives Considered**:
- Full-screen error state for every failure including refreshes: destroys context and overreacts (rejected)
- Retry only via toast action: `MatSnackBar` actions are small; a dedicated state is clearer for first-load failures (rejected)

---

## 8. Parallel Requests on One Screen (edge case)

**Decision**: The primary data request drives the skeleton. Secondary lookups (filter-bar statuses/priorities/categories, todo-form category dropdown) are non-blocking: they populate as they resolve and never block or re-trigger the screen skeleton.

**Rationale**:
- Filter-bar and form dropdowns load near-instantly from small endpoints/constants; gating the whole screen on them would add delay with no UX benefit
- Keeps the skeleton-to-content transition tied to the content the user actually navigated for (SC-002)
- Documented as the resolution of the spec edge case "categories + todos loading together" — the screen stays in loading state until all *required* data (primary content) arrives

**Alternatives Considered**:
- `combineLatest` on all requests before clearing the skeleton: over-synchronizes with negligible visual gain (rejected)

---

## 9. Navigation Away Mid-Request (edge case)

**Decision**: Data-screen subscriptions use `takeUntilDestroyed()` (Angular 21 `DestroyRef`) so pending loads are cancelled on navigation and never apply a loading state to a destroyed screen.

**Rationale**:
- Resolves the spec edge case "loading state when the user navigates away mid-request — must be cancelled, never applied to the wrong screen"
- `takeUntilDestroyed()` is the idiomatic Angular 21 replacement for manual `ngOnDestroy` unsubscriptions
- Prevents stray `loading.set(false)`/data updates on a destroyed component instance (memory/state leak)

**Alternatives Considered**:
- Manual `Subscription` tracking + `ngOnDestroy`: more boilerplate; `takeUntilDestroyed()` is declarative and standard (rejected)

---

## 10. Long-Running Operations (edge case)

**Decision**: No artificial timeout or false-failure on slow-but-eventual responses. Indicators persist (skeleton or busy state with clear messaging) until the request resolves.

**Rationale**:
- Resolves the spec edge case "slow-but-eventual responses, e.g., long-running imports — indicator persists with clear messaging; no false timeout"
- The error interceptor already surfaces genuine timeouts/failures as toasts; inventing a client-side timeout would add a false-failure path with no server contract to justify it

---

## 11. Testing & Verification Strategy

**Decision**: Vitest (`ng test`, `@angular/build:unit-test`) unit tests for `SkeletonComponent` (renders each variant, reduced-motion class, `role="status"`), `ErrorStateComponent` (Retry emission, busy state), and `withLoadingState` operator (defer suppresses sub-threshold ops, min display time enforced, error path resets). Screen specs (todo-list, category-list, todo-detail, profile) updated for skeleton/refresh/retry state transitions. Build verified via `ng build`.

**Rationale**:
- Project migrated from Jasmine/Karma to Vitest in 004; all `.spec.ts` use `vitest/globals` (project spec files reference `@angular/build:unit-test`; `provideHttpClientTesting()` + `provideNativeDateAdapter()` are established provider patterns)
- The operator is pure logic — ideal for timer-based unit tests (fake timers/`TestScheduler`)
- CSS/visual behaviors (shimmer appearance, layout mirroring, reduced-motion) verified via build + manual quickstart scenarios, consistent with prior frontend features
- Constitution requires tests alongside changes (Principle IV)

**Alternatives Considered**:
- Playwright E2E for every loading state: disproportionate for a presentational feature; quickstart manual checks suffice (rejected)

---

## Summary

All technical unknowns resolved. The design introduces two small shared presentational components (`SkeletonComponent`, `ErrorStateComponent`) and one RxJS utility operator (`withLoadingState`), all under `client/src/app/shared/`, plus a global `.btn-spinner` style. No new dependencies. Every motion is gated behind `prefers-reduced-motion`, announcements follow the existing `role="status"`/`aria-live="polite"` convention, and the `loading`/`refreshing` split plus `takeUntilDestroyed()` resolve the spec's edge cases. Decisions are mapped to tasks T001-T033 in `plan.md`.
