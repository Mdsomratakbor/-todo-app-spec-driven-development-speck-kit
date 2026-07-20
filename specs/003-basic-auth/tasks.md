# Task List: Basic Authentication

**Feature**: 003-basic-auth
**Generated**: 2026-07-15
**Total Tasks**: 72
**User Stories**: 5 (US1-US5)
**Platforms**: API (.NET 10) + Client (Angular 21)

---

## Phase 1: Setup

- [x] T001 Add BCrypt.Net-Next package to `api/src/TodoApp.Application/TodoApp.Application.csproj`
- [x] T002 [P] Create User entity in `api/src/TodoApp.Domain/Entities/User.cs`
- [x] T003 [P] Create RefreshToken entity in `api/src/TodoApp.Domain/Entities/RefreshToken.cs`
- [x] T004 [P] Create IUserRepository interface in `api/src/TodoApp.Application/Common/Interfaces/IUserRepository.cs`
- [x] T004 [P] Create IRefreshTokenRepository interface in `api/src/TodoApp.Application/Common/Interfaces/IRefreshTokenRepository.cs`

---

## Phase 2: Foundational

- [x] T005 [P] Create UserConfiguration in `api/src/TodoApp.Infrastructure/Data/EntityConfigurations/UserConfiguration.cs`
- [x] T005 [P] Create RefreshTokenConfiguration in `api/src/TodoApp.Infrastructure/Data/EntityConfigurations/RefreshTokenConfiguration.cs`
- [x] T006 Update AppDbContext in `api/src/TodoApp.Infrastructure/Data/AppDbContext.cs`
- [x] T007 [P] Create UserRepository in `api/src/TodoApp.Infrastructure/Repositories/UserRepository.cs`
- [x] T007 [P] Create RefreshTokenRepository in `api/src/TodoApp.Infrastructure/Repositories/RefreshTokenRepository.cs`
- [x] T008 Register DI services in `api/src/TodoApp.Infrastructure/DependencyInjection.cs`
- [x] T009 Create IJwtTokenService interface in `api/src/TodoApp.Infrastructure/Services/IJwtTokenService.cs`
- [x] T009 Create JwtTokenService implementation in `api/src/TodoApp.Infrastructure/Services/JwtTokenService.cs`
- [x] T010 Configure JWT authentication in `api/src/TodoApp.Api/Program.cs`
- [x] T011 Create EF Core migration (AddAuthentication)
- [x] T012 Update ExceptionMapping middleware in `api/src/TodoApp.Api/Middleware/ExceptionMappingMiddleware.cs`

---

## Phase 3: User Story 1 — Registration

**Goal**: As a new user, I want to register with my email and password so that I can create an account and access the application.

**Independent Test**: A user can register with a valid email and password, receive a JWT token, and access protected endpoints. Registration with an existing email fails with appropriate error.

- [x] T013 [US1] Create RegisterCommand in `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommand.cs`
- [x] T014 [US1] Create RegisterCommandValidator in `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommandValidator.cs`
- [x] T015 [US1] Create RegisterCommandHandler in `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommandHandler.cs`
- [x] T016 [US1] Create AuthResponse DTO in `api/src/TodoApp.Application/Auth/Dtos/AuthResponse.cs`
- [x] T017 [US1] Create RegisterCommandValidator tests in `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Register/RegisterCommandValidatorTests.cs`
- [x] T018 [US1] Create RegisterCommandHandler tests in `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Register/RegisterCommandHandlerTests.cs`

---

## Phase 4: User Story 2 — Login

**Goal**: As a registered user, I want to log in with my email and password so that I can access my account and protected resources.

**Independent Test**: A user can log in with valid credentials and receive JWT tokens. Invalid credentials result in appropriate error responses.

- [x] T019 [US2] Create LoginCommand in `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommand.cs`
- [x] T020 [US2] Create LoginCommandValidator in `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommandValidator.cs`
- [x] T021 [US2] Create LoginCommandHandler in `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommandHandler.cs`
- [x] T022 [US2] Create LoginCommandValidator tests in `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Login/LoginCommandValidatorTests.cs`
- [x] T023 [US2] Create LoginCommandHandler tests in `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Login/LoginCommandHandlerTests.cs`
- [x] T024 [US2] Create Auth integration tests in `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`

---

## Phase 5: User Story 3 — Token Refresh

**Goal**: As a user, I want to refresh my access token when it expires so that I can maintain my session without re-logging in.

**Independent Test**: A user can exchange a valid refresh token for a new access token. Expired or invalid refresh tokens are rejected.

- [x] T025 [US3] Create RefreshTokenCommand in `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommand.cs`
- [x] T026 [US3] Create RefreshTokenCommandValidator in `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommandValidator.cs`
- [x] T027 [US3] Create RefreshTokenCommandHandler in `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs`
- [x] T028 [US3] Create RefreshTokenCommandValidator tests in `api/tests/TodoApp.UnitTests/Application/Auth/Commands/RefreshToken/RefreshTokenCommandValidatorTests.cs`
- [x] T029 [US3] Create RefreshTokenCommandHandler tests in `api/tests/TodoApp.UnitTests/Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandlerTests.cs`
- [x] T030 [US3] Add refresh token integration tests in `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`

---

## Phase 6: User Story 4 — Logout

**Goal**: As a user, I want to log out so that my session is terminated and my tokens are invalidated.

**Independent Test**: A user can log out and their refresh token is invalidated. Subsequent requests with the old refresh token fail.

- [x] T031 [US4] Create LogoutCommand in `api/src/TodoApp.Application/Auth/Commands/Logout/LogoutCommand.cs`
- [x] T032 [US4] Create LogoutCommandHandler in `api/src/TodoApp.Application/Auth/Commands/Logout/LogoutCommandHandler.cs`
- [x] T033 [US4] Create LogoutCommandHandler tests in `api/tests/TodoApp.UnitTests/Application/Auth/Commands/Logout/LogoutCommandHandlerTests.cs`
- [x] T034 [US4] Add logout integration tests in `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`

---

## Phase 7: User Story 5 — Profile

**Goal**: As a logged-in user, I want to retrieve my profile information so that I can view my account details.

**Independent Test**: An authenticated user can retrieve their profile. Unauthenticated requests are rejected.

- [x] T035 [US5] Create GetCurrentUserQuery in `api/src/TodoApp.Application/Auth/Queries/GetCurrentUser/GetCurrentUserQuery.cs`
- [x] T036 [US5] Create UserProfileResponse DTO in `api/src/TodoApp.Application/Auth/Dtos/UserProfileResponse.cs`
- [x] T037 [US5] Create GetCurrentUserQueryHandler in `api/src/TodoApp.Application/Auth/Queries/GetCurrentUser/GetCurrentUserQueryHandler.cs`
- [x] T038 [US5] Create GetCurrentUserQueryHandler tests in `api/tests/TodoApp.UnitTests/Application/Auth/Queries/GetCurrentUser/GetCurrentUserQueryHandlerTests.cs`

---

## Phase 8: API Layer

- [x] T039 Create AuthController in `api/src/TodoApp.Api/Controllers/AuthController.cs`
- [x] T040 [P] Create RegisterRequest DTO in `api/src/TodoApp.Application/Auth/Dtos/RegisterRequest.cs`
- [x] T040 [P] Create LoginRequest DTO in `api/src/TodoApp.Application/Auth/Dtos/LoginRequest.cs`
- [x] T040 [P] Create RefreshRequest DTO in `api/src/TodoApp.Application/Auth/Dtos/RefreshRequest.cs`
- [x] T041 Configure JWT in Program.cs in `api/src/TodoApp.Api/Program.cs`
- [x] T042 Create AuthController integration tests in `api/tests/TodoApp.IntegrationTests/Api/AuthEndpointsTests.cs`
- [x] T043 Create RefreshTokenCleanupService in `api/src/TodoApp.Infrastructure/Services/RefreshTokenCleanupService.cs`
- [x] T044 Register background service in `api/src/TodoApp.Infrastructure/DependencyInjection.cs`

---

## Phase 9: Polish & Cross-Cutting

- [x] T045 Verify rate limiting applies to auth endpoints in `api/src/TodoApp.Api/Middleware/RateLimitingMiddleware.cs`
- [x] T046 Add logging to RegisterCommandHandler in `api/src/TodoApp.Application/Auth/Commands/Register/RegisterCommandHandler.cs`
- [x] T046 Add logging to LoginCommandHandler in `api/src/TodoApp.Application/Auth/Commands/Login/LoginCommandHandler.cs`
- [x] T046 Add logging to LogoutCommandHandler in `api/src/TodoApp.Application/Auth/Commands/Logout/LogoutCommandHandler.cs`
- [x] T046 Add logging to RefreshTokenCommandHandler in `api/src/TodoApp.Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs`
- [x] T047 Update AGENTS.md context in `AGENTS.md`
- [x] T048 Run all tests (`dotnet test`)
- [x] T049 Verify build (`dotnet build`)
- [x] T050 Create final EF Core migration

---

## Phase 10: Client Setup

- [x] T051 [P] Create auth models in `client/src/app/shared/models/auth.model.ts`
- [x] T052 [P] Create AuthResponse interface in `client/src/app/shared/models/auth.model.ts`
- [x] T053 [P] Create LoginRequest interface in `client/src/app/shared/models/auth.model.ts`
- [x] T054 [P] Create RegisterRequest interface in `client/src/app/shared/models/auth.model.ts`
- [x] T055 [P] Create UserProfile interface in `client/src/app/shared/models/auth.model.ts`

---

## Phase 11: Client Foundational

- [x] T056 Create AuthService in `client/src/app/shared/services/auth.service.ts`
- [x] T057 Create TokenService in `client/src/app/shared/services/token.service.ts`
- [x] T058 Create auth interceptor in `client/src/app/shared/interceptors/auth.interceptor.ts`
- [x] T059 Create auth guard in `client/src/app/shared/guards/auth.guard.ts`
- [x] T060 Register interceptor in `client/src/app/app.config.ts`

---

## Phase 12: Client User Story 1 — Registration

**Goal**: As a new user, I want to register with my email and password via the UI.

**Independent Test**: User can navigate to registration page, fill form, submit, and be redirected to the app.

- [x] T061 [US1] Create register component in `client/src/app/features/auth/register/register.component.ts`
- [x] T062 [US1] Create register form with validation in `client/src/app/features/auth/register/register.component.ts`
- [x] T063 [US1] Add register route in `client/src/app/app.routes.ts`

---

## Phase 13: Client User Story 2 — Login

**Goal**: As a registered user, I want to log in via the UI and access protected resources.

**Independent Test**: User can navigate to login page, enter credentials, submit, and be redirected to the app with valid session.

- [x] T064 [US2] Create login component in `client/src/app/features/auth/login/login.component.ts`
- [x] T065 [US2] Create login form with validation in `client/src/app/features/auth/login/login.component.ts`
- [x] T066 [US2] Add login route in `client/src/app/app.routes.ts`
- [x] T067 [US2] Protect existing routes with auth guard in `client/src/app/app.routes.ts`

---

## Phase 14: Client User Story 4 — Logout

**Goal**: As a user, I want to log out via the UI and have my session terminated.

**Independent Test**: User can click logout, tokens are cleared, and user is redirected to login page.

- [x] T068 [US4] Add logout method to AuthService in `client/src/app/shared/services/auth.service.ts`
- [x] T069 [US4] Add logout button to app header/navigation in `client/src/app/app.html`

---

## Phase 15: Client User Story 5 — Profile

**Goal**: As a logged-in user, I want to see my profile information in the UI.

**Independent Test**: User can view their email, role, and account creation date.

- [x] T070 [US5] Create profile component in `client/src/app/features/auth/profile/profile.component.ts`
- [x] T071 [US5] Add profile route in `client/src/app/app.routes.ts`
- [x] T072 [US5] Add profile link to app header/navigation in `client/src/app/app.html`

---

## Phase 16: Client Polish

- [x] T073 Add redirect logic after login/register in `client/src/app/features/auth/login/login.component.ts` and `register.component.ts`
- [x] T074 Handle 401 interceptor redirect in `client/src/app/shared/interceptors/auth.interceptor.ts`
- [x] T075 Add Angular Material styling to auth forms
- [x] T076 Run client tests (`npm test`)

---

## Dependencies

```
API:
Phase 1 (Setup) ──→ Phase 2 (Foundational) ──→ Phase 3-7 (User Stories)
                                                  ├── Phase 3: US1 Registration
                                                  ├── Phase 4: US2 Login
                                                  ├── Phase 5: US3 Token Refresh
                                                  ├── Phase 6: US4 Logout
                                                  └── Phase 7: US5 Profile
                                                          │
                                                          ▼
                                                  Phase 8 (API Layer) ──→ Phase 9 (Polish)

Client:
Phase 10 (Client Setup) ──→ Phase 11 (Client Foundational) ──→ Phase 12-15 (Client User Stories)
                                                                      ├── Phase 12: US1 Registration
                                                                      ├── Phase 13: US2 Login
                                                                      ├── Phase 14: US4 Logout
                                                                      └── Phase 15: US5 Profile
                                                                              │
                                                                              ▼
                                                                      Phase 16 (Client Polish)
```

**Parallel Execution Opportunities**:
- T002, T003, T004 (API: Setup entities and interfaces)
- T005 (API: EF configurations)
- T007 (API: Repository implementations)
- T040 (API: Request DTOs)
- T051-T055 (Client: Auth models)
- T061, T064 (Client: Register and Login components can be built in parallel)

---

## Implementation Strategy

**MVP Scope**: Phase 1-4 (API) + Phase 10-13 (Client) — Registration + Login end-to-end

**Incremental Delivery**:
1. **MVP**: Users can register and login (US1 + US2 — API + Client)
2. **Session Management**: Token refresh and logout (US3 + US4 — API + Client)
3. **Complete**: Profile endpoint, polish, and client completion (US5 + Phase 8-9 + Phase 15-16)

---

## Task Count Summary

| Phase | Tasks | User Story | Platform |
|-------|-------|------------|----------|
| Phase 1: Setup | 5 | — | API |
| Phase 2: Foundational | 7 | — | API |
| Phase 3: Registration | 6 | US1 | API |
| Phase 4: Login | 6 | US2 | API |
| Phase 5: Token Refresh | 6 | US3 | API |
| Phase 6: Logout | 4 | US4 | API |
| Phase 7: Profile | 4 | US5 | API |
| Phase 8: API Layer | 7 | — | API |
| Phase 9: Polish | 5 | — | API |
| Phase 10: Client Setup | 5 | — | Client |
| Phase 11: Client Foundational | 5 | — | Client |
| Phase 12: Client Registration | 3 | US1 | Client |
| Phase 13: Client Login | 4 | US2 | Client |
| Phase 14: Client Logout | 2 | US4 | Client |
| Phase 15: Client Profile | 3 | US5 | Client |
| Phase 16: Client Polish | 4 | — | Client |
| **Total** | **76** | **5** | **Both** |
