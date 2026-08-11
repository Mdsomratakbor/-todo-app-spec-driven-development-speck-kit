# Implementation Plan: Basic Authentication

**Feature**: 003-basic-auth
**Date**: 2026-07-15
**Status**: Ready for Implementation

---

## Technical Context

| Item | Value |
|------|-------|
| Feature Directory | `specs/003-basic-auth/` |
| Spec File | `specs/003-basic-auth/spec.md` |
| Implementation Plan | `specs/003-basic-auth/plan.md` |
| Data Model | `specs/003-basic-auth/data-model.md` |
| Research | `specs/003-basic-auth/research.md` |
| Contracts | `specs/003-basic-auth/contracts/auth-api.md` |
| Quickstart | `specs/003-basic-auth/quickstart.md` |
| Branch | `003-basic-auth` |

---

## Constitution Check

| Principle | Status | Notes |
|-----------|--------|-------|
| Principle I: Clean Architecture | ✅ PASS | Auth logic in Application layer; infrastructure in Infrastructure layer |
| Principle II: CQRS | ✅ PASS | Commands/Queries follow existing MediatR pattern |
| Principle III: RESTful API | ✅ PASS | Endpoints under `/api/v1/auth/` with proper HTTP methods |
| Principle IV: Validation | ✅ PASS | FluentValidation for all inputs; RFC 7807 error responses |
| Principle V: Security | ✅ PASS | Passwords hashed (BCrypt), JWT tokens, rate limiting |

---

## Phase 0: Setup (T001-T004)

### T001: Add NuGet Packages

**Files Modified**:
- `api/src/TodoApp.Domain/TodoApp.Domain.csproj` — No changes
- `api/src/TodoApp.Application/TodoApp.Application.csproj` — Add `BCrypt.Net-Next` v4.0.3
- `api/src/TodoApp.Infrastructure/TodoApp.Infrastructure.csproj` — No changes
- `api/src/TodoApp.Api/TodoApp.Api.csproj` — Add `System.IdentityModel.Tokens.Jwt` v8.x (if not already present)

**Acceptance Criteria**:
- [ ] `BCrypt.Net-Next` added to Application project
- [ ] Solution builds without errors

---

### T002: Create User Entity

**Files Created**:
- `api/src/TodoApp.Domain/Entities/User.cs`

**Entity Definition**:
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

**Acceptance Criteria**:
- [ ] User entity created with all fields
- [ ] Entity follows existing conventions (Guid PK, timestamps)
- [ ] Domain project builds without errors

---

### T003: Create RefreshToken Entity

**Files Created**:
- `api/src/TodoApp.Domain/Entities/RefreshToken.cs`

**Entity Definition**:
```csharp
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; } = null!;
}
```

**Acceptance Criteria**:
- [ ] RefreshToken entity created with all fields
- [ ] Navigation property to User
- [ ] Domain project builds without errors

---

### T004: Create Repository Interfaces

**Files Created**:
- `api/src/TodoApp.Application/Common/Interfaces/IUserRepository.cs`
- `api/src/TodoApp.Application/Common/Interfaces/IRefreshTokenRepository.cs`

**IUserRepository Interface**:
```csharp
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
}
```

**IRefreshTokenRepository Interface**:
```csharp
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task<IReadOnlyList<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<RefreshToken> CreateAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<int> DeleteExpiredAsync(DateTime cutoffDate, CancellationToken ct = default);
}
```

**Acceptance Criteria**:
- [ ] Interfaces created following existing patterns
- [ ] Methods match data model query patterns
- [ ] Application project builds without errors

---

## Phase 1: Foundational (T005-T012)

### T005: Create EF Core Configurations

**Files Created**:
- `api/src/TodoApp.Infrastructure/Data/EntityConfigurations/UserConfiguration.cs`
- `api/src/TodoApp.Infrastructure/Data/EntityConfigurations/RefreshTokenConfiguration.cs`

**UserConfiguration**:
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Email).HasMaxLength(254);
        builder.Property(e => e.HashedPassword).HasMaxLength(255);
        builder.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("User");
        builder.HasIndex(e => e.Email).IsUnique();
    }
}
```

**RefreshTokenConfiguration**:
```csharp
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TokenHash).HasMaxLength(64);
        builder.HasIndex(e => e.TokenHash).IsUnique();
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.ExpiresAt);
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Acceptance Criteria**:
- [ ] Configurations created following existing patterns
- [ ] Indexes match data model specifications
- [ ] Foreign key relationship configured

---

### T006: Update AppDbContext

**Files Modified**:
- `api/src/TodoApp.Infrastructure/Data/AppDbContext.cs`

**Changes**:
- Add `DbSet<User> Users`
- Add `DbSet<RefreshToken> RefreshTokens`
- Apply new configurations in `OnModelCreating`

**Acceptance Criteria**:
- [ ] DbSets added for User and RefreshToken
- [ ] Configurations applied
- [ ] Existing tests still pass

---

### T007: Create Repository Implementations

**Files Created**:
- `api/src/TodoApp.Infrastructure/Repositories/UserRepository.cs`
- `api/src/TodoApp.Infrastructure/Repositories/RefreshTokenRepository.cs`

**Implementation Pattern**: Follow LunchPreferenceRepository pattern (inject AppDbContext directly)

**Acceptance Criteria**:
- [ ] Repositories implement interfaces
- [ ] Follow existing patterns (CancellationToken, SaveChangesAsync)
- [ ] Handle case-insensitive email lookups

---

### T008: Register DI Services

**Files Modified**:
- `api/src/TodoApp.Infrastructure/DependencyInjection.cs` — Register repositories
- `api/src/TodoApp.Application/DependencyInjection.cs` — Register auth services (if needed)

**Acceptance Criteria**:
- [ ] IUserRepository registered as scoped
- [ ] IRefreshTokenRepository registered as scoped
- [ ] No duplicate registrations

---

### T009: Create JWT Token Service

**Files Created**:
- `api/src/TodoApp.Infrastructure/Services/IJwtTokenService.cs`
- `api/src/TodoApp.Infrastructure/Services/JwtTokenService.cs`

**IJwtTokenService Interface**:
```csharp
public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
    ClaimsPrincipal? ValidateAccessToken(string token);
}
```

**Acceptance Criteria**:
- [ ] Service generates JWT with user ID, email, role claims
- [ ] Service generates cryptographically random refresh tokens
- [ ] Service hashes refresh tokens with SHA256
- [ ] Service validates JWT tokens

---

### T010: Configure JWT in Program.cs

**Files Modified**:
- `api/src/TodoApp.Api/Program.cs`

**Changes**:
- Configure JWT Bearer authentication with signing key
- Add JWT settings from configuration
- Ensure authentication middleware is in correct pipeline position

**Acceptance Criteria**:
- [ ] JWT authentication configured
- [ ] Signing key from configuration
- [ ] Token validation parameters set (issuer, audience, lifetime)

---

### T011: Create EF Core Migration

**Files Created**:
- `api/src/TodoApp.Infrastructure/Data/Migrations/YYYYMMDDHHMMSS_AddAuthentication.cs` (auto-generated)

**Commands**:
```bash
dotnet ef migrations add AddAuthentication --project src/TodoApp.Infrastructure --startup-project src/TodoApp.Api
```

**Acceptance Criteria**:
- [ ] Migration created successfully
- [ ] Tables created: Users, RefreshTokens
- [ ] Indexes created as specified
- [ ] Foreign key relationship configured

---

### T012: Update ExceptionMapping Middleware

**Files Modified**:
- `api/src/TodoApp.Api/Middleware/ExceptionMappingMiddleware.cs`

**Changes**:
- Map `AuthenticationException` to 401 Unauthorized
- Map token-specific exceptions to appropriate 401 messages

**Acceptance Criteria**:
- [ ] Authentication exceptions map to 401
- [ ] Different messages for expired vs. missing tokens
- [ ] Existing exception mappings unchanged

---

## Phase 2: User Story 1 — Registration (T013-T018)

### T013: Create RegisterCommand

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommand.cs`

**Command Definition**:
```csharp
public record RegisterCommand : IRequest<AuthResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
```

**Acceptance Criteria**:
- [ ] Command implements IRequest<AuthResponse>
- [ ] Properties match API contract

---

### T014: Create RegisterCommandValidator

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommandValidator.cs`

**Validation Rules**:
- Email: NotEmpty, MaximumLength(254), ValidEmailFormat
- Password: NotEmpty, MinimumLength(8), Uppercase, Lowercase, Digit

**Acceptance Criteria**:
- [ ] Email validation enforced
- [ ] Password strength validation enforced
- [ ] Follows existing validator patterns

---

### T015: Create RegisterCommandHandler

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommandHandler.cs`

**Handler Logic**:
1. Check if email already exists → 409 Conflict
2. Hash password with BCrypt (cost 12)
3. Create User entity
4. Generate JWT tokens
5. Save to database
6. Return AuthResponse

**Acceptance Criteria**:
- [ ] Email uniqueness checked
- [ ] Password hashed with BCrypt
- [ ] User created with default "User" role
- [ ] JWT tokens generated
- [ ] Returns AuthResponse

---

### T016: Create AuthResponse DTO

**Files Created**:
- `api/src/TodoApp.Application/Auth/Dtos/AuthResponse.cs`

**DTO Definition**:
```csharp
public record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
    public string TokenType { get; init; } = "Bearer";
}
```

**Acceptance Criteria**:
- [ ] DTO matches API contract
- [ ] TokenType defaults to "Bearer"

---

### T017: Create RegisterCommandValidator Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Register/RegisterCommandValidatorTests.cs`

**Test Cases**:
- Valid email and password passes
- Invalid email format fails
- Missing email fails
- Weak password fails (missing uppercase, lowercase, digit, too short)

**Acceptance Criteria**:
- [ ] All validation rules tested
- [ ] Tests follow existing patterns

---

### T018: Create RegisterCommandHandler Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Register/RegisterCommandHandlerTests.cs`

**Test Cases**:
- Successful registration returns AuthResponse
- Duplicate email throws ConflictException
- Password is hashed (not stored plaintext)

**Acceptance Criteria**:
- [ ] Mock repository used
- [ ] Tests follow existing patterns

---

## Phase 3: User Story 2 — Login (T019-T024)

### T019: Create LoginCommand

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommand.cs`

**Command Definition**:
```csharp
public record LoginCommand : IRequest<AuthResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
```

**Acceptance Criteria**:
- [ ] Command implements IRequest<AuthResponse>
- [ ] Properties match API contract

---

### T020: Create LoginCommandValidator

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommandValidator.cs`

**Validation Rules**:
- Email: NotEmpty, ValidEmailFormat
- Password: NotEmpty

**Acceptance Criteria**:
- [ ] Email validation enforced
- [ ] Password presence enforced

---

### T021: Create LoginCommandHandler

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommandHandler.cs`

**Handler Logic**:
1. Find user by email (case-insensitive)
2. Verify password with BCrypt
3. Generate JWT tokens
4. Create refresh token record
5. Return AuthResponse

**Acceptance Criteria**:
- [ ] Email lookup is case-insensitive
- [ ] Password verified with BCrypt
- [ ] Returns same error for non-existent email and wrong password
- [ ] Refresh token stored in database

---

### T022: Create LoginCommandValidator Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Login/LoginCommandValidatorTests.cs`

**Test Cases**:
- Valid email and password passes
- Missing email fails
- Missing password fails

**Acceptance Criteria**:
- [ ] All validation rules tested
- [ ] Tests follow existing patterns

---

### T023: Create LoginCommandHandler Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Login/LoginCommandHandlerTests.cs`

**Test Cases**:
- Successful login returns AuthResponse
- Wrong password throws UnauthorizedException
- Non-existent email throws UnauthorizedException

**Acceptance Criteria**:
- [ ] Mock repository used
- [ ] Tests follow existing patterns

---

### T024: Create Auth Integration Tests

**Files Created**:
- `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`

**Test Cases**:
- Register returns 201 with tokens
- Duplicate register returns 409
- Login returns 200 with tokens
- Invalid login returns 401
- Protected endpoint returns 401 without token
- Protected endpoint returns 200 with valid token

**Acceptance Criteria**:
- [ ] Uses existing IntegrationTestFactory
- [ ] Tests cover all auth endpoints
- [ ] Tests follow existing patterns

---

## Phase 4: User Story 3 — Token Refresh (T025-T030)

### T025: Create RefreshTokenCommand

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommand.cs`

**Command Definition**:
```csharp
public record RefreshTokenCommand : IRequest<AuthResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}
```

**Acceptance Criteria**:
- [ ] Command implements IRequest<AuthResponse>
- [ ] Property matches API contract

---

### T026: Create RefreshTokenCommandValidator

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommandValidator.cs`

**Validation Rules**:
- RefreshToken: NotEmpty

**Acceptance Criteria**:
- [ ] Presence validation enforced

---

### T027: Create RefreshTokenCommandHandler

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs`

**Handler Logic**:
1. Hash provided refresh token
2. Find token in database by hash
3. Validate token is not expired or revoked
4. Revoke old token (rotation)
5. Generate new token pair
6. Create new refresh token record
7. Return AuthResponse

**Acceptance Criteria**:
- [ ] Token validated (not expired, not revoked)
- [ ] Old token revoked immediately
- [ ] New token pair generated
- [ ] Returns same error for invalid/expired/revoked tokens

---

### T028: Create RefreshTokenCommandValidator Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Commands/RefreshToken/RefreshTokenCommandValidatorTests.cs`

**Test Cases**:
- Valid token passes
- Empty token fails

**Acceptance Criteria**:
- [ ] All validation rules tested
- [ ] Tests follow existing patterns

---

### T029: Create RefreshTokenCommandHandler Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandlerTests.cs`

**Test Cases**:
- Valid refresh token returns new tokens
- Expired token throws UnauthorizedException
- Revoked token throws UnauthorizedException
- Invalid token throws UnauthorizedException
- Old token is revoked after use

**Acceptance Criteria**:
- [ ] Mock repository used
- [ ] Tests follow existing patterns

---

### T030: Add Refresh Token Integration Tests

**Files Modified**:
- `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`

**Additional Test Cases**:
- Refresh returns 200 with new tokens
- Reuse old refresh token returns 401

**Acceptance Criteria**:
- [ ] Refresh flow tested end-to-end
- [ ] Token rotation verified

---

## Phase 5: User Story 4 — Logout (T031-T034)

### T031: Create LogoutCommand

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Logout/LogoutCommand.cs`

**Command Definition**:
```csharp
public record LogoutCommand : IRequest<Unit>
{
    public Guid UserId { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
}
```

**Acceptance Criteria**:
- [ ] Command implements IRequest<Unit>
- [ ] Properties match requirements

---

### T032: Create LogoutCommandHandler

**Files Created**:
- `api/src/TodoApp.Application/Auth/Commands/Logout/LogoutCommandHandler.cs`

**Handler Logic**:
1. Hash provided refresh token
2. Find token in database by hash and user ID
3. Revoke token (set RevokedAt)
4. Return Unit

**Acceptance Criteria**:
- [ ] Only current session's token revoked
- [ ] Other sessions remain valid
- [ ] Returns success even if token not found (idempotent)

---

### T033: Create LogoutCommandHandler Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Logout/LogoutCommandHandlerTests.cs`

**Test Cases**:
- Successful logout revokes token
- Logout with invalid token still succeeds (idempotent)

**Acceptance Criteria**:
- [ ] Mock repository used
- [ ] Tests follow existing patterns

---

### T034: Add Logout Integration Tests

**Files Modified**:
- `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`

**Additional Test Cases**:
- Logout returns 204
- Logout without token returns 401
- Refresh token invalidated after logout

**Acceptance Criteria**:
- [ ] Logout flow tested end-to-end
- [ ] Token revocation verified

---

## Phase 6: User Story 5 — Profile (T035-T038)

### T035: Create GetCurrentUserQuery

**Files Created**:
- `api/src/TodoApp.Application/Auth/Queries/GetCurrentUser/GetCurrentUserQuery.cs`

**Query Definition**:
```csharp
public record GetCurrentUserQuery : IRequest<UserProfileResponse>
{
    public Guid UserId { get; init; }
}
```

**Acceptance Criteria**:
- [ ] Query implements IRequest<UserProfileResponse>
- [ ] Property matches requirements

---

### T036: Create UserProfileResponse DTO

**Files Created**:
- `api/src/TodoApp.Application/Auth/Dtos/UserProfileResponse.cs`

**DTO Definition**:
```csharp
public record UserProfileResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
```

**Acceptance Criteria**:
- [ ] DTO matches API contract
- [ ] Includes role field for future RBAC

---

### T037: Create GetCurrentUserQueryHandler

**Files Created**:
- `api/src/TodoApp.Application/Auth/Queries/GetCurrentUser/GetCurrentUserQueryHandler.cs`

**Handler Logic**:
1. Find user by ID
2. If not found, throw KeyNotFoundException
3. Map to UserProfileResponse
4. Return response

**Acceptance Criteria**:
- [ ] User found by ID
- [ ] Returns UserProfileResponse
- [ ] Throws KeyNotFoundException if not found

---

### T038: Create GetCurrentUserQueryHandler Tests

**Files Created**:
- `api/tests/TodoApp.UnitTests/Application/Auth/Queries/GetCurrentUser/GetCurrentUserQueryHandlerTests.cs`

**Test Cases**:
- Valid user ID returns profile
- Invalid user ID throws KeyNotFoundException

**Acceptance Criteria**:
- [ ] Mock repository used
- [ ] Tests follow existing patterns

---

## Phase 7: API Layer (T039-T044)

### T039: Create AuthController

**Files Created**:
- `api/src/TodoApp.Api/Controllers/AuthController.cs`

**Controller Definition**:
```csharp
[ApiController]
[Route("api/v1/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetCurrentUser()
}
```

**Acceptance Criteria**:
- [ ] Controller follows existing patterns
- [ ] Route matches API contract (`/api/v1/auth/*`)
- [ ] Auth attributes applied correctly
- [ ] Returns proper HTTP status codes

---

### T040: Create Request DTOs

**Files Created**:
- `api/src/TodoApp.Application/Auth/Dtos/RegisterRequest.cs`
- `api/src/TodoApp.Application/Auth/Dtos/LoginRequest.cs`
- `api/src/TodoApp.Application/Auth/Dtos/RefreshRequest.cs`

**Acceptance Criteria**:
- [ ] DTOs match API contract
- [ ] Used by controller

---

### T041: Register JWT in Program.cs

**Files Modified**:
- `api/src/TodoApp.Api/Program.cs`

**Changes**:
- Add JWT Bearer authentication configuration
- Configure token validation parameters
- Add authorization services

**Acceptance Criteria**:
- [ ] JWT authentication configured
- [ ] Token validation parameters set
- [ ] Authorization middleware enabled

---

### T042: Create AuthController Integration Tests

**Files Created**:
- `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`

**Test Cases**:
- Register: 201, 400, 409, 429
- Login: 200, 400, 401, 429
- Refresh: 200, 401, 429
- Logout: 204, 401
- Me: 200, 401

**Acceptance Criteria**:
- [ ] All endpoints tested
- [ ] Error responses verified
- [ ] Uses TestAuthHandler

---

### T043: Create RefreshTokenCleanupService

**Files Created**:
- `api/src/TodoApp.Infrastructure/Services/RefreshTokenCleanupService.cs`

**Service Logic**:
- Extends BackgroundService
- Runs daily
- Deletes expired refresh tokens (older than 30 days)
- Enforces max 5 tokens per user

**Acceptance Criteria**:
- [ ] Follows TodoCleanupService pattern
- [ ] Cleans up expired tokens
- [ ] Enforces max token limit

---

### T044: Register Background Service

**Files Modified**:
- `api/src/TodoApp.Infrastructure/DependencyInjection.cs`

**Changes**:
- Register RefreshTokenCleanupService as hosted service

**Acceptance Criteria**:
- [ ] Service registered
- [ ] Runs on application startup

---

## Phase 8: Polish & Cross-Cutting (T045-T050)

### T045: Add Rate Limiting to Auth Endpoints

**Files Modified**:
- `api/src/TodoApp.Api/Middleware/RateLimitingMiddleware.cs` (if needed)

**Changes**:
- Verify existing middleware applies to auth endpoints
- Ensure 100 req/min limit is enforced

**Acceptance Criteria**:
- [ ] Auth endpoints rate-limited
- [ ] 429 response with proper headers

---

### T046: Add Logging for Auth Events

**Files Modified**:
- `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommandHandler.cs`
- `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommandHandler.cs`
- `api/src/TodoApp.Application/Auth/Commands/Logout/LogoutCommandHandler.cs`
- `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs`

**Changes**:
- Log successful registration with user ID
- Log successful login with user ID
- Log failed login with email (no user ID)
- Log logout with user ID
- Log token refresh with user ID

**Acceptance Criteria**:
- [ ] All auth events logged
- [ ] User ID included where available
- [ ] Failed attempts logged with email

---

### T047: Update AGENTS.md Context

**Files Modified**:
- `AGENTS.md`

**Changes**:
- Update plan reference to point to `specs/003-basic-auth/plan.md`
- Add session summary for 003-basic-auth

**Acceptance Criteria**:
- [ ] Plan reference updated
- [ ] Session summary added

---

### T048: Run All Tests

**Commands**:
```bash
cd api
dotnet test
```

**Acceptance Criteria**:
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] No regressions

---

### T049: Verify Build

**Commands**:
```bash
cd api
dotnet build
```

**Acceptance Criteria**:
- [ ] Build succeeds with 0 errors
- [ ] No warnings

---

### T050: Create EF Core Migration (Final)

**Commands**:
```bash
cd api
dotnet ef migrations add AddAuthentication --project src/TodoApp.Infrastructure --startup-project src/TodoApp.Api
```

**Acceptance Criteria**:
- [ ] Migration created
- [ ] Database updated successfully
- [ ] Tables and indexes created

---

## Task Summary

| Phase | Tasks | Description |
|-------|-------|-------------|
| Phase 0: Setup | T001-T004 | NuGet packages, entities, interfaces |
| Phase 1: Foundational | T005-T012 | EF config, repositories, JWT service, middleware |
| Phase 2: US1 Registration | T013-T018 | Register command, handler, validator, tests |
| Phase 3: US2 Login | T019-T024 | Login command, handler, validator, tests |
| Phase 4: US3 Token Refresh | T025-T030 | Refresh command, handler, validator, tests |
| Phase 5: US4 Logout | T031-T034 | Logout command, handler, tests |
| Phase 6: US5 Profile | T035-T038 | Profile query, handler, tests |
| Phase 7: API Layer | T039-T044 | Controller, DTOs, JWT config, integration tests |
| Phase 8: Polish | T045-T050 | Rate limiting, logging, migration, verification |
| Phase 9: Client Setup | T051-T055 | Auth models, interfaces |
| Phase 10: Client Foundational | T056-T060 | AuthService, TokenService, interceptor, guard |
| Phase 11: Client US1 | T061-T063 | Register component, form, route |
| Phase 12: Client US2 | T064-T067 | Login component, form, route, guard |
| Phase 13: Client US4 | T068-T069 | Logout method, button |
| Phase 14: Client US5 | T070-T072 | Profile component, route |
| Phase 15: Client Polish | T073-T076 | Redirects, styling, tests |

**Total Tasks**: 76

---

## Dependencies

- T001 → T002, T003 (packages before entities)
- T002, T003 → T004 (entities before interfaces)
- T005, T006 → T007 (config before repositories)
- T007 → T008 (repositories before DI)
- T009 → T010 (JWT service before config)
- T011 → T012 (migration before middleware)
- T013-T016 → T017, T018 (command before tests)
- T019-T021 → T022-T024 (command before tests)
- T025-T027 → T028-T030 (command before tests)
- T031-T032 → T033, T034 (command before tests)
- T035-T037 → T038 (query before tests)
- T039-T041 → T042 (controller before integration tests)
- T043 → T044 (service before DI)
- T045-T050 → Final verification
- T051-T055 → T056-T060 (models before services)
- T056-T060 → T061-T072 (services before components)
- T073-T076 → Final client verification

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| JWT signing key compromise | Use secure key storage; configurable expiry |
| Refresh token table growth | Daily cleanup job; max 5 per user |
| Brute-force attacks | Rate limiting (100 req/min); logging |
| Token replay attacks | Strict rotation; immediate invalidation |
| Race conditions on token refresh | Database row-level locking |
