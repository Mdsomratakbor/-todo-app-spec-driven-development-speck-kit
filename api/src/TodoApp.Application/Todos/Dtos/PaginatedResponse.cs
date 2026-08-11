namespace TodoApp.Application.Todos.Dtos;

public class PaginatedResponse<T>
{
    public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
