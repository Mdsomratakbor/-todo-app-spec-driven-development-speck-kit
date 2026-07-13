using MediatR;
using TodoApp.Application.LunchPreferences.Dtos;

namespace TodoApp.Application.LunchPreferences.Commands.CreateOrUpdateLunchPreference;

public class CreateOrUpdateLunchPreferenceCommand : IRequest<LunchPreferenceResponse>
{
    public Guid UserId { get; set; }
    public List<string>? DietaryRestrictions { get; set; }
    public string? LunchStartTime { get; set; }
    public string? LunchEndTime { get; set; }
    public int? BreakDurationMinutes { get; set; }
    public List<string>? FavoriteMeals { get; set; }
    public List<string>? ExcludedItems { get; set; }
    public bool? NotificationsEnabled { get; set; }
}
