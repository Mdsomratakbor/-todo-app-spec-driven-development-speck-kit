# Feature Specification: Lunch Setting

**Feature Branch**: `002-lunch-setting`

**Created**: 2026-07-04

**Status**: Draft

**Input**: User description: "add new plan for lunchsetting for backend project"

---

## Context

### Feature Purpose

Provide a backend API for managing lunch settings, allowing users to configure their meal preferences, dietary restrictions, lunch schedule, and ordering preferences. The feature enables organizations to streamline lunch management with configurable settings per user.

### Business Objective

Deliver a reliable, maintainable API that supports lunch preference management, enabling integration with frontend applications or third-party services for lunch ordering, meal planning, and dietary compliance tracking.

### Scope

- Create, read, update, and delete lunch preference profiles
- Configure dietary restrictions and allergies
- Set meal timing preferences (lunch window, break duration)
- Manage favorite meal choices and exclusions
- Configure notification preferences for lunch reminders
- API-only feature (no frontend)

### Explicit Boundaries

- Backend API only - no UI components
- Single-tenant organization scope with per-user settings
- All data persisted to PostgreSQL database
- RESTful API under versioned prefix

### Out of Scope

- User authentication and registration (assumes existing auth)
- Meal ordering or payment processing
- Menu management or cafeteria integration
- Nutritional tracking or calorie counting
- Mobile push notifications
- Reporting or analytics dashboards
- Team collaboration or shared lunch planning
- Calendar integration

### Assumptions

- An existing authentication system provides user identity via JWT
- Users access the API through an existing frontend application
- PostgreSQL database is pre-configured
- Organizations manage their own lunch programs independently

### Dependencies

- .NET 10 Web API runtime
- PostgreSQL database server
- Cartographer.Mapper for object mapping
- FluentResponse.ApiWrapper for API responses
- FluentValidation for input validation

### Technology Stack

Per the TodoApp Constitution (v1.3.0):
- **Backend**: .NET 10 Web API with Clean Architecture (API, Application, Domain, Infrastructure layers)
- **Database**: PostgreSQL via Entity Framework Core
- **Mapping**: Cartographer.Mapper
- **API Responses**: FluentResponse.ApiWrapper
- **API Documentation**: Scalar (OpenAPI 3.x)
- **API Versioning**: URL path prefix (`/api/v1/`)

### Existing Conventions

- RESTful API design with plural nouns under versioned prefix
- URL path API versioning (`/api/v1/...`)
- Consistent response envelopes via FluentResponse.ApiWrapper
- Clean Architecture with dependency inversion

---

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Manage Lunch Preferences (Priority: P1)

As a user, I want to view and update my lunch preferences so that the system knows my dietary needs and meal choices.

**Why this priority**: Core capability - without preference management, no downstream integration (ordering, planning) is possible.

**Independent Test**: A user can retrieve their current lunch preferences, update any field (dietary restrictions, meal timing, favorites), and see the changes persisted.

**Acceptance Scenarios**:

1. **Given** I have an existing lunch profile, **When** I GET my preferences, **Then** all fields (dietary restrictions, meal timing, favorites, exclusions) are returned.
2. **Given** I want to update my dietary restrictions, **When** I PUT my preferences with new restrictions, **Then** the updated preferences are persisted and returned.
3. **Given** I am a new user with no preferences, **When** I GET my preferences, **Then** a default preference set is returned.

---

### User Story 2 — Configure Lunch Schedule (Priority: P2)

As a user, I want to set my preferred lunch time window so that meal preparation and delivery can align with my schedule.

**Why this priority**: Enhances the value of preferences by adding temporal context, but the system functions without it (defaults apply).

**Independent Test**: A user can set their lunch start time, end time, and duration; the API validates the time window constraints and persists the schedule.

**Acceptance Scenarios**:

1. **Given** I want to set my lunch window, **When** I update my preferences with a start time and end time, **Then** the schedule is persisted with valid time range.
2. **Given** I set a lunch end time before start time, **When** I submit, **Then** the API returns 400 with validation error.
3. **Given** I want to remove my lunch schedule, **When** I clear the time fields, **Then** the schedule defaults to the organization's standard lunch window.

---

### User Story 3 — Manage Favorite Meals & Exclusions (Priority: P3)

As a user, I want to maintain a list of favorite meals and excluded items so that suggestions and orders align with my preferences.

**Why this priority**: Improves user satisfaction but preference management functions without favorites/exclusions.

**Independent Test**: A user can add and remove favorite meals and excluded items from their preference profile with proper validation.

**Acceptance Scenarios**:

1. **Given** I have a list of favorite meals, **When** I add a new favorite, **Then** the favorites list is updated with the new entry.
2. **Given** I have excluded items, **When** I remove an exclusion, **Then** it is removed from the exclusion list.
3. **Given** I try to add a duplicate favorite, **Then** the API returns 409 Conflict.

---

### Edge Cases

- What happens when a user submits dietary restrictions with invalid values? — 400 with per-field validation errors.
- How does the system handle a user with no lunch profile? — Default preferences are created on first GET.
- What happens when the lunch window spans midnight? — System supports 24-hour time and handles wrap-around correctly.
- How does the system handle concurrent updates to the same preference profile? — Last-write-wins strategy.
- What happens when an organization removes a dietary option that users have selected? — Existing preferences are preserved but flagged as unavailable.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to create a lunch preference profile with dietary restrictions, meal timing, favorites, and exclusions. **Priority**: P1. **Rationale**: Core capability for preference management. **Acceptance Criteria**: POST creates profile with default values; subsequent GET returns the profile.
- **FR-002**: System MUST allow users to retrieve their lunch preference profile. **Priority**: P1. **Rationale**: Users need to view their settings. **Acceptance Criteria**: GET returns full preference profile with all fields.
- **FR-003**: System MUST allow users to update any field in their lunch preference profile. **Priority**: P1. **Rationale**: Preferences change over time. **Acceptance Criteria**: PUT updates only provided fields (partial update) and returns the updated profile.
- **FR-004**: System MUST allow users to reset their preferences to defaults. **Priority**: P2. **Rationale**: Users may want to start over. **Acceptance Criteria**: DELETE (reset) returns profile with default values.
- **FR-005**: System MUST validate dietary restrictions against a predefined list. **Priority**: P2. **Rationale**: Prevent invalid dietary entries. **Acceptance Criteria**: Invalid restriction returns 400 with validation error.
- **FR-006**: System MUST support configurable meal timing (lunch start, lunch end, break duration). **Priority**: P2. **Rationale**: Allow schedule customization. **Acceptance Criteria**: Time window validated end >= start + minimum duration.
- **FR-007**: System MUST allow users to maintain a list of favorite meal choices. **Priority**: P3. **Rationale**: Personalize meal suggestions. **Acceptance Criteria**: Favorites list is managed and persisted with the profile.
- **FR-008**: System MUST allow users to maintain a list of excluded items. **Priority**: P3. **Rationale**: Prevent unwanted meal assignments. **Acceptance Criteria**: Exclusions list is managed and persisted with the profile.
- **FR-009**: System MUST support organization-level default preferences applied to new users. **Priority**: P2. **Rationale**: Consistent onboarding experience. **Acceptance Criteria**: New user profile inherits org defaults.

### Non-Functional Requirements

- **NFR-001 (Performance)**: Preference profile API MUST respond in under 200ms for read operations. **Rationale**: Preferences are loaded frequently; fast response is critical for UX.
- **NFR-002 (Security)**: All API endpoints MUST require authentication (JWT token). Input MUST be sanitized. API rate limiting MUST be applied (100 requests/minute per user). **Rationale**: Protect user data per constitution Principle V.
- **NFR-003 (Validation)**: All input fields MUST be validated server-side. Backend MUST return RFC 7807 Problem Details for validation failures. **Rationale**: Defense-in-depth validation per constitution Principle IV.
- **NFR-004 (Logging)**: Every API request MUST log method, path, status code, duration, and correlation ID. **Rationale**: Observability per constitution Operational Standards.
- **NFR-005 (Maintainability)**: Backend MUST follow Clean Architecture layering. **Rationale**: Enforced by constitution Principle I.
- **NFR-006 (Scalability)**: System MUST support up to 10,000 user preference profiles without performance degradation. **Rationale**: Standard organizational scale.

### Business Rules

| Rule ID | Description | Reason | Impact |
|---------|-------------|--------|--------|
| BR-001 | Dietary restrictions MUST be from a predefined list of valid values | Ensure data consistency | Validation constraint |
| BR-002 | Lunch end time MUST be after start time | Logical time window | Validation on update |
| BR-003 | Lunch duration MUST be between 15 and 120 minutes | Reasonable meal break range | Validation on update |
| BR-004 | Favorite meals list MUST not exceed 20 items | Prevent unbounded list growth | List size constraint |
| BR-005 | Exclusions list MUST not exceed 20 items | Prevent unbounded list growth | List size constraint |
| BR-006 | A default preference profile MUST exist for each user on first access | Ensure every user has preference data | Auto-creation on first GET |
| BR-007 | Organization defaults MUST include standard lunch window (12:00-13:00) | Consistent baseline | Seed data configuration |

### Key Entities

- **LunchPreference**: Represents a user's complete lunch settings including dietary restrictions, meal timing, favorite meals, exclusions, and notification preferences. One-to-one with user identity.
- **DietaryRestriction**: A controlled vocabulary entry representing a dietary rule (e.g., vegetarian, gluten-free, halal). Referenced by LunchPreference.
- **FavoriteMeal**: A user's preferred meal choice. Belongs to a LunchPreference.
- **ExcludedItem**: A food item or ingredient the user excludes. Belongs to a LunchPreference.

### API Contracts

#### Lunch Preferences

| Endpoint | HTTP Method | Authentication | Request | Response | Status Codes |
|----------|-------------|----------------|---------|----------|--------------|
| `/api/v1/lunch-preferences` | GET | JWT | None | LunchPreferenceResponse | 200, 401 |
| `/api/v1/lunch-preferences` | PUT | JWT | UpdateLunchPreferenceRequest body | LunchPreferenceResponse | 200, 400, 401, 422 |
| `/api/v1/lunch-preferences` | DELETE | JWT | None | LunchPreferenceResponse (defaults) | 200, 401 |

#### Response Models

##### LunchPreferenceResponse

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| Id | Guid | No | Unique identifier | Generated server-side |
| UserId | Guid | No | Associated user | From JWT claim |
| DietaryRestrictions | string[] | Yes | List of dietary restrictions | Valid values from controlled list |
| LunchStartTime | TimeOnly | Yes | Preferred lunch start | HH:mm format |
| LunchEndTime | TimeOnly | Yes | Preferred lunch end | Must be after start |
| BreakDurationMinutes | int | Yes | Break duration in minutes | 15-120 |
| FavoriteMeals | string[] | Yes | Favorite meal choices | Max 20 items |
| ExcludedItems | string[] | Yes | Excluded food items | Max 20 items |
| NotificationsEnabled | bool | No | Lunch reminder notifications | Default true |
| CreatedAt | DateTime | No | Creation timestamp | ISO 8601 UTC |
| UpdatedAt | DateTime | Yes | Last update timestamp | ISO 8601 UTC |

##### UpdateLunchPreferenceRequest

| Field | Type | Required | Validation Rules | Constraints |
|-------|------|----------|------------------|-------------|
| DietaryRestrictions | string[] | No | Each value must be from valid list | Max 10 items |
| LunchStartTime | TimeOnly | No | Valid 24-hour time | Must be before end |
| LunchEndTime | TimeOnly | No | Valid 24-hour time | Must be after start |
| BreakDurationMinutes | int | No | 15-120 | Inclusive range |
| FavoriteMeals | string[] | No | Non-empty strings, trimmed | Max 20 items |
| ExcludedItems | string[] | No | Non-empty strings, trimmed | Max 20 items |
| NotificationsEnabled | bool | No | Boolean | — |

### Error Codes

| HTTP Status | Error Code | Trigger Condition | Returned Message |
|-------------|------------|-------------------|------------------|
| 400 | VALIDATION_ERROR | One or more input fields fail validation | "One or more validation errors occurred." |
| 401 | UNAUTHORIZED | Missing or invalid JWT token | "Authentication is required." |
| 404 | NOT_FOUND | User has no preference profile | "No lunch preference profile found." |
| 409 | CONFLICT | Duplicate favorite or exclusion entry | "The item already exists in the list." |
| 422 | UNPROCESSABLE_ENTITY | Invalid state transition or business rule violation | "The requested operation is not allowed." |
| 429 | RATE_LIMIT_EXCEEDED | API rate limit exceeded | "Too many requests. Please try again later." |
| 500 | INTERNAL_ERROR | Unexpected server error | "An unexpected error occurred." |

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can create/retrieve/update their lunch preferences in under 200ms API response time.
- **SC-002**: 100% of input fields are validated with appropriate error responses.
- **SC-003**: New users automatically receive default preferences on first access.
- **SC-004**: All CRUD operations return consistent FluentResponse.ApiWrapper envelopes.

## Assumptions

- An existing authentication/authorization system provides user identity and JWT tokens.
- The organization maintains a predefined list of dietary restriction options.
- The standard organization lunch window is 12:00-13:00 (configurable).
- Users access the API through an existing frontend or third-party integration.
- PostgreSQL database is pre-configured and accessible.
- No meal ordering or payment processing is required for this feature.

## Open Questions

- None. All defaults are based on the TodoApp Constitution (v1.3.0) and standard lunch management practices.
