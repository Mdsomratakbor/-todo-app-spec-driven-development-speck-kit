using MediatR;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Todos.Queries.GetTodoById;

public class GetTodoByIdQuery : IRequest<TodoItemResponse>
{
    public Guid Id { get; set; }
}
