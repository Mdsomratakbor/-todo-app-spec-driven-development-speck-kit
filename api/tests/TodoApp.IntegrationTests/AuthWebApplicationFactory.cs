using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Data;

namespace TodoApp.IntegrationTests;

public class AuthWebApplicationFactory : WebApplicationFactory<Program>
{
    public static readonly Guid TestUserId = new("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    public const string TestUserEmail = "testuser@example.com";
    private const string DatabaseName = "AuthTestDb";

    private static readonly object _lock = new();
    private static bool _databaseSeeded;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testJwtKey = "TestSecretKeyForIntegrationTests2024!ThisIsAtLeast32CharsLong";
        Environment.SetEnvironmentVariable("Jwt__Key", testJwtKey);
        Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
        Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");

        SeedDatabase();

        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptors = services
                .Where(d => d.ServiceType == typeof(AppDbContext))
                .ToList();
            foreach (var d in dbContextDescriptors)
                services.Remove(d);

            var optionsDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                .ToList();
            foreach (var d in optionsDescriptors)
                services.Remove(d);

            services.AddScoped<AppDbContext>(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseInMemoryDatabase(DatabaseName);
                return new AppDbContext(optionsBuilder.Options);
            });

            var jwtPostConfigure = services
                .Where(d => d.ServiceType == typeof(IConfigureOptions<JwtBearerOptions>))
                .ToList();
            foreach (var d in jwtPostConfigure)
                services.Remove(d);

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "https://localhost/";
                options.TokenValidationParameters.ValidateIssuer = false;
                options.TokenValidationParameters.ValidateAudience = false;
            });

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, AuthTestHandler>("Test", options => { });
            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });
        });
    }

    private static void SeedDatabase()
    {
        lock (_lock)
        {
            if (_databaseSeeded) return;
            _databaseSeeded = true;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(DatabaseName);
            using var ctx = new AppDbContext(optionsBuilder.Options);

            if (!ctx.Users.Any(u => u.Id == TestUserId))
            {
                ctx.Users.Add(new User
                {
                    Id = TestUserId,
                    Email = TestUserEmail,
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("TestPass1"),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                });
                ctx.SaveChanges();
            }
        }
    }
}

public class AuthTestHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public AuthTestHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, AuthWebApplicationFactory.TestUserId.ToString()),
            new Claim(ClaimTypes.Name, AuthWebApplicationFactory.TestUserEmail),
            new Claim(ClaimTypes.Role, "User"),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
