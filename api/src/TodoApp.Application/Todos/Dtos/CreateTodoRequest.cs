namespace TodoApp.Application.Todos.Dtos;

public class CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int PriorityId { get; set; }
    public Guid? CategoryId { get; set; }
}
