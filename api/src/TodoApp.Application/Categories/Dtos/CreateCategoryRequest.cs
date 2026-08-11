namespace TodoApp.Application.Categories.Dtos;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}
