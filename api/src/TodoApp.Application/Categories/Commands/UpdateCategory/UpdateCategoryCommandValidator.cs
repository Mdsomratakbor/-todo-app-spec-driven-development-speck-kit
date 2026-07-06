using FluentValidation;

namespace TodoApp.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Must(name => !string.IsNullOrWhiteSpace(name));

        When(v => !string.IsNullOrEmpty(v.Color), () =>
        {
            RuleFor(v => v.Color)
                .Matches("^#[0-9A-Fa-f]{6}$")
                .WithMessage("Color must be a valid hex color code (e.g., #FF5733).");
        });
    }
}
