using MediatR;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Todos.Commands.DeleteTodo;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand>
{
    private readonly ITodoRepository _repository;

    public DeleteTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Todo item with Id {request.Id} not found.");

        item.DeletedAt = DateTime.UtcNow;
        _repository.Delete(item);
    }
}
