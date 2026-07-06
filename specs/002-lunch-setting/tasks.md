# Tasks: Lunch Setting

**Input**: Design documents from `specs/002-lunch-setting/`

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

---

## Phase 1: Setup (Extend Existing Project)

**Purpose**: Initialize feature-specific project structure within the existing Clean Architecture solution

- [ ] T001 Create domain entity `LunchPreference.cs` with all fields per data-model.md at `api/src/TodoApp.Domain/Entities/LunchPreference.cs`
- [ ] T002 [P] Create `DietaryRestriction` enum with predefined values (None, Vegetarian, Vegan, Gluten-Free, Dairy-Free, Halal, Kosher, Nut-Free, Low-Carb, Diabetic) at `api/src/TodoApp.Domain/Enums/DietaryRestriction.cs`
- [ ] T003 [P] Create domain repository interface `ILunchPreferenceRepository` at `api/src/TodoApp.Domain/Interfaces/ILunchPreferenceRepository.cs`
- [ ] T004 [P] Create application repository interface `ILunchPreferenceRepository` at `api/src/TodoApp.Application/Common/Interfaces/ILunchPreferenceRepository.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core data layer and configuration that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T005 Create EF Core entity configuration for `LunchPreference` (table mapping, column types, indexes, constraints) at `api/src/TodoApp.Infrastructure/Data/EntityConfigurations/LunchPreferenceConfiguration.cs`
- [ ] T006 [P] Extend `AppDbContext` with `LunchPreference` DbSet and apply configuration at `api/src/TodoApp.Infrastructure/Data/AppDbContext.cs`
- [ ] T007 [P] Extend `SeedData` with dietary restriction seed values and organization default preferences (lunch window 12:00-13:00, 60min break, notifications enabled) at `api/src/TodoApp.Infrastructure/Data/SeedData.cs`
- [ ] T008 [P] Create `LunchPreferenceRepository` implementing `ILunchPreferenceRepository` (CRUD + get-or-create pattern) at `api/src/TodoApp.Infrastructure/Repositories/LunchPreferenceRepository.cs`
- [ ] T009 Register `LunchPreferenceRepository` and `ILunchPreferenceRepository` in `api/src/TodoApp.Infrastructure/DependencyInjection.cs`
- [ ] T010 Create `Cartographer.Mapper` mapping profile for `LunchPreference <-> LunchPreferenceResponse` at `api/src/TodoApp.Application/Common/Mappings/LunchPreferenceMappingProfile.cs`
- [ ] T011 Create request/response DTOs (`LunchPreferenceResponse`, `UpdateLunchPreferenceRequest`) at `api/src/TodoApp.Application/LunchPreferences/Dtos/`

**Checkpoint**: Foundation ready — user story implementation can now begin

---

## Phase 3: User Story 1 — Manage Lunch Preferences (Priority: P1) 🎯 MVP

**Goal**: Core CRUD operations for lunch preference profiles — get, update, reset to defaults

**Independent Test**: A user can GET their lunch preferences (auto-creates default if new), PUT to update any field, DELETE to reset to defaults

### Tests for User Story 1 (OPTIONAL — only if tests requested) ⚠️

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation

- [ ] T012 [P] [US1] Unit test for `GetLunchPreferenceQueryHandler` (returns existing profile, auto-creates defaults for new user) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/GetLunchPreferenceQueryHandlerTests.cs`
- [ ] T013 [P] [US1] Unit test for `UpdateLunchPreferenceCommandHandler` (updates fields, validates dietary restrictions, handles partial update) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/UpdateLunchPreferenceCommandHandlerTests.cs`
- [ ] T014 [P] [US1] Unit test for `ResetLunchPreferenceCommandHandler` (resets to defaults, preserves user ID) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/ResetLunchPreferenceCommandHandlerTests.cs`
- [ ] T015 [P] [US1] Unit test for `UpdateLunchPreferenceCommandValidator` (validates time range, break duration, list sizes, dietary restrictions) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/UpdateLunchPreferenceCommandValidatorTests.cs`
- [ ] T016 [P] [US1] Integration test for lunch preference endpoints (GET/PUT/DELETE, auth, responses) at `api/tests/TodoApp.IntegrationTests/Api/LunchPreferenceEndpointsTests.cs`

### Implementation for User Story 1

- [ ] T017 [P] [US1] Create `GetLunchPreferenceQuery` and `GetLunchPreferenceQueryHandler` (with get-or-create default logic) at `api/src/TodoApp.Application/LunchPreferences/Queries/GetLunchPreference/`
- [ ] T018 [P] [US1] Create `UpdateLunchPreferenceCommand` and `UpdateLunchPreferenceCommandHandler` (partial update, returns updated profile) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/`
- [ ] T019 [P] [US1] Create `UpdateLunchPreferenceCommandValidator` (FluentValidation: time range, break duration, list sizes, dietary restrictions) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/UpdateLunchPreferenceCommandValidator.cs`
- [ ] T020 [P] [US1] Create `ResetLunchPreferenceCommand` and `ResetLunchPreferenceCommandHandler` (replaces with org defaults) at `api/src/TodoApp.Application/LunchPreferences/Commands/ResetLunchPreference/`
- [ ] T021 [P] [US1] Create `ResetLunchPreferenceCommandValidator` (basic validation) at `api/src/TodoApp.Application/LunchPreferences/Commands/ResetLunchPreference/ResetLunchPreferenceCommandValidator.cs`
- [ ] T022 [P] [US1] Create `GetLunchPreferenceQueryValidator` (basic validation) at `api/src/TodoApp.Application/LunchPreferences/Queries/GetLunchPreference/GetLunchPreferenceQueryValidator.cs`
- [ ] T023 [US1] Register MediatR handlers and application services in `api/src/TodoApp.Application/DependencyInjection.cs`
- [ ] T024 [US1] Implement `LunchPreferencesController` with GET, PUT, DELETE endpoints at `api/src/TodoApp.Api/Controllers/LunchPreferencesController.cs`

**Checkpoint**: User Story 1 fully functional — user can manage their lunch preference profile

---

## Phase 4: User Story 2 — Configure Lunch Schedule (Priority: P2)

**Goal**: Lunch time window configuration with validation (start < end, duration 15-120 min)

**Independent Test**: A user can set lunch start/end times and break duration; API rejects invalid time ranges with 400 or 422

### Tests for User Story 2 (OPTIONAL — only if tests requested) ⚠️

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation

- [ ] T025 [P] [US2] Unit test for time range validation (end before start, start after end, equal times) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/TimeRangeValidationTests.cs`
- [ ] T026 [P] [US2] Unit test for break duration validation (below 15, above 120, boundary values) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/BreakDurationValidationTests.cs`

### Implementation for User Story 2

- [ ] T027 [P] [US2] Add time range validation rules to `UpdateLunchPreferenceCommandValidator` (LunchEndTime > LunchStartTime) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/UpdateLunchPreferenceCommandValidator.cs`
- [ ] T028 [P] [US2] Add break duration validation rules to `UpdateLunchPreferenceCommandValidator` (15-120 min inclusive) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/UpdateLunchPreferenceCommandValidator.cs`
- [ ] T029 [P] [US2] Add default schedule fallback logic to `GetLunchPreferenceQueryHandler` (apply org defaults when times are null) at `api/src/TodoApp.Application/LunchPreferences/Queries/GetLunchPreference/GetLunchPreferenceQueryHandler.cs`
- [ ] T030 [P] [US2] Add cleared-time-fields handling to `UpdateLunchPreferenceCommandHandler` (reset to org defaults when fields are null) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/UpdateLunchPreferenceCommandHandler.cs`

**Checkpoint**: User Story 2 complete — schedule validation and defaults working

---

## Phase 5: User Story 3 — Manage Favorite Meals & Exclusions (Priority: P3)

**Goal**: Maintain favorite meals and excluded items lists with size limits and duplicate detection

**Independent Test**: A user can add/remove favorites and exclusions; API rejects overflow (>20 items) with 422 and duplicates with 409

### Tests for User Story 3 (OPTIONAL — only if tests requested) ⚠️

> **NOTE**: Write these tests FIRST, ensure they FAIL before implementation

- [ ] T031 [P] [US3] Unit test for favorites list size validation (max 20 items, boundary) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/FavoritesListValidationTests.cs`
- [ ] T032 [P] [US3] Unit test for exclusions list size validation (max 20 items, boundary) at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/ExclusionsListValidationTests.cs`
- [ ] T033 [P] [US3] Unit test for duplicate exclusion/favorite detection in handler at `api/tests/TodoApp.UnitTests/Application/LunchPreferences/DuplicateItemDetectionTests.cs`

### Implementation for User Story 3

- [ ] T034 [P] [US3] Add favorites/exclusions size validation to `UpdateLunchPreferenceCommandValidator` (max 20 items, non-empty strings, trimmed) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/UpdateLunchPreferenceCommandValidator.cs`
- [ ] T035 [P] [US3] Add dietary restrictions validation to `UpdateLunchPreferenceCommandValidator` (each value from seed list, max 10 items) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/UpdateLunchPreferenceCommandValidator.cs`
- [ ] T036 [US3] Add duplicate item detection to `UpdateLunchPreferenceCommandHandler` (return 409 Conflict for duplicate favorites/exclusions) at `api/src/TodoApp.Application/LunchPreferences/Commands/UpdateLunchPreference/UpdateLunchPreferenceCommandHandler.cs`

**Checkpoint**: User Story 3 complete — list management with all constraints enforced

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Database migration, integration verification, and final validation

- [ ] T037 [P] Generate and apply EF Core migration for `LunchPreferences` table at `api/src/TodoApp.Infrastructure/Migrations/`
- [ ] T038 [P] Register `LunchPreferencesController` routes and ensure Scalar OpenAPI docs include new endpoints at `api/src/TodoApp.Api/Program.cs`
- [ ] T039 [P] Add integration test for auto-create default on first GET (BH-003) at `api/tests/TodoApp.IntegrationTests/Api/LunchPreferenceEndpointsTests.cs`
- [ ] T040 [P] Add integration test for duplicate exclusion returning 409 (BH-007) at `api/tests/TodoApp.IntegrationTests/Api/LunchPreferenceEndpointsTests.cs`
- [ ] T041 [P] Add integration test for invalid dietary restriction returning 400 (BH-006) at `api/tests/TodoApp.IntegrationTests/Api/LunchPreferenceEndpointsTests.cs`
- [ ] T042 [P] Add integration test for favorites overflow returning 422 (BH-004) at `api/tests/TodoApp.IntegrationTests/Api/LunchPreferenceEndpointsTests.cs`
- [ ] T043 Run quickstart.md validation scenarios end-to-end
- [ ] T044 Code cleanup, final review, and verify all integration tests pass

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion — BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational phase completion
  - User stories are ordered by priority; US2 and US3 modify the same handler/validator files as US1
- **Polish (Phase 6)**: Depends on all user stories being complete

### Within Each User Story

- DTOs before commands/queries
- Commands/queries before handlers
- Validators before handlers
- All application layer complete before controller
- Story complete before moving to next priority

### Parallel Opportunities

- Phase 1: T002, T003, T004 can run in parallel with T001
- Phase 2: T006, T007, T008, T010, T011 can run in parallel with T005
- Phase 3: All test tasks (T012-T016) can run in parallel; all implementation tasks (T017-T024) where [P] marked can run in parallel
- Phase 4: T025-T026 parallel; T027-T030 can run in parallel
- Phase 5: T031-T033 parallel; T034-T035 parallel; T036 sequential after validators
- Phase 6: T037-T042 can run in parallel where [P] marked

---

## Parallel Example: User Story 1

```bash
# Launch all test tasks together (if tests requested):
Task: "Create GetLunchPreferenceQueryHandlerTests in api/tests/TodoApp.UnitTests/..."
Task: "Create UpdateLunchPreferenceCommandHandlerTests in api/tests/TodoApp.UnitTests/..."
Task: "Create ResetLunchPreferenceCommandHandlerTests in api/tests/TodoApp.UnitTests/..."

# Launch all commands/queries together:
Task: "Create GetLunchPreferenceQuery + Handler in api/src/TodoApp.Application/LunchPreferences/Queries/"
Task: "Create UpdateLunchPreferenceCommand + Handler in api/src/TodoApp.Application/LunchPreferences/Commands/"
Task: "Create ResetLunchPreferenceCommand + Handler in api/src/TodoApp.Application/LunchPreferences/Commands/"

# Create controller last:
Task: "Implement LunchPreferencesController in api/src/TodoApp.Api/Controllers/"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T004)
2. Complete Phase 2: Foundational (T005-T011)
3. Complete Phase 3: User Story 1 (T012-T024)
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. Add User Story 1 (Core CRUD) → Test independently → Deploy/Demo (**MVP!**)
3. Add User Story 2 (Schedule Validation) → Test independently → Deploy/Demo
4. Add User Story 3 (Favorites & Exclusions) → Test independently → Deploy/Demo
5. Add Polish (Phase 6) → Final validation

---

## Notes

- [P] tasks = different files, no dependencies on other tasks within same phase
- [US1/2/3] label maps task to specific user story for traceability
- Each user story is independently completable and testable via its own API endpoints (shared controller)
- Commit after each task or logical group (per conventional commits)
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same-file conflicts, cross-story dependencies that break independence
- All file paths follow the structure defined in `plan.md`
- US2 and US3 enhance the same `UpdateLunchPreferenceCommandValidator` and handler — sequential ordering ensures no conflicts
