using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.LunchPreferences.Commands.CreateOrUpdateLunchPreference;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.LunchPreferences;

public class CreateOrUpdateLunchPreferenceCommandHandlerTests
{
    private readonly Mock<ILunchPreferenceRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateOrUpdateLunchPreferenceCommandHandler _handler;

    public CreateOrUpdateLunchPreferenceCommandHandlerTests()
    {
        _repositoryMock = new Mock<ILunchPreferenceRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateOrUpdateLunchPreferenceCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenExistingPreference_ShouldUpdateAndReturn()
    {
        var userId = Guid.NewGuid();
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = userId,
            DietaryRestrictions = ["Vegan"],
            LunchStartTime = "13:00",
            LunchEndTime = "14:00",
            BreakDurationMinutes = 45,
            NotificationsEnabled = false,
            FavoriteMeals = ["Salad"],
            ExcludedItems = ["Meat"]
        };

        var existing = new LunchPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DietaryRestrictions = ["Vegetarian"],
            LunchStartTime = new TimeOnly(12, 0),
            LunchEndTime = new TimeOnly(13, 0),
            BreakDurationMinutes = 60,
            NotificationsEnabled = true,
            FavoriteMeals = ["Pasta"],
            ExcludedItems = ["Peanuts"],
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var response = new LunchPreferenceResponse
        {
            Id = existing.Id,
            UserId = userId,
            DietaryRestrictions = ["Vegan"],
            LunchStartTime = "13:00",
            LunchEndTime = "14:00",
            BreakDurationMinutes = 45,
            NotificationsEnabled = false,
            FavoriteMeals = ["Salad"],
            ExcludedItems = ["Meat"],
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
    public async Task Handle_WhenNoExistingPreference_ShouldCreateNew()
    {
        var userId = Guid.NewGuid();
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = userId,
            DietaryRestrictions = ["Halal"],
            LunchStartTime = "12:30",
            LunchEndTime = "13:30",
            BreakDurationMinutes = 60,
            FavoriteMeals = ["Chicken"],
            ExcludedItems = ["Pork"]
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LunchPreference?)null);

        _mapperMock.Setup(m => m.Map<LunchPreference>(It.IsAny<CreateOrUpdateLunchPreferenceCommand>()))
            .Returns((CreateOrUpdateLunchPreferenceCommand cmd) => new LunchPreference { UserId = cmd.UserId });

        LunchPreference? capturedPreference = null;
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()))
            .Callback<LunchPreference, CancellationToken>((p, _) => capturedPreference = p)
            .ReturnsAsync((LunchPreference p, CancellationToken _) => p);

        var response = new LunchPreferenceResponse
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DietaryRestrictions = ["Halal"],
            LunchStartTime = "12:30",
            LunchEndTime = "13:30",
            BreakDurationMinutes = 60,
            NotificationsEnabled = true,
            FavoriteMeals = ["Chicken"],
            ExcludedItems = ["Pork"],
            CreatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(It.IsAny<LunchPreference>()))
            .Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.NotNull(capturedPreference);
        Assert.Equal(userId, capturedPreference!.UserId);
        Assert.True(capturedPreference.NotificationsEnabled);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDuplicateFavorites_ShouldThrowConflictException()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            FavoriteMeals = ["Pasta", "pasta"]
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LunchPreference?)null);

        await Assert.ThrowsAsync<ConflictException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenDuplicateExclusions_ShouldThrowConflictException()
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = Guid.NewGuid(),
            ExcludedItems = ["Peanuts", "peanuts"]
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LunchPreference?)null);

        await Assert.ThrowsAsync<ConflictException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldTrimListItems()
    {
        var userId = Guid.NewGuid();
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = userId,
            FavoriteMeals = ["  Pasta  ", "  Salad  "],
            ExcludedItems = ["  Peanuts  "]
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LunchPreference?)null);

        _mapperMock.Setup(m => m.Map<LunchPreference>(It.IsAny<CreateOrUpdateLunchPreferenceCommand>()))
            .Returns((CreateOrUpdateLunchPreferenceCommand cmd) => new LunchPreference { UserId = cmd.UserId });

        LunchPreference? capturedPreference = null;
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()))
            .Callback<LunchPreference, CancellationToken>((p, _) => capturedPreference = p)
            .ReturnsAsync((LunchPreference p, CancellationToken _) => p);

        var response = new LunchPreferenceResponse
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NotificationsEnabled = true,
            CreatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(It.IsAny<LunchPreference>()))
            .Returns(response);

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedPreference);
        Assert.Equal(new List<string> { "Pasta", "Salad" }, capturedPreference!.FavoriteMeals);
        Assert.Equal(new List<string> { "Peanuts" }, capturedPreference.ExcludedItems);
    }

    [Fact]
    public async Task Handle_WhenNotificationsEnabledFalse_ShouldPreserve()
    {
        var userId = Guid.NewGuid();
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = userId,
            NotificationsEnabled = false
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LunchPreference?)null);

        _mapperMock.Setup(m => m.Map<LunchPreference>(It.IsAny<CreateOrUpdateLunchPreferenceCommand>()))
            .Returns((CreateOrUpdateLunchPreferenceCommand cmd) => new LunchPreference { UserId = cmd.UserId });

        LunchPreference? capturedPreference = null;
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()))
            .Callback<LunchPreference, CancellationToken>((p, _) => capturedPreference = p)
            .ReturnsAsync((LunchPreference p, CancellationToken _) => p);

        var response = new LunchPreferenceResponse
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NotificationsEnabled = false,
            CreatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(It.IsAny<LunchPreference>()))
            .Returns(response);

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedPreference);
        Assert.False(capturedPreference!.NotificationsEnabled);
    }
}
