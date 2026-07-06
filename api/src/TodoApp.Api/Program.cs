using FluentResponse.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using TodoApp.Api.Middleware;
using TodoApp.Application;
using TodoApp.Infrastructure;
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
    builder.Services.AddInfrastructure();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddControllers();

    builder.Services.AddFluentResponse(options =>
    {
        options.IncludeTimestamp = true;
        options.IncludeTraceId = true;
        options.EnableExecutionTimeTracking = true;
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Jwt:Authority"];
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"]
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseMiddleware<ExceptionMappingMiddleware>();
    app.UseFluentResponseExceptionHandler();
    app.UseFluentResponseCorrelationId();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<RateLimitingMiddleware>();
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        app.MapScalarApiReference();
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
