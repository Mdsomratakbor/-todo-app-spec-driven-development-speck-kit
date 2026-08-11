using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Application.Auth.Commands.Register;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<ILogger<RegisterCommandHandler>> _loggerMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _loggerMock = new Mock<ILogger<RegisterCommandHandler>>();
        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _jwtTokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnAuthResponse()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "SecurePass1"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
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

        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<TodoApp.Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldThrowConflictException()
    {
        var command = new RegisterCommand
        {
            Email = "existing@example.com",
            Password = "SecurePass1"
        };

        var existingUser = new User { Id = Guid.NewGuid(), Email = "existing@example.com" };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldHashPassword()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "SecurePass1"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _jwtTokenServiceMock.Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access-token");
        _jwtTokenServiceMock.Setup(s => s.GenerateRefreshToken())
            .Returns("refresh-token");
        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken(It.IsAny<string>()))
            .Returns("refresh-token-hash");

        await _handler.Handle(command, CancellationToken.None);

        _userRepositoryMock.Verify(r => r.CreateAsync(
            It.Is<User>(u => u.HashedPassword != command.Password && BCrypt.Net.BCrypt.Verify(command.Password, u.HashedPassword)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetUserRole()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "SecurePass1"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _jwtTokenServiceMock.Setup(s => s.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access-token");
        _jwtTokenServiceMock.Setup(s => s.GenerateRefreshToken())
            .Returns("refresh-token");
        _jwtTokenServiceMock.Setup(s => s.HashRefreshToken(It.IsAny<string>()))
            .Returns("refresh-token-hash");

        await _handler.Handle(command, CancellationToken.None);

        _userRepositoryMock.Verify(r => r.CreateAsync(
            It.Is<User>(u => u.Role == "User"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
