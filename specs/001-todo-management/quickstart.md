# Quickstart: Todo Management System

**Created**: 2026-06-30
**Source**: [spec.md](spec.md), [data-model.md](data-model.md), [contracts/api-contracts.md](contracts/api-contracts.md)

---

## Prerequisites

- .NET 10 SDK
- Node.js 22+
- Angular CLI 21 (`npm install -g @angular/cli`)
- PostgreSQL 16+ (local or container)
- Docker (for TestContainers in integration tests)

---

## Setup

### 1. Clone and restore

```bash
git clone <repo-url>
cd <repo-root>
```

### 2. Database

```bash
# Start PostgreSQL (Docker)
docker run -d --name todoapp-postgres \
  -e POSTGRES_USER=todoapp \
  -e POSTGRES_PASSWORD=todoapp \
  -e POSTGRES_DB=todoapp \
  -p 5432:5432 \
  postgres:16

# Apply EF Core migrations
cd api/src/TodoApp.Api
dotnet ef database update
```

### 3. Backend

```bash
cd api/src/TodoApp.Api
dotnet run
# API available at https://localhost:5001
# Scalar docs at https://localhost:5001/scalar/v1
```

### 4. Frontend

```bash
cd client
npm install
ng serve
# App available at http://localhost:4200
```

---

## Validation Scenarios

### Scenario 1: Create a todo

**Steps**:
1. Open the app at http://localhost:4200
2. Click "Add Todo"
3. Fill in: Title = "Buy groceries", Priority = Medium, Category = Personal
4. Click "Save"

**Expected**:
- New todo appears in the list
- Success snackbar: "Todo created successfully"

### Scenario 2: Update a todo status

**Steps**:
1. Click on a todo item
2. Change status from "Pending" to "In Progress"
3. Click "Save"

**Expected**:
- Status updates in the list
- Success snackbar: "Todo updated successfully"

### Scenario 3: Filter todos by category

**Steps**:
1. Select a category from the filter dropdown
2. Observe the list

**Expected**:
- Only todos in the selected category are displayed
- If no results, empty-state illustration is shown

### Scenario 4: Validate form fields

**Steps**:
1. Click "Add Todo"
2. Leave Title empty
3. Click "Save"

**Expected**:
- Inline validation error: "Title is required"
- Form is not submitted
- Error snackbar is NOT shown (validation prevented submission)

### Scenario 5: Delete a category

**Steps**:
1. Navigate to category management
2. Click "Delete" on a category that has assigned todos
3. Confirm deletion

**Expected**:
- Category is removed
- Warning snackbar: "Category deleted. Todos reassigned to Uncategorized."
- Affected todos now show "Uncategorized"

---

## Running Tests

```bash
# Backend unit tests
cd api/tests/TodoApp.UnitTests
dotnet test

# Backend integration tests (requires Docker for TestContainers)
cd api/tests/TodoApp.IntegrationTests
dotnet test

# Frontend component tests
cd client
ng test

# E2E tests
cd client
npx playwright test
```

---

## API Verification (curl)

```bash
# Get todos (page 1, page size 20)
curl -H "Authorization: Bearer <token>" \
  http://localhost:5001/api/v1/todos?page=1&pageSize=20

# Create a todo
curl -X POST -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"title":"Test todo","priorityId":2}' \
  http://localhost:5001/api/v1/todos

# Get Scalar docs
curl http://localhost:5001/scalar/v1
```
