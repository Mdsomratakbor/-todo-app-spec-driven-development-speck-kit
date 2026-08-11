using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Application.Auth.Commands.RefreshToken;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.UnitTests.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<ILogger<RefreshTokenCommandHandler>> _loggerMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _loggerMock = new Mock<ILogger<RefreshTokenCommandHandler>>();
        _handler = new RefreshTokenCommandHandler(
            _refreshTokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _jwtTokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidToken_ShouldReturnNewTokens()
    {
        var command = new RefreshTokenCommand { Token = "valid-refresh-token" };
        var userId = Guid.NewGuid();

        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("valid-refresh-token"))
            .Returns("token-hash");
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenHashAsync("token-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TodoApp.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = "token-hash",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = null
            });
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TodoApp.Domain.Entities.User { Id = userId, Email = "user@example.com", Role = "User" });
        _jwtTokenServiceMock.Setup(s => s.GenerateAccessToken(It.IsAny<TodoApp.Domain.Entities.User>()))
            .Returns("new-access-token");
        _jwtTokenServiceMock.Setup(s => s.GenerateRefreshToken())
            .Returns("new-refresh-token");
        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("new-refresh-token"))
            .Returns("new-token-hash");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
        _refreshTokenRepositoryMock.Verify(r => r.RevokeAsync(It.IsAny<TodoApp.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidToken_ShouldThrowUnauthorizedException()
    {
        var command = new RefreshTokenCommand { Token = "invalid-token" };

        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("invalid-token"))
            .Returns("invalid-hash");
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenHashAsync("invalid-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoApp.Domain.Entities.RefreshToken?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExpiredToken_ShouldThrowUnauthorizedException()
    {
        var command = new RefreshTokenCommand { Token = "expired-token" };

        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("expired-token"))
            .Returns("expired-hash");
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenHashAsync("expired-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TodoApp.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TokenHash = "expired-hash",
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                RevokedAt = null
            });

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RevokedToken_ShouldThrowUnauthorizedException()
    {
        var command = new RefreshTokenCommand { Token = "revoked-token" };

        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken("revoked-token"))
            .Returns("revoked-hash");
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenHashAsync("revoked-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TodoApp.Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TokenHash = "revoked-hash",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = DateTime.UtcNow
            });

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
