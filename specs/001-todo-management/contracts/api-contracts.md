# API Contracts: Todo Management System

**Created**: 2026-06-30
**Base URL**: `/api/v1`
**Authentication**: JWT Bearer token (all endpoints)
**Response Envelope**: FluentResponse.ApiWrapper

---

## Todo Endpoints

### List Todos

```
GET /api/v1/todos?page={page}&pageSize={pageSize}&statusId={statusId}&priorityId={priorityId}&categoryId={categoryId}&dueDateFrom={dueDateFrom}&dueDateTo={dueDateTo}&search={search}
```

**Query Parameters**:

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| page | int | No | 1 | Page number (1-indexed) |
| pageSize | int | No | 20 | Items per page (max 50) |
| statusId | int | No | — | Filter by status |
| priorityId | int | No | — | Filter by priority |
| categoryId | Guid | No | — | Filter by category |
| dueDateFrom | DateTime | No | — | Filter due date >= this value |
| dueDateTo | DateTime | No | — | Filter due date <= this value |
| search | string | No | — | Text search on title and description |

**Response**: `PaginatedResponse<TodoItemResponse>`

**Status Codes**: 200, 401

### Get Todo

```
GET /api/v1/todos/{id}
```

**Path Parameters**: `id` (Guid) — Todo identifier

**Response**: `TodoItemResponse`

**Status Codes**: 200, 401, 404

### Create Todo

```
POST /api/v1/todos
```

**Request Body**: `CreateTodoRequest`

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | Yes | 1–200 characters |
| description | string | No | 0–2000 characters |
| dueDate | DateTime | No | ISO 8601; max 5 years future |
| priorityId | int | Yes | 1–4 (Low/Medium/High/Critical) |
| categoryId | Guid | No | Must reference existing category |

**Response**: `TodoItemResponse` (HTTP 201)

**Status Codes**: 201, 400, 401, 422

### Update Todo

```
PUT /api/v1/todos/{id}
```

**Path Parameters**: `id` (Guid) — Todo identifier

**Request Body**: `UpdateTodoRequest`

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | Yes | 1–200 characters |
| description | string | No | 0–2000 characters |
| dueDate | DateTime | No | ISO 8601; max 5 years future |
| priorityId | int | Yes | 1–4 |
| categoryId | Guid | No | Must reference existing category |
| statusId | int | Yes | 1–3; valid forward transitions only |

**Response**: `TodoItemResponse`

**Status Codes**: 200, 400, 401, 404, 422

### Delete Todo

```
DELETE /api/v1/todos/{id}
```

**Path Parameters**: `id` (Guid) — Todo identifier

**Response**: No content (204)

**Status Codes**: 204, 401, 404

---

## Category Endpoints

### List Categories

```
GET /api/v1/categories
```

**Response**: `List<CategoryResponse>`

**Status Codes**: 200, 401

### Create Category

```
POST /api/v1/categories
```

**Request Body**: `CreateCategoryRequest`

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | 1–100 characters, unique |
| color | string | No | Hex `#RRGGBB` |

**Response**: `CategoryResponse` (HTTP 201)

**Status Codes**: 201, 400, 401, 409

### Update Category

```
PUT /api/v1/categories/{id}
```

**Path Parameters**: `id` (Guid) — Category identifier

**Request Body**: `UpdateCategoryRequest`

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | 1–100 characters, unique (excluding self) |
| color | string | No | Hex `#RRGGBB` |

**Response**: `CategoryResponse`

**Status Codes**: 200, 400, 401, 404, 409

### Delete Category

```
DELETE /api/v1/categories/{id}
```

**Path Parameters**: `id` (Guid) — Category identifier

**Response**: No content (204)

**Notes**: Associated todos are reassigned to the default "Uncategorized" category.

**Status Codes**: 204, 401, 404

---

## Response Models

### TodoItemResponse

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Buy groceries",
  "description": "Milk, eggs, bread",
  "dueDate": "2026-07-15T00:00:00Z",
  "priority": { "id": 2, "name": "Medium", "color": "#3498DB" },
  "category": { "id": "...", "name": "Personal", "color": "#2ECC71", "todoCount": 5 },
  "status": { "id": 1, "name": "Pending", "color": "#95A5A6" },
  "createdAt": "2026-06-30T12:00:00Z",
  "updatedAt": null
}
```

### CategoryResponse

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Work",
  "color": "#3498DB",
  "todoCount": 3
}
```

### PriorityResponse

```json
{
  "id": 2,
  "name": "Medium",
  "color": "#3498DB"
}
```

### StatusResponse

```json
{
  "id": 1,
  "name": "Pending",
  "color": "#95A5A6"
}
```

### PaginatedResponse\<T\>

```json
{
  "data": [ ... ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 42,
  "totalPages": 3
}
```
