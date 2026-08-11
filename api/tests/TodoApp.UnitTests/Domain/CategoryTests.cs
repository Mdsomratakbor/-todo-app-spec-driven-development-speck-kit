using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Domain;

public class CategoryTests
{
    [Fact]
    public void SetProperties_ShouldAssignCorrectly()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Work",
            Color = "#3498DB",
            CreatedAt = DateTime.UtcNow
        };

        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal("Work", category.Name);
        Assert.Equal("#3498DB", category.Color);
    }

    [Fact]
    public void DefaultColor_ShouldBeNull()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Personal",
            CreatedAt = DateTime.UtcNow
        };

        Assert.Null(category.Color);
    }

    [Fact]
    public void TodoItemsCollection_ShouldBeInitialized()
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            CreatedAt = DateTime.UtcNow
        };

        Assert.NotNull(category.TodoItems);
        Assert.Empty(category.TodoItems);
    }
}
