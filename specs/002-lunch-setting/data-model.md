# Data Model: Lunch Preference

## Entity: LunchPreference

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| Id | Guid | No | Primary key | Generated server-side |
| UserId | Guid | No | Foreign key to user identity | Unique index; from JWT |
| DietaryRestrictions | string[] | Yes | List of dietary restrictions | Each value from seed list; max 10 items |
| LunchStartTime | TimeOnly | Yes | Preferred lunch start time | 24h format; must be before LunchEndTime |
| LunchEndTime | TimeOnly | Yes | Preferred lunch end time | 24h format; must be after LunchStartTime |
| BreakDurationMinutes | int | Yes | Break duration in minutes | 15-120 inclusive |
| NotificationsEnabled | bool | No | Reminder notification opt-in | Default: true |
| FavoriteMeals | string[] | Yes | Favorite meal choices | Max 20 items; owned collection |
| ExcludedItems | string[] | Yes | Excluded food items | Max 20 items; owned collection |
| CreatedAt | DateTime | No | Creation timestamp | UTC; set on first creation |
| UpdatedAt | DateTime | Yes | Last update timestamp | UTC; set on every update |

## Value Objects

### TimeRange

| Field | Type | Description |
|-------|------|-------------|
| Start | TimeOnly | Lunch window start |
| End | TimeOnly | Lunch window end |

Validation: End > Start.

### MealList

| Field | Type | Description |
|-------|------|-------------|
| Items | string[] | List of meal names or excluded items |

Validation: Max 20 items; each item non-empty, trimmed.

## Relationships

```
User (external) ──1:1── LunchPreference
                                ├── FavoriteMeals (owned collection)
                                ├── ExcludedItems (owned collection)
                                └── DietaryRestrictions (owned collection)
```

## Database Design

### Table: LunchPreferences

```sql
CREATE TABLE "LunchPreferences" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL,
    "DietaryRestrictions" TEXT[] NULL,
    "LunchStartTime" TIME NULL,
    "LunchEndTime" TIME NULL,
    "BreakDurationMinutes" INTEGER NULL,
    "NotificationsEnabled" BOOLEAN NOT NULL DEFAULT TRUE,
    "FavoriteMeals" TEXT[] NULL,
    "ExcludedItems" TEXT[] NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMPTZ NULL,
    CONSTRAINT "CK_LunchPreferences_BreakDuration" CHECK (
        "BreakDurationMinutes" IS NULL OR
        ("BreakDurationMinutes" >= 15 AND "BreakDurationMinutes" <= 120)
    )
);

CREATE UNIQUE INDEX "IX_LunchPreferences_UserId" ON "LunchPreferences" ("UserId");
```

### Seed Data

Dietary restrictions seed values:
- None
- Vegetarian
- Vegan
- Gluten-Free
- Dairy-Free
- Halal
- Kosher
- Nut-Free
- Low-Carb
- Diabetic

Organization default preferences:
- LunchStartTime: 12:00
- LunchEndTime: 13:00
- BreakDurationMinutes: 60
- NotificationsEnabled: true
- No dietary restrictions, favorites, or exclusions

### Entity Configuration (EF Core)

```csharp
public class LunchPreferenceConfiguration : IEntityTypeConfiguration<LunchPreference>
{
    public void Configure(EntityTypeBuilder<LunchPreference> builder)
    {
        builder.ToTable("LunchPreferences");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.DietaryRestrictions)
            .HasColumnType("text[]")
            .IsRequired(false);
        builder.Property(x => x.LunchStartTime).IsRequired(false);
        builder.Property(x => x.LunchEndTime).IsRequired(false);
        builder.Property(x => x.BreakDurationMinutes).IsRequired(false);
        builder.Property(x => x.NotificationsEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.FavoriteMeals)
            .HasColumnType("text[]")
            .IsRequired(false);
        builder.Property(x => x.ExcludedItems)
            .HasColumnType("text[]")
            .IsRequired(false);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired(false);
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}
```

## Validation Rules

| Validation | Entity | Rule | Enforcement Layer |
|------------|--------|------|-------------------|
| UserId unique | LunchPreference | One profile per user | DB unique index + Application |
| Dietary restrictions valid | LunchPreference | Must be from seed list | Application (FluentValidation) |
| Time range valid | LunchPreference | Start < End | Application (FluentValidation) |
| Break duration range | LunchPreference | 15-120 minutes | Application (FluentValidation) + DB check constraint |
| Favorites size | LunchPreference | <= 20 items | Application (FluentValidation) |
| Exclusions size | LunchPreference | <= 20 items | Application (FluentValidation) |
