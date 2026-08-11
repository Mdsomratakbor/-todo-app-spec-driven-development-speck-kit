using FluentValidation;

namespace TodoApp.Application.Todos.Commands.CreateTodo;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(v => v.PriorityId)
            .InclusiveBetween(1, 4).WithMessage("Priority must be between 1 and 4.");

        RuleFor(v => v.DueDate)
            .Must(d => d == null || d.Value <= DateTime.UtcNow.AddYears(5))
            .WithMessage("Due date cannot be more than 5 years in the future.");
    }
}
