using MediatR;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Todos.Queries.GetTodoList;

public class GetTodoListQuery : IRequest<PaginatedResponse<TodoItemResponse>>
{
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
