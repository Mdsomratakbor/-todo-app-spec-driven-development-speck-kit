using Cartographer.Core.Abstractions;
using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Todos.Queries.GetTodoById;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoItemResponse>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public GetTodoByIdQueryHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoItemResponse> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Todo item with Id {request.Id} not found.");

        return _mapper.Map<TodoItemResponse>(item);
    }
}
