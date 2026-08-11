using MediatR;

namespace TodoApp.Application.Auth.Commands.Logout;

public class LogoutCommand : IRequest
{
    public Guid UserId { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
}
