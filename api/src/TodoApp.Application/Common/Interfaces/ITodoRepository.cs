using TodoApp.Domain.Entities;

namespace TodoApp.Application.Common.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<TodoItem> Items, int TotalCount)> GetListAsync(
        int? statusId, int? priorityId, Guid? categoryId,
        DateTime? dueDateFrom, DateTime? dueDateTo, string? search,
        int page, int pageSize, CancellationToken cancellationToken = default);
    void Add(TodoItem item);
    void Update(TodoItem item);
    void Delete(TodoItem item);
}
