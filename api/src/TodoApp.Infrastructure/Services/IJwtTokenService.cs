using System.Security.Claims;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
    ClaimsPrincipal? ValidateAccessToken(string token);
}
