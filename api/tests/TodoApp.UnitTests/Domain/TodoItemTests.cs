using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Domain;

public class TodoItemTests
{
    [Fact]
    public void SetProperties_ShouldAssignCorrectly()
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Buy groceries",
            Description = "Milk, eggs, bread",
            DueDate = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            PriorityId = 2,
            CategoryId = Guid.NewGuid(),
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("Buy groceries", item.Title);
        Assert.Equal("Milk, eggs, bread", item.Description);
        Assert.NotNull(item.DueDate);
        Assert.Equal(2, item.PriorityId);
        Assert.Equal(1, item.StatusId);
        Assert.NotNull(item.CategoryId);
        Assert.Null(item.DeletedAt);
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedAt()
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            PriorityId = 1,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        Assert.Null(item.DeletedAt);
        item.DeletedAt = DateTime.UtcNow;
        Assert.NotNull(item.DeletedAt);
    }

    [Fact]
    public void NavigationProperties_ShouldBeNullByDefault()
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            PriorityId = 1,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        Assert.Null(item.Category);
    }
}
