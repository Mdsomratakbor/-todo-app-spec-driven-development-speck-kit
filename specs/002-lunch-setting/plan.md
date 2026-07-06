# Implementation Plan: Lunch Setting

**Branch**: `002-lunch-setting` | **Date**: 2026-07-04 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-lunch-setting/spec.md`

---

## Summary

Backend API for managing user lunch preferences including dietary restrictions, meal timing, favorite meals, and excluded items. .NET 10 Web API with Clean Architecture backed by PostgreSQL. No frontend changes — this is a backend-only feature.

---

## Technical Context

**Language/Version**: C# (.NET 10)

**Primary Dependencies**:
- Backend: Entity Framework Core (PostgreSQL), Cartographer.Mapper, FluentResponse.ApiWrapper, FluentValidation, Serilog, Scalar, MediatR

**Storage**: PostgreSQL via EF Core migrations; single LunchPreference table with owned collections for favorites/exclusions

**Testing**: xUnit + Moq (unit), WebApplicationFactory + TestContainers (integration)

**Target Platform**: Windows/Linux web server

**Project Type**: web-service (backend API only)

**Performance Goals**: Preference profile API < 200ms p95 for read; < 500ms for write

**Constraints**: Single LunchPreference per user; time validation (end > start); duration 15-120 min; favorites/exclusions max 20 items each

**Scale/Scope**: Up to 10,000 user profiles; single organization

---

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Constitution v1.6.0 Compliance**:

| Principle | Assessment | Notes |
|-----------|------------|-------|
| I. Architecture & Design First | ✅ Compliant | Clean Architecture (API/Application/Domain/Infrastructure); Cartographer.Mapper + FluentResponse.ApiWrapper; URL path API versioning; Scalar |
| II. Naming & Structure Conventions | ✅ Compliant | `api/` root folder; RESTful plural nouns under `/api/v1/`; PascalCase C# |
| III. Git & Development Workflow | ✅ Compliant | Trunk-based dev, conventional commits, PR with squash merge — applied project-wide |
| IV. Quality & Testing Strategy | ✅ Compliant | Per-field validation; xUnit + Moq + WebApplicationFactory + TestContainers; RFC 7807 ProblemDetails; structured logging |
| V. Security & AI Collaboration | ✅ Compliant | JWT auth, rate limiting, input sanitization, EF Core parameterized queries; AI collaboration rules documented |
| Operational Standards | ✅ Compliant | ProblemDetails middleware, structured logging via Serilog, FluentValidation pipeline |
| Delivery Standards | ✅ Compliant | Testing strategy, DoD checklist, Scalar OpenAPI docs, reversible migrations |

**No violations detected.**

---

## Project Structure

### Documentation (this feature)

```text
specs/002-lunch-setting/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output (not created by /speckit.plan)
```

### Source Code (repository root)

```text
api/
├── TodoApp.sln
├── src/
│   ├── TodoApp.Api/
│   │   └── Controllers/
│   │       └── LunchPreferencesController.cs    # CRUD for preferences
│   ├── TodoApp.Application/
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   └── ILunchPreferenceRepository.cs
│   │   │   ├── Mappings/
│   │   │   │   └── LunchPreferenceMappingProfile.cs
│   │   │   └── Behaviors/
│   │   │       ├── ValidationBehavior.cs        # Reuse existing
│   │   │       ├── LoggingBehavior.cs           # Reuse existing
│   │   │       └── PerformanceBehavior.cs       # Reuse existing
│   │   └── LunchPreferences/
│   │       ├── Commands/
│   │       │   ├── UpdateLunchPreference/
│   │       │   │   ├── UpdateLunchPreferenceCommand.cs
│   │       │   │   ├── UpdateLunchPreferenceCommandHandler.cs
│   │       │   │   └── UpdateLunchPreferenceCommandValidator.cs
│   │       │   └── ResetLunchPreference/
│   │       │       ├── ResetLunchPreferenceCommand.cs
│   │       │       ├── ResetLunchPreferenceCommandHandler.cs
│   │       │       └── ResetLunchPreferenceCommandValidator.cs
│   │       ├── Queries/
│   │       │   └── GetLunchPreference/
│   │       │       ├── GetLunchPreferenceQuery.cs
│   │       │       ├── GetLunchPreferenceQueryHandler.cs
│   │       │       └── GetLunchPreferenceQueryValidator.cs
│   │       └── Dtos/
│   │           ├── LunchPreferenceResponse.cs
│   │           └── UpdateLunchPreferenceRequest.cs
│   ├── TodoApp.Domain/
│   │   ├── Entities/
│   │   │   └── LunchPreference.cs
│   │   ├── Enums/
│   │   │   └── DietaryRestriction.cs
│   │   └── Interfaces/
│   │       └── ILunchPreferenceRepository.cs    # Domain interface
│   └── TodoApp.Infrastructure/
│       ├── Data/
│       │   ├── AppDbContext.cs                  # Extended with LunchPreference DbSet
│       │   ├── EntityConfigurations/
│       │   │   └── LunchPreferenceConfiguration.cs
│       │   └── SeedData.cs                      # Dietary restrictions + org defaults
│       ├── Repositories/
│       │   └── LunchPreferenceRepository.cs
│       └── Migrations/
└── tests/
    ├── TodoApp.UnitTests/
    │   └── Application/
    │       └── LunchPreferences/
    │           ├── UpdateLunchPreferenceCommandHandlerTests.cs
    │           ├── ResetLunchPreferenceCommandHandlerTests.cs
    │           └── GetLunchPreferenceQueryHandlerTests.cs
    └── TodoApp.IntegrationTests/
        └── Api/
            └── LunchPreferenceEndpointsTests.cs
```

**Structure Decision**: Backend-only addition to the existing Clean Architecture solution under `api/`. No new projects — reuse existing layers (TodoApp.Api, TodoApp.Application, TodoApp.Domain, TodoApp.Infrastructure). All paths live under the existing `api/src/TodoApp.*` structure.

---

## Complexity Tracking

> No violations. Complexity is justified at standard Clean Architecture adherence.

---

## Layer 3 — Behaviour Specifications

### BH-001: Retrieve lunch preferences successfully (Happy Path)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-001 |
| **Related Requirements** | FR-002, NFR-001 |
| **Preconditions** | Authenticated user; lunch preference profile exists |
| **Given** | I am an authenticated user with existing lunch preferences |
| **When** | I GET my lunch preferences |
| **Then** | The API returns 200 with the full preference profile including dietary restrictions, timing, favorites, and exclusions |
| **Expected Result** | GET `/api/v1/lunch-preferences` returns LunchPreferenceResponse within 200ms |
| **Success Criteria** | SC-001, SC-004 |

### BH-002: Update preference with invalid time range (Error Scenario)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-002 |
| **Related Requirements** | FR-003, FR-006, BR-002, NFR-003 |
| **Preconditions** | Authenticated user; preference profile exists |
| **Given** | I am updating my lunch preferences |
| **When** | I set LunchStartTime = 14:00 and LunchEndTime = 13:00 (end before start) |
| **Then** | The API returns 400 with a validation error: "Lunch end time must be after start time" |
| **Expected Result** | PUT `/api/v1/lunch-preferences` returns 400 with ProblemDetails; no data changed |
| **Success Criteria** | SC-002, SC-004 |

### BH-003: First-time user auto-creates default preferences (Business Rule)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-003 |
| **Related Requirements** | FR-002, BR-006 |
| **Preconditions** | Authenticated user; no preference profile exists |
| **Given** | I am a new user accessing lunch preferences for the first time |
| **When** | I GET my preferences |
| **Then** | The API returns 200 with a default preference profile (org standard lunch window 12:00-13:00, notifications enabled, no restrictions/favorites/exclusions) |
| **Expected Result** | GET `/api/v1/lunch-preferences` creates default profile and returns it |
| **Success Criteria** | SC-003 |

### BH-004: Add favorite meal exceeding max limit (Boundary)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-004 |
| **Related Requirements** | FR-007, BR-004 |
| **Preconditions** | Authenticated user; preference profile exists with 20 favorite meals |
| **Given** | I have 20 favorite meals in my profile |
| **When** | I add a 21st favorite meal |
| **Then** | The API returns 422 with "Favorite meals list cannot exceed 20 items" |
| **Expected Result** | PUT request with 21 favorites returns 422; existing profile unchanged |
| **Success Criteria** | SC-002 |

### BH-005: Reset preferences to defaults (Business Rule)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-005 |
| **Related Requirements** | FR-004 |
| **Preconditions** | Authenticated user; preference profile exists with custom values |
| **Given** | I have a customized lunch preference profile |
| **When** | I DELETE my preferences (reset) |
| **Then** | The API returns 200 with a default preference profile; all custom values are replaced with defaults |
| **Expected Result** | DELETE `/api/v1/lunch-preferences` replaces profile with defaults and returns them |
| **Success Criteria** | SC-003, SC-004 |

### BH-006: Invalid dietary restriction value (Error Scenario)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-006 |
| **Related Requirements** | FR-005, BR-001, NFR-003 |
| **Preconditions** | Authenticated user; preference profile exists |
| **Given** | I am updating my lunch preferences with a dietary restriction |
| **When** | I set DietaryRestrictions to `["invalid-restriction-value"]` that does not exist in the predefined list |
| **Then** | The API returns 400 with a validation error: "Dietary restriction 'invalid-restriction-value' is not valid" |
| **Expected Result** | PUT `/api/v1/lunch-preferences` returns 400 with ProblemDetails; no data changed |
| **Success Criteria** | SC-002, SC-004 |

### BH-007: Duplicate exclusion item (Conflict)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-007 |
| **Related Requirements** | FR-008, BR-005 |
| **Preconditions** | Authenticated user; preference profile exists with "Peanuts" in ExcludedItems |
| **Given** | I have "Peanuts" in my excluded items list |
| **When** | I add "Peanuts" again to the exclusions list |
| **Then** | The API returns 409 Conflict with "The item already exists in the list." |
| **Expected Result** | PUT `/api/v1/lunch-preferences` with duplicate exclusion returns 409; existing profile unchanged |
| **Success Criteria** | SC-002 |

---

## Layer 4 — Constraints

### SEC-001: Authentication Required

| Field | Value |
|-------|-------|
| **Constraint ID** | SEC-001 |
| **Related Requirement** | NFR-002 |
| **Forbidden Action** | Accessing `/api/v1/lunch-preferences` without a valid JWT Bearer token |
| **Reason** | Protect user preference data |
| **Consequence** | 401 Unauthorized with ProblemDetails response |
| **Responsible Layer** | API (Controller-level `[Authorize]` attribute) |
| **Verification Method** | Integration test: send request without token → assert 401 |

### VAL-001: Server-Side Validation Rejection

| Field | Value |
|-------|-------|
| **Constraint ID** | VAL-001 |
| **Related Requirement** | NFR-003, Principle IV |
| **Forbidden Action** | Accepting invalid data (invalid time ranges, out-of-bounds duration, invalid dietary restrictions) |
| **Reason** | Data integrity; client is not trusted |
| **Consequence** | 400 Bad Request with ProblemDetails containing per-field validation errors |
| **Responsible Layer** | API (FluentValidation pipeline behavior) |
| **Verification Method** | Integration test: send invalid payload → assert 400 with errors |

### LOG-001: Structured Logging with Correlation ID

| Field | Value |
|-------|-------|
| **Constraint ID** | LOG-001 |
| **Related Requirement** | NFR-004 |
| **Forbidden Action** | Logging without a correlation ID or using string interpolation |
| **Reason** | Observability; tracing requests |
| **Consequence** | Violation of Operational Standards |
| **Responsible Layer** | API (Middleware; reuse existing) |
| **Verification Method** | Integration test: inspect log output for correlation ID |

### DB-001: One Preference Profile Per User

| Field | Value |
|-------|-------|
| **Constraint ID** | DB-001 |
| **Related Requirement** | BR-006 |
| **Forbidden Action** | Creating more than one LunchPreference per user |
| **Reason** | One-to-one mapping between user and preference profile |
| **Consequence** | Unique constraint on UserId; GET auto-creates if missing; PUT updates existing |
| **Responsible Layer** | Infrastructure (unique index on UserId) + Application (get-or-create pattern) |
| **Verification Method** | Integration test: PUT twice → assert same single record updated |

### VAL-002: Time Range Validation

| Field | Value |
|-------|-------|
| **Constraint ID** | VAL-002 |
| **Related Requirement** | FR-006, BR-002, BR-003 |
| **Forbidden Action** | Saving LunchEndTime <= LunchStartTime or BreakDuration outside 15-120 min |
| **Reason** | Logical time constraints; reasonable break duration |
| **Consequence** | 422 Unprocessable Entity with ProblemDetails |
| **Responsible Layer** | Application (FluentValidation) |
| **Verification Method** | Unit test: validate command with invalid times → assert validation error |

### API-001: FluentResponse.ApiWrapper Envelope

| Field | Value |
|-------|-------|
| **Constraint ID** | API-001 |
| **Related Requirement** | Principle I |
| **Forbidden Action** | Returning raw DTOs directly from controllers without the FluentResponse.ApiWrapper envelope |
| **Reason** | Consistent API contract |
| **Consequence** | All responses wrapped in `ApiResponse<T>` with `isSuccess`, `data`, `message` |
| **Responsible Layer** | API (Controller layer) |
| **Verification Method** | Integration test: inspect response body for envelope structure |

---

## Technical Architecture

### High-Level Architecture

```
┌──────────────────────────────────────────────────────┐
│     External Client (Frontend / Third-party)         │
│                      │ HTTP/JSON                     │
│                      │ JWT Bearer Auth               │
└──────────────────────┼───────────────────────────────┘
                       │
┌──────────────────────┼───────────────────────────────┐
│            .NET 10 Web API                            │
│  ┌──────────────────────────────────────────────┐   │
│  │  API Layer (TodoApp.Api)                     │   │
│  │  ┌─────────────────────────────────────────┐ │   │
│  │  │ LunchPreferencesController              │ │   │
│  │  │ [Authorize] [ApiController]             │ │   │
│  │  │ GET /api/v1/lunch-preferences           │ │   │
│  │  │ PUT /api/v1/lunch-preferences           │ │   │
│  │  │ DELETE /api/v1/lunch-preferences        │ │   │
│  │  └─────────────────────────────────────────┘ │   │
│  └──────────────────────┬───────────────────────┘   │
│                         │ MediatR                    │
│  ┌──────────────────────┼───────────────────────┐   │
│  │  Application Layer (TodoApp.Application)    │   │
│  │  ┌─────────────────────────────────────────┐ │   │
│  │  │ Commands/Queries                        │ │   │
│  │  │ • GetLunchPreferenceQuery               │ │   │
│  │  │ • UpdateLunchPreferenceCommand          │ │   │
│  │  │ • ResetLunchPreferenceCommand           │ │   │
│  │  │ Validation, Mapping (Cartographer)      │ │   │
│  │  └─────────────────────────────────────────┘ │   │
│  └──────────────────────┬───────────────────────┘   │
│                         │                            │
│  ┌──────────────────────┼───────────────────────┐   │
│  │  Domain Layer (TodoApp.Domain)              │   │
│  │  ┌─────────────────────────────────────────┐ │   │
│  │  │ LunchPreference entity                  │ │   │
│  │  │ Value objects (TimeRange, MealList)     │ │   │
│  │  │ ILunchPreferenceRepository interface    │ │   │
│  │  └─────────────────────────────────────────┘ │   │
│  └──────────────────────┬───────────────────────┘   │
│                         │                            │
│  ┌──────────────────────┼───────────────────────┐   │
│  │  Infrastructure Layer (TodoApp.Infrastructure)│   │
│  │  ┌─────────────────────────────────────────┐ │   │
│  │  │ AppDbContext (extended)                 │ │   │
│  │  │ LunchPreferenceConfiguration            │ │   │
│  │  │ LunchPreferenceRepository               │ │   │
│  │  │ SeedData (dietary restrictions, org     │ │   │
│  │  │           defaults)                     │ │   │
│  │  └─────────────────────────────────────────┘ │   │
│  └──────────────────────┬───────────────────────┘   │
└─────────────────────────┼───────────────────────────┘
                          │
                   ┌──────┴──────┐
                   │  PostgreSQL  │
                   └─────────────┘
```

### Component Responsibilities

| Layer | Responsibility |
|-------|---------------|
| **API** | HTTP endpoint exposure, auth enforcement, request validation, response formatting, middleware pipeline |
| **Application** | CQRS command/query handlers, business orchestration, mapping, pipeline behaviors |
| **Domain** | Core LunchPreference entity, value objects, repository interface, dietary restriction enum |
| **Infrastructure** | EF Core DbContext extension, repository implementation, data seeding, migrations |

### Dependency Flow

```
API → Application → Domain (interfaces)
API → Infrastructure (DI registration)
Infrastructure → Domain (implements interfaces)
Application → Domain (uses interfaces, entities)
```

## API Design

### Endpoints

| Endpoint | Method | Purpose | Key Validation |
|----------|--------|---------|---------------|
| `/api/v1/lunch-preferences` | GET | Get user's preference profile (auto-creates defaults if missing) | JWT auth; UserId from token |
| `/api/v1/lunch-preferences` | PUT | Update preference fields (partial update) | JWT auth; time range; duration; list sizes |
| `/api/v1/lunch-preferences` | DELETE | Reset preferences to defaults | JWT auth; confirms reset |

### Validation Strategy

- **Request DTOs**: Data annotations for basic rules
- **FluentValidation validators**: Per-command validators in MediatR pipeline
- **Domain validation**: Entity-level validation in constructors
- **Database constraints**: Unique index on UserId, check constraints

### Error Handling

Reuse existing global exception middleware (same as Todo Management feature) per Operational Standards.

## Data Model Design

See [data-model.md](data-model.md) for full entity definitions.

### Entities

| Entity | Key Fields | Relationships |
|--------|-----------|---------------|
| **LunchPreference** | Id (Guid, PK), UserId (Guid, unique), DietaryRestrictions (string[]), LunchStartTime (TimeOnly?), LunchEndTime (TimeOnly?), BreakDurationMinutes (int?), NotificationsEnabled (bool), FavoriteMeals (string[]), ExcludedItems (string[]), CreatedAt, UpdatedAt | Owns collections for FavoriteMeals, ExcludedItems |

### Key Constraints

- BR-001: Dietary restrictions validated against seed data list
- BR-002: LunchEndTime > LunchStartTime (application-level)
- BR-003: BreakDurationMinutes 15-120 (application-level)
- BR-004: FavoriteMeals <= 20 items (application-level)
- BR-005: ExcludedItems <= 20 items (application-level)
- BR-006: Auto-create default on first GET (application-level)
- DB-001: Unique index on UserId

## Security Design

### Authentication

- JWT Bearer token required on `/api/v1/lunch-preferences`
- Token validated via `AddJwtBearer()` middleware (reuse existing)
- UserId extracted from JWT claims for data ownership

### Authorization

- Single-user-per-profile; user can only access their own preferences
- `[Authorize]` attribute on controller
- UserId from token used in all queries (no user ID in URL)

### Input Validation

| Layer | Strategy |
|-------|----------|
| **API** | ASP.NET `[ApiController]` automatic model validation |
| **Application** | FluentValidation validators per command via MediatR pipeline |
| **Infrastructure** | EF Core parameterized queries; unique index on UserId |

### Rate Limiting

- 100 requests per minute per user (reuse existing middleware)
- Response header `X-RateLimit-Remaining` included

## Performance Considerations

| Endpoint | Target p95 | Max |
|----------|-----------|-----|
| GET /api/v1/lunch-preferences | 100ms | 200ms |
| PUT /api/v1/lunch-preferences | 200ms | 500ms |
| DELETE /api/v1/lunch-preferences | 200ms | 500ms |

### Caching

- Preference profile can be cached per-user (in-memory, 5-minute TTL) since changes are infrequent

## Risk Assessment

| Risk | Severity | Mitigation |
|------|----------|------------|
| Time validation edge cases (midnight, DST) | Low | Use TimeOnly type (timezone-independent); validation ensures start < end |
| List size unbounded growth | Low | Enforce max 20 items on favorites and exclusions (validation + documentation) |
| User ID mismatch between token and data | Low | Always derive UserId from JWT claims, never from request body |

## Testing Strategy

| Scenario ID | Unit Test | Integration Test |
|-------------|-----------|------------------|
| BH-001: Get preferences | GetLunchPreferenceQueryHandler: returns existing profile or creates default | GET returns 200 with correct body |
| BH-002: Invalid time range | UpdateLunchPreferenceCommandValidator: rejects end < start | PUT with invalid times returns 400 |
| BH-003: Auto-create defaults | GetLunchPreferenceQueryHandler: creates default for new user | GET for new user returns default profile |
| BH-004: Favorites overflow | UpdateLunchPreferenceCommandValidator: rejects >20 favorites | PUT with 21 favorites returns 422 |
| BH-005: Reset to defaults | ResetLunchPreferenceCommandHandler: replaces with defaults | DELETE returns default profile |
| BH-006: Invalid dietary restriction | UpdateLunchPreferenceCommandValidator: rejects invalid restriction value | PUT with invalid restriction returns 400 |
| BH-007: Duplicate exclusion | UpdateLunchPreferenceCommandHandler: rejects duplicate exclusion | PUT with duplicate exclusion returns 409 |

## Traceability Matrix

| Requirement | BH Scenario | Constraint | Component |
|-------------|-------------|------------|-----------|
| FR-001 — Create profile | BH-003 (auto-create on GET) | DB-001, VAL-001 | Application (GetOrCreate), API (GET) |
| FR-002 — Retrieve profile | BH-001 (get existing), BH-003 (auto-create) | SEC-001, LOG-001 | API (GET endpoint) |
| FR-003 — Update profile | BH-002 (invalid time validation) | VAL-001, VAL-002 | API (PUT endpoint), Application (handler) |
| FR-004 — Reset to defaults | BH-005 (DELETE reset) | SEC-001, API-001 | API (DELETE endpoint), Application (handler) |
| FR-005 — Validate dietary restrictions | BH-006 (invalid restriction) | BR-001, VAL-001 | Application (FluentValidation) |
| FR-006 — Configurable meal timing | BH-002 (end before start) | BR-002, BR-003, VAL-002 | Domain (value objects), Application (validation) |
| FR-007 — Favorite meals list | BH-004 (max 20 overflow) | BR-004, VAL-001 | Application (FluentValidation) |
| FR-008 — Excluded items list | BH-007 (duplicate exclusion) | BR-005, VAL-001 | Application (FluentValidation + handler) |
| FR-009 — Org default preferences | BH-003 (new user defaults) | BR-006, BR-007 | Infrastructure (SeedData) |
| NFR-001 — Performance (<200ms) | BH-001 (p95 target) | — | Infrastructure (caching), API (middleware) |
| NFR-002 — Auth & rate limiting | SEC-001 | SEC-001 | API (JWT middleware, rate limiting) |
| NFR-003 — Server-side validation | BH-002, BH-004, BH-006, BH-007 | VAL-001, VAL-002 | API (FluentValidation pipeline) |
| NFR-004 — Structured logging | LOG-001 | LOG-001 | API (Serilog middleware) |
