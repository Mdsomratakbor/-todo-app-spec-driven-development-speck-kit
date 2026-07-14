# Data Model: Basic Authentication

**Feature**: 003-basic-auth
**Date**: 2026-07-15

---

## Entities

### User

| Field | Type | Nullable | Default | Description |
|-------|------|----------|---------|-------------|
| Id | Guid | No | GENERATED ALWAYS AS IDENTITY | Primary key |
| Email | string | No | — | Normalized lowercase email (max 254 chars) |
| HashedPassword | string | No | — | BCrypt hash (max 255 chars) |
| Role | string | No | "User" | User role (extensible for future RBAC) |
| CreatedAt | DateTime | No | NOW() | Account creation timestamp |
| UpdatedAt | DateTime | Yes | NULL | Last update timestamp |

**Indexes**:
- `IX_Users_Email` (unique) — Email lookup and uniqueness enforcement
- `IX_Users_Role` — Future role-based queries

**Validation Rules** (from spec):
- Email: Valid format, max 254 characters, case-insensitive uniqueness
- HashedPassword: Never stored as plaintext; BCrypt cost >= 12
- Role: Must be a valid role string (default "User")

---

### RefreshToken

| Field | Type | Nullable | Default | Description |
|-------|------|----------|---------|-------------|
| Id | Guid | No | GENERATED ALWAYS AS IDENTITY | Primary key |
| UserId | Guid | No | — | Foreign key to User |
| TokenHash | string | No | — | SHA256 hash of opaque token (max 64 chars) |
| ExpiresAt | DateTime | No | — | Token expiration timestamp |
| RevokedAt | DateTime | Yes | NULL | Revocation timestamp (null = active) |
| CreatedAt | DateTime | No | NOW() | Token creation timestamp |

**Indexes**:
- `IX_RefreshTokens_UserId` — User's tokens lookup
- `IX_RefreshTokens_TokenHash` (unique) — Token validation lookup
- `IX_RefreshTokens_ExpiresAt` — Cleanup job index

**Validation Rules** (from spec):
- TokenHash: SHA256 hex string (64 characters)
- ExpiresAt: Must be in the future on creation; 7-day default (BR-007)
- UserId: Must reference existing User (cascade delete)

---

## Relationships

```
User (1) ──── (0..*) RefreshToken
```

- **Delete Behavior**: Cascade (deleting a User deletes all their RefreshTokens)
- **Maximum Refresh Tokens per User**: 5 (BR-009) — enforced in application layer

---

## State Transitions

### RefreshToken States

```
[Active] ──→ [Revoked] (on logout or rotation)
[Active] ──→ [Expired] (on TTL expiry)
```

- **Active**: `RevokedAt` is NULL AND `ExpiresAt` > NOW()
- **Revoked**: `RevokedAt` is NOT NULL
- **Expired**: `ExpiresAt` <= NOW()

---

## Seed Data

- **Users**: None (accounts created via registration)
- **RefreshTokens**: None (tokens created on login/refresh)

---

## Migration

- **Migration Name**: `AddAuthentication`
- **Tables Created**: `Users`, `RefreshTokens`
- **Indexes Created**: 4 (as specified above)
- **Foreign Keys**: `RefreshTokens.UserId → Users.Id` (cascade delete)

---

## Query Patterns

| Pattern | Query | Purpose |
|---------|-------|---------|
| Get user by email | `WHERE Email = @email` | Login, registration check |
| Get user by ID | `WHERE Id = @userId` | Profile, token generation |
| Get active refresh token | `WHERE TokenHash = @hash AND RevokedAt IS NULL AND ExpiresAt > NOW()` | Token validation |
| Get user's refresh tokens | `WHERE UserId = @userId` | Token rotation, max count check |
| Cleanup expired tokens | `WHERE ExpiresAt < NOW() - INTERVAL '30 days'` | Daily cleanup job |
| Enforce max 5 tokens | `COUNT(*) WHERE UserId = @userId AND RevokedAt IS NULL` | Token limit enforcement |
