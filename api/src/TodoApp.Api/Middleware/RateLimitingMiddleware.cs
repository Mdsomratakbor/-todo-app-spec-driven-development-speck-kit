using System.Collections.Concurrent;
using System.Text.Json;
using Serilog;

namespace TodoApp.Api.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    private static readonly ConcurrentDictionary<string, RateLimitEntry> _clients = new();

    public RateLimitingMiddleware(RequestDelegate next, int maxRequests = 100, int windowSeconds = 60)
    {
        _next = next;
        _maxRequests = maxRequests;
        _window = TimeSpan.FromSeconds(windowSeconds);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (IsRateLimited(clientIp, out var remaining))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/problem+json";
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["Retry-After"] = _window.TotalSeconds.ToString();

            Log.Warning("Rate limit exceeded for {ClientIp} on {Endpoint}", clientIp, context.Request.Path);

            var problem = new
            {
                type = "https://tools.ietf.org/html/rfc6585#section-4",
                title = "Too Many Requests",
                status = StatusCodes.Status429TooManyRequests,
                detail = $"Rate limit of {_maxRequests} requests per {_window.TotalSeconds} seconds exceeded."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            return;
        }

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private bool IsRateLimited(string clientIp, out int remaining)
    {
        var entry = _clients.GetOrAdd(clientIp, _ => new RateLimitEntry(_maxRequests, _window));
        return entry.TryAcquire(out remaining);
    }

    private class RateLimitEntry
    {
        private readonly int _maxRequests;
        private readonly TimeSpan _window;
        private readonly ConcurrentQueue<DateTime> _timestamps = new();

        public RateLimitEntry(int maxRequests, TimeSpan window)
        {
            _maxRequests = maxRequests;
            _window = window;
        }

        public bool TryAcquire(out int remaining)
        {
            var now = DateTime.UtcNow;
            while (_timestamps.TryPeek(out var ts) && now - ts > _window)
            {
                _timestamps.TryDequeue(out _);
            }

            remaining = _maxRequests - _timestamps.Count;

            if (_timestamps.Count >= _maxRequests)
            {
                return true;
            }

            _timestamps.Enqueue(now);
            return false;
        }
    }
}
