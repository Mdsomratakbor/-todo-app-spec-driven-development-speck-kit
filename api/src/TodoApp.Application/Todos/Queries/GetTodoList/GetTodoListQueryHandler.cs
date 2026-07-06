using Cartographer.Core.Abstractions;
using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Todos.Queries.GetTodoList;

public class GetTodoListQueryHandler : IRequestHandler<GetTodoListQuery, PaginatedResponse<TodoItemResponse>>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public GetTodoListQueryHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<TodoItemResponse>> Handle(GetTodoListQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Min(request.PageSize, 50);
        var page = Math.Max(request.Page, 1);

        var (items, totalCount) = await _repository.GetListAsync(
            request.StatusId, request.PriorityId, request.CategoryId,
            request.DueDateFrom, request.DueDateTo, request.Search,
            page, pageSize, cancellationToken);

        return new PaginatedResponse<TodoItemResponse>
        {
            Data = _mapper.Map<IReadOnlyList<TodoItemResponse>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
