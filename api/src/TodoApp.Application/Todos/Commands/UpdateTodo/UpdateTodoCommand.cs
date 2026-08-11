using MediatR;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Todos.Commands.UpdateTodo;

public class UpdateTodoCommand : IRequest<TodoItemResponse>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int PriorityId { get; set; }
    public Guid? CategoryId { get; set; }
    public int StatusId { get; set; }
}
