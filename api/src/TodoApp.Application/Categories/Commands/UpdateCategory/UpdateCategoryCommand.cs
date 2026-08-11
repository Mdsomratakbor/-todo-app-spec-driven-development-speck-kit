using MediatR;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<CategoryResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}
