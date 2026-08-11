using TodoApp.Application.Categories.Commands.UpdateCategory;

namespace TodoApp.UnitTests.Application.Categories;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = "Work", Color = "#3498DB" };
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithValidCommandNoColor_ShouldPass()
    {
        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = "Work" };
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldFail()
    {
        var command = new UpdateCategoryCommand { Id = Guid.Empty, Name = "Work" };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = "" };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithNameExceedingMaxLength_ShouldFail()
    {
        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = new string('a', 101) };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidColorFormat_ShouldFail()
    {
        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = "Work", Color = "invalid" };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }
}
