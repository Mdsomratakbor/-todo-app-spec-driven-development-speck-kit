# API Contracts: Lunch Preference

## Endpoints

### GET /api/v1/lunch-preferences

Retrieve the authenticated user's lunch preference profile. Auto-creates with defaults if no profile exists.

**Request**:
- Headers: `Authorization: Bearer <token>`
- Body: None

**Response 200**:
```json
{
  "isSuccess": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "dietaryRestrictions": ["Vegetarian", "Gluten-Free"],
    "lunchStartTime": "12:00",
    "lunchEndTime": "13:00",
    "breakDurationMinutes": 60,
    "favoriteMeals": ["Caesar Salad", "Grilled Chicken"],
    "excludedItems": ["Mushrooms", "Eggplant"],
    "notificationsEnabled": true,
    "createdAt": "2026-07-04T12:00:00Z",
    "updatedAt": "2026-07-04T12:30:00Z"
  },
  "message": null
}
```

**Response 401**:
```json
{
  "isSuccess": false,
  "data": null,
  "message": "Authentication is required."
}
```

---

### PUT /api/v1/lunch-preferences

Update the authenticated user's lunch preference profile. Partial update — only provided fields are changed.

**Request**:
- Headers: `Authorization: Bearer <token>`
- Body:
```json
{
  "dietaryRestrictions": ["Vegetarian"],
  "lunchStartTime": "11:30",
  "lunchEndTime": "12:30",
  "breakDurationMinutes": 45,
  "favoriteMeals": ["Caesar Salad", "Grilled Chicken", "Vegetable Stir Fry"],
  "excludedItems": ["Mushrooms"],
  "notificationsEnabled": true
}
```

**Response 200**: Same shape as GET response.

**Response 400** (Validation Error):
```json
{
  "isSuccess": false,
  "data": null,
  "message": "One or more validation errors occurred.",
  "errors": {
    "LunchEndTime": ["Lunch end time must be after start time."],
    "BreakDurationMinutes": ["Break duration must be between 15 and 120 minutes."]
  }
}
```

**Response 422** (Business Rule):
```json
{
  "isSuccess": false,
  "data": null,
  "message": "The requested operation is not allowed.",
  "errors": {
    "FavoriteMeals": ["Favorite meals list cannot exceed 20 items."]
  }
}
```

---

### DELETE /api/v1/lunch-preferences

Reset the authenticated user's lunch preference profile to organization defaults.

**Request**:
- Headers: `Authorization: Bearer <token>`
- Body: None

**Response 200**: Default profile returned (same shape as GET).

---

## Request Model: UpdateLunchPreferenceRequest

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| dietaryRestrictions | string[] | No | Each value from valid seed list; max 10 items |
| lunchStartTime | string (TimeOnly) | No | Format "HH:mm"; must be before lunchEndTime |
| lunchEndTime | string (TimeOnly) | No | Format "HH:mm"; must be after lunchStartTime |
| breakDurationMinutes | int | No | 15-120 inclusive |
| favoriteMeals | string[] | No | Each non-empty, trimmed; max 20 items |
| excludedItems | string[] | No | Each non-empty, trimmed; max 20 items |
| notificationsEnabled | bool | No | Boolean |

## Response Model: LunchPreferenceResponse

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | string (Guid) | No | Unique identifier |
| userId | string (Guid) | No | Associated user |
| dietaryRestrictions | string[] | Yes | List of dietary restrictions |
| lunchStartTime | string | Yes | "HH:mm" format |
| lunchEndTime | string | Yes | "HH:mm" format |
| breakDurationMinutes | int | Yes | Minutes |
| favoriteMeals | string[] | Yes | List of favorite meal names |
| excludedItems | string[] | Yes | List of excluded items |
| notificationsEnabled | bool | No | Notification opt-in |
| createdAt | string (DateTime) | No | ISO 8601 UTC |
| updatedAt | string (DateTime) | Yes | ISO 8601 UTC |

## Error Codes

| HTTP Status | Error Code | Condition |
|-------------|------------|-----------|
| 400 | VALIDATION_ERROR | Input validation failure |
| 401 | UNAUTHORIZED | Missing/invalid JWT |
| 404 | NOT_FOUND | No profile (should not occur with auto-create) |
| 409 | CONFLICT | Duplicate entry in list |
| 422 | UNPROCESSABLE_ENTITY | Business rule violation |
| 429 | RATE_LIMIT_EXCEEDED | Rate limit hit |
| 500 | INTERNAL_ERROR | Unexpected error |
