# Quickstart: Basic Authentication

**Feature**: 003-basic-auth
**Date**: 2026-07-15

---

## Prerequisites

- .NET 10 SDK installed
- PostgreSQL running and accessible
- `dotnet-ef` tool installed (`dotnet tool install --global dotnet-ef`)
- Existing TodoApp database (from previous features)

---

## Setup

### 1. Apply Migration

```bash
cd api
dotnet ef migrations add AddAuthentication --project src/TodoApp.Infrastructure --startup-project src/TodoApp.Api
dotnet ef database update --project src/TodoApp.Infrastructure --startup-project src/TodoApp.Api
```

### 2. Configure JWT Settings

Add to `appsettings.json` (or user secrets):
```json
{
  "Jwt": {
    "Secret": "your-256-bit-secret-key-here",
    "Issuer": "TodoApp",
    "Audience": "TodoApp",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  }
}
```

### 3. Build and Run

```bash
cd api
dotnet build
dotnet run --project src/TodoApp.Api
```

API starts at `https://localhost:5001` (or `http://localhost:5000`)

---

## Validation Scenarios

### Scenario 1: User Registration

```bash
curl -X POST https://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"SecurePass1"}'
```

**Expected**: 201 Created with `accessToken`, `refreshToken`, `expiresIn`, `tokenType`

---

### Scenario 2: Duplicate Registration

```bash
# Register same email again
curl -X POST https://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"SecurePass1"}'
```

**Expected**: 409 Conflict with message "An account with this email already exists."

---

### Scenario 3: Weak Password

```bash
curl -X POST https://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"new@example.com","password":"weak"}'
```

**Expected**: 400 Bad Request with validation errors for password requirements

---

### Scenario 4: User Login

```bash
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"SecurePass1"}'
```

**Expected**: 200 OK with `accessToken`, `refreshToken`, `expiresIn`, `tokenType`

---

### Scenario 5: Invalid Login

```bash
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"WrongPassword"}'
```

**Expected**: 401 Unauthorized with message "Invalid email or password."

---

### Scenario 6: Access Protected Endpoint

```bash
# Use token from login/register response
curl https://localhost:5001/api/v1/todos \
  -H "Authorization: Bearer <access_token>"
```

**Expected**: 200 OK with todo list

---

### Scenario 7: Access Without Token

```bash
curl https://localhost:5001/api/v1/todos
```

**Expected**: 401 Unauthorized with message "Authentication is required."

---

### Scenario 8: Token Refresh

```bash
curl -X POST https://localhost:5001/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"<refresh_token>"}'
```

**Expected**: 200 OK with new `accessToken` and `refreshToken`

---

### Scenario 9: Reuse Old Refresh Token

```bash
# Use old refresh token after refresh
curl -X POST https://localhost:5001/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"<old_refresh_token>"}'
```

**Expected**: 401 Unauthorized with message "Invalid or expired refresh token."

---

### Scenario 10: Get User Profile

```bash
curl https://localhost:5001/api/v1/auth/me \
  -H "Authorization: Bearer <access_token>"
```

**Expected**: 200 OK with `id`, `email`, `role`, `createdAt`

---

### Scenario 11: Logout

```bash
curl -X POST https://localhost:5001/api/v1/auth/logout \
  -H "Authorization: Bearer <access_token>"
```

**Expected**: 204 No Content

---

### Scenario 12: Use Refresh Token After Logout

```bash
curl -X POST https://localhost:5001/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"<refresh_token_from_logout>"}'
```

**Expected**: 401 Unauthorized

---

## Running Tests

### Unit Tests

```bash
cd api
dotnet test tests/TodoApp.UnitTests --filter "FullyQualifiedName~Auth"
```

### Integration Tests

```bash
cd api
dotnet test tests/TodoApp.IntegrationTests --filter "FullyQualifiedName~Auth"
```

---

## Verification Checklist

- [ ] Registration creates account and returns tokens
- [ ] Duplicate email returns 409
- [ ] Weak password returns 400 with validation errors
- [ ] Login with correct credentials returns tokens
- [ ] Login with wrong password returns 401
- [ ] Protected endpoints require JWT
- [ ] Expired token returns 401 "Token has expired."
- [ ] Missing token returns 401 "Authentication is required."
- [ ] Refresh token exchange returns new pair
- [ ] Old refresh token is invalidated after use
- [ ] Logout revokes current refresh token
- [ ] Profile returns user ID, email, role, createdAt
- [ ] Rate limiting returns 429 after 100 requests/min
