using Moq;
using TodoApp.Application.Auth.Queries.GetCurrentUser;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetCurrentUserQueryHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUserId_ShouldReturnUserProfile()
    {
        var userId = Guid.NewGuid();
        var query = new GetCurrentUserQuery { UserId = userId };

        var user = new User
        {
            Id = userId,
            Email = "user@example.com",
            Role = "User",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(userId, result.Id);
        Assert.Equal("user@example.com", result.Email);
        Assert.Equal("User", result.Role);
        Assert.Equal(user.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task Handle_InvalidUserId_ShouldThrowKeyNotFoundException()
    {
        var query = new GetCurrentUserQuery { UserId = Guid.NewGuid() };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}
