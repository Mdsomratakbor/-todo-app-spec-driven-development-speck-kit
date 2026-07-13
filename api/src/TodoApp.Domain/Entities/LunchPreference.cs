namespace TodoApp.Domain.Entities;

public class LunchPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<string> DietaryRestrictions { get; set; } = [];
    public TimeOnly? LunchStartTime { get; set; }
    public TimeOnly? LunchEndTime { get; set; }
    public int? BreakDurationMinutes { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public List<string> FavoriteMeals { get; set; } = [];
    public List<string> ExcludedItems { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
