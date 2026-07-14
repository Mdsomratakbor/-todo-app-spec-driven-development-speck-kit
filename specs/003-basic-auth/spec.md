# Feature Specification: Basic Authentication

**Feature Branch**: `003-basic-auth`

**Created**: 2026-07-15

**Status**: Draft

**Input**: User description: "add a new feature for basic authentication and update the backend request url"

---

## Context

### Feature Purpose

Implement basic authentication (username and password) to secure the TodoApp backend API. This enables user registration, login, session management, and secure access to all protected endpoints. Additionally, update backend request URLs to follow consistent versioned API patterns.

### Business Objective

Establish a secure authentication layer to protect user data, enable multi-user support, and provide a foundation for role-based access control. Secure API access ensures that only authenticated users can interact with their own todo items and lunch preferences.

### Scope

- User registration with email and password
- User login with email and password
- JWT token generation and validation
- Password hashing and storage
- Session/token management (refresh, logout)
- API endpoint protection with authentication middleware
- Backend URL updates for consistent versioned API patterns
- User profile retrieval for authenticated users

### Explicit Boundaries

- Basic username/password authentication only (no OAuth2, SSO, or social login)
- Backend API implementation with JWT-based sessions
- All data persisted to PostgreSQL database
- Passwords stored with secure hashing (bcrypt or similar)
- Token-based stateless authentication

### Out of Scope

- Multi-factor authentication (MFA/2FA)
- OAuth2, OpenID Connect, or social login (Google, GitHub, etc.)
- Password reset or recovery via email
- Role-based access control (RBAC) beyond basic authenticated/unauthenticated
- Account lockout after failed attempts
- Email verification for registration
- Session management UI (cookie-based sessions)
- Account deletion or data export
- User profile management beyond authentication (name, avatar, etc.)
- API key authentication

### Assumptions

- Users have valid email addresses for registration
- A mail service is NOT required (email verification is out of scope)
- The PostgreSQL database is pre-configured and accessible
- HTTPS is enforced in production environments
- Existing todo and lunch preference features are already implemented
- The frontend will handle JWT token storage (localStorage or secure cookie)

### Dependencies

- .NET 10 Web API runtime
- PostgreSQL database server
- Entity Framework Core for database access
- Existing TodoItem and LunchPreference domain models

### Technology Stack

Per the TodoApp Constitution (v1.3.0):
- **Backend**: .NET 10 Web API with Clean Architecture (API, Application, Domain, Infrastructure layers)
- **Database**: PostgreSQL via Entity Framework Core
- **API Responses**: FluentResponse.ApiWrapper
- **API Documentation**: Scalar (OpenAPI 3.x)
- **API Versioning**: URL path prefix (`/api/v1/`)
- **Authentication**: JWT Bearer tokens

### Existing Conventions

- RESTful API design with plural nouns under versioned prefix
- URL path API versioning (`/api/v1/...`)
- Consistent response envelopes via FluentResponse.ApiWrapper
- Clean Architecture with dependency inversion

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 — User Registration (Priority: P1)

As a new user, I want to register with my email and password so that I can create an account and access the application.

**Why this priority**: Registration is the entry point for new users — without it, no authenticated features are accessible.

**Independent Test**: A user can register with a valid email and password, receive a JWT token, and access protected endpoints. Registration with an existing email fails with appropriate error.

**Acceptance Scenarios**:

1. **Given** I am on the registration page, **When** I provide a valid email and password and submit, **Then** my account is created and I receive a JWT access token and refresh token.
2. **Given** I am on the registration page, **When** I provide an email that already exists, **Then** I receive a 409 Conflict error with message "An account with this email already exists."
3. **Given** I am on the registration page, **When** I provide an invalid email format, **Then** I receive a 400 validation error with message indicating invalid email format.
4. **Given** I am on the registration page, **When** I provide a password that does not meet requirements, **Then** I receive a 400 validation error with specific password requirement failures.

---

### User Story 2 — User Login (Priority: P1)

As a registered user, I want to log in with my email and password so that I can access my account and protected resources.

**Why this priority**: Login is essential for returning users to access their data.

**Independent Test**: A user can log in with valid credentials and receive JWT tokens. Invalid credentials result in appropriate error responses.

**Acceptance Scenarios**:

1. **Given** I have a registered account, **When** I provide correct email and password, **Then** I receive a JWT access token and refresh token.
2. **Given** I have a registered account, **When** I provide incorrect password, **Then** I receive a 401 Unauthorized error with message "Invalid email or password."
3. **Given** I provide an email that does not exist, **When** I submit login, **Then** I receive a 401 Unauthorized error with message "Invalid email or password." (same message for security).
4. **Given** I have an access token, **When** I make an authenticated request, **Then** the request succeeds and I can access protected resources.

---

### User Story 3 — Token Refresh (Priority: P1)

As a user, I want to refresh my access token when it expires so that I can maintain my session without re-logging in.

**Why this priority**: Token refresh provides seamless user experience and is critical for session continuity.

**Independent Test**: A user can exchange a valid refresh token for a new access token. Expired or invalid refresh tokens are rejected.

**Acceptance Scenarios**:

1. **Given** I have a valid refresh token, **When** I request a new access token, **Then** I receive a new access token and refresh token.
2. **Given** I have an expired refresh token, **When** I request a new access token, **Then** I receive a 401 Unauthorized error and must re-login.
3. **Given** I have a refresh token that has been revoked, **When** I request a new access token, **Then** I receive a 401 Unauthorized error.

---

### User Story 4 — User Logout (Priority: P2)

As a user, I want to log out so that my session is terminated and my tokens are invalidated.

**Why this priority**: Logout protects user data on shared devices and is a security best practice.

**Independent Test**: A user can log out and their refresh token is invalidated. Subsequent requests with the old refresh token fail.

**Acceptance Scenarios**:

1. **Given** I am logged in, **When** I click logout, **Then** my current session's refresh token is revoked and I receive a success response.
2. **Given** I have logged out, **When** I attempt to use my old refresh token, **Then** I receive a 401 Unauthorized error.
3. **Given** I am logged in on two devices, **When** I log out on one device, **Then** the other device's session remains valid.

---

### User Story 5 — Get Current User Profile (Priority: P2)

As a logged-in user, I want to retrieve my profile information so that I can view my account details.

**Why this priority**: Profile retrieval confirms authentication is working and provides user context.

**Independent Test**: An authenticated user can retrieve their profile. Unauthenticated requests are rejected.

**Acceptance Scenarios**:

1. **Given** I am authenticated, **When** I request my profile, **Then** I receive my user ID, email, role, and account creation date.
2. **Given** I am not authenticated, **When** I request a profile, **Then** I receive a 401 Unauthorized error.

---

### Edge Cases

- What happens when a user registers with an email that is already registered but different case? — Email comparison is case-insensitive; 409 Conflict returned.
- How does the system handle concurrent login attempts from the same user? — Each login generates new tokens; previous tokens remain valid until expiry.
- What happens when a user attempts to access a protected endpoint with an expired token? — 401 Unauthorized with message "Token has expired."
- How does the system handle a refresh token that has been used (rotation)? — Old refresh token is invalidated; new pair issued on successful refresh.
- What happens when a user provides a malformed JWT token? — 401 Unauthorized with message "Invalid token format."
- How does the system handle password requirements not met? — 400 with specific validation errors (e.g., "Password must be at least 8 characters", "Password must contain uppercase, lowercase, and digit").

---

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to register with email and password. **Priority**: P1. **Rationale**: Entry point for new users. **Acceptance Criteria**: POST `/api/v1/auth/register` creates account and returns JWT tokens; duplicate email returns 409.
- **FR-002**: System MUST allow users to login with email and password. **Priority**: P1. **Rationale**: Returning users need access. **Acceptance Criteria**: POST `/api/v1/auth/login` returns JWT tokens for valid credentials; invalid credentials return 401.
- **FR-003**: System MUST issue JWT access tokens with configurable expiry (default: 15 minutes). **Priority**: P1. **Rationale**: Time-limited access tokens improve security. **Acceptance Criteria**: Access token contains user ID, email, and role claims; expires after configured duration.
- **FR-004**: System MUST issue refresh tokens with longer expiry (default: 7 days). **Priority**: P1. **Rationale**: Seamless session renewal. **Acceptance Criteria**: Refresh token is a separate opaque token stored server-side.
- **FR-005**: System MUST support token refresh via refresh token exchange. **Priority**: P1. **Rationale**: Session continuity without re-login. **Acceptance Criteria**: POST `/api/v1/auth/refresh` returns new token pair; old refresh token is invalidated (rotation).
- **FR-006**: System MUST support user logout with refresh token revocation. **Priority**: P2. **Rationale**: Session termination for security. **Acceptance Criteria**: POST `/api/v1/auth/logout` revokes only the current session's refresh token; subsequent refresh attempts with that specific token fail; other active sessions remain valid.
- **FR-007**: System MUST hash passwords using bcrypt with cost factor >= 12 before storage. **Priority**: P1. **Rationale**: Protect user credentials at rest. **Acceptance Criteria**: Plaintext passwords are never stored; hashed password is stored in database.
- **FR-008**: System MUST protect all API endpoints except auth endpoints with JWT validation middleware. **Priority**: P1. **Rationale**: Enforce authentication. **Acceptance Criteria**: Requests without valid JWT receive 401 Unauthorized.
- **FR-009**: System MUST return current user profile for authenticated requests. **Priority**: P2. **Rationale**: User identity confirmation. **Acceptance Criteria**: GET `/api/v1/auth/me` returns user ID, email, role, and createdAt for authenticated user.
- **FR-010**: System MUST update backend API URLs to follow consistent versioned pattern (`/api/v1/auth/*`). **Priority**: P1. **Rationale**: Consistent API design. **Acceptance Criteria**: All auth endpoints are under `/api/v1/auth/` prefix; existing endpoints remain unchanged.
- **FR-011**: System MUST validate password strength on registration. **Priority**: P1. **Rationale**: Enforce minimum security. **Acceptance Criteria**: Password must be at least 8 characters, contain uppercase, lowercase, digit; validation errors returned for each unmet requirement.
- **FR-012**: System MUST store refresh tokens server-side with expiry tracking. **Priority**: P1. **Rationale**: Enable token revocation and rotation. **Acceptance Criteria**: Refresh tokens are stored in database with userId, token hash, expiry, and revoked status.

### Non-Functional Requirements

- **NFR-001 (Performance)**: Authentication endpoints MUST respond in under 300ms for registration and login. Token refresh MUST respond in under 100ms. **Rationale**: Authentication is in the critical path; slow auth degrades UX.
- **NFR-002 (Security)**: Passwords MUST be hashed with bcrypt (cost >= 12). JWT tokens MUST use HS256 or RS256 signing. Refresh tokens MUST be stored as opaque hashes (not plaintext). HTTPS MUST be enforced in production. Auth endpoints (register, login, refresh) MUST be rate-limited to 100 requests per minute per IP address. **Rationale**: Protect credentials and tokens per constitution Principle V; prevent brute-force attacks on authentication.
- **NFR-003 (Validation)**: All inputs MUST be validated server-side. Email format MUST be validated. Password strength MUST be validated. RFC 7807 Problem Details for validation failures. **Rationale**: Defense-in-depth per constitution Principle IV.
- **NFR-004 (Logging)**: Authentication events (register, login, logout, token refresh) MUST be logged with user identifier and timestamp. Failed login attempts MUST be logged with email and timestamp. **Rationale**: Security audit trail.
- **NFR-005 (Maintainability)**: Backend MUST follow Clean Architecture layering. Auth logic MUST be in Application layer; infrastructure concerns in Infrastructure layer. **Rationale**: Enforced by constitution Principle I.
- **NFR-006 (Scalability)**: System MUST support up to 10,000 registered users. Token validation MUST be stateless (JWT verification without database lookup). **Rationale**: Standard web application scale; stateless validation improves performance.

### Business Rules

| Rule ID | Description | Reason | Impact |
|---------|-------------|--------|--------|
| BR-001 | Email addresses MUST be unique and case-insensitive | Prevent duplicate accounts | Unique index; normalized storage |
| BR-002 | Passwords MUST be at least 8 characters | Enforce minimum security | Validation on registration |
| BR-003 | Passwords MUST contain at least one uppercase letter | Enforce password complexity | Validation on registration |
| BR-004 | Passwords MUST contain at least one lowercase letter | Enforce password complexity | Validation on registration |
| BR-005 | Passwords MUST contain at least one digit | Enforce password complexity | Validation on registration |
| BR-006 | Access tokens MUST expire after 15 minutes (configurable) | Limit exposure window | JWT expiry claim |
| BR-007 | Refresh tokens MUST expire after 7 days (configurable) | Balance security vs. UX | Database expiry check |
| BR-008 | Refresh tokens MUST be invalidated after use (rotation) | Prevent token reuse attacks | Token rotation on refresh |
| BR-009 | A maximum of 5 refresh tokens per user MUST be enforced | Prevent token proliferation | Cleanup on new login |
| BR-010 | Email comparison MUST be case-insensitive | Consistent user identification | Normalized email storage |
| BR-011 | Auth endpoints MUST be rate-limited to 100 requests per minute per IP | Prevent brute-force attacks | Rate limit enforcement |
| BR-012 | New users MUST be assigned "User" role by default | Consistent onboarding | Default role assignment |

### Key Entities

- **User**: Represents a registered user with email, hashed password, role, and account metadata. One-to-one with authentication identity.
- **RefreshToken**: Represents an active refresh token associated with a user. Contains token hash, expiry, device info, and revocation status. Multiple tokens per user (up to 5).

---

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can register and receive tokens in under 300ms (measured from request to response).
- **SC-002**: Users can login and receive tokens in under 300ms.
- **SC-003**: Token refresh completes in under 100ms.
- **SC-004**: 100% of protected endpoints reject unauthenticated requests with 401 status.
- **SC-005**: Passwords are never stored in plaintext (verified by database inspection).
- **SC-006**: All authentication events are logged with timestamp and user identifier.
- **SC-007**: Expired or revoked refresh tokens are rejected 100% of the time.
- **SC-008**: Email uniqueness is enforced at database level (no duplicate accounts possible).

---

## Assumptions

- HTTPS is enforced in production environments (required for secure token transmission).
- The frontend application handles JWT token storage (localStorage or httpOnly cookie).
- Existing todo management and lunch preference features are already implemented and functional.
- No email verification is required for this version (email is trusted at registration).
- No password reset functionality is required for this version.
- The application does not require multi-tenant or organization-based isolation.
- JWT signing secret is securely stored in environment variables or secret management.
- PostgreSQL is the sole database; no caching layer (Redis) is required for token storage.

---

## Clarifications

### Session 2026-07-15

- Q: Should authentication endpoints have rate limiting? → A: Yes, 100 req/min per IP on auth endpoints only (register, login, refresh).
- Q: What happens to the old refresh token when a new pair is issued? → A: Old refresh token is invalidated immediately on use (strict rotation).
- Q: Should logout revoke all refresh tokens or just the current one? → A: Revoke only the current session's refresh token (single device logout).
- Q: Should the error message distinguish between expired vs. missing tokens? → A: Yes, distinct messages: "Token has expired." vs. "Authentication is required."
- Q: What claims should the JWT access token contain? → A: User ID, email, and role claim (extensible for future RBAC).

---

## Risks

| Risk | Category | Severity | Mitigation |
|------|----------|----------|------------|
| JWT signing key compromise allows token forgery | Security | High | Secure key storage; key rotation support; short-lived access tokens |
| Refresh token database grows unbounded | Technical | Medium | Token cleanup job; max 5 tokens per user; expired token purge |
| Case-insensitive email comparison may have edge cases | Technical | Low | Normalize email to lowercase on storage and comparison |
| Password brute-force attacks | Security | Medium | Rate limiting on login endpoint; account lockout (future enhancement) |
| Token refresh race conditions | Technical | Low | Database row-level locking on token rotation |

---

## Open Questions

- None. All clarifications resolved via session 2026-07-15.

---

## Contract

### User Inputs

#### Register

| Field | Data Type | Required | Validation Rules | Constraints | Example |
|-------|-----------|----------|------------------|-------------|---------|
| Email | String | Yes | Valid email format, max 254 characters | RFC 5321 compliant | "user@example.com" |
| Password | String | Yes | Min 8 chars, uppercase, lowercase, digit | Not stored in plaintext | "SecurePass1" |

#### Login

| Field | Data Type | Required | Validation Rules | Constraints | Example |
|-------|-----------|----------|------------------|-------------|---------|
| Email | String | Yes | Valid email format | Case-insensitive match | "user@example.com" |
| Password | String | Yes | Non-empty | Verified against hash | "SecurePass1" |

#### Refresh Token

| Field | Data Type | Required | Validation Rules | Constraints | Example |
|-------|-----------|----------|------------------|-------------|---------|
| RefreshToken | String | Yes | Non-empty, valid opaque token | Must be unexpired and not revoked | "eyJhbGciOi..." |

### API Contracts

#### Authentication

| Endpoint | HTTP Method | Authentication | Request | Response | Status Codes |
|----------|-------------|----------------|---------|----------|--------------|
| `/api/v1/auth/register` | POST | None | RegisterRequest body | AuthResponse (tokens) | 201, 400, 409, 429 |
| `/api/v1/auth/login` | POST | None | LoginRequest body | AuthResponse (tokens) | 200, 400, 401, 429 |
| `/api/v1/auth/refresh` | POST | None | RefreshRequest body | AuthResponse (tokens) | 200, 401, 429 |
| `/api/v1/auth/logout` | POST | JWT | None | No content | 204, 401 |
| `/api/v1/auth/me` | GET | JWT | None | UserProfileResponse | 200, 401 |

#### Response Models

##### AuthResponse

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| AccessToken | String | No | JWT access token | Expiry per config (default 15min) |
| RefreshToken | String | No | Opaque refresh token | Expiry per config (default 7 days) |
| ExpiresIn | Integer | No | Access token lifetime in seconds | 900 (15 min) |
| TokenType | String | No | Token type | "Bearer" |

##### UserProfileResponse

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| Id | Guid | No | User unique identifier | Generated server-side |
| Email | String | No | User email address | Normalized lowercase |
| Role | String | No | User role | Default "User"; extensible for future RBAC |
| CreatedAt | DateTime | No | Account creation timestamp | ISO 8601 UTC |

#### Error Codes

| HTTP Status | Error Code | Trigger Condition | Returned Message |
|-------------|------------|-------------------|------------------|
| 400 | VALIDATION_ERROR | Invalid input (email format, password strength) | "One or more validation errors occurred." |
| 401 | UNAUTHORIZED | Missing JWT token | "Authentication is required." |
| 401 | TOKEN_EXPIRED | Expired JWT access token | "Token has expired." |
| 401 | UNAUTHORIZED | Invalid credentials | "Invalid email or password." |
| 409 | CONFLICT | Email already registered | "An account with this email already exists." |
| 429 | RATE_LIMIT_EXCEEDED | Too many login attempts | "Too many requests. Please try again later." |
| 500 | INTERNAL_ERROR | Unexpected server error | "An unexpected error occurred. Please try again later." |
