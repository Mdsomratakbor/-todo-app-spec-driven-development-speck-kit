using TodoApp.Application.Todos.Commands.CreateTodo;

namespace TodoApp.UnitTests.Application.Todos;

public class CreateTodoCommandValidatorTests
{
    private readonly CreateTodoCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        var command = new CreateTodoCommand
        {
            Title = "Valid Todo",
            PriorityId = 2
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldFail()
    {
        var command = new CreateTodoCommand
        {
            Title = "",
            PriorityId = 2
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WithTitleExceedingMaxLength_ShouldFail()
    {
        var command = new CreateTodoCommand
        {
            Title = new string('x', 201),
            PriorityId = 2
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_WithInvalidPriorityId_ShouldFail()
    {
        var command = new CreateTodoCommand
        {
            Title = "Valid Title",
            PriorityId = 99
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PriorityId");
    }

    [Fact]
    public void Validate_WithDueDateBeyond5Years_ShouldFail()
    {
        var command = new CreateTodoCommand
        {
            Title = "Valid Title",
            PriorityId = 2,
            DueDate = DateTime.UtcNow.AddYears(5).AddDays(1)
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DueDate");
    }

    [Fact]
    public void Validate_WithDueDateWithin5Years_ShouldPass()
    {
        var command = new CreateTodoCommand
        {
            Title = "Valid Title",
            PriorityId = 2,
            DueDate = DateTime.UtcNow.AddYears(4).AddMonths(11)
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
