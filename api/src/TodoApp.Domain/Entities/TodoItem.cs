namespace TodoApp.Domain.Entities;

public class TodoItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int PriorityId { get; set; }
    public Guid? CategoryId { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Priority Priority { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public Category? Category { get; set; }
}
