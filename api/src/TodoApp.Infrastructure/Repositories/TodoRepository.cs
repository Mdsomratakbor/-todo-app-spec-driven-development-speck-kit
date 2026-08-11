using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;

    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<TodoItem> Items, int TotalCount)> GetListAsync(
        int? statusId, int? priorityId, Guid? categoryId,
        DateTime? dueDateFrom, DateTime? dueDateTo, string? search,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.TodoItems
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Category)
            .AsQueryable();

        if (statusId.HasValue)
            query = query.Where(t => t.StatusId == statusId.Value);

        if (priorityId.HasValue)
            query = query.Where(t => t.PriorityId == priorityId.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (dueDateFrom.HasValue)
            query = query.Where(t => t.DueDate >= dueDateFrom.Value);

        if (dueDateTo.HasValue)
            query = query.Where(t => t.DueDate <= dueDateTo.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t =>
                EF.Functions.ILike(t.Title, $"%{search}%") ||
                EF.Functions.ILike(t.Description ?? "", $"%{search}%"));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(TodoItem item)
    {
        _context.TodoItems.Add(item);
    }

    public void Update(TodoItem item)
    {
        _context.TodoItems.Update(item);
    }

    public void Delete(TodoItem item)
    {
        _context.TodoItems.Update(item);
    }
}
