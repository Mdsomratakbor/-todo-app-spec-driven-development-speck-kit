using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Application.LunchPreferences.Queries.GetLunchPreferenceByUser;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.LunchPreferences;

public class GetLunchPreferenceQueryHandlerTests
{
    private readonly Mock<ILunchPreferenceRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetLunchPreferenceByUserQueryHandler _handler;

    public GetLunchPreferenceQueryHandlerTests()
    {
        _repositoryMock = new Mock<ILunchPreferenceRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetLunchPreferenceByUserQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPreferenceExists_ShouldReturnMappedResponse()
    {
        var userId = Guid.NewGuid();
        var query = new GetLunchPreferenceByUserQuery { UserId = userId };

        var preference = new LunchPreference
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
            CreatedAt = DateTime.UtcNow
        };

        var response = new LunchPreferenceResponse
        {
            Id = preference.Id,
            UserId = userId,
            DietaryRestrictions = ["Vegetarian"],
            LunchStartTime = "12:00",
            LunchEndTime = "13:00",
            BreakDurationMinutes = 60,
            NotificationsEnabled = true,
            FavoriteMeals = ["Pasta"],
            ExcludedItems = ["Peanuts"],
            CreatedAt = preference.CreatedAt
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(preference))
            .Returns(response);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Single(result.DietaryRestrictions);
        Assert.Single(result.FavoriteMeals);
        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<LunchPreference>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoPreferenceExists_ShouldCreateDefaultAndReturn()
    {
        var userId = Guid.NewGuid();
        var query = new GetLunchPreferenceByUserQuery { UserId = userId };

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
            LunchStartTime = "12:00",
            LunchEndTime = "13:00",
            BreakDurationMinutes = 60,
            NotificationsEnabled = true,
            DietaryRestrictions = [],
            FavoriteMeals = [],
            ExcludedItems = [],
            CreatedAt = DateTime.UtcNow
        };

        _mapperMock.Setup(m => m.Map<LunchPreferenceResponse>(It.IsAny<LunchPreference>()))
            .Returns(response);

        var result = await _handler.Handle(query, CancellationToken.None);

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
}
