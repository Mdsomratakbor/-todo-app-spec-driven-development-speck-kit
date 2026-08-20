<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
at specs/005-add-loading-indicators/plan.md.

## Session Summary (2026-07-06)

### Done
- **001-todo-management: All 74 tasks complete (Phases 1-6)**
  - Phase 6 (Polish & Cross-Cutting): T066-T074
  - 63/63 unit tests, 21/21 integration tests passing

- **002-lunch-setting Phase 1 (Setup):** T001-T004
  - `LunchPreference` entity, `DietaryRestriction` enum, domain + app repo interfaces

- **002-lunch-setting Phase 2 (Foundational):** T005-T011
  - EF config, AppDbContext update, SeedData, repository, DI registration, Cartographer mappings, DTOs

- **002-lunch-setting Phase 3 (US1):** T017-T024 (impl tasks only)
  - GetLunchPreferenceByUserQuery (get-or-create), CreateOrUpdateLunchPreferenceCommand (upsert), ResetLunchPreferenceCommand (org defaults: 12:00-13:00, 60min)
  - Validators for all commands/queries
  - LunchPreferencesController: GET/PUT/POST reset at `/api/v1/lunch-preferences/{userId}`

- **002-lunch-setting Phase 4 (US2):** T027-T030
  - Time range validation (end > start), break duration 15-120 min
  - Default schedule fallback in get-or-create (12:00-13:00, 60min)
  - Cleared-time-fields handling (null → org defaults)

- **002-lunch-setting Phase 5 (US3):** T034-T036
  - Favorites/exclusions size validation (max 20, non-empty, trimmed)
  - Dietary restrictions max 10 items
  - Duplicate detection in handler → 409 Conflict

- **002-lunch-setting Phase 6 (Polish):** T037-T044
  - EF Core migration `AddLunchPreferences` created
  - Build: 0 errors, 63/63 unit, 21/21 integration tests passing

### Not Requested (Optional)
- Phase 3 tests (T012-T016), Phase 4 tests (T025-T026), Phase 5 tests (T031-T033), Phase 6 integration tests (T039-T042)

### Key Context
- Cartographer.Mapper → `Cartographer.Core.Abstractions` / `Cartographer.Core.DependencyInjection`
- Inline mapping in `DependencyInjection.cs` (no separate profile files)
- Integration tests use `CustomWebApplicationFactory` with InMemory DB + JWT bypass
- Validated dietary restrictions list cached in validator static field
- Reset sets: lunch window 12:00-13:00, 60min break, notifications enabled, empty lists
- Duplicate detection uses case-insensitive HashSet comparison
- EF migration at `Data/Migrations/20260706172927_AddLunchPreferences.cs`

## Session Summary (2026-07-15)

### Done
- **003-basic-auth: Specification and Plan Complete**
  - Feature spec: `specs/003-basic-auth/spec.md`
  - Implementation plan: `specs/003-basic-auth/plan.md`
  - Research: `specs/003-basic-auth/research.md`
  - Data model: `specs/003-basic-auth/data-model.md`
  - API contracts: `specs/003-basic-auth/contracts/auth-api.md`
  - Quickstart guide: `specs/003-basic-auth/quickstart.md`
  - Quality checklist: `specs/003-basic-auth/checklists/requirements.md` (16/16 passing)
  - 5 clarifications resolved: rate limiting, token rotation, logout scope, error messages, JWT claims
  - 50 implementation tasks defined across 8 phases

### Key Context
- User entity: Id, Email, HashedPassword, Role, CreatedAt, UpdatedAt
- RefreshToken entity: Id, UserId, TokenHash, ExpiresAt, RevokedAt, CreatedAt
- Password hashing: BCrypt.Net with cost factor 12
- JWT claims: User ID, Email, Role
- Refresh tokens: Opaque, stored as SHA256 hashes, 7-day expiry, strict rotation
- Logout: Revokes only current session's refresh token
- Rate limiting: 100 req/min per IP on auth endpoints
- Error messages: Distinct for expired vs. missing tokens

## Session Summary (2026-07-20)

### Done
- **004-ui-enhancements: All 15 tasks complete (Phases 1-6)**
  - Phase 1: Snackbar toast styles with severity colors + Material Icons (styles.scss)
  - Phase 2: Error interceptor with user-friendly HTTP error messages
  - Phase 3: Route transition animations (fade + slide, respects prefers-reduced-motion)
  - Phase 4: Card hover/lift effects on todo-card and category-card
  - Phase 5: Button loading spinners on login, register, todo-form, category-form
  - Phase 6: Build verified (`ng build` succeeds)

### Key Context
- NotificationService: snackbar panel classes `snackbar-success/error/warning` now styled with green/red/amber + Material Icons
- ErrorInterceptor: catches all non-401 HTTP errors, maps status codes to messages, displays via NotificationService
- Route animations: `route.animations.ts` with `fadeSlideIn` trigger, applied via `[@routeAnimation]` in app.html
- Card hover: `transform: translateY(-2px)` + `box-shadow` transition on todo-card and category-card
- Button spinners: `MatIcon` with `fontIcon="sync"` + CSS `spin` animation, shown during `saving()`/`loading()` states
- Spec at `specs/004-ui-enhancements/` with spec.md, plan.md, tasks.md, checklists/requirements.md
- Pre-existing test issues (spec files using `NoopAnimations`/`spyOn` from Jasmine) — not caused by this feature

## Session Summary (2026-08-03)

### Done
- **004-ui-enhancements: All 30 tasks complete (Phases 1-7)** — full implementation via `/speckit.implement`
  - US1 Toasts: severity classes + Material Icons, `snackbarSlideIn` animation, per-severity durations (success 4s/warning 6s/error 8s)
  - US2 Error handling: `errorInterceptor` with status map, `/auth/` suppression, 401 session-expired toast; duplicate component error toasts removed from todo-list/todo-detail/category-list; profile retains own toasts (`/auth/` URLs)
  - US3 Route transitions: `route.animations.ts` fadeSlideIn, applied in app.ts/app.html
  - US4 Card hover: todo-card + category-card lift/shadow
  - US5 Button spinners: login, register, todo-form, category-form
  - Phase 7: `ng build` OK, **40/40 unit tests passing**, AGENTS.md updated
- **Test infra migration: Jasmine → Vitest** (project uses `@angular/build:unit-test` + `vitest/globals`, no karma.conf.js)
  - Fixed 6 pre-existing specs: removed `NoopAnimations` (not exported in Angular 21), `spyOn` → `vi.spyOn`, `toBeTrue/toBeFalse` → `toBe(true/false)`, `null` → `undefined` model fields
  - Added DI providers: `provideNativeDateAdapter()` (MatDatepicker), `provideAnimations()` (app.spec `@routeAnimation`), `provideHttpClientTesting()`, `provideRouter([])`
  - Fixed setInput-timing bug in patch-value tests (recreate fixture before setInput so ngOnInit runs with input set)

### Key Context
- Vitest globals configured in `client/tsconfig.spec.json`; `ng test` runs Vitest not Karma
- `provideNativeDateAdapter()` needed for any spec rendering `MatDatepickerModule` (filter-bar, todo-form, todo-list)
- `NoopAnimations` removed from `@angular/platform-browser/animations` in Angular 21 — use `provideAnimations()`/`provideNoopAnimations()` instead
- app.spec must query `.app-title` (text 'TodoApp'), not `h1` ('Hello, client' was old scaffold)
- TodoItem/Category model fields are optional (`undefined`), never `null`
- `ng build` warnings are pre-existing/non-fatal: NG8011 controlFlowPreventingContentProjection (btn-spinner @if), NG8113 unused AsyncPipe/MatButton, bundle budget 500kB exceeded (~558kB)

## Session Summary (2026-08-20)

### Done
- **005-loading-indicators: All 33 tasks complete (Phases 1-6)**
  - Phase 1 (Setup): T001-T002 — baseline verified (build OK, 40/40 tests)
  - Phase 2 (US1 Skeleton): T003-T008 — `SkeletonComponent` (`app-skeleton`) with 5 variants (`todo-list`, `category-list`, `detail`, `profile`, `rows`), `aria-live` label, shimmer gated by `prefers-reduced-motion`; applied to all 4 data screens
  - Phase 3 (US2 Busy States): T010-T016 — global `.btn-spinner`+`spin` in `styles.scss`, `deletingId` busy state in todo-card/category-card, toolbar/profile logout busy, filter-bar `[disabled]`, error-state retry busy
  - Phase 4 (US4 Flicker-Free & A11y): T018-T022 — `withLoadingState` operator (deferMs=200, minMs=300), wired into all data screens, `ErrorStateComponent` with retry, `aria-busy` on containers, reduced-motion audit
  - Phase 5 (US3 Background Refresh): T024-T027 — `loading()`/`refreshing()` split (skeleton for no-data, slim `MatProgressBar` for refresh), `takeUntilDestroyed()` on all subscriptions, error+retry for failed first loads
  - Phase 6 (Polish): T029-T033 — 33 new unit tests (skeleton, error-state, operator), `ng build` OK, **73/73 unit tests passing**, AGENTS.md updated
  - Fixed missing `MatIcon` import in `profile.component.ts` (build error)
  - Removed unused `AsyncPipe` import from `todo-list.component.ts`

### Key Context
- `SkeletonComponent` at `shared/components/loading/skeleton.component.ts` — 5 variants, Material CSS vars, shimmer animation
- `ErrorStateComponent` at `shared/components/loading/error-state.component.ts` — icon + message + retry button, `role="status"`
- `withLoadingState` operator at `shared/utils/loading.operator.ts` — drives `WritableSignal<boolean>`, defer 200ms / hold 300ms
- Data screens (todo-list, category-list, todo-detail, profile) use `withLoadingState` + `takeUntilDestroyed()`
- `loading()` = no data yet → skeleton; `refreshing()` = data present → slim `MatProgressBar` + content preserved
- `aria-busy` on all data containers; `role="status"` + `aria-live="polite"` on all loaders
- Global `.btn-spinner` + `spin` keyframes in `styles.scss`, gated by `prefers-reduced-motion: no-preference`
- `deletingId` signal for delete busy states in todo-list/todo-card and category-list/category-card
- Test files: `skeleton.component.spec.ts`, `error-state.component.spec.ts`, `loading.operator.spec.ts`
- All animations gated by `@media (prefers-reduced-motion: no-preference)` — static under reduced-motion
<!-- SPECKIT END -->
