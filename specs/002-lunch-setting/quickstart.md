# Quickstart: Lunch Setting

## Prerequisites

- .NET 10 SDK
- PostgreSQL database (local or remote)
- Clone of TodoApp repo with `api/` and existing solution
- Access to JWT token from authentication system

## Setup

```bash
# 1. Navigate to API project
cd api/src/TodoApp.Api

# 2. Apply migrations
dotnet ef database update

# 3. Run the API
dotnet run
```

The API starts at `https://localhost:5001` with Scalar docs at `/scalar/v1`.

## Validation Scenarios

### Scenario 1: First-time user gets defaults

```bash
curl -s -H "Authorization: Bearer <token>" \
  https://localhost:5001/api/v1/lunch-preferences | jq .
```

Expected: 200 response with default profile (12:00-13:00 lunch window, 60min break, notifications enabled, no restrictions/favorites/exclusions).

### Scenario 2: Update preferences

```bash
curl -s -X PUT \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "dietaryRestrictions": ["Vegetarian"],
    "lunchStartTime": "12:00",
    "lunchEndTime": "13:00",
    "breakDurationMinutes": 60,
    "notificationsEnabled": true
  }' \
  https://localhost:5001/api/v1/lunch-preferences | jq .
```

Expected: 200 response with updated profile reflecting the new dietary restriction.

### Scenario 3: Validation error — invalid time range

```bash
curl -s -X PUT \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "lunchStartTime": "14:00",
    "lunchEndTime": "13:00"
  }' \
  https://localhost:5001/api/v1/lunch-preferences | jq .
```

Expected: 400 response with `isSuccess: false` and validation error message about lunch end time needing to be after start time.

### Scenario 4: Reset to defaults

```bash
curl -s -X DELETE \
  -H "Authorization: Bearer <token>" \
  https://localhost:5001/api/v1/lunch-preferences | jq .
```

Expected: 200 response with profile reset to organization defaults.

### Scenario 5: List size overflow

```bash
curl -s -X PUT \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "favoriteMeals": ["A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U"]
  }' \
  https://localhost:5001/api/v1/lunch-preferences | jq .
```

Expected: 422 response with error about exceeding max 20 items.

## Run Tests

```bash
# Unit tests
dotnet test api/tests/TodoApp.UnitTests --filter "FullyQualifiedName~LunchPreference"

# Integration tests
dotnet test api/tests/TodoApp.IntegrationTests --filter "FullyQualifiedName~LunchPreference"
```

## Contracts & Data Model

- API contracts: [contracts/api-contracts.md](contracts/api-contracts.md)
- Data model: [data-model.md](data-model.md)
- Full plan: [plan.md](plan.md)
