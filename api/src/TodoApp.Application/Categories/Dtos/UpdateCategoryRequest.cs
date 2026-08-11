namespace TodoApp.Application.Categories.Dtos;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}
