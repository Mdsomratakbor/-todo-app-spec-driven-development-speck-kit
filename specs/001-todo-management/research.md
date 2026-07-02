# Research: Todo Management System

**Created**: 2026-06-30
**Input**: [spec.md](spec.md) + [plan.md](plan.md)

## Research Tasks

### 1. API Versioning Strategy

| Field | Value |
|-------|-------|
| **Decision** | URL path prefix versioning (`/api/v1/...`) |
| **Rationale** | Simple, explicit, easy to route. Aligns with standard .NET Web API conventions and the constitution requirement. |
| **Alternatives considered** | Header-based versioning (more complex routing), query-string versioning (less discoverable) |

### 2. Object Mapping Library

| Field | Value |
|-------|-------|
| **Decision** | Cartographer.Mapper |
| **Rationale** | Explicitly required by the user. Lightweight mapping library suitable for DTO-to-entity mapping. |
| **Alternatives considered** | AutoMapper (popular but not chosen), manual mapping (more boilerplate) |

### 3. API Response Wrapper

| Field | Value |
|-------|-------|
| **Decision** | FluentResponse.ApiWrapper |
| **Rationale** | Explicitly required by the user. Provides consistent response envelopes for all API endpoints. |
| **Alternatives considered** | Custom middleware wrapper, ProblemDetails only |

### 4. Database Choice

| Field | Value |
|-------|-------|
| **Decision** | PostgreSQL |
| **Rationale** | Explicitly specified by the user. Robust, open-source, well-supported by EF Core. |
| **Alternatives considered** | SQL Server (originally planned, switched to PostgreSQL per user), SQLite (not suitable for multi-user web) |

### 5. Frontend Framework

| Field | Value |
|-------|-------|
| **Decision** | Angular 21 with Angular Material + Bootstrap |
| **Rationale** | Specified by the user. Angular Material provides snackbar (MatSnackBar), form components (MatError), and theming. Bootstrap provides layout utilities. |
| **Alternatives considered** | Angular alone (without Material/Bootstrap), React (different framework) |

### 6. API Documentation

| Field | Value |
|-------|-------|
| **Decision** | Scalar (OpenAPI 3.x) |
| **Rationale** | Explicitly required by the user. Modern, clean API documentation UI that consumes standard OpenAPI specs. |
| **Alternatives considered** | Swagger UI (replaced by Scalar per user request) |

### 7. Concurrent Edit Strategy

| Field | Value |
|-------|-------|
| **Decision** | Last-write-wins (no explicit conflict detection) |
| **Rationale** | Single-user app; actual conflicts from multiple browser tabs are rare. ETags or locking add unnecessary complexity. |
| **Alternatives considered** | Optimistic concurrency with ETags (over-engineered for single user), pessimistic locking (too restrictive) |

### 8. Localization Strategy

| Field | Value |
|-------|-------|
| **Decision** | English-only UI, no i18n |
| **Rationale** | v1 scope does not include multi-language support. Can be added later without breaking changes. |
| **Alternatives considered** | @angular/localize setup (scope creep for v1), full i18n framework (unnecessary) |

### 9. Data Import/Export

| Field | Value |
|-------|-------|
| **Decision** | Out of scope for v1 |
| **Rationale** | Clarified during the /clarify phase. Interactive task management only; no bulk data portability. |
| **Alternatives considered** | CSV export (future enhancement), JSON export (future enhancement) |

### 10. Soft-Delete Strategy

| Field | Value |
|-------|-------|
| **Decision** | Soft-delete with 30-day retention, then hard-delete via scheduled cleanup |
| **Rationale** | BR-007 from spec. Allows accidental deletion recovery within a reasonable window. |
| **Alternatives considered** | Hard-delete immediately (no recovery), soft-delete indefinitely (DB bloat) |
