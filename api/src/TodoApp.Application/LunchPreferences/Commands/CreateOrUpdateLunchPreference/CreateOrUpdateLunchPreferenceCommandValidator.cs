using FluentValidation;

namespace TodoApp.Application.LunchPreferences.Commands.CreateOrUpdateLunchPreference;

public class CreateOrUpdateLunchPreferenceCommandValidator : AbstractValidator<CreateOrUpdateLunchPreferenceCommand>
{
    private static readonly HashSet<string> ValidRestrictions =
    [
        "None", "Vegetarian", "Vegan", "Gluten-Free", "Dairy-Free",
        "Halal", "Kosher", "Nut-Free", "Low-Carb", "Diabetic"
    ];

    public CreateOrUpdateLunchPreferenceCommandValidator()
    {
        RuleFor(x => x.BreakDurationMinutes)
            .GreaterThanOrEqualTo(15).When(x => x.BreakDurationMinutes is not null)
            .WithMessage("Break duration must be at least 15 minutes.")
            .LessThanOrEqualTo(120).When(x => x.BreakDurationMinutes is not null)
            .WithMessage("Break duration must not exceed 120 minutes.");

        When(x => x.LunchStartTime is not null, () =>
        {
            RuleFor(x => x.LunchStartTime)
                .Must(v => TimeOnly.TryParse(v, out _))
                .WithMessage("Lunch start time must be a valid time (HH:mm).");
        });

        When(x => x.LunchEndTime is not null, () =>
        {
            RuleFor(x => x.LunchEndTime)
                .Must(v => TimeOnly.TryParse(v, out _))
                .WithMessage("Lunch end time must be a valid time (HH:mm).");
        });

        When(x => x.LunchStartTime is not null && x.LunchEndTime is not null, () =>
        {
            RuleFor(x => x.LunchEndTime)
                .Must((cmd, endTime) =>
                {
                    if (!TimeOnly.TryParse(cmd.LunchStartTime, out var start) ||
                        !TimeOnly.TryParse(endTime, out var end))
                        return true;
                    return end > start;
                })
                .WithMessage("Lunch end time must be after lunch start time.");
        });

        When(x => x.DietaryRestrictions is not null, () =>
        {
            RuleFor(x => x.DietaryRestrictions)
                .Must(list => list.Count <= 10)
                .WithMessage("Dietary restrictions must not exceed 10 items.");

            RuleForEach(x => x.DietaryRestrictions)
                .Must(v => ValidRestrictions.Contains(v))
                .WithMessage("'{PropertyValue}' is not a valid dietary restriction.");
        });

        When(x => x.FavoriteMeals is not null, () =>
        {
            RuleFor(x => x.FavoriteMeals)
                .Must(list => list.Count <= 20)
                .WithMessage("Favorite meals must not exceed 20 items.");

            RuleForEach(x => x.FavoriteMeals)
                .NotEmpty().WithMessage("Favorite meal items must not be empty.")
                .Must(v => v.Trim().Length > 0).WithMessage("Favorite meal items must not be whitespace.");
        });

        When(x => x.ExcludedItems is not null, () =>
        {
            RuleFor(x => x.ExcludedItems)
                .Must(list => list.Count <= 20)
                .WithMessage("Excluded items must not exceed 20 items.");

            RuleForEach(x => x.ExcludedItems)
                .NotEmpty().WithMessage("Excluded items must not be empty.")
                .Must(v => v.Trim().Length > 0).WithMessage("Excluded items must not be whitespace.");
        });
    }
}
