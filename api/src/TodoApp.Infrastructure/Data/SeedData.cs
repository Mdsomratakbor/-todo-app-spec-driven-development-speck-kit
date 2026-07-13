using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Data;

public static class SeedData
{
    public static readonly string[] DietaryRestrictionValues =
    [
        "None", "Vegetarian", "Vegan", "Gluten-Free", "Dairy-Free",
        "Halal", "Kosher", "Nut-Free", "Low-Carb", "Diabetic"
    ];

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedPriorities(modelBuilder);
        SeedStatuses(modelBuilder);
        SeedDefaultCategory(modelBuilder);
        SeedDietaryRestrictions(modelBuilder);
    }

    private static void SeedDietaryRestrictions(ModelBuilder modelBuilder)
    {
        // Dietary restrictions are validated against this list at the application layer.
        // No database seed needed for enum-like validation values.
    }

    private static void SeedPriorities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Priority>().HasData(
            new Priority { Id = 1, Name = "Low", Color = "#8BC34A" },
            new Priority { Id = 2, Name = "Medium", Color = "#FFC107" },
            new Priority { Id = 3, Name = "High", Color = "#FF9800" },
            new Priority { Id = 4, Name = "Urgent", Color = "#F44336" }
        );
    }

    private static void SeedStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Name = "Pending", Color = "#9E9E9E" },
            new Status { Id = 2, Name = "In Progress", Color = "#2196F3" },
            new Status { Id = 3, Name = "Completed", Color = "#4CAF50" }
        );
    }

    private static void SeedDefaultCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "Uncategorized",
                Color = "#9E9E9E",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
