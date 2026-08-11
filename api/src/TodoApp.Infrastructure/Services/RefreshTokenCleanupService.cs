using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Infrastructure.Services;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshTokenCleanupService> _logger;

    public RefreshTokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<RefreshTokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupExpiredTokens(stoppingToken);

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task CleanupExpiredTokens(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            var cutoff = DateTime.UtcNow.AddDays(-30);
            var deletedCount = await repository.DeleteExpiredAsync(cutoff, ct);

            if (deletedCount > 0)
                _logger.LogInformation("Deleted {Count} expired/revoked refresh token(s) older than {Cutoff}.", deletedCount, cutoff);
            else
                _logger.LogInformation("No expired refresh tokens to clean up.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh token cleanup.");
        }
    }
}
