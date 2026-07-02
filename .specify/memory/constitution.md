<!--
  Sync Impact Report
  Version change: 1.5.0 → 1.6.0
  Modified principles:
    - Principle V: Expanded Global Implementation Rules from 5 to 10
      comprehensive rules (Specification First, Scope Control, Respect
      Dependencies, Code Quality, Project Standards, Safety Rules,
      Validation, Traceability, Completion Report, Approval Gate)
  Added sections: None
  Removed sections: None
  Templates requiring updates:
    - .specify/templates/plan-template.md: ✅ No changes needed
    - .specify/templates/spec-template.md: ✅ No changes needed
    - .specify/templates/tasks-template.md: ✅ No changes needed
    - .specify/templates/checklist-template.md: ✅ No changes needed
    - .specify/templates/constitution-template.md: ✅ No changes needed
  Follow-up TODOs: None
-->
# TodoApp Constitution

## Core Principles

### I. Architecture & Design First

Architecture decisions MUST be documented before implementation. The system
follows a clean client-server architecture: a .NET 10 Web API backend with
Entity Framework Core and PostgreSQL, and an Angular 21 SPA frontend with
Angular Material and Bootstrap.

- Backend MUST follow Clean Architecture layers: API, Application, Domain,
  and Infrastructure
- Frontend MUST follow component-based architecture with smart/container
  and presentational/dumb component separation
- Frontend MUST use Angular Material `MatSnackBar` for user notifications:
  errors, warnings, and success responses, each with distinct visual
  styling per severity level
- Every form field MUST have client-side validation with informative,
  human-readable error messages displayed inline on touch or submit
- UI design MUST be informative: clear status indicators, loading states,
  empty-state illustrations, and contextual help for all user-facing
  actions
- Coding standards MUST enforce async/await for all I/O operations,
  dependency injection throughout, and interface-based abstractions for
  testability
- Backend MUST use `Cartographer.Mapper` for object-to-object mapping and
  `FluentResponse.ApiWrapper` for consistent API response envelopes
- All public API endpoints MUST return consistent response envelopes
- API versioning MUST use URL path prefix (`/api/v{version}/`) for all
  endpoints to enable backward-compatible evolution
- API documentation MUST be served via Scalar (replaces Swagger UI) using
  the OpenAPI 3.x specification generated from the .NET Web API
- No business logic in presentation or infrastructure layers

### II. Naming & Structure Conventions

All naming and structure decisions MUST follow the rules below:

- **Folder structure**: Backend lives under `api/` root with Clean Architecture
  layers in `api/src/TodoApp.{Layer}/` (Api, Application, Domain,
  Infrastructure). Frontend lives under `client/` root with features in
  `client/src/app/features/{feature}/` and shared code in
  `client/src/app/shared/`.
- **File naming**: C# files use PascalCase (`TodoController.cs`). Angular
  files use kebab-case (`todo-list.component.ts`).
- **API naming**: RESTful conventions with plural nouns under versioned
  prefix (`/api/v1/todos`, `/api/v1/todos/{id}`). HTTP verbs: GET = read,
  POST = create, PUT = replace, PATCH = partial update, DELETE = remove.
- **Database naming**: Tables in PascalCase plural (`Todos`, `Categories`).
  Columns in PascalCase singular (`CreatedAt`, `IsCompleted`). Primary keys
  named `Id`. Foreign keys named `{RelatedEntity}Id` (`CategoryId`). Indexes
  named `IX_{Table}_{Column}`. Constraints named `CK_{Table}_{Column}`.

### III. Git & Development Workflow

- **Git workflow**: Trunk-based development with short-lived feature branches.
  Main branch MUST always be deployable.
- **Branch naming**: `type/short-description` (kebab-case). Examples:
  `feat/add-todo-crud`, `fix/title-validation`, `refactor/extract-service`.
- **Conventional Commits**: Every commit MUST follow the format
  `type(scope): description` where type is `feat`, `fix`, `docs`, `style`,
  `refactor`, `test`, or `chore`. Scope is optional (`api`, `web`, `db`).
- **Pull Requests**: MUST include a descriptive title, linked issue reference,
  summary of changes, testing notes, and screenshots for UI changes. All PRs
  require at least one approval and a passing CI pipeline before merge. Squash
  merge is the default strategy.

### IV. Quality & Testing Strategy

- **Error handling**: Global exception middleware returning Problem Details
  (RFC 7807). Never expose stack traces in production. Categorize errors as
  client (4xx), server (5xx), or domain (custom).
- **Notifications**: Frontend MUST use Angular Material `MatSnackBar` for
  all user-facing notifications: error (red), warning (amber), and success
  or informational responses (green/blue). Notifications MUST be informative
  with clear messages and actionable context.
- **Logging**: Structured logging via `ILogger<T>` using Serilog. Log levels:
  Trace (noisy debug), Debug (development info), Information (business events),
  Warning (unexpected but handled), Error (failures), Fatal (unrecoverable).
  Every request includes a correlation ID.
- **Validation**: All user input fields MUST have client-side validation
  before submission. Backend uses FluentValidation with a MediatR pipeline
  behavior. Frontend uses Angular Reactive Forms with per-field validators
  and informative error messages displayed inline.
  Validate at every boundary: controller, service, repository.
- **Testing**: Unit tests for all services and utilities (xUnit + Moq on
  backend; Jasmine/Karma on frontend). Integration tests for API endpoints
  (WebApplicationFactory + TestContainers for PostgreSQL). E2E tests for
  critical user journeys (Playwright). Test coverage MUST be >= 80%.

### V. Security & AI Collaboration

**Security**:
- HTTPS enforced at all times. JWT-based authentication with refresh tokens.
- Role-based authorization for protected endpoints.
- Input sanitization on all user-supplied data.
- SQL injection prevented via EF Core parameterized queries.
- CORS configured to allow only the Angular dev server origin in development.
- Rate limiting on public endpoints (100 requests/minute default).

**AI Collaboration**:
- AI MUST never generate or expose credentials, secrets, or tokens.
- AI MUST justify every architectural decision with a rationale.
- AI MUST follow existing code patterns and conventions in the project.
- AI MUST produce or update tests alongside every implementation change.
- AI MUST flag security concerns when introducing new dependencies or patterns.

**Code Review Checklist**:
- [ ] SOLID principles respected
- [ ] Error handling covers all failure paths
- [ ] No hardcoded values (use configuration)
- [ ] Logging present for key operations
- [ ] Tests cover the change adequately
- [ ] Security constraints applied (auth, validation, sanitization)
- [ ] UI displays informative feedback (notifications, validation errors,
      loading/empty states) for every user action
- [ ] No TODO or FIXME left without a tracking reference

**Definition of Done**:
- Code is complete and compiles without warnings
- All tests pass (unit + integration + E2E)
- Code reviewed and approved by at least one peer
- Documentation updated (if applicable)
- No known regressions
- Deployed to staging environment

**Global Implementation Rules**:

These rules apply to **every implementation task** unless explicitly overridden by the approved specification.

1. **Specification First** — Follow the approved spec, plan, and task documents. Never make undocumented assumptions. If any requirement is ambiguous, stop and ask for clarification before implementing.

2. **Scope Control** — Implement **only** the assigned task. Do not implement future tasks. Do not modify unrelated files or introduce undocumented features.

3. **Respect Dependencies** — Verify prerequisite tasks are completed. If a dependency is missing, stop and report it.

4. **Code Quality** — Write clean, readable, maintainable code. Follow SOLID principles. Keep methods small. Avoid duplication. Use meaningful names.

5. **Project Standards** — Follow the approved architecture, folder structure, naming conventions, and technology stack.

6. **Safety Rules** — Never remove existing functionality unless required by the spec. Never commit secrets, passwords, tokens, or connection strings. Never introduce breaking changes outside the task scope.

7. **Validation** — Ensure the project builds successfully with no compilation errors. Verify the implementation satisfies the Definition of Done and Acceptance Criteria.

8. **Traceability** — Every code change must be traceable to: Task ID, Related Requirement(s), Related Design Section(s).

9. **Completion Report** — After each task, report: Task ID, Requirements implemented, Files created/modified, Summary of changes, DoD verification, Acceptance Criteria verification, Known limitations, Confirmation no future tasks were implemented.

10. **Approval Gate** — After completing a task, stop and wait for explicit approval before starting the next task.

Implemented tasks MUST reference the applicable principle IDs from this constitution. When in doubt, prefer simplicity over complexity (YAGNI). Code reuse is mandatory before duplication. Every public API change MUST be backward compatible or go through a deprecation cycle. Database migrations MUST be reversible.

### VI. Task Implementation Lifecycle

Every implementation task MUST follow a strict lifecycle to ensure consistency,
traceability, and compliance with the constitution. The lifecycle consists of
13 sequential steps:

1. **Verify Constitution** — Confirm the task aligns with all applicable
   constitution principles, referencing principle IDs where relevant.
2. **Verify Specification** — Confirm the task maps to a specific requirement
   in the feature specification (FR, NFR, or BR ID).
3. **Verify Clarification** — Confirm all open questions for the related
   requirement are resolved and documented.
4. **Verify Plan** — Confirm the task is listed in the implementation plan
   and follows its architectural design and contract decisions.
5. **Verify Task** — Confirm the task definition (checklist item) is clear,
   has exact file paths, and is scoped to a single concern.
6. **Verify Dependencies** — Confirm all prerequisite tasks marked as
   dependencies are complete before starting.
7. **Check Specification Drift** — Compare current implementation state
   against the specification; flag any deviation before proceeding.
8. **Implement Only Assigned Task** — Implement exactly what the task
   describes; do not add scope, fix unrelated issues, or refactor beyond
   the task boundary.
9. **Build** — Compile/build the project and verify zero errors and zero
   warnings.
10. **Execute Tests** — Run all applicable tests (unit, integration, E2E)
    and confirm they pass. If no tests exist for the change, this step
    is N/A.
11. **Verify Definition of Done** — Check all DoD items from the
    constitution: code complete, tests pass, no regressions, documentation
    updated if applicable.
12. **Produce Implementation Report** — Summarize what was implemented,
    files changed, tests passed, and any deviations, blockers, or open
    questions.
13. **Wait for Human Approval** — Stop and present the report for human
    review. Do not proceed to the next task until approved.

The lifecycle applies to every single task, from setup through polish.
Skipping or reordering steps is not permitted unless explicitly waived by
the task definition.

**Rationale**: Enforces implementation discipline, prevents scope creep,
maintains specification compliance, and ensures full traceability from
requirement to delivered code with human oversight at each delivery
boundary.

### Traceability Rules

Every implementation MUST map through the following traceability chain:

```
Requirement ID (FR, NFR, BR)
    ↓
Behaviour Scenario (BH-ID)
    ↓
Constraint (C-ID)
    ↓
Task (T-ID)
    ↓
Code (file paths)
    ↓
Test (test names)
```

- Every implementation report (Principle VI, step 12) MUST include these
  mappings for the completed task.
- No implementation may introduce code that cannot be traced back to an
  approved Requirement ID (FR, NFR, or BR from the feature specification).
- Code introduced without a Requirement ID trace MUST be reverted or
  retroactively approved through a spec amendment.
- Mapping MUST be documented in the Implementation Report, not assumed
  from directory structure or naming conventions.

**Rationale**: Prevents orphan code, ensures every change serves a
documented business need, enables impact analysis when requirements
change, and enforces the specification-first workflow central to the
project methodology.

## Operational Standards

**Error Handling Details**:
- Use FluentResponse.ApiWrapper's built-in exception handler (`UseFluentResponseExceptionHandler()`) registered early in the pipeline, which returns ProblemDetails (RFC 7807) formatted responses
- Map domain exceptions to appropriate HTTP status codes via a custom
  exception-to-problem-details converter
- Return structured error envelopes with `type`, `title`, `status`, `detail`,
  `instance`, and an `errors` collection
- Never swallow exceptions in catch blocks without logging and rethrowing
- Implement a global exception handler that logs and returns sanitized
  responses suitable for the environment

**Logging Details**:
- Log every request (method, path, status code, duration) via middleware
- Log every database query exceeding 100ms threshold
- Include correlation ID in every structured log entry
- Use structured properties, never string interpolation
- Configure Serilog sinks: Console (development), File (staging),
  Seq or Azure Application Insights (production)

**Validation Details**:
- Backend: Create `ValidationBehavior<TRequest, TResponse>` for the MediatR
  pipeline to automatically validate all commands and queries
- Define reusable validators (e.g., `StringLengthValidator`, `EmailValidator`)
- Frontend: Create shared validation rules in
  `client/src/app/shared/validators/` covering all field types: required,
  min/max length, pattern, email, and custom business rules
- Every form field MUST display inline validation errors using Angular
  Material `MatError` or custom error components on touch or submit
- Validate DTOs at the controller boundary before they reach the application
  layer
- Return `400 Bad Request` with a validation errors collection

**Security Details**:
- JWT access tokens: 15-minute expiry; refresh tokens: 7-day expiry
- Hash passwords with ASP.NET Core Identity PasswordHasher (PBKDF2)
- Enforce HTTPS redirection and HSTS headers in production
- Apply `[Authorize]` attribute by default at the controller level; opt out
  for explicitly public endpoints
- Configure anti-forgery tokens for state-changing requests in the SPA

## Delivery Standards

**Testing Strategy Details**:
- **Unit tests**: Backend services in `api/tests/TodoApp.UnitTests/`, frontend
  components in `client/src/app/**/*.spec.ts`
- **Integration tests**: API tests in `api/tests/TodoApp.IntegrationTests/`
  using `WebApplicationFactory<T>` with a test PostgreSQL container
- **E2E tests**: `client/e2e/` with Playwright covering login, CRUD flows,
  and error scenarios
- Tests MUST run in CI and block merge on failure
- Test data MUST be isolated and cleaned up after each test run

**Definition of Done — Expanded**:
- All acceptance criteria from the feature spec are met
- No known bugs of severity >= medium
- API contract documented via Scalar (OpenAPI 3.x spec)
- Database migrations generated and tested against a clean database
- Feature flag removed (if the change was gated)
- Post-deployment verification smoke test passes

## Governance

This constitution is the authoritative governing document for the TodoApp
project. It supersedes all informal guides, README notes, and personal
preferences.

**Amendment Procedure**:
1. Propose changes via a PR with the `constitution` label
2. Include rationale for each change and an impact analysis
3. Requires approval from at least one maintainer
4. Version MUST be bumped per semantic versioning rules
5. After merge, propagate updates to dependent templates and artifacts
6. Amendments affecting Principle VI MUST update the Task Lifecycle steps
   and verify compliance across all active tasks

**Versioning Policy**:
- MAJOR: Backward-incompatible governance or principle removals/redefinitions
- MINOR: New principle or section added, or materially expanded guidance
- PATCH: Clarifications, wording, typo fixes, non-semantic refinements

**Compliance Review**:
- Every spec MUST reference the constitution principles it satisfies
- Every plan MUST pass a "Constitution Check" gate (see plan-template.md)
- Every task MUST follow the 13-step lifecycle defined in Principle VI
- Code review MUST verify constitution compliance
- Annual audit of all artifacts against the current constitution version

**Version**: 1.6.0 | **Ratified**: 2026-06-30 | **Last Amended**: 2026-07-02
