using TodoApp.Application.Todos.Commands.UpdateTodo;

namespace TodoApp.UnitTests.Application.Todos;

public class UpdateTodoCommandValidatorTests
{
    private readonly UpdateTodoCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        var command = new UpdateTodoCommand
        {
            Id = Guid.NewGuid(),
            Title = "Valid Title",
            PriorityId = 2,
            StatusId = 1
        };
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldFail()
    {
        var command = new UpdateTodoCommand
        {
            Id = Guid.NewGuid(),
            Title = "",
            PriorityId = 2,
            StatusId = 1
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithTitleExceedingMaxLength_ShouldFail()
    {
        var command = new UpdateTodoCommand
        {
            Id = Guid.NewGuid(),
            Title = new string('a', 201),
            PriorityId = 2,
            StatusId = 1
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPriorityId_ShouldFail()
    {
        var command = new UpdateTodoCommand
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            PriorityId = 5,
            StatusId = 1
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidStatusId_ShouldFail()
    {
        var command = new UpdateTodoCommand
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            PriorityId = 2,
            StatusId = 4
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
    }
}
