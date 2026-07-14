# Research: Basic Authentication

**Feature**: 003-basic-auth
**Date**: 2026-07-15
**Status**: Complete

---

## 1. Password Hashing

**Decision**: BCrypt.Net with cost factor 12

**Rationale**:
- Industry-standard for password hashing
- Adaptive cost factor (can be increased over time)
- Built-in salt generation prevents rainbow table attacks
- Widely supported in .NET ecosystem via `BCrypt.Net` NuGet package

**Alternatives Considered**:
- Argon2id: More memory-hard (better against GPU attacks), but less mature .NET libraries
- PBKDF2: Built into .NET, but requires manual salt management and is slower to verify
- scrypt: Good alternative, but less common in .NET ecosystem

**Package**: `BCrypt.Net-Next` v4.0.3

---

## 2. JWT Token Generation

**Decision**: System.IdentityModel.Tokens.Jwt (Microsoft's official library)

**Rationale**:
- Official Microsoft library, well-maintained
- Full support for JWT creation, validation, and signing
- Compatible with ASP.NET Core authentication middleware
- Supports HS256 (symmetric) and RS256 (asymmetric) signing

**Alternatives Considered**:
- JWT.net: Lightweight, but lacks integration with ASP.NET Core auth middleware
- Joe JWT: Minimal, but less feature-complete
- Manual Base64 encoding: Error-prone, not recommended

**Signing Algorithm**: HS256 (symmetric) for simplicity; can upgrade to RS256 later

**Package**: `System.IdentityModel.Tokens.Jwt` v8.x (included in ASP.NET Core)

---

## 3. Refresh Token Storage

**Decision**: Store SHA256 hash of refresh token in database

**Rationale**:
- Opaque tokens (not JWT) stored as hashes prevent token theft from DB compromise
- SHA256 is fast and sufficient for token lookup
- Database stores only the hash, not the plaintext token
- Enables token revocation and rotation tracking

**Alternatives Considered**:
- Store plaintext tokens: Simpler, but major security risk if DB is compromised
- Use JWT for refresh tokens: More complex, harder to revoke
- Use Redis for token storage: Adds infrastructure dependency, not needed at current scale

---

## 4. Rate Limiting

**Decision**: Leverage existing `RateLimitingMiddleware` (100 req/min per IP)

**Rationale**:
- Already implemented in the codebase
- Uses in-memory `ConcurrentDictionary` (sufficient for single-server deployment)
- Returns proper 429 status with headers
- Applied globally; auth endpoints will inherit the limit

**Alternatives Considered**:
- Separate rate limit per endpoint: More granular, but adds complexity
- Redis-based rate limiting: Needed for multi-server, but overkill for current scale
- ASP.NET Core's built-in rate limiting: Available in .NET 10, but existing custom middleware works

---

## 5. Email Normalization

**Decision**: Store emails in lowercase; normalize on registration/login

**Rationale**:
- Case-insensitive comparison per BR-010
- PostgreSQL `citext` extension provides case-insensitive text, but lowercase normalization is simpler
- Consistent with existing patterns (no `citext` in current schema)

**Alternatives Considered**:
- PostgreSQL `citext` extension: Native case-insensitive, but requires extension installation
- Case-insensitive index: More complex, unnecessary with normalized storage
- Store as-is, compare with `ILIKE`: Slower, index-unfriendly

---

## 6. Refresh Token Cleanup

**Decision**: Background service (`RefreshTokenCleanupService`) runs daily

**Rationale**:
- Existing pattern: `TodoCleanupService` runs daily for soft-deleted todos
- Prevents unbounded growth of refresh tokens
- Removes expired tokens and enforces max 5 per user (BR-009)

**Alternatives Considered**:
- Cleanup on every login: Adds latency to login flow
- No cleanup: Token table grows unbounded
- Database-level TTL: PostgreSQL doesn't have native TTL; requires extension

---

## 7. User Entity Design

**Decision**: Separate `User` entity with `Id`, `Email`, `HashedPassword`, `Role`, `CreatedAt`, `UpdatedAt`

**Rationale**:
- Aligns with existing entity conventions (Guid PK, timestamps)
- Role field for future RBAC extensibility (clarification Q5)
- No soft delete (account deletion is out of scope)
- One-to-many with `RefreshToken`

**Alternatives Considered**:
- Extend existing entity: No existing user entity to extend
- Use ASP.NET Identity: Too heavy for basic auth; conflicts with Clean Architecture
- Store role in JWT only: Simpler, but makes role changes require token refresh

---

## 8. JWT Claims

**Decision**: User ID, Email, Role claims in access token

**Rationale**:
- Clarification Q5 confirmed: extensible for future RBAC
- User ID enables stateless authentication (no DB lookup per request)
- Email reduces need for /auth/me calls
- Role enables future authorization without token refresh

**Alternatives Considered**:
- User ID only: Minimal, but requires /auth/me for any user info
- User ID + email: Missing role, requires re-issue for RBAC
- All claims in JWT: Token too large, harder to revoke

---

## 9. Error Handling

**Decision**: Extend existing `ExceptionMappingMiddleware` with new exception types

**Rationale**:
- Existing pattern: `ConflictException` → 409, `KeyNotFoundException` → 404
- Add `AuthenticationException` → 401, `TokenExpiredException` → 401
- Consistent with RFC 7807 Problem Details format

**Alternatives Considered**:
- Return results directly from handlers: Breaks CQRS pattern
- Use FluentResponse for auth errors: Possible, but middleware is more consistent
- Return raw HTTP responses: Bypasses middleware, harder to test

---

## 10. Migration Strategy

**Decision**: Create single `AddAuthentication` migration with `Users` and `RefreshTokens` tables

**Rationale**:
- Follows existing naming convention (`AddLunchPreferences`, `InitialCreate`)
- Both tables are tightly coupled (FK relationship)
- Single migration simplifies rollback

**Alternatives Considered**:
- Separate migrations for Users and RefreshTokens: Unnecessary complexity
- Code-first with auto-migration: Not used in this project (manual migrations preferred)

---

## 11. Testing Strategy

**Decision**: Follow existing patterns (unit tests for handlers/validators, integration tests with TestAuthHandler)

**Rationale**:
- Existing integration tests use `CustomWebApplicationFactory` with InMemory DB
- `TestAuthHandler` already exists for bypassing JWT validation
- Unit tests use Moq for repository mocking
- xUnit + FluentAssertions (implied by existing tests)

**Alternatives Considered**:
- Test containers for real PostgreSQL: More realistic, but slower and more complex
- End-to-end tests with real JWT: Requires full token generation, adds complexity

---

## Summary

All technical unknowns have been resolved. The implementation will follow existing project conventions and leverage established patterns. No architectural changes required.
