using FluentResponse.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TodoApp.Api.Middleware;
using TodoApp.Application;
using TodoApp.Infrastructure.Data;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddApplication();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddFluentResponse(options =>
    {
        options.IncludeTimestamp = true;
        options.IncludeTraceId = true;
        options.EnableExecutionTimeTracking = true;
    });

    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseFluentResponseExceptionHandler();
    app.UseFluentResponseCorrelationId();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
