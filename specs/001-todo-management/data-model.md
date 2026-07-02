# Data Model: Todo Management System

**Created**: 2026-06-30
**Source**: [spec.md](spec.md) — Key Entities, Business Rules, Contract

---

## Entity: TodoItem

| Field | Type | Required | Default | Constraints |
|-------|------|----------|---------|-------------|
| Id | Guid | Yes | New Guid | Primary key, generated server-side |
| Title | string | Yes | — | 1–200 characters, trimmed, no leading/trailing whitespace |
| Description | string | No | null | 0–2000 characters |
| DueDate | DateTime | No | null | ISO 8601 UTC; max 5 years in future |
| PriorityId | int | Yes | 2 (Medium) | FK → Priority.Id (1–4) |
| CategoryId | Guid | No | null (uncategorized) | FK → Category.Id; nullable |
| StatusId | int | Yes | 1 (Pending) | FK → Status.Id (1–3); must follow valid transitions |
| CreatedAt | DateTime | Yes | UTC now | ISO 8601 UTC; set on create |
| UpdatedAt | DateTime | No | null | ISO 8601 UTC; set on update |
| DeletedAt | DateTime | No | null | ISO 8601 UTC; set on soft-delete |

**Relationships**:
- TodoItem → Category (optional Many-to-One): A category can have many todo items; a todo item optionally belongs to one category.
- TodoItem → Priority (Many-to-One via PriorityId enum)
- TodoItem → Status (Many-to-One via StatusId enum)

**Indexes**:
- IX_TodoItems_Title (for search)
- IX_TodoItems_DueDate (for date range filtering)
- IX_TodoItems_StatusId (for status filtering)
- IX_TodoItems_PriorityId (for priority filtering)
- IX_TodoItems_CategoryId (for category filtering)
- IX_TodoItems_CreatedAt (for default sort order)

**State Machine** (StatusId):
```
Pending (1) ──→ In Progress (2) ──→ Completed (3)
     ↑                                  │
     └────────── (no reverse) ──────────┘
```
- Forward transitions only: Pending → In Progress → Completed
- Completed → anything: not allowed (returns 422)
- All others: not allowed (returns 422)

---

## Entity: Category

| Field | Type | Required | Default | Constraints |
|-------|------|----------|---------|-------------|
| Id | Guid | Yes | New Guid | Primary key, generated server-side |
| Name | string | Yes | — | 1–100 characters, unique, trimmed |
| Color | string | No | null | Hex code `#RRGGBB`; null = system default |
| CreatedAt | DateTime | Yes | UTC now | ISO 8601 UTC |

**Relationships**:
- Category → TodoItem (optional One-to-Many): A category can have many todo items.

**Uniqueness**: Name is unique across all categories (case-insensitive).

**Special Instance**: A default "Uncategorized" category is created during database seeding. It cannot be deleted. Its Id is a well-known constant.

---

## Enum: Priority

| Id | Name | Display Color | Description |
|----|------|---------------|-------------|
| 1 | Low | #2ECC71 (green) | Non-urgent tasks |
| 2 | Medium | #3498DB (blue) | Normal tasks |
| 3 | High | #F39C12 (amber) | Important tasks |
| 4 | Critical | #E74C3C (red) | Urgent tasks |

---

## Enum: Status

| Id | Name | Display Color | Description |
|----|------|---------------|-------------|
| 1 | Pending | #95A5A6 (grey) | Not yet started |
| 2 | In Progress | #3498DB (blue) | Currently being worked on |
| 3 | Completed | #2ECC71 (green) | Finished |

---

## Soft-Delete Behavior

- `DELETE /api/v1/todos/{id}` sets `DeletedAt` to UTC now (soft-delete).
- Soft-deleted todos are excluded from all queries.
- A scheduled cleanup job hard-deletes todos where `DeletedAt < UTC now - 30 days`.
- Soft-deleted todos cannot be restored via API (no undelete endpoint in v1).

---

## Validation Summary

| Entity | Field | Rules |
|--------|-------|-------|
| TodoItem | Title | Required, 1–200 chars, trimmed |
| TodoItem | Description | Optional, 0–2000 chars |
| TodoItem | DueDate | Optional, max 5 years from now |
| TodoItem | PriorityId | Required, must be 1–4 |
| TodoItem | CategoryId | Optional, must reference existing Category |
| TodoItem | StatusId | Required; create = 1 only; update = valid forward transition only |
| Category | Name | Required, 1–100 chars, unique, trimmed |
| Category | Color | Optional, must match `^#[0-9A-Fa-f]{6}$` |
