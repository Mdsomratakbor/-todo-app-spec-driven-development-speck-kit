using Cartographer.Core.Abstractions;
using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Todos.Commands.UpdateTodo;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, TodoItemResponse>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public UpdateTodoCommandHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoItemResponse> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Todo item with Id {request.Id} not found.");

        ValidateStatusTransition(item.StatusId, request.StatusId);

        _mapper.Map(request, item);
        item.UpdatedAt = DateTime.UtcNow;

        _repository.Update(item);

        var updated = await _repository.GetByIdAsync(item.Id, cancellationToken);
        return _mapper.Map<TodoItemResponse>(updated!);
    }

    private static void ValidateStatusTransition(int currentStatusId, int newStatusId)
    {
        if (currentStatusId == 3 && newStatusId != 3)
        {
            throw new InvalidOperationException("The requested status transition is not allowed.");
        }

        if (currentStatusId == 2 && newStatusId == 1)
        {
            throw new InvalidOperationException("The requested status transition is not allowed.");
        }
    }
}
