using Cartographer.Core.Abstractions;
using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Todos.Commands.CreateTodo;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoItemResponse>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public CreateTodoCommandHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoItemResponse> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<TodoItem>(request);
        item.Id = Guid.NewGuid();
        item.StatusId = 1;
        item.CreatedAt = DateTime.UtcNow;

        _repository.Add(item);

        var fullItem = await _repository.GetByIdAsync(item.Id, cancellationToken);
        return _mapper.Map<TodoItemResponse>(fullItem!);
    }
}
