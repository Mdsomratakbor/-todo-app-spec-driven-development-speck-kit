# Tasks: Todo Management System

**Input**: Design documents from `specs/001-todo-management/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/api-contracts.md

**Tests**: The phases below include optional test tasks. Only generate test tasks if explicitly requested.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on one another within the same phase)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `api/src/TodoApp.{Api,Application,Domain,Infrastructure}/`
- **Backend tests**: `api/tests/TodoApp.{UnitTests,IntegrationTests}/`
- **Frontend**: `client/src/app/features/{todos,categories}/`
- **Frontend shared**: `client/src/app/shared/`
- **E2E**: `client/e2e/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create .NET 10 solution with Clean Architecture projects (Api, Application, Domain, Infrastructure) in `api/TodoApp.slnx`
- [X] T002 [P] Initialize Angular 21 project with Angular Material and Bootstrap in `client/`
- [X] T003 [P] Add NuGet packages: EF Core (PostgreSQL), Cartographer.Mapper, FluentResponse.ApiWrapper, FluentValidation, Serilog, Scalar.AspNetCore, MediatR to `api/src/TodoApp.Api/TodoApp.Api.csproj`
- [X] T004 [P] Add npm packages: @angular/material, @angular/cdk, bootstrap, @playwright/test to `client/package.json`
- [X] T005 Configure .editorconfig, .gitignore, and directory structure for `api/` and `client/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T006 [P] Configure EF Core DbContext with PostgreSQL connection string in `api/src/TodoApp.Infrastructure/Data/AppDbContext.cs`
- [X] T007 [P] Implement MediatR pipeline with FluentValidation behavior (ValidationBehavior) in `api/src/TodoApp.Application/Common/Behaviors/ValidationBehavior.cs`
- [ ] T008 [P] Configure FluentResponse.ApiWrapper response envelope middleware in `api/src/TodoApp.Api/Program.cs`
- [ ] T009 [P] Implement global exception handler with ProblemDetails (RFC 7807) in `api/src/TodoApp.Api/Middleware/ExceptionHandlingMiddleware.cs`
- [ ] T010 [P] Configure Serilog structured logging with correlation ID middleware in `api/src/TodoApp.Api/Middleware/RequestLoggingMiddleware.cs` and `api/src/TodoApp.Api/Program.cs`
- [ ] T011 [P] Configure Scalar OpenAPI documentation in `api/src/TodoApp.Api/Program.cs`
- [ ] T012 [P] Configure JWT Bearer authentication middleware in `api/src/TodoApp.Api/Program.cs`
- [ ] T013 [P] Configure rate limiting middleware (100 req/min) in `api/src/TodoApp.Api/Middleware/RateLimitingMiddleware.cs`
- [ ] T014 [P] Create domain entities (TodoItem, Category) and enums (Priority, Status) in `api/src/TodoApp.Domain/Entities/` and `api/src/TodoApp.Domain/Enums/`
- [ ] T015 [P] Create EF Core entity configurations and seed data (priorities, statuses, Uncategorized category) in `api/src/TodoApp.Infrastructure/Data/EntityConfigurations/` and `api/src/TodoApp.Infrastructure/Data/SeedData.cs`
- [ ] T016 Create and apply initial database migration in `api/src/TodoApp.Infrastructure/Migrations/`

**Checkpoint**: Foundation ready — user story implementation can now begin in parallel

---

## Phase 3: User Story 1 — Manage Todo Items (Priority: P1) 🎯 MVP

**Goal**: CRUD operations for todo items — create, list, view detail, update, delete

**Independent Test**: Create a todo via POST `/api/v1/todos`, verify it appears in GET list; update a field via PUT; delete it via DELETE; verify it no longer appears in list

### Tests for User Story 1 (OPTIONAL — only if tests requested) ⚠️

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation

- [ ] T017 [P] [US1] Unit test for CreateTodoCommandHandler in `api/tests/TodoApp.UnitTests/Application/Todos/CreateTodoCommandHandlerTests.cs`
- [ ] T018 [P] [US1] Unit test for UpdateTodoCommandHandler in `api/tests/TodoApp.UnitTests/Application/Todos/UpdateTodoCommandHandlerTests.cs`
- [ ] T019 [P] [US1] Unit test for GetTodoListQueryHandler in `api/tests/TodoApp.UnitTests/Application/Todos/GetTodoListQueryHandlerTests.cs`
- [ ] T020 [P] [US1] Integration test for todo endpoints (CRUD) in `api/tests/TodoApp.IntegrationTests/Api/TodoEndpointsTests.cs`
- [ ] T021 [P] [US1] Component tests for TodoListComponent, TodoFormComponent in `client/src/app/features/todos/`

### Implementation for User Story 1

- [ ] T022 [P] [US1] Create ITodoRepository interface in `api/src/TodoApp.Application/Common/Interfaces/ITodoRepository.cs`
- [ ] T023 [P] [US1] Implement TodoRepository in `api/src/TodoApp.Infrastructure/Repositories/TodoRepository.cs`
- [ ] T024 [US1] Create Cartographer.Mapper TodoMappingProfile in `api/src/TodoApp.Application/Common/Mappings/TodoMappingProfile.cs`
- [ ] T025 [P] [US1] Create CreateTodo command, handler, and validator in `api/src/TodoApp.Application/Todos/Commands/CreateTodo/`
- [ ] T026 [P] [US1] Create UpdateTodo command, handler, and validator in `api/src/TodoApp.Application/Todos/Commands/UpdateTodo/`
- [ ] T027 [P] [US1] Create DeleteTodo command and handler in `api/src/TodoApp.Application/Todos/Commands/DeleteTodo/`
- [ ] T028 [P] [US1] Create GetTodoList query and handler in `api/src/TodoApp.Application/Todos/Queries/GetTodoList/`
- [ ] T029 [P] [US1] Create GetTodoById query and handler in `api/src/TodoApp.Application/Todos/Queries/GetTodoById/`
- [ ] T030 [US1] Implement TodosController with all 5 endpoints in `api/src/TodoApp.Api/Controllers/TodosController.cs`
- [ ] T031 [P] [US1] Create Angular request/response models in `client/src/app/shared/models/todo.model.ts`, `category.model.ts`, `priority.model.ts`, `status.model.ts`, `paginated-response.model.ts`
- [ ] T032 [P] [US1] Create TodoService (HttpClient wrapper) in `client/src/app/shared/services/todo.service.ts`
- [ ] T033 [P] [US1] Create NotificationService (MatSnackBar wrapper) in `client/src/app/shared/services/notification.service.ts`
- [ ] T034 [P] [US1] Create shared validators (required, maxLength, hexColor, futureDate) in `client/src/app/shared/validators/`
- [ ] T035 [P] [US1] Create shared components: LoadingSpinner, EmptyState, ConfirmDialog in `client/src/app/shared/components/`
- [ ] T036 [P] [US1] Create TodoFormComponent (dumb, with Angular Reactive Forms validation) in `client/src/app/features/todos/todo-form/`
- [ ] T037 [P] [US1] Create TodoCardComponent (dumb, status/priority badges, category chip) in `client/src/app/features/todos/todo-card/`
- [ ] T038 [P] [US1] Create TodoListComponent (smart, stateful with pagination) in `client/src/app/features/todos/todo-list/`
- [ ] T039 [P] [US1] Create TodoDetailComponent (smart, loads single todo) in `client/src/app/features/todos/todo-detail/`
- [ ] T040 [US1] Wire up Angular routing, navigation, and app module registration in `client/src/app/app-routes.ts` and `client/src/app/app.module.ts`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 — Organize Todos with Categories (Priority: P2)

**Goal**: CRUD for categories, category selector on todo form, filter todos by category, delete with reassignment

**Independent Test**: Create a category via POST `/api/v1/categories`; assign it to a todo; filter by that category; delete the category; verify todos moved to Uncategorized

### Tests for User Story 2 (OPTIONAL — only if tests requested) ⚠️

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation

- [ ] T041 [P] [US2] Unit test for CreateCategoryCommandHandler in `api/tests/TodoApp.UnitTests/Application/Categories/CreateCategoryCommandHandlerTests.cs`
- [ ] T042 [P] [US2] Unit test for DeleteCategoryCommandHandler (reassignment logic) in `api/tests/TodoApp.UnitTests/Application/Categories/DeleteCategoryCommandHandlerTests.cs`
- [ ] T043 [P] [US2] Integration test for category endpoints in `api/tests/TodoApp.IntegrationTests/Api/CategoryEndpointsTests.cs`
- [ ] T044 [US2] Component tests for CategoryListComponent, CategoryFormComponent in `client/src/app/features/categories/`

### Implementation for User Story 2

- [ ] T045 [P] [US2] Create ICategoryRepository interface in `api/src/TodoApp.Application/Common/Interfaces/ICategoryRepository.cs`
- [ ] T046 [P] [US2] Implement CategoryRepository in `api/src/TodoApp.Infrastructure/Repositories/CategoryRepository.cs`
- [ ] T047 [P] [US2] Create Cartographer.Mapper CategoryMappingProfile in `api/src/TodoApp.Application/Common/Mappings/CategoryMappingProfile.cs`
- [ ] T048 [P] [US2] Create CreateCategory command, handler, and validator in `api/src/TodoApp.Application/Categories/Commands/CreateCategory/`
- [ ] T049 [P] [US2] Create UpdateCategory command, handler, and validator in `api/src/TodoApp.Application/Categories/Commands/UpdateCategory/`
- [ ] T050 [P] [US2] Create DeleteCategory command and handler (with todo reassignment logic) in `api/src/TodoApp.Application/Categories/Commands/DeleteCategory/`
- [ ] T051 [P] [US2] Create GetCategoryList query and handler in `api/src/TodoApp.Application/Categories/Queries/GetCategoryList/`
- [ ] T052 [US2] Implement CategoriesController with all 4 endpoints in `api/src/TodoApp.Api/Controllers/CategoriesController.cs`
- [ ] T053 [P] [US2] Create CategoryService in `client/src/app/shared/services/category.service.ts`
- [ ] T054 [P] [US2] Create CategoryCardComponent (dumb) in `client/src/app/features/categories/category-card/`
- [ ] T055 [P] [US2] Create CategoryFormComponent (dumb, with validation) in `client/src/app/features/categories/category-form/`
- [ ] T056 [P] [US2] Create CategoryListComponent (smart) in `client/src/app/features/categories/category-list/`
- [ ] T057 [US2] Integrate category selector dropdown into TodoFormComponent (reuse CategoryService) in `client/src/app/features/todos/todo-form/`
- [ ] T058 [US2] Wire up category management routes and navigation in `client/src/app/app-routes.ts`

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 — Search and Filter Todos (Priority: P3)

**Goal**: Filter todos by status, priority, category, due date range; text search on title/description; clear filters

**Independent Test**: Create todos with different statuses/priorities/categories; apply multiple filters simultaneously; verify filtered results; clear filters; verify full list restored

### Tests for User Story 3 (OPTIONAL — only if tests requested) ⚠️

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation

- [ ] T059 [P] [US3] Integration test for filtering + search query parameters in `api/tests/TodoApp.IntegrationTests/Api/TodoEndpointsTests.cs`
- [ ] T060 [US3] Component tests for FilterBarComponent in `client/src/app/features/todos/filter-bar/`

### Implementation for User Story 3

- [ ] T061 [P] [US3] Add filtering and search query parameters to GetTodoListQuery in `api/src/TodoApp.Application/Todos/Queries/GetTodoList/GetTodoListQuery.cs`
- [ ] T062 [US3] Implement filtering logic (statusId, priorityId, categoryId, dueDateFrom, dueDateTo) and text search (ILike on Title/Description) in GetTodoListQueryHandler at `api/src/TodoApp.Application/Todos/Queries/GetTodoList/GetTodoListQueryHandler.cs`
- [ ] T063 [P] [US3] Create FilterBarComponent (dumb, dropdowns + date picker + search input) in `client/src/app/features/todos/filter-bar/`
- [ ] T064 [US3] Integrate FilterBarComponent with TodoListComponent (pass filter values to GetTodoListQuery) in `client/src/app/features/todos/todo-list/`
- [ ] T065 [US3] Implement clear-filters functionality (resets all filters, reloads full list) in `client/src/app/features/todos/todo-list/`

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T066 [P] Implement soft-delete cleanup background service (30-day hard-delete) in `api/src/TodoApp.Infrastructure/Services/TodoCleanupService.cs`
- [ ] T067 [P] Add performance logging behavior (log queries exceeding 100ms) in `api/src/TodoApp.Application/Common/Behaviors/PerformanceBehavior.cs`
- [ ] T068 [P] Add UI polish: loading skeletons for todo list, transitions, empty-state illustrations in `client/src/app/shared/components/`
- [ ] T069 [P] Create E2E tests for todo CRUD flow in `client/e2e/todo-crud.spec.ts`
- [ ] T070 [P] Create E2E tests for category management in `client/e2e/category-management.spec.ts`
- [ ] T071 [P] Create E2E tests for search and filter in `client/e2e/search-filter.spec.ts`
- [ ] T072 Add WCAG 2.1 AA accessibility attributes (labels, aria, keyboard nav) across all components
- [ ] T073 Run quickstart.md validation scenarios end-to-end
- [ ] T074 Code cleanup, final review, and README update

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion — BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational phase completion
  - User stories can proceed in parallel (if staffed) or sequentially in priority order
- **Polish (Phase 6)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1) Todo CRUD**: No dependencies on other stories — MVP
- **User Story 2 (P2) Categories**: Integrates with US1 (category selector on todo form) but independently testable via dedicated endpoints + category management page
- **User Story 3 (P3) Search/Filter**: Modifies US1's list query and TodoListComponent — best implemented after or in parallel with US1

### Within Each User Story

- Models/entities before services/repositories
- Services/repositories before handlers
- Handlers before controllers
- Backend endpoints before frontend integration
- Story complete before moving to next priority

### Parallel Opportunities

- Phase 1: T002, T003, T004 can run in parallel with T001, T005
- Phase 2: T006-T015 all independent and can run in parallel
- Phase 3-5: Once Foundational is complete, all 3 stories can start in parallel
- Within each story: [P] tasks can run in parallel
- Multiple stories can be worked on by different team members simultaneously

---

## Parallel Example: User Story 1

```bash
# Launch all models + interfaces together:
Task: "Create ITodoRepository interface in api/src/TodoApp.Application/Common/Interfaces/ITodoRepository.cs"
Task: "Create Angular models in client/src/app/shared/models/todo.model.ts"

# Launch all backend commands/queries together:
Task: "Create CreateTodo in api/src/TodoApp.Application/Todos/Commands/CreateTodo/"
Task: "Create UpdateTodo in api/src/TodoApp.Application/Todos/Commands/UpdateTodo/"
Task: "Create GetTodoList in api/src/TodoApp.Application/Todos/Queries/GetTodoList/"

# Launch all frontend components together:
Task: "Create TodoFormComponent in client/src/app/features/todos/todo-form/"
Task: "Create TodoCardComponent in client/src/app/features/todos/todo-card/"
Task: "Create TodoListComponent in client/src/app/features/todos/todo-list/"
```

## Parallel Example: User Story 2

```bash
# Launch all models + interfaces together:
Task: "Create ICategoryRepository in api/src/TodoApp.Application/Common/Interfaces/ICategoryRepository.cs"
Task: "Create CategoryService in client/src/app/shared/services/category.service.ts"

# Launch all backend commands/queries together:
Task: "Create CreateCategory in api/src/TodoApp.Application/Categories/Commands/CreateCategory/"
Task: "Create UpdateCategory in api/src/TodoApp.Application/Categories/Commands/UpdateCategory/"
Task: "Create DeleteCategory in api/src/TodoApp.Application/Categories/Commands/DeleteCategory/"

# Launch all frontend components together:
Task: "Create CategoryListComponent in client/src/app/features/categories/category-list/"
Task: "Create CategoryFormComponent in client/src/app/features/categories/category-form/"
Task: "Create CategoryCardComponent in client/src/app/features/categories/category-card/"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001–T005)
2. Complete Phase 2: Foundational (T006–T016) — CRITICAL: blocks all stories
3. Complete Phase 3: User Story 1 (T017–T040)
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add User Story 1 (Todo CRUD) → Test independently → Deploy/Demo (**MVP!**)
3. Add User Story 2 (Categories) → Test independently → Deploy/Demo
4. Add User Story 3 (Search/Filter) → Test independently → Deploy/Demo
5. Add Polish (Phase 6) → Final validation

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - **Developer A**: User Story 1 (Todo CRUD) — Todo CRUD backend + frontend
   - **Developer B**: User Story 2 (Categories) — Category CRUD backend + frontend
   - **Developer C**: User Story 3 (Search/Filter) — Filter bar + query logic
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies on other tasks within same phase
- [US1/2/3] label maps task to specific user story for traceability
- Each user story is independently completable and testable via its own API endpoints
- Commit after each task or logical group (per conventional commits)
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same-file conflicts, cross-story dependencies that break independence
- All file paths follow the structure defined in `plan.md`
