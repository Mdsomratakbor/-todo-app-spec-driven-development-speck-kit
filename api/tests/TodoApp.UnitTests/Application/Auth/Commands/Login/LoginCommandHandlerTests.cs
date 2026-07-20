using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Application.Auth.Commands.Login;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();
        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _jwtTokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnAuthResponse()
    {
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "SecurePass1"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("SecurePass1", 12);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            HashedPassword = hashedPassword,
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtTokenServiceMock.Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access-token");
        _jwtTokenServiceMock.Setup(s => s.GenerateRefreshToken())
            .Returns("refresh-token");
        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken(It.IsAny<string>()))
            .Returns("refresh-token-hash");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal(900, result.ExpiresIn);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Fact]
    public async Task Handle_WrongPassword_ShouldThrowUnauthorizedException()
    {
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "WrongPassword"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("SecurePass1", 12);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            HashedPassword = hashedPassword
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonExistentEmail_ShouldThrowUnauthorizedException()
    {
        var command = new LoginCommand
        {
            Email = "nonexistent@example.com",
            Password = "SecurePass1"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldCreateRefreshToken()
    {
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "SecurePass1"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("SecurePass1", 12);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            HashedPassword = hashedPassword
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtTokenServiceMock.Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access-token");
        _jwtTokenServiceMock.Setup(s => s.GenerateRefreshToken())
            .Returns("refresh-token");
        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken(It.IsAny<string>()))
            .Returns("refresh-token-hash");

        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepositoryMock.Verify(r => r.CreateAsync(
            It.IsAny<TodoApp.Domain.Entities.RefreshToken>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
