namespace TodoApp.Application.Auth.Dtos;

public record RefreshRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
