using TodoApp.Application.LunchPreferences.Commands.CreateOrUpdateLunchPreference;

namespace TodoApp.UnitTests.Application.LunchPreferences;

public class CreateOrUpdateLunchPreferenceCommandValidatorTests
{
    private readonly CreateOrUpdateLunchPreferenceCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            DietaryRestrictions = ["Vegetarian"],
            LunchStartTime = "12:00",
            LunchEndTime = "13:00",
            BreakDurationMinutes = 60,
            FavoriteMeals = ["Pasta"],
            ExcludedItems = ["Peanuts"]
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithNullFields_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    // Time range validation (T025)

    [Fact]
    public void Validate_WithEndTimeBeforeStartTime_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            LunchStartTime = "14:00",
            LunchEndTime = "13:00"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LunchEndTime");
    }

    [Fact]
    public void Validate_WithEndTimeEqualToStartTime_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            LunchStartTime = "12:00",
            LunchEndTime = "12:00"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LunchEndTime");
    }

    [Fact]
    public void Validate_WithValidTimeRange_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            LunchStartTime = "12:00",
            LunchEndTime = "13:00"
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidStartTimeFormat_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            LunchStartTime = "25:00"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LunchStartTime");
    }

    [Fact]
    public void Validate_WithInvalidEndTimeFormat_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            LunchEndTime = "abc"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LunchEndTime");
    }

    // Break duration validation (T026)

    [Fact]
    public void Validate_WithBreakDurationBelow15_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            BreakDurationMinutes = 10
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BreakDurationMinutes");
    }

    [Fact]
    public void Validate_WithBreakDurationAbove120_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            BreakDurationMinutes = 150
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BreakDurationMinutes");
    }

    [Fact]
    public void Validate_WithBreakDurationAtBoundary15_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            BreakDurationMinutes = 15
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithBreakDurationAtBoundary120_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            BreakDurationMinutes = 120
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    // Dietary restrictions validation (T035)

    [Fact]
    public void Validate_WithValidDietaryRestriction_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            DietaryRestrictions = ["Vegetarian", "Gluten-Free", "Halal"]
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidDietaryRestriction_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            DietaryRestrictions = ["invalid-restriction"]
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("not a valid dietary restriction"));
    }

    [Fact]
    public void Validate_WithDietaryRestrictionsExceedingMax_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            DietaryRestrictions = ["Vegetarian", "Vegan", "Gluten-Free", "Dairy-Free",
                "Halal", "Kosher", "Nut-Free", "Low-Carb", "Diabetic", "None", "Extra"]
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DietaryRestrictions");
    }

    [Fact]
    public void Validate_WithAllValidDietaryRestrictions_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            DietaryRestrictions = ["None", "Vegetarian", "Vegan", "Gluten-Free", "Dairy-Free",
                "Halal", "Kosher", "Nut-Free", "Low-Carb", "Diabetic"]
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    // Favorites list validation (T031)

    [Fact]
    public void Validate_WithFavoriteMealsExceedingMax_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            FavoriteMeals = Enumerable.Range(1, 21).Select(i => $"Meal {i}").ToList()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FavoriteMeals");
    }

    [Fact]
    public void Validate_WithFavoriteMealsAtMax_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            FavoriteMeals = Enumerable.Range(1, 20).Select(i => $"Meal {i}").ToList()
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyFavoriteMeal_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            FavoriteMeals = ["Pasta", ""]
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Favorite meal"));
    }

    [Fact]
    public void Validate_WithWhitespaceFavoriteMeal_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            FavoriteMeals = ["   "]
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Favorite meal"));
    }

    // Exclusions list validation (T032)

    [Fact]
    public void Validate_WithExcludedItemsExceedingMax_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            ExcludedItems = Enumerable.Range(1, 21).Select(i => $"Item {i}").ToList()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ExcludedItems");
    }

    [Fact]
    public void Validate_WithExcludedItemsAtMax_ShouldPass()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            ExcludedItems = Enumerable.Range(1, 20).Select(i => $"Item {i}").ToList()
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyExcludedItem_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            ExcludedItems = ["Peanuts", ""]
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Excluded items"));
    }

    [Fact]
    public void Validate_WithWhitespaceExcludedItem_ShouldFail()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            ExcludedItems = ["   "]
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Excluded items"));
    }
}
