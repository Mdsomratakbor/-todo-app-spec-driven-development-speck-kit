namespace TodoApp.Application.Todos.Dtos;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int TodoCount { get; set; }
}
