using FluentValidation;

namespace TodoApp.Application.Todos.Commands.UpdateTodo;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(v => v.PriorityId)
            .InclusiveBetween(1, 4).WithMessage("Priority must be between 1 and 4.");

        RuleFor(v => v.StatusId)
            .InclusiveBetween(1, 3).WithMessage("Status must be between 1 and 3.");

        RuleFor(v => v.DueDate)
            .Must(d => d == null || d.Value <= DateTime.UtcNow.AddYears(5))
            .WithMessage("Due date cannot be more than 5 years in the future.");
    }
}
