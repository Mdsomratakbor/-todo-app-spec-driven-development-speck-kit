# Implementation Plan: Todo Management System

**Branch**: `001-todo-management` | **Date**: 2026-06-30 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-todo-management/spec.md`

---

## Summary

Full CRUD todo management system with categories, priority levels, status tracking, search/filter capabilities, pagination, informative UI feedback, and client-side validation. Single-user web application with .NET 10 API + Angular 21 frontend backed by PostgreSQL. Architecture per Constitution v1.3.0.

---

## Technical Context

**Language/Version**: C# (.NET 10), TypeScript (Angular 21)

**Primary Dependencies**:
- Backend: Entity Framework Core (PostgreSQL), Cartographer.Mapper, FluentResponse.ApiWrapper, FluentValidation, Serilog, Scalar, MediatR
- Frontend: Angular Material, Bootstrap, Playwright (E2E), Jasmine/Karma (unit)

**Storage**: PostgreSQL via EF Core migrations; soft-delete with 30-day retention cleanup

**Testing**: xUnit + Moq (unit), WebApplicationFactory + TestContainers (integration), Jasmine/Karma (component), Playwright (E2E)

**Target Platform**: Windows/Linux web server; modern browsers (Chrome, Firefox, Edge, Safari)

**Project Type**: web-service + frontend (SPA)

**Performance Goals**: Todo list API < 500ms for 10k todos at page size 20; page load < 2s for up to 1,000 todos

**Constraints**: Single-user, English-only UI, last-write-wins concurrency, standard PostgreSQL backups, no formal uptime SLA

**Scale/Scope**: Up to 100 concurrent users, up to 10,000 active todos per user

---

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Constitution v1.3.0 Compliance**:

| Principle | Assessment | Notes |
|-----------|------------|-------|
| I. Architecture & Design First | ✅ Compliant | Clean Architecture (API/Application/Domain/Infrastructure); Cartographer.Mapper + FluentResponse.ApiWrapper; component-based Angular frontend; informative UI (loading, empty, error states); MatSnackBar notifications; per-field client validation; URL path API versioning; Scalar |
| II. Naming & Structure Conventions | ✅ Compliant | `api/` + `client/` root folders; RESTful plural nouns under `/api/v1/`; PascalCase C# / kebab-case Angular files |
| III. Git & Development Workflow | ✅ Compliant | Trunk-based dev, conventional commits, PR with squash merge — applied project-wide |
| IV. Quality & Testing Strategy | ✅ Compliant | MatSnackBar (error/warning/success); per-field inline validation; xUnit + Moq + WebApplicationFactory + TestContainers + Playwright; >= 80% coverage target |
| V. Security & AI Collaboration | ✅ Compliant | JWT auth, rate limiting, input sanitization, EF Core parameterized queries; AI collaboration rules documented |
| Operational Standards | ✅ Compliant | ProblemDetails middleware, structured logging via Serilog, FluentValidation pipeline |
| Delivery Standards | ✅ Compliant | Testing strategy, DoD checklist, Scalar OpenAPI docs, reversible migrations |

**No violations detected.**

---

## Layer 3 — Behaviour Specifications

All scenarios are traceable to automated tests. Scenario IDs use `BH-` prefix.

### BH-001: Create a complete todo item successfully (Happy Path)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-001 |
| **Related Requirements** | FR-001, FR-013, NFR-003, NFR-008, BR-001 |
| **Preconditions** | Authenticated user; at least one category exists; no todos exist |
| **Given** | I am on the todo list page which displays an empty state |
| **When** | I click "Add Todo", fill in title ("Buy groceries"), description ("Milk, eggs, bread"), due date (2026-07-15), priority (Medium), category (Personal), and click Submit |
| **Then** | The form submits; a success snackbar appears ("Todo created successfully"); the new todo appears in the list with correct title, priority badge, category chip, and due date |
| **Expected Result** | A new TodoItem is persisted in the database with StatusId = 1 (Pending), CreatedAt = UTC now, and all provided field values |
| **Success Criteria** | SC-001, SC-002, SC-004 |

### BH-002: Filter todos by multiple criteria (Happy Path)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-002 |
| **Related Requirements** | FR-002, FR-010, FR-011, NFR-001 |
| **Preconditions** | 50 todos exist across 3 categories, 4 priorities, and 2 statuses (Pending, Completed); mixed due dates |
| **Given** | I am on the todo list page with all 50 todos visible across pages |
| **When** | I set filter: status = Pending, priority = High, category = "Work", and click "Apply Filters" |
| **Then** | The list updates to show only todos matching all three criteria; pagination reflects the filtered count; the URL updates with query parameters |
| **Expected Result** | GET `/api/v1/todos?statusId=1&priorityId=3&categoryId={workId}` returns filtered, paginated results within 500ms |
| **Success Criteria** | SC-002 |

### BH-003: Submit create todo form with empty title (Error Scenario)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-003 |
| **Related Requirements** | FR-001, NFR-003, BR-001 |
| **Preconditions** | Authenticated user; on the create todo form |
| **Given** | I am on the create todo form with all fields empty |
| **When** | I click "Submit" without entering a title |
| **Then** | The form is not submitted; an inline validation error appears below the title input ("Title is required"); the title field is highlighted with error styling; no snackbar appears |
| **Expected Result** | No API call is made; the Angular form marks `title` as invalid with `required` error; MatError displays message |
| **Success Criteria** | SC-003 |

### BH-004: Attempt invalid status transition Completed → Pending (Error Scenario)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-004 |
| **Related Requirements** | FR-004, BR-006, NFR-003 |
| **Preconditions** | A todo exists with StatusId = 3 (Completed) |
| **Given** | I am editing a todo that is already marked as Completed |
| **When** | I change the status dropdown to "Pending" and click Save |
| **Then** | The backend returns 422 Unprocessable Entity with a ProblemDetails response containing "The requested status transition is not allowed."; an error snackbar appears; the UI preserves the Completed status |
| **Expected Result** | PUT `/api/v1/todos/{id}` with statusId = 1 returns 422; the todo remains Completed in the database |
| **Success Criteria** | SC-004 |

### BH-005: Create todo with title at maximum length 200 characters (Boundary Condition)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-005 |
| **Related Requirements** | FR-001, BR-001, NFR-003 |
| **Preconditions** | Authenticated user; on the create todo form |
| **Given** | I am creating a todo with a title that is exactly 200 characters long |
| **When** | I submit the form |
| **Then** | The form submits successfully; the todo is created; the title in the list displays the full 200-character text (truncated via CSS if needed) |
| **Expected Result** | POST `/api/v1/todos` with a 200-char title returns 201; the title is stored and returned without truncation |
| **Success Criteria** | SC-001, SC-004 |

### BH-006: Set due date exactly 5 years in the future (Boundary Condition)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-006 |
| **Related Requirements** | FR-001, BR-003, NFR-003 |
| **Preconditions** | Authenticated user; today is 2026-06-30 |
| **Given** | I am creating a todo with a due date set to 2031-06-30 (exactly 5 years from today) |
| **When** | I submit the form |
| **Then** | The form submits successfully; the todo is created with the specified due date |
| **Expected Result** | POST `/api/v1/todos` with dueDate = "2031-06-30T00:00:00Z" returns 201; dueDate 5 years + 1 day returns 400 |
| **Success Criteria** | SC-003, SC-004 |

### BH-007: Delete a category that has assigned todos (Business Rule)

| Field | Value |
|-------|-------|
| **Scenario ID** | BH-007 |
| **Related Requirements** | FR-009, BR-004 |
| **Preconditions** | A category "Work" exists with 3 todos assigned to it; the default "Uncategorized" category exists |
| **Given** | I am on the category management page; the "Work" category shows todoCount = 3 |
| **When** | I click "Delete" on the "Work" category and confirm the deletion |
| **Then** | The category is removed; all 3 todos now display "Uncategorized" as their category; a warning snackbar appears ("Category deleted. 3 todos moved to Uncategorized."); the category list no longer shows "Work" |
| **Expected Result** | DELETE `/api/v1/categories/{id}` returns 204; CategoryId on the 3 todos is set to the Uncategorized category's Id |
| **Success Criteria** | SC-004 |

---

## Layer 4 — Constraints

### SEC-001: Authentication Required

| Field | Value |
|-------|-------|
| **Constraint ID** | SEC-001 |
| **Related Requirement** | NFR-002 |
| **Forbidden Action** | Accessing any `/api/v1/*` endpoint without a valid JWT Bearer token |
| **Reason** | Protect user data; all todo data is user-specific |
| **Consequence** | 401 Unauthorized with ProblemDetails response; no data returned |
| **Responsible Layer** | API (Controller-level `[Authorize]` attribute + middleware) |
| **Verification Method** | Integration test: send request without token → assert 401 |

### VAL-001: Client-Side Validation Before Submission

| Field | Value |
|-------|-------|
| **Constraint ID** | VAL-001 |
| **Related Requirement** | NFR-003, Principle IV |
| **Forbidden Action** | Submitting a form with invalid input without first showing inline validation errors |
| **Reason** | Defense-in-depth; immediate user feedback; prevent unnecessary API calls |
| **Consequence** | Form is not submitted; MatError messages displayed for each invalid field on touch or submit |
| **Responsible Layer** | Frontend (Angular Reactive Forms validators) |
| **Verification Method** | Component test: set invalid value, trigger submit, assert error message displayed and API not called |

### VAL-002: Server-Side Validation Rejection

| Field | Value |
|-------|-------|
| **Constraint ID** | VAL-002 |
| **Related Requirement** | NFR-003, Principle IV |
| **Forbidden Action** | Accepting invalid data that bypasses client-side validation (e.g., via curl/Postman) |
| **Reason** | Defense-in-depth; client is not trusted |
| **Consequence** | 400 Bad Request with ProblemDetails containing per-field validation errors |
| **Responsible Layer** | API (FluentValidation pipeline behavior + Controller validation) |
| **Verification Method** | Integration test: send invalid payload directly to API → assert 400 with errors collection |

### LOG-001: Structured Logging with Correlation ID

| Field | Value |
|-------|-------|
| **Constraint ID** | LOG-001 |
| **Related Requirement** | NFR-004 |
| **Forbidden Action** | Logging without a correlation ID or using string interpolation instead of structured properties |
| **Reason** | Observability; tracing requests across components; querying logs by correlation ID |
| **Consequence** | Violation of Operational Standards; difficult to debug production issues |
| **Responsible Layer** | API (Middleware generates correlation ID; Serilog enriches with it) |
| **Verification Method** | Integration test: inspect log output for correlation ID presence; code review for structured logging |

### PERF-001: Pagination Limit

| Field | Value |
|-------|-------|
| **Constraint ID** | PERF-001 |
| **Related Requirement** | NFR-005 |
| **Forbidden Action** | Returning more than 50 items in a single page |
| **Reason** | Prevent excessive data transfer and DB load; bound response times |
| **Consequence** | pageSize > 50 is silently capped to 50; no error returned |
| **Responsible Layer** | API (Query handler) |
| **Verification Method** | Integration test: request pageSize = 100 → assert response contains max 50 items |

### ERR-001: Problem Details for All Errors

| Field | Value |
|-------|-------|
| **Constraint ID** | ERR-001 |
| **Related Requirement** | NFR-003, Operational Standards |
| **Forbidden Action** | Returning raw exception details, stack traces, or non-standard error bodies in production |
| **Reason** | Security (information disclosure); client consistency; RFC 7807 compliance |
| **Consequence** | Global exception handler catches all unhandled exceptions and returns sanitized ProblemDetails |
| **Responsible Layer** | API (ExceptionMiddleware) |
| **Verification Method** | Integration test: trigger 500 error → assert ProblemDetails response with no stack trace |

### API-001: FluentResponse.ApiWrapper Envelope

| Field | Value |
|-------|-------|
| **Constraint ID** | API-001 |
| **Related Requirement** | Principle I |
| **Forbidden Action** | Returning raw DTOs directly from controllers without the FluentResponse.ApiWrapper envelope |
| **Reason** | Consistent API contract; envelope provides success/error/status metadata |
| **Consequence** | All successful responses wrapped in `ApiResponse<T>` with `isSuccess`, `data`, `message`; all error responses wrapped in `ApiErrorResponse` |
| **Responsible Layer** | API (Controller layer / Result filter) |
| **Verification Method** | Integration test: inspect response body shape for envelope structure |

### DB-001: Soft-Delete with 30-Day Retention

| Field | Value |
|-------|-------|
| **Constraint ID** | DB-001 |
| **Related Requirement** | BR-007 |
| **Forbidden Action** | Hard-deleting todos from the database on DELETE request (exception: the cleanup job after 30 days) |
| **Reason** | Allow accidental deletion recovery within 30-day window |
| **Consequence** | DELETE sets `DeletedAt`; all queries filter `WHERE DeletedAt IS NULL`; a background job hard-deletes expired records |
| **Responsible Layer** | Infrastructure (Repository layer, background service) |
| **Verification Method** | Integration test: DELETE todo → assert DeletedAt set; query list → assert todo excluded; cleanup job test |

### UI-001: Snackbar Notification on Every User Action

| Field | Value |
|-------|-------|
| **Constraint ID** | UI-001 |
| **Related Requirement** | NFR-008, Principle I |
| **Forbidden Action** | Completing a user action (create, update, delete, filter error) without showing a MatSnackBar notification |
| **Reason** | Informative UX; user must receive clear feedback for every action |
| **Consequence** | Success (green), error (red), warning (amber) snackbars with descriptive messages; auto-dismiss after 4 seconds |
| **Responsible Layer** | Frontend (Angular service wrapping MatSnackBar) |
| **Verification Method** | Component/E2E test: perform action → assert snackbar appeared with correct severity and message |

### UI-002: Loading and Empty States

| Field | Value |
|-------|-------|
| **Constraint ID** | UI-002 |
| **Related Requirement** | FR-012, FR-013, Principle I |
| **Forbidden Action** | Showing a blank or partially rendered view while data is loading or when no data exists |
| **Reason** | Informative UX; prevent confusion during async operations |
| **Consequence** | Loading spinner during API calls; empty-state illustration + message + CTA when no todos/categories exist |
| **Responsible Layer** | Frontend (Angular components with `*ngIf` / `@if` blocks) |
| **Verification Method** | Component test: mock slow API response → assert spinner shown; mock empty response → assert empty state shown |

### AI-001: No Generated Secrets or Credentials

| Field | Value |
|-------|-------|
| **Constraint ID** | AI-001 |
| **Related Requirement** | Principle V (AI Collaboration) |
| **Forbidden Action** | Generating or committing real secrets, API keys, connection strings with passwords, JWT signing keys, or any credential values |
| **Reason** | Security; credentials must be injected via environment variables or secure key stores |
| **Consequence** | All credentials referenced via `appsettings.json` placeholders, environment variables, or User Secrets; never hardcoded |
| **Responsible Layer** | All layers (code review enforces) |
| **Verification Method** | Code review; grep for hardcoded secrets pre-commit hook |

---

## Technical Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Angular 21 SPA                          │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────────┐  │
│  │ Smart/       │  │ Services     │  │ Shared            │  │
│  │ Container    │→ │ (HttpClient) │  │ Components/       │  │
│  │ Components   │  │              │  │ Validators/Models │  │
│  └─────────────┘  └──────┬───────┘  └───────────────────┘  │
│                          │ HTTP/JSON                       │
└──────────────────────────┼─────────────────────────────────┘
                           │ JWT Bearer Auth
┌──────────────────────────┼─────────────────────────────────┐
│            .NET 10 Web API                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  API Layer (TodoApp.Api)                             │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────────────┐    │  │
│  │  │Controllers│ │Middleware│ │Exception Filter   │    │  │
│  │  │[Authorize]│ │(Logging) │ │(ProblemDetails)   │    │  │
│  │  └─────┬────┘ └──────────┘ └──────────────────┘    │  │
│  └────────┼───────────────────────────────────────────┘  │
│           │ MediatR                                      │
│  ┌────────┼───────────────────────────────────────────┐  │
│  │  Application Layer (TodoApp.Application)           │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────────────┐  │  │
│  │  │ Commands  │ │ Queries  │ │ Behaviors         │  │  │
│  │  │ (CQRS)    │ │ (CQRS)   │ │(Validation/Logging│  │  │
│  │  └─────┬────┘ └─────┬────┘ │ /Performance)     │  │  │
│  │        │            │      └──────────────────┘  │  │
│  │        └─────┬──────┘                            │  │
│  │              │ Maps via Cartographer.Mapper        │  │
│  └──────────────┼────────────────────────────────────┘  │
│                 │                                        │
│  ┌──────────────┼────────────────────────────────────┐  │
│  │  Domain Layer (TodoApp.Domain)                    │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────────────┐  │  │
│  │  │ Entities  │ │ Enums    │ │ Interfaces        │  │  │
│  │  │(TodoItem, │ │(Priority,│ │(IRepository,      │  │  │
│  │  │ Category) │ │ Status)  │ │ IUnitOfWork)      │  │  │
│  │  └──────────┘ └──────────┘ └──────────────────┘  │  │
│  └──────────────┼────────────────────────────────────┘  │
│                 │                                        │
│  ┌──────────────┼────────────────────────────────────┐  │
│  │  Infrastructure Layer (TodoApp.Infrastructure)    │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────────────┐  │  │
│  │  │ DbContext │ │Repos.   │ │Background Job    │  │  │
│  │  │(Postgres) │ │(EF Core)│ │(Cleanup Service) │  │  │
│  │  └──────────┘ └──────────┘ └──────────────────┘  │  │
│  └──────────────┼────────────────────────────────────┘  │
└─────────────────┼────────────────────────────────────────┘
                  │
           ┌──────┴──────┐
           │  PostgreSQL  │
           └─────────────┘
```

### Component Responsibilities

| Layer | Responsibility |
|-------|---------------|
| **API** | HTTP endpoint exposure, auth enforcement, request validation, response formatting (FluentResponse.ApiWrapper + Scalar), middleware pipeline (logging, correlation ID, rate limiting, exception handling) |
| **Application** | CQRS command/query handlers, business orchestration, mapping (Cartographer.Mapper between domain entities and DTOs), pipeline behaviors (FluentValidation, logging, performance monitoring) |
| **Domain** | Core entities (TodoItem, Category), value objects, enums (Priority, Status), repository interfaces, domain validation rules, state machine enforcement |
| **Infrastructure** | EF Core DbContext + migrations, repository implementations, PostgreSQL-specific query optimizations, background cleanup job, date/time provider abstraction |
| **Frontend** | Smart components (stateful, data fetching), presentational components (pure rendering), services (HttpClient wrappers), shared validators, notification service (MatSnackBar wrapper) |

### Dependency Flow

```
API → Application → Domain (interfaces)
API → Infrastructure (DI registration)
Infrastructure → Domain (implements interfaces)
Application → Domain (uses interfaces, entities)
Frontend → API (HTTP calls only)
```

All dependencies point inward toward the Domain layer (Clean Architecture / Dependency Inversion per Constitution Principle I).

### Data Flow (Create Todo Example)

```
User fills form → Angular validates (inline errors if invalid)
→ TodoService.createTodo() → HttpClient POST /api/v1/todos
→ [Auth middleware] → [Rate limiter] → [Logging middleware]
→ TodosController.Create()
→ ValidatorBehavior validates request
→ CreateTodoCommandHandler:
    → Validate business rules (title length, due date range, category exists)
    → Cartographer.Mapper: CreateTodoRequest → TodoItem
    → Repository.Add(todoItem)
    → UnitOfWork.SaveChanges()
    → Cartographer.Mapper: TodoItem → TodoItemResponse
    → Return TodoItemResponse via FluentResponse.ApiWrapper
→ Angular receives response → MatSnackBar success → Update list
```

---

## Solution Structure

### Backend — `api/`

```
api/
├── TodoApp.sln
├── src/
│   ├── TodoApp.Api/
│   │   ├── Controllers/
│   │   │   ├── TodosController.cs          # CRUD for todos
│   │   │   └── CategoriesController.cs     # CRUD for categories
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs  # ProblemDetails
│   │   │   ├── RequestLoggingMiddleware.cs     # Serilog + correlation ID
│   │   │   └── RateLimitingMiddleware.cs       # 100 req/min
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs
│   │   ├── Program.cs                      # DI, middleware pipeline, Scalar config
│   │   └── appsettings.json
│   ├── TodoApp.Application/
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── ITodoRepository.cs
│   │   │   │   ├── ICategoryRepository.cs
│   │   │   │   └── IUnitOfWork.cs
│   │   │   ├── Mappings/
│   │   │   │   ├── TodoMappingProfile.cs   # Cartographer.Mapper profile
│   │   │   │   └── CategoryMappingProfile.cs
│   │   │   └── Behaviors/
│   │   │       ├── ValidationBehavior.cs   # FluentValidation pipeline
│   │   │       ├── LoggingBehavior.cs      # Request/response logging
│   │   │       └── PerformanceBehavior.cs  # Query duration tracking
│   │   ├── Todos/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTodo/
│   │   │   │   │   ├── CreateTodoCommand.cs
│   │   │   │   │   ├── CreateTodoCommandHandler.cs
│   │   │   │   │   └── CreateTodoCommandValidator.cs
│   │   │   │   ├── UpdateTodo/
│   │   │   │   │   ├── UpdateTodoCommand.cs
│   │   │   │   │   ├── UpdateTodoCommandHandler.cs
│   │   │   │   │   └── UpdateTodoCommandValidator.cs
│   │   │   │   └── DeleteTodo/
│   │   │   │       ├── DeleteTodoCommand.cs
│   │   │   │       ├── DeleteTodoCommandHandler.cs
│   │   │   │       └── DeleteTodoCommandValidator.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetTodoList/
│   │   │   │   │   ├── GetTodoListQuery.cs
│   │   │   │   │   ├── GetTodoListQueryHandler.cs
│   │   │   │   │   └── GetTodoListQueryValidator.cs
│   │   │   │   └── GetTodoById/
│   │   │   │       ├── GetTodoByIdQuery.cs
│   │   │   │       ├── GetTodoByIdQueryHandler.cs
│   │   │   │       └── GetTodoByIdQueryValidator.cs
│   │   │   └── Dtos/
│   │   │       ├── TodoItemResponse.cs
│   │   │       ├── CreateTodoRequest.cs
│   │   │       └── UpdateTodoRequest.cs
│   │   └── Categories/
│   │       ├── Commands/
│   │       │   ├── CreateCategory/
│   │       │   ├── UpdateCategory/
│   │       │   └── DeleteCategory/
│   │       ├── Queries/
│   │       │   ├── GetCategoryListQuery.cs
│   │       │   └── GetCategoryListQueryHandler.cs
│   │       └── Dtos/
│   │           ├── CategoryResponse.cs
│   │           ├── CreateCategoryRequest.cs
│   │           └── UpdateCategoryRequest.cs
│   ├── TodoApp.Domain/
│   │   ├── Entities/
│   │   │   ├── TodoItem.cs
│   │   │   └── Category.cs
│   │   ├── Enums/
│   │   │   ├── Priority.cs
│   │   │   └── Status.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs              # Generic repository interface
│   │       └── ISoftDeletable.cs           # Soft-delete contract
│   └── TodoApp.Infrastructure/
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   ├── EntityConfigurations/
│       │   │   ├── TodoItemConfiguration.cs
│       │   │   └── CategoryConfiguration.cs
│       │   └── SeedData.cs                 # Uncategorized + default priorities/statuses
│       ├── Repositories/
│       │   ├── TodoRepository.cs
│       │   └── CategoryRepository.cs
│       ├── Services/
│       │   ├── DateTimeProvider.cs
│       │   └── TodoCleanupService.cs       # Background service for 30-day cleanup
│       └── Migrations/
└── tests/
    ├── TodoApp.UnitTests/
    │   ├── Application/
    │   │   ├── Todos/
    │   │   │   ├── CreateTodoCommandHandlerTests.cs
    │   │   │   ├── UpdateTodoCommandHandlerTests.cs
    │   │   │   ├── DeleteTodoCommandHandlerTests.cs
    │   │   │   └── GetTodoListQueryHandlerTests.cs
    │   │   └── Categories/
    │   │       ├── CreateCategoryCommandHandlerTests.cs
    │   │       ├── UpdateCategoryCommandHandlerTests.cs
    │   │       ├── DeleteCategoryCommandHandlerTests.cs
    │   │       └── GetCategoryListQueryHandlerTests.cs
    │   └── Domain/
    │       ├── TodoItemTests.cs
    │       └── CategoryTests.cs
    └── TodoApp.IntegrationTests/
        ├── Api/
        │   ├── TodoEndpointsTests.cs
        │   └── CategoryEndpointsTests.cs
        └── Infrastructure/
            └── TodoCleanupServiceTests.cs
```

### Frontend — `client/`

```
client/
├── src/
│   └── app/
│       ├── features/
│       │   ├── todos/
│       │   │   ├── todo-list/
│       │   │   │   ├── todo-list.component.ts        # Smart: manages state, fetch, filter
│       │   │   │   ├── todo-list.component.html
│       │   │   │   └── todo-list.component.scss
│       │   │   ├── todo-detail/
│       │   │   │   ├── todo-detail.component.ts      # Smart: loads single todo
│       │   │   │   ├── todo-detail.component.html
│       │   │   │   └── todo-detail.component.scss
│       │   │   ├── todo-form/
│       │   │   │   ├── todo-form.component.ts        # Dumb: form with validation
│       │   │   │   ├── todo-form.component.html
│       │   │   │   └── todo-form.component.scss
│       │   │   ├── todo-card/
│       │   │   │   ├── todo-card.component.ts        # Dumb: single todo display
│       │   │   │   ├── todo-card.component.html
│       │   │   │   └── todo-card.component.scss
│       │   │   └── filter-bar/
│       │   │       ├── filter-bar.component.ts       # Dumb: filter controls
│       │   │       ├── filter-bar.component.html
│       │   │       └── filter-bar.component.scss
│       │   └── categories/
│       │       ├── category-list/
│       │       │   ├── category-list.component.ts
│       │       │   ├── category-list.component.html
│       │       │   └── category-list.component.scss
│       │       ├── category-form/
│       │       │   ├── category-form.component.ts
│       │       │   ├── category-form.component.html
│       │       │   └── category-form.component.scss
│       │       └── category-card/
│       │           ├── category-card.component.ts
│       │           ├── category-card.component.html
│       │           └── category-card.component.scss
│       └── shared/
│           ├── components/
│           │   ├── confirm-dialog/
│           │   ├── empty-state/
│           │   ├── loading-spinner/
│           │   └── page-header/
│           ├── validators/
│           │   ├── required.validator.ts
│           │   ├── max-length.validator.ts
│           │   ├── hex-color.validator.ts
│           │   └── future-date.validator.ts
│           ├── services/
│           │   ├── todo.service.ts
│           │   ├── category.service.ts
│           │   └── notification.service.ts           # MatSnackBar wrapper
│           ├── models/
│           │   ├── todo.model.ts
│           │   ├── category.model.ts
│           │   ├── priority.model.ts
│           │   ├── status.model.ts
│           │   └── paginated-response.model.ts
│           └── constants/
│               ├── api-paths.ts
│               └── app-routes.ts
├── e2e/
│   ├── todo-crud.spec.ts
│   ├── category-management.spec.ts
│   └── search-filter.spec.ts
└── package.json
```

### State Management

Since this is a single-user application with no real-time collaboration, service-based state management (Angular services with `BehaviorSubject`) is sufficient per YAGNI (Constitution Principle V, Global Implementation Rules). No NgRx, Akita, or Signal Store required.

- `TodoService` holds the current todo list as a `BehaviorSubject<TodoItem[]>`
- `CategoryService` holds the category list as a `BehaviorSubject<Category[]>`
- Components subscribe to these subjects for reactive updates
- After any mutation (create/update/delete), the affected lists are refreshed from the API

---

## API Design

See [contracts/api-contracts.md](contracts/api-contracts.md) for full endpoint definitions.

### Summary

| Endpoint | Method | Purpose | Key Validation |
|----------|--------|---------|---------------|
| `/api/v1/todos` | GET | List with pagination, filtering, search | Query param types; pageSize max 50 |
| `/api/v1/todos/{id}` | GET | Get single todo | Guid format; 404 if not found |
| `/api/v1/todos` | POST | Create todo | Title required 1-200; dueDate <= 5yr; priorityId 1-4 |
| `/api/v1/todos/{id}` | PUT | Update todo | Same as create + statusId must be valid transition |
| `/api/v1/todos/{id}` | DELETE | Soft-delete todo | 404 if not found; sets DeletedAt |
| `/api/v1/categories` | GET | List all categories | No pagination (small dataset) |
| `/api/v1/categories` | POST | Create category | Name required 1-100 unique; color hex pattern |
| `/api/v1/categories/{id}` | PUT | Update category | Name unique excluding self |
| `/api/v1/categories/{id}` | DELETE | Delete with reassignment | Cannot delete Uncategorized (404 or 422) |

### Validation Strategy

- **Request DTOs**: Data annotations for basic rules (`[Required]`, `[MaxLength]`, `[RegularExpression]`)
- **FluentValidation validators**: Per-command/per-query validators registered in MediatR pipeline
- **Domain validation**: Entity-level validation in constructors and factory methods
- **Database constraints**: Unique indexes, foreign keys, check constraints via EF Core configuration

### Error Handling

Global exception middleware per Operational Standards:
1. Catch exception
2. Log with correlation ID and structured properties
3. Map to appropriate HTTP status + ProblemDetails response
4. Never expose stack traces in production

---

## Data Model Design

See [data-model.md](data-model.md) for full entity definitions.

### Entities

| Entity | Key Fields | Relationships |
|--------|-----------|---------------|
| **TodoItem** | Id (Guid, PK), Title (string, 1-200), Description (string?, 0-2000), DueDate (DateTime?), PriorityId (int, FK), CategoryId (Guid?, FK), StatusId (int, FK), CreatedAt, UpdatedAt, DeletedAt | Optional M:1 → Category; M:1 → Priority (enum); M:1 → Status (enum) |
| **Category** | Id (Guid, PK), Name (string, 1-100, unique), Color (string?, hex), CreatedAt | 1:M → TodoItem |
| **Priority** | Id (int, PK), Name (string), Color (string) | Enumeration table (seeded) |
| **Status** | Id (int, PK), Name (string), Color (string) | Enumeration table (seeded) |

### Key Constraints

- BR-001: Title 1-200 chars (check constraint `CK_Todos_TitleLength`)
- BR-003: DueDate <= 5 years from now (application-level validation)
- BR-004: "Uncategorized" category always exists (seed data)
- BR-005: Category name unique (unique index `IX_Categories_Name`)
- BR-006: Status transitions: Pending → In Progress → Completed (application-level)
- BR-007: Soft-delete via `DeletedAt`; hard-delete after 30 days (background job)
- BR-008: Priority 1-4, Status 1-3 (enum FK constraints)

### Business Validations

| Validation | Entity | Rule | Enforcement Layer |
|------------|--------|------|-------------------|
| Title length | TodoItem | 1-200 chars, trimmed | Frontend + Backend (FluentValidation) + DB (check constraint) |
| Description length | TodoItem | 0-2000 chars | Frontend + Backend |
| Due date range | TodoItem | <= 5 years from now | Frontend + Backend |
| Priority valid | TodoItem | Must exist in Priority enum | FK constraint |
| Status transition | TodoItem | P1→P2→P3 only | Application handler |
| Category exists | TodoItem | If set, must reference existing | FK constraint |
| Category name unique | Category | Case-insensitive unique | Unique index + Backend |
| Category name length | Category | 1-100 chars | Frontend + Backend |
| Color hex format | Category | `^#[0-9A-Fa-f]{6}$` | Frontend + Backend |

---

## UI/UX Design

### User Flow

```
Home / Todo List
  │
  ├── [Empty State] → "No todos yet. Create your first todo!"
  │                        └── [Create Todo] button
  │
  ├── [List View] → Todo cards showing title, priority badge, category chip,
  │                   status badge, due date (with overdue indicator)
  │   ├── Click card → [Detail View] → Full todo details
  │   │                              ├── [Edit] → Opens todo-form (pre-filled)
  │   │                              └── [Delete] → Confirm dialog → soft-delete
  │   ├── Filter bar → Status/Priority/Category dropdowns + Date range picker
  │   │                  + Search text input + [Apply] / [Clear] buttons
  │   └── Pagination controls at bottom
  │
  └── [+ Add Todo] FAB or button → Todo form (inline or dialog)
       └── [Create] → Success snackbar → list refreshes

Category Management
  │
  ├── [Category List] → Category cards showing colored name + todo count
  │   ├── Click [Edit] → Inline edit or dialog
  │   └── Click [Delete] → Confirm with warning about todo reassignment
  │
  └── [+ Add Category] → Form with name + color picker
```

### Navigation

- Top nav bar with links: "Todos" (default), "Categories"
- Angular Router: `/todos` (default), `/todos/:id`, `/categories`
- Breadcrumb navigation on detail/edit views

### Loading States

| State | Component | Visual |
|-------|-----------|--------|
| List loading | todo-list | MatSpinner overlay or skeleton cards (3-4 card placeholders with shimmer) |
| Form submission | todo-form | Submit button disabled + spinner icon |
| Detail loading | todo-detail | MatSpinner centered in card |
| Category list loading | category-list | MatSpinner |

### Empty States

| Scenario | Component | Message | CTA |
|----------|-----------|---------|-----|
| No todos exist | todo-list | "No todos yet. Create your first todo to get started!" | "Create Todo" button |
| No filter results | todo-list | "No todos match your filters. Try adjusting your criteria." | "Clear Filters" button |
| No categories | category-list | "No categories yet. Create one to organize your todos!" | "Create Category" button |

### Error States

| Scenario | Component | Behavior |
|----------|-----------|----------|
| API call fails | todo-list / todo-detail | Error snackbar (red) with message; preserved existing data |
| Validation error | todo-form / category-form | Inline MatError messages below each field; form not submitted |
| 404 on detail | todo-detail | "Todo not found" message + "Back to list" link |
| 409 on category create | category-form | Error snackbar "A category with this name already exists." |
| 422 on status change | todo-form | Error snackbar; status reverts to previous value |
| Network offline | All | Error snackbar "Unable to connect. Check your internet connection." |

### Success Notifications

| Action | Snackbar Message | Severity | Duration |
|--------|-----------------|----------|----------|
| Create todo | "Todo created successfully!" | Success (green) | 4s |
| Update todo | "Todo updated successfully!" | Success (green) | 4s |
| Delete todo | "Todo deleted successfully!" | Success (green) | 4s |
| Create category | "Category created successfully!" | Success (green) | 4s |
| Update category | "Category updated successfully!" | Success (green) | 4s |
| Delete category | "Category deleted. {n} todos moved to Uncategorized." | Warning (amber) | 6s |

### Validation Behavior

- All fields validate on **submit** (show all errors at once)
- Fields also validate on **blur** (show individual error as user tabs through)
- Errors clear when the user starts fixing the field
- Forms cannot be submitted while invalid
- Submit button is disabled until all required fields are filled and valid

### Responsive Behavior

- Breakpoints: sm (576px), md (768px), lg (992px), xl (1200px)
- Mobile: Single-column layout; filter bar collapses into expandable section; todo cards stack vertically
- Desktop: Multi-column grid for todo cards (2-3 columns); filter bar always visible in sidebar or top section
- Modals/dialogs used for forms on mobile instead of side panels

---

## Security Design

### Authentication

- JWT Bearer token required on all `/api/v1/*` endpoints
- Token obtained from external authentication system (assumed pre-existing, out of spec scope)
- Token validated on every request via ASP.NET Core `AddJwtBearer()` middleware
- Access token: 15-minute expiry; refresh token: 7-day expiry (handled by auth system)
- No authentication endpoints implemented in this feature

### Authorization

- Single-user application; role-based authorization not required for v1
- `[Authorize]` attribute applied at controller level for all endpoints
- Resource ownership implicit (single user); no user ID scoping needed

### Input Validation

| Layer | Strategy |
|-------|----------|
| **Frontend** | Angular Reactive Forms validators: required, minLength, maxLength, pattern (hex color), custom future-date validator |
| **API (Controller)** | ASP.NET `[ApiController]` automatic model validation + `ValidationFilter` |
| **API (Application)** | FluentValidation validators per command/query via MediatR pipeline behavior |
| **Infrastructure** | EF Core parameterized queries (no SQL injection); entity configuration value conversions |

### Data Protection

- All data transmitted over HTTPS
- No PII or sensitive data stored (task titles and descriptions only)
- Connection string with credentials stored in User Secrets (dev) or environment variables (prod) — never in source control
- Soft-delete preserves data for 30 days before hard-delete

### Rate Limiting

- 100 requests per minute per user (configurable in `appsettings.json`)
- Applied via custom middleware or ASP.NET Core built-in rate limiting (.NET 7+)
- Response header `X-RateLimit-Remaining` included in all responses
- 429 Too Many Requests with ProblemDetails when exceeded

### Logging Requirements

- All authentication failures logged as Warnings
- All validation failures logged as Warnings with field details
- All 4xx responses logged as Information
- All 5xx responses logged as Errors
- Rate limit exceeded events logged as Warnings

### Audit Considerations

- `CreatedAt` and `UpdatedAt` timestamps on all entities
- Soft-delete preserves deletion timestamp (`DeletedAt`)
- No full audit log of changes required for v1 (timestamps suffice)
- Application-level logging captures who performed which operation via correlation ID

---

## Performance Considerations

### Expected Response Times

| Endpoint | Target P95 | Max Acceptable |
|----------|-----------|----------------|
| GET /api/v1/todos (page 1, no filters) | 200ms | 500ms |
| GET /api/v1/todos (with filters) | 300ms | 500ms |
| GET /api/v1/todos (with search) | 400ms | 500ms |
| GET /api/v1/todos/{id} | 100ms | 200ms |
| POST /api/v1/todos | 200ms | 500ms |
| PUT /api/v1/todos/{id} | 200ms | 500ms |
| DELETE /api/v1/todos/{id} | 200ms | 500ms |
| GET /api/v1/categories | 50ms | 100ms |
| POST/PUT/DELETE categories | 100ms | 200ms |

### Pagination Strategy

- All list endpoints use offset-based pagination (`page` + `pageSize`)
- Page 1-indexed for user-friendly display
- Maximum pageSize: 50 (enforced server-side cap, PERF-001)
- Default pageSize: 20
- Response includes `totalCount` and `totalPages` for UI pagination controls
- `Skip`/`Take` applied in EF Core query (database-level pagination, not in-memory)

### Search Strategy

- Text search performed on `Title` and `Description` fields
- Case-insensitive via EF.Functions.ILike() (PostgreSQL)
- Search term applied after filters but before pagination
- No full-text search engine required for v1 (YAGNI)
- Minimum search term length: 1 character

### Filtering Strategy

- Filters applied server-side as SQL WHERE clauses before pagination
- Multiple filters combined with AND logic
- Each filter is optional; omitted filters are not applied
- Due date range: `>= dueDateFrom AND <= dueDateTo` (inclusive)
- Category filter: exact match on CategoryId
- Status/Priority: exact match on integer IDs

### Sorting Strategy

- Default sort: `CreatedAt DESC` (newest first)
- Future enhancement: sort by DueDate, Priority, Title (not required for v1, but EF Core query supports it)
- Sort order applied before pagination

### Caching Opportunities

| Cache Target | Strategy | Rationale |
|-------------|----------|-----------|
| Category list | In-memory cache on API (5-minute TTL) | Changes infrequently; read on every todo form |
| Priority/Status enums | Static in application memory | Never changes after seed |
| Todo list | No cache | Real-time accuracy needed; single-user scale doesn't justify complexity |
| Frontend API responses | Angular HTTP interceptor cache for category list | Redundant API calls when opening todo form |

### Database Optimization Considerations

| Optimization | Target | Approach |
|-------------|--------|----------|
| Indexes | Todo queries | Indexed columns: Title, DueDate, StatusId, PriorityId, CategoryId, CreatedAt (as defined in data-model.md) |
| Query projection | List endpoint | Select only required fields (not full entity) via `.Select()` before `.ToListAsync()` |
| N+1 prevention | Category name in todo list | Include Category via `.Include(t => t.Category)` |
| Connection pooling | General | Use default Npgsql connection pooling (PoolSize=100) |
| Query logging | Slow queries | Log queries exceeding 100ms as Warnings (NFR-004) |

---

## Logging & Monitoring

### Events to Log

| Event | Log Level | Structured Properties | Source |
|-------|-----------|----------------------|--------|
| Incoming request | Information | Method, Path, QueryString, CorrelationId, UserId | Middleware |
| Request complete | Information | Method, Path, StatusCode, Duration(ms), CorrelationId | Middleware |
| Database query > 100ms | Warning | Query, Duration(ms), CorrelationId | EF Core interceptor |
| Rate limit exceeded | Warning | ClientIp, Endpoint, CorrelationId | Rate limiting middleware |
| Validation failure | Warning | CommandType, Errors (list), CorrelationId | ValidationBehavior |
| Todo created | Information | TodoId, Title, CorrelationId | Command handler |
| Todo updated | Information | TodoId, FieldsChanged, CorrelationId | Command handler |
| Todo deleted (soft) | Information | TodoId, CorrelationId | Command handler |
| Category created | Information | CategoryId, Name, CorrelationId | Command handler |
| Category deleted | Information | CategoryId, TodosReassigned (count), CorrelationId | Command handler |
| Unhandled exception | Error | Exception type, Message, StackTrace, CorrelationId | Exception handler |
| Authentication failure | Warning | ClientIp, Reason, CorrelationId | JWT middleware |
| Cleanup job run | Information | RecordsDeleted, Duration(ms) | Background service |
| Cleanup job error | Error | Exception message | Background service |

### Correlation ID

- Generated as a GUID at the start of each HTTP request (middleware)
- Passed through the MediatR pipeline via `IRequest` interface or `IMessage` header
- Included in all structured log entries
- Returned in HTTP response header `X-Correlation-ID`
- Enables end-to-end tracing: Frontend → API → Database

### Error Logging

- All unhandled exceptions logged at Error level with full stack trace (development) or sanitized (production)
- Domain exceptions (validation failures, invalid transitions) logged at Warning level
- 4xx client errors logged at Information level (expected behavior)
- 5xx server errors logged at Error level
- Never log sensitive data (passwords, tokens, connection strings)

### Audit Logging

- Timestamp-based audit via `CreatedAt` / `UpdatedAt` entity fields
- No full change-tracking audit required for v1
- Application logs provide who (UserId) did what (action) and when (timestamp)

### Monitoring Points

| Monitor | Metric | Threshold | Action |
|---------|--------|-----------|--------|
| API response time | P95 duration | > 500ms | Investigate query/index |
| Error rate | % 5xx responses | > 1% | Alert on-call |
| Rate limit hits | Count/minute | > 50 | Review rate limit config |
| DB query duration | P95 duration | > 100ms | Add/optimize index |
| Soft-deleted records | Count | > 10000 | Check cleanup job |
| Active todos per user | Count | > 9000 | Warn approaching 10000 limit |

---

## Testing Strategy

### Behaviour-to-Test Mapping

| Scenario ID | Unit Test | Integration Test | E2E Test |
|-------------|-----------|------------------|----------|
| BH-001: Create todo (happy path) | CreateTodoCommandHandler: validates mapping, calls repository, returns response | POST /api/v1/todos returns 201 with correct body | Create todo via UI, verify in list |
| BH-002: Filter by multiple criteria | GetTodoListQueryHandler: verifies filter application and pagination | GET /api/v1/todos with filters returns correct subset | Apply filters in UI, verify results |
| BH-003: Empty title validation | CreateTodoCommandValidator: verifies title required rule | POST /api/v1/todos with empty title returns 400 | Submit empty form, verify inline error |
| BH-004: Invalid status transition | UpdateTodoCommandHandler: verifies transition rejection | PUT /api/v1/todos/{id} with invalid status returns 422 | Change Completed → Pending, verify error snackbar |
| BH-005: Max length title boundary | CreateTodoCommandValidator: verifies 200-char boundary | POST /api/v1/todos with 200-char title returns 201 | Fill exactly 200 chars, submit, verify |
| BH-006: Due date 5 years boundary | CreateTodoCommandValidator: verifies 5-year boundary | POST with dueDate at 5yr returns 201; 5yr+1day returns 400 | Set date picker to limit, verify behavior |
| BH-007: Delete category with todos | DeleteCategoryCommandHandler: verifies reassignment logic | DELETE /api/v1/categories/{id} → todos reassigned | Delete category, verify todos show Uncategorized |

### Test Types

| Test Type | Tool/Framework | Scope | Target Coverage |
|-----------|---------------|-------|----------------|
| **Unit Tests** | xUnit + Moq | Individual handlers, validators, domain logic, mapping profiles | >= 80% of Application + Domain layers |
| **Integration Tests** | WebApplicationFactory + TestContainers | Full API endpoint behavior with real PostgreSQL | All 9 endpoints; 200/400/401/404/409/422/500 status codes |
| **Component Tests** | Jasmine + Karma | Angular component rendering, form validation, state display | All smart + dumb components |
| **E2E Tests** | Playwright | Critical user journeys | 3 journeys: todo CRUD, category management, search/filter |

### What To Test

**Unit Tests (Backend)**:
- Command/Query handlers: verify correct repository calls, mapping, response construction
- Validators: verify each validation rule triggers correctly (valid and invalid inputs)
- Domain entities: verify state machine transitions, property constraints, factory methods
- Mapping profiles: verify Cartographer.Mapper configurations are valid via `AssertConfigurationIsValid()`

**Unit Tests (Frontend)**:
- Services: verify HTTP calls, error handling, response parsing
- Validators: verify each custom validator function
- Presentational components: verify rendering with given inputs
- Smart components: verify state management, data fetching on init

**Integration Tests (Backend)**:
- Each endpoint with valid input → expected success status + response shape
- Each endpoint with invalid input → expected error status + ProblemDetails shape
- Auth: no token → 401; expired token → 401
- Edge cases: delete non-existent → 404; duplicate category name → 409; invalid status transition → 422

**E2E Tests (Frontend)**:
- Full CRUD flow: create → view → edit → delete a todo with verification at each step
- Category management: create → assign → filter → delete with reassignment verification
- Search and filter: apply filters, verify results, clear filters, verify full list restored

---

## Traceability Matrix

| Requirement | Behaviour Scenario | Constraint(s) | Planned Component |
|-------------|-------------------|---------------|-------------------|
| FR-001 (Create todo) | BH-001, BH-003, BH-005, BH-006 | VAL-001, VAL-002, API-001 | TodosController.Create, CreateTodoCommandHandler, TodoFormComponent |
| FR-002 (List paginated) | BH-002 | PERF-001, API-001 | TodosController.List, GetTodoListQueryHandler, TodoListComponent |
| FR-003 (View detail) | — | API-001 | TodosController.GetById, GetTodoByIdQueryHandler, TodoDetailComponent |
| FR-004 (Update todo) | BH-004 | VAL-002, BR-006, API-001 | TodosController.Update, UpdateTodoCommandHandler, TodoFormComponent |
| FR-005 (Delete todo) | — | DB-001, API-001 | TodosController.Delete, DeleteTodoCommandHandler, ConfirmDialogComponent |
| FR-006 (Create category) | — | VAL-001, VAL-002, BR-005 | CategoriesController.Create, CreateCategoryCommandHandler, CategoryFormComponent |
| FR-007 (List categories) | — | API-001 | CategoriesController.List, GetCategoryListQueryHandler, CategoryListComponent |
| FR-008 (Update category) | — | VAL-002, BR-005 | CategoriesController.Update, UpdateCategoryCommandHandler, CategoryFormComponent |
| FR-009 (Delete category) | BH-007 | BR-004 | CategoriesController.Delete, DeleteCategoryCommandHandler, ConfirmDialogComponent |
| FR-010 (Filter todos) | BH-002 | PERF-001 | GetTodoListQueryHandler (filter params), FilterBarComponent |
| FR-011 (Search todos) | BH-002 | — | GetTodoListQueryHandler (search param) |
| FR-012 (Empty states) | — | UI-002 | EmptyStateComponent |
| FR-013 (Loading states) | — | UI-002 | LoadingSpinnerComponent |
| NFR-001 (Performance) | BH-002 | PERF-001 | GetTodoListQueryHandler, DB indexes |
| NFR-002 (Security) | — | SEC-001, AI-001 | JWT middleware, RateLimitingMiddleware |
| NFR-003 (Validation) | BH-003, BH-004, BH-005, BH-006 | VAL-001, VAL-002, ERR-001 | ValidationBehavior, FluentValidators, ReactiveForm validators |
| NFR-004 (Logging) | — | LOG-001 | RequestLoggingMiddleware, LoggingBehavior |
| NFR-005 (Scalability) | — | PERF-001 | Pagination, DB indexes |
| NFR-006 (Maintainability) | — | — | Clean Architecture, component separation |
| NFR-007 (Accessibility) | — | — | Angular Material accessibility, labels, keyboard nav |
| NFR-008 (Usability) | BH-001, BH-004, BH-007 | UI-001, UI-002 | NotificationService, MatSnackBar, inline validation |
| BR-001 (Title length) | BH-003, BH-005 | VAL-001, VAL-002 | CreateTodoCommandValidator, max-length.validator.ts |
| BR-002 (Description length) | — | VAL-002 | UpdateTodoCommandValidator |
| BR-003 (Due date range) | BH-006 | VAL-002 | CreateTodoCommandValidator, future-date.validator.ts |
| BR-004 (Uncategorized always exists) | BH-007 | — | SeedData, DeleteCategoryCommandHandler |
| BR-005 (Category name unique) | — | VAL-002 | CreateCategoryCommandValidator, IX_Categories_Name |
| BR-006 (Status transitions) | BH-004 | — | UpdateTodoCommandHandler (state machine check) |
| BR-007 (Soft-delete) | — | DB-001 | DeleteTodoCommandHandler, TodoCleanupService |
| BR-008 (Priority levels) | — | — | Priority enum, seed data |

---

## Planning Risk Assessment

| Risk ID | Category | Description | Priority | Mitigation |
|---------|----------|-------------|----------|------------|
| R-001 | Technical | Cartographer.Mapper may not support complex mapping scenarios (e.g., nested response objects, custom type converters) | Medium | Verify mapping capabilities during setup spike; fall back to AutoMapper if Cartographer.Mapper proves insufficient |
| R-002 | Technical | FluentResponse.ApiWrapper envelope may conflict with ProblemDetails middleware response format | Medium | Integration test both paths; configure FluentResponse.ApiWrapper to pass through ProblemDetails responses unmodified |
| R-003 | Technical | PostgreSQL full-text search (ILike) performance may degrade at 10k+ todos | Low | Add composite index on (Title, Description); consider trigram index (pg_trgm) if needed |
| R-004 | Security | JWT token validation configuration errors could expose endpoints | High | Integration test: verify 401 for missing/invalid/expired tokens; validate audience, issuer, signing key |
| R-005 | Performance | Without caching, category list is fetched on every todo form open | Low | Add 5-minute in-memory cache on category list endpoint |
| R-006 | Integration | Frontend-backend contract mismatch on FluentResponse.ApiWrapper envelope shape | Medium | Generate TypeScript types from OpenAPI spec; contract tests in CI |
| R-007 | Technical | Angular 21 + Angular Material compatibility issues | Low | Pin versions in package.json; verify compatibility before implementation |
| R-008 | Performance | Soft-delete cleanup job may overlap with active requests | Low | Run cleanup during low-traffic window; use efficient batch delete |
| R-009 | Technical | TestContainers for PostgreSQL may be slow in CI | Medium | Use pre-pulled container images; configure resource limits |
| R-010 | Security | Rate limiting at 100 req/min may be too restrictive for initial page load with multiple API calls | Medium | Audit initial page load API calls; increase limit or batch requests |

---

## Gate 2 Readiness Review

| Criterion | Status | Notes |
|-----------|--------|-------|
| Layer 3 — Behaviour complete | ✅ | 7 scenarios defined: 2 Happy Path, 2 Error, 2 Boundary, 1 Business Rule |
| Layer 4 — Constraints complete | ✅ | 11 constraints covering security, validation, logging, performance, error handling, API design, DB, UI, AI |
| Architecture defined | ✅ | Clean Architecture; component diagram; layer responsibilities; dependency flow; data flow |
| API design complete | ✅ | 9 endpoints defined in contracts/api-contracts.md with request/response DTOs, validation strategy, error handling |
| Data model defined | ✅ | 2 entities + 2 enums with fields, relationships, constraints, indexes in data-model.md |
| Security considerations documented | ✅ | JWT auth, input validation (3-layer), rate limiting, data protection, logging requirements |
| Performance considerations documented | ✅ | Response time targets, pagination strategy, search/filter/sort, caching, DB optimizations |
| Logging & monitoring documented | ✅ | Event table, correlation IDs, error/audit logging, monitoring metrics |
| Traceability established | ✅ | Full matrix linking: Requirement → Behaviour → Constraint → Planned Component |
| No specification drift | ✅ | All plan elements trace to spec requirements; no scope creep; all clarifications from /speckit.clarify reflected |

### Missing Prerequisites

- **Constitution approved**: ✅ v1.3.0 in effect
- **Specification approved**: ✅ spec.md Gate 1 passed
- **Clarification complete**: ✅ 3/3 questions resolved
- **Gate 1 passed**: ✅ No NEEDS CLARIFICATION markers

### Verdict

**✅ Gate 2 Ready**

The implementation plan is complete and ready for the `/tasks` phase. All required deliverables (Behaviour, Constraints, Architecture, Solution Structure, API Design, Data Model, UI/UX, Security, Performance, Logging, Testing Strategy, Traceability Matrix, Risk Assessment) are documented and traceable to the specification.

---

## Project Structure

```
specs/001-todo-management/
├── plan.md              # This file — comprehensive implementation plan
├── spec.md              # Feature specification (Gate 1 passed)
├── research.md          # Phase 0 research findings
├── data-model.md        # Entity definitions, relationships, validations
├── quickstart.md        # Validation guide with curl/test scenarios
├── contracts/
│   └── api-contracts.md # Full API endpoint contracts with request/response models
└── tasks.md             # Generated by /speckit.tasks (next step)
```
