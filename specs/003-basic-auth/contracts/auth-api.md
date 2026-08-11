# API Contracts: Authentication

**Feature**: 003-basic-auth
**Date**: 2026-07-15

---

## Endpoints

### POST `/api/v1/auth/register`

**Description**: Register a new user account

**Authentication**: None

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "SecurePass1"
}
```

**Request Schema**:
| Field | Type | Required | Validation | Constraints |
|-------|------|----------|------------|-------------|
| email | string | Yes | Valid email format | Max 254 chars, RFC 5321 |
| password | string | Yes | Password strength | Min 8 chars, uppercase, lowercase, digit |

**Success Response** (201 Created):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJl...",
  "expiresIn": 900,
  "tokenType": "Bearer"
}
```

**Error Responses**:
| Status | Code | Message | Trigger |
|--------|------|---------|---------|
| 400 | VALIDATION_ERROR | "One or more validation errors occurred." | Invalid email or weak password |
| 409 | CONFLICT | "An account with this email already exists." | Duplicate email |
| 429 | RATE_LIMIT_EXCEEDED | "Too many requests. Please try again later." | >100 req/min from IP |

---

### POST `/api/v1/auth/login`

**Description**: Authenticate user and receive tokens

**Authentication**: None

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "SecurePass1"
}
```

**Request Schema**:
| Field | Type | Required | Validation | Constraints |
|-------|------|----------|------------|-------------|
| email | string | Yes | Valid email format | Max 254 chars |
| password | string | Yes | Non-empty | Verified against hash |

**Success Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJl...",
  "expiresIn": 900,
  "tokenType": "Bearer"
}
```

**Error Responses**:
| Status | Code | Message | Trigger |
|--------|------|---------|---------|
| 400 | VALIDATION_ERROR | "One or more validation errors occurred." | Invalid input |
| 401 | UNAUTHORIZED | "Invalid email or password." | Wrong credentials |
| 429 | RATE_LIMIT_EXCEEDED | "Too many requests. Please try again later." | >100 req/min from IP |

---

### POST `/api/v1/auth/refresh`

**Description**: Exchange refresh token for new token pair

**Authentication**: None (uses refresh token in body)

**Request Body**:
```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJl..."
}
```

**Request Schema**:
| Field | Type | Required | Validation | Constraints |
|-------|------|----------|------------|-------------|
| refreshToken | string | Yes | Non-empty | Must be valid, unexpired, not revoked |

**Success Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "bmV3IHJlZnJl...",
  "expiresIn": 900,
  "tokenType": "Bearer"
}
```

**Error Responses**:
| Status | Code | Message | Trigger |
|--------|------|---------|---------|
| 400 | VALIDATION_ERROR | "One or more validation errors occurred." | Empty token |
| 401 | UNAUTHORIZED | "Invalid or expired refresh token." | Token invalid/expired/revoked |
| 429 | RATE_LIMIT_EXCEEDED | "Too many requests. Please try again later." | >100 req/min from IP |

---

### POST `/api/v1/auth/logout`

**Description**: Revoke current session's refresh token

**Authentication**: JWT (access token in Authorization header)

**Request Body**: None

**Success Response** (204 No Content): No body

**Error Responses**:
| Status | Code | Message | Trigger |
|--------|------|---------|---------|
| 401 | UNAUTHORIZED | "Authentication is required." | Missing or invalid JWT |

---

### GET `/api/v1/auth/me`

**Description**: Get current user profile

**Authentication**: JWT (access token in Authorization header)

**Request Body**: None

**Success Response** (200 OK):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "role": "User",
  "createdAt": "2026-07-15T10:30:00Z"
}
```

**Response Schema**:
| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | string (Guid) | No | User unique identifier |
| email | string | No | User email (normalized lowercase) |
| role | string | No | User role (default "User") |
| createdAt | string (DateTime) | No | Account creation timestamp (ISO 8601) |

**Error Responses**:
| Status | Code | Message | Trigger |
|--------|------|---------|---------|
| 401 | UNAUTHORIZED | "Authentication is required." | Missing or invalid JWT |

---

## JWT Access Token Claims

```json
{
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "role": "User",
  "iat": 1689388200,
  "exp": 1689389100,
  "iss": "TodoApp",
  "aud": "TodoApp"
}
```

| Claim | Type | Description |
|-------|------|-------------|
| sub | string (Guid) | User ID |
| email | string | User email |
| role | string | User role |
| iat | int | Issued at (Unix timestamp) |
| exp | int | Expiration (Unix timestamp, +15 min default) |
| iss | string | Issuer ("TodoApp") |
| aud | string | Audience ("TodoApp") |

---

## Headers

All responses include:
- `X-Correlation-Id`: Request correlation ID (for tracing)
- `Content-Type`: `application/json` (or `application/problem+json` for errors)

Rate-limited responses include:
- `X-RateLimit-Remaining`: Remaining requests in window
- `Retry-After`: Seconds until rate limit resets (429 responses only)

---

## Authentication Header Format

For endpoints requiring JWT:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```
