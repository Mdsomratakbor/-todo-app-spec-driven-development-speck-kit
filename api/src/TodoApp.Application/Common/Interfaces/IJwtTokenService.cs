using System.Security.Claims;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
    ClaimsPrincipal? ValidateAccessToken(string token);
}
