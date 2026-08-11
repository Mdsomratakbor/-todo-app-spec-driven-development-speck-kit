using Microsoft.AspNetCore.Http;
using TodoApp.Api.Middleware;

namespace TodoApp.IntegrationTests.Middleware;

public class RateLimitingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_FirstRequest_ShouldPassThrough()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
        var middleware = new RateLimitingMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_UnderLimit_ShouldPassThrough()
    {
        var ip = "192.168.1.2";
        var middleware = new RateLimitingMiddleware(_ => Task.CompletedTask);

        for (var i = 0; i < 50; i++)
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip);
            await middleware.InvokeAsync(context);
            Assert.Equal(200, context.Response.StatusCode);
        }
    }

    [Fact]
    public async Task InvokeAsync_ExceedsLimit_ShouldReturn429()
    {
        var ip = "192.168.1.3";
        var middleware = new RateLimitingMiddleware(_ => Task.CompletedTask, maxRequests: 5, windowSeconds: 60);

        for (var i = 0; i < 5; i++)
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip);
            await middleware.InvokeAsync(context);
            Assert.Equal(200, context.Response.StatusCode);
        }

        var blockedContext = new DefaultHttpContext();
        blockedContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip);
        await middleware.InvokeAsync(blockedContext);

        Assert.Equal(StatusCodes.Status429TooManyRequests, blockedContext.Response.StatusCode);
        Assert.Equal("0", blockedContext.Response.Headers["X-RateLimit-Remaining"]);
        Assert.True(blockedContext.Response.Headers.ContainsKey("Retry-After"));
    }

    [Fact]
    public async Task InvokeAsync_DifferentClients_ShouldBeIndependent()
    {
        var middleware = new RateLimitingMiddleware(_ => Task.CompletedTask, maxRequests: 3, windowSeconds: 60);

        for (var i = 0; i < 3; i++)
        {
            var ctx = new DefaultHttpContext();
            ctx.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.1");
            await middleware.InvokeAsync(ctx);
            Assert.Equal(200, ctx.Response.StatusCode);
        }

        var client2 = new DefaultHttpContext();
        client2.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.2");
        await middleware.InvokeAsync(client2);
        Assert.Equal(200, client2.Response.StatusCode);

        var blocked = new DefaultHttpContext();
        blocked.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.1");
        await middleware.InvokeAsync(blocked);
        Assert.Equal(StatusCodes.Status429TooManyRequests, blocked.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_UnknownIp_UsesUnknownKey()
    {
        var context = new DefaultHttpContext();
        var middleware = new RateLimitingMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.Equal(200, context.Response.StatusCode);
    }
}
