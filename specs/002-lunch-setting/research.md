# Research: Lunch Setting Feature

## Decisions

| Decision | Choice | Rationale | Alternatives Considered |
|----------|--------|-----------|------------------------|
| Language | C# .NET 10 | Existing project standard per Constitution | — |
| Architecture | Clean Architecture | Existing project standard per Constitution | — |
| Database | PostgreSQL via EF Core | Existing project standard per Constitution | — |
| Mapping | Cartographer.Mapper | Existing project standard per Constitution | — |
| API Responses | FluentResponse.ApiWrapper | Existing project standard per Constitution | — |
| Validation | FluentValidation + MediatR pipeline | Existing project standard per Constitution | — |
| API Documentation | Scalar (OpenAPI 3.x) | Existing project standard per Constitution | — |
| API Versioning | URL path prefix `/api/v1/` | Existing project standard per Constitution | — |
| Testing | xUnit + Moq + WebApplicationFactory + TestContainers | Existing project standard per Constitution | — |
| Collections for lists | Owned entity collections (EF Core) | Simpler than separate join tables for simple string lists; avoids over-engineering | Separate Favorites/Exclusions tables |

## Architecture Decisions

1. **Single controller, not feature-split**: Lunch preferences is a single aggregate with one natural endpoint (`/api/v1/lunch-preferences`). No sub-resources needed.

2. **Get-or-create pattern**: On first GET, if no profile exists, auto-create with organization defaults. Simplifies client code by eliminating a separate "create profile" step.

3. **PUT for updates, not PATCH**: Since the entire preference profile is loaded and updated atomically, PUT semantics are simpler than partial PATCH. Clients send complete updated state.

4. **Reset via DELETE**: DELETE on the single-profile endpoint resets to defaults. This is semantically cleaner than a separate "reset" action on PUT.

5. **Owned string collections**: FavoriteMeals and ExcludedItems are stored as JSON columns (owned entity collections) rather than separate tables. This avoids join complexity for simple list-of-string data.

## Dependency Analysis

All dependencies already exist in the project. No new NuGet packages required. The feature extends the existing `TodoApp` solution with a new aggregate (LunchPreference) and its supporting layers.

## Risk Assessment

| Risk | Severity | Mitigation |
|------|----------|------------|
| Time validation edge cases (midnight, DST) | Low | Use TimeOnly type (timezone-independent); validation ensures start < end |
| List size unbounded growth | Low | Enforce max 20 items on favorites and exclusions (validation + documentation) |
| User ID mismatch between token and data | Low | Always derive UserId from JWT claims, never from request body |
