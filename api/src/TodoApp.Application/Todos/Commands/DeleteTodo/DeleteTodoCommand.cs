using MediatR;

namespace TodoApp.Application.Todos.Commands.DeleteTodo;

public class DeleteTodoCommand : IRequest
{
    public Guid Id { get; set; }
}
