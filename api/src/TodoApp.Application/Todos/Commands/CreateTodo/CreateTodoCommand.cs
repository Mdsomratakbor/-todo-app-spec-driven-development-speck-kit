using MediatR;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Todos.Commands.CreateTodo;

public class CreateTodoCommand : IRequest<TodoItemResponse>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int PriorityId { get; set; }
    public Guid? CategoryId { get; set; }
}
