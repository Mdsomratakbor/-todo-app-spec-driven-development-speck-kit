using System.Diagnostics;
using FluentResponse.Extensions;
using Serilog;

namespace TodoApp.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var correlationId = context.GetCorrelationId();

        Log.Information("HTTP {Method} {Path} started. CorrelationId: {CorrelationId}",
            context.Request.Method, context.Request.Path, correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            Log.Information("HTTP {Method} {Path} completed {StatusCode} in {Duration}ms. CorrelationId: {CorrelationId}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds, correlationId);
        }
    }
}
