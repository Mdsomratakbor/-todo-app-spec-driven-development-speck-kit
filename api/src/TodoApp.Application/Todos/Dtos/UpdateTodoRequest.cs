namespace TodoApp.Application.Todos.Dtos;

public class UpdateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int PriorityId { get; set; }
    public Guid? CategoryId { get; set; }
    public int StatusId { get; set; }
}
