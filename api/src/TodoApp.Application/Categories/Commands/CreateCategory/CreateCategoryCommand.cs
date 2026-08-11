using MediatR;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<CategoryResponse>
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}
