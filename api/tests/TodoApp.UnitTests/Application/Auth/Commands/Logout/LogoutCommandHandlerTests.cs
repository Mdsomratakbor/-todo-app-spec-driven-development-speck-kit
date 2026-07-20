using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Application.Auth.Commands.Logout;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.UnitTests.Application.Auth.Commands.Logout;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<ILogger<LogoutCommandHandler>> _loggerMock;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _loggerMock = new Mock<ILogger<LogoutCommandHandler>>();
        _handler = new LogoutCommandHandler(
            _refreshTokenRepositoryMock.Object,
            _jwtTokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidToken_ShouldRevokeToken()
    {
        var userId = Guid.NewGuid();
        var command = new LogoutCommand
        {
            UserId = userId,
            RefreshToken = "valid-refresh-token"
        };

        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("valid-refresh-token"))
            .Returns("token-hash");
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TodoApp.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = "token-hash",
                RevokedAt = null
            });

        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepositoryMock.Verify(r => r.RevokeAsync(It.IsAny<TodoApp.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyRefreshToken_ShouldNotThrow()
    {
        var command = new LogoutCommand
        {
            UserId = Guid.NewGuid(),
            RefreshToken = ""
        };

        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepositoryMock.Verify(r => r.RevokeAsync(It.IsAny<TodoApp.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TokenNotFound_ShouldNotThrow()
    {
        var command = new LogoutCommand
        {
            UserId = Guid.NewGuid(),
            RefreshToken = "nonexistent-token"
        };

        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("nonexistent-token"))
            .Returns("nonexistent-hash");
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenHashAsync("nonexistent-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoApp.Domain.Entities.RefreshToken?)null);

        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepositoryMock.Verify(r => r.RevokeAsync(It.IsAny<TodoApp.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TokenBelongsToDifferentUser_ShouldNotRevoke()
    {
        var command = new LogoutCommand
        {
            UserId = Guid.NewGuid(),
            RefreshToken = "other-users-token"
        };

        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("other-users-token"))
            .Returns("other-hash");
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenHashAsync("other-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TodoApp.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TokenHash = "other-hash",
                RevokedAt = null
            });

        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepositoryMock.Verify(r => r.RevokeAsync(It.IsAny<TodoApp.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
