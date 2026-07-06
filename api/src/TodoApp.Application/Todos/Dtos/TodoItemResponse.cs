namespace TodoApp.Application.Todos.Dtos;

public class TodoItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public PriorityResponse Priority { get; set; } = null!;
    public CategoryResponse? Category { get; set; }
    public StatusResponse Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
