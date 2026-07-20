using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.LunchPreferences.Commands.ResetLunchPreference;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.LunchPreferences;

public class ResetLunchPreferenceCommandHandlerTests
{
    private readonly Mock<ILunchPreferenceRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ResetLunchPreferenceCommandHandler _handler;

    public ResetLunchPreferenceCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILunchPreferenceRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ResetLunchPreferenceCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenExistingPreference_ShouldResetToDefaults()
    {
        var userId = Guid.NewGuid();
        var command = new ResetLunchPreferenceCommand { UserId = userId };

        var existing = new LunchPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DietaryRestrictions = ["Vegan"],
            LunchStartTime = new TimeOnly(14, 0),
            LunchEndTime = new TimeOnly(15, 0),
            BreakDurationMinutes = 90,
            NotificationsEnabled = false,
            FavoriteMeals = ["Burger"],
            ExcludedItems = ["Cheese"],
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };

        var response = new LunchPreferenceResponse
        {
            Id = existing.Id,
            UserId = userId,
            LunchStartTime = new TimeOnly(12, 0),
            LunchEndTime = new TimeOnly(13, 0),
            BreakDurationMinutes = 60,
            NotificationsEnabled = true,
            DietaryRestrictions = [],
            FavoriteMeals = [],
            ExcludedItems = [],
            CreatedAt = existing.CreatedAt
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(It.IsAny<LunchPreference>()))
            .Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoExistingPreference_ShouldCreateDefault()
    {
        var userId = Guid.NewGuid();
        var command = new ResetLunchPreferenceCommand { UserId = userId };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LunchPreference?)null);

        LunchPreference? capturedPreference = null;
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()))
            .Callback<LunchPreference, CancellationToken>((p, _) => capturedPreference = p)
            .ReturnsAsync((LunchPreference p, CancellationToken _) => p);

        var response = new LunchPreferenceResponse
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LunchStartTime = new TimeOnly(12, 0),
            LunchEndTime = new TimeOnly(13, 0),
            BreakDurationMinutes = 60,
            NotificationsEnabled = true,
            DietaryRestrictions = [],
            FavoriteMeals = [],
            ExcludedItems = [],
            CreatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(It.IsAny<LunchPreference>()))
            .Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.NotNull(capturedPreference);
        Assert.Equal(userId, capturedPreference!.UserId);
        Assert.Equal(new TimeOnly(12, 0), capturedPreference.LunchStartTime);
        Assert.Equal(new TimeOnly(13, 0), capturedPreference.LunchEndTime);
        Assert.Equal(60, capturedPreference.BreakDurationMinutes);
        Assert.True(capturedPreference.NotificationsEnabled);
        Assert.Empty(capturedPreference.DietaryRestrictions);
        Assert.Empty(capturedPreference.FavoriteMeals);
        Assert.Empty(capturedPreference.ExcludedItems);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExisting_ShouldSetUpdatedAt()
    {
        var userId = Guid.NewGuid();
        var command = new ResetLunchPreferenceCommand { UserId = userId };

        var existing = new LunchPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DietaryRestrictions = ["Vegan"],
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };

        LunchPreference? capturedPreference = null;
        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()))
            .Callback<LunchPreference, CancellationToken>((p, _) => capturedPreference = p)
            .ReturnsAsync((LunchPreference p, CancellationToken _) => p);

        var response = new LunchPreferenceResponse
        {
            Id = existing.Id,
            UserId = userId,
            CreatedAt = existing.CreatedAt
        };

        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(It.IsAny<LunchPreference>()))
            .Returns(response);

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedPreference);
        Assert.NotNull(capturedPreference!.UpdatedAt);
        Assert.True(capturedPreference.UpdatedAt > existing.CreatedAt);
    }
}
