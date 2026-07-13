using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Services;

public class TodoCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TodoCleanupService> _logger;

    public TodoCleanupService(IServiceScopeFactory scopeFactory, ILogger<TodoCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupSoftDeletedTodos(stoppingToken);

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task CleanupSoftDeletedTodos(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cutoff = DateTime.UtcNow.AddDays(-30);
            var expired = await db.TodoItems
                .IgnoreQueryFilters()
                .Where(t => t.DeletedAt != null && t.DeletedAt < cutoff)
                .ToListAsync(ct);

            if (expired.Count == 0)
            {
                _logger.LogInformation("No expired soft-deleted todos to clean up.");
                return;
            }

            db.TodoItems.RemoveRange(expired);
            await db.SaveChangesAsync(ct);

            _logger.LogInformation("Hard-deleted {Count} expired todo(s) older than {Cutoff}.", expired.Count, cutoff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during soft-delete cleanup.");
        }
    }
}
