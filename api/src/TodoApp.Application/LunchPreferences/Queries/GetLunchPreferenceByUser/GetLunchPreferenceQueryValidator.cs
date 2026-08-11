using FluentValidation;

namespace TodoApp.Application.LunchPreferences.Queries.GetLunchPreferenceByUser;

public class GetLunchPreferenceQueryValidator : AbstractValidator<GetLunchPreferenceByUserQuery>
{
    public GetLunchPreferenceQueryValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
