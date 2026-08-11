using FluentValidation;

namespace TodoApp.Application.LunchPreferences.Commands.ResetLunchPreference;

public class ResetLunchPreferenceCommandValidator : AbstractValidator<ResetLunchPreferenceCommand>
{
    public ResetLunchPreferenceCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
