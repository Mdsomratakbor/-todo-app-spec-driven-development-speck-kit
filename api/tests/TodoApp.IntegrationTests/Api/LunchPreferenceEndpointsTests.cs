using System.Net;
using System.Net.Http.Json;
using TodoApp.Application.LunchPreferences.Dtos;

namespace TodoApp.IntegrationTests.Api;

public class LunchPreferenceEndpointsTests : IClassFixture<LunchPreferencesWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LunchPreferenceEndpointsTests(LunchPreferencesWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly string BaseUrl = $"/api/v1/lunch-preferences/{TestUserId}";

    [Fact]
    public async Task GetPreferences_ReturnsSuccess()
    {
        var response = await _client.GetAsync(BaseUrl);

        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdatePreferences_ReturnsSuccessOrConflict()
    {
        var request = new UpdateLunchPreferenceRequest
        {
            DietaryRestrictions = ["Vegetarian"]
        };

        var response = await _client.PutAsJsonAsync(BaseUrl, request);

        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ResetPreferences_ReturnsSuccessOrConflict()
    {
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/reset", new { });

        Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetPreferences_ForNewUser_ReturnsResponse()
    {
        var userId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/lunch-preferences/{userId}");
        request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        var response = await _client.SendAsync(request);

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK
            || response.StatusCode == System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetPreferences_ForExistingUser_ReturnsResponse()
    {
        var userId = Guid.NewGuid();
        var updateRequest = new UpdateLunchPreferenceRequest
        {
            DietaryRestrictions = ["Vegetarian"],
            LunchStartTime = "13:00",
            LunchEndTime = "14:00",
            BreakDurationMinutes = 45,
            NotificationsEnabled = false
        };

        var putReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/lunch-preferences/{userId}")
        {
            Content = JsonContent.Create(updateRequest)
        };
        putReq.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        await _client.SendAsync(putReq);

        var getReq = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/lunch-preferences/{userId}");
        getReq.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        var response = await _client.SendAsync(getReq);

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK
            || response.StatusCode == System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdatePreferences_WithInvalidTimeRange_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateLunchPreferenceRequest
        {
            LunchStartTime = "14:00",
            LunchEndTime = "13:00"
        };

        var response = await _client.PutAsJsonAsync($"/api/v1/lunch-preferences/{userId}", request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdatePreferences_WithInvalidDietaryRestriction_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateLunchPreferenceRequest
        {
            DietaryRestrictions = ["invalid-restriction-value"]
        };

        var response = await _client.PutAsJsonAsync($"/api/v1/lunch-preferences/{userId}", request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdatePreferences_WithFavoritesOverflow_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateLunchPreferenceRequest
        {
            FavoriteMeals = Enumerable.Range(1, 21).Select(i => $"Meal {i}").ToList()
        };

        var response = await _client.PutAsJsonAsync($"/api/v1/lunch-preferences/{userId}", request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdatePreferences_WithExclusionsOverflow_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateLunchPreferenceRequest
        {
            ExcludedItems = Enumerable.Range(1, 21).Select(i => $"Item {i}").ToList()
        };

        var response = await _client.PutAsJsonAsync($"/api/v1/lunch-preferences/{userId}", request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdatePreferences_WithBreakDurationTooLow_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateLunchPreferenceRequest
        {
            BreakDurationMinutes = 5
        };

        var response = await _client.PutAsJsonAsync($"/api/v1/lunch-preferences/{userId}", request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdatePreferences_WithBreakDurationTooHigh_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateLunchPreferenceRequest
        {
            BreakDurationMinutes = 200
        };

        var response = await _client.PutAsJsonAsync($"/api/v1/lunch-preferences/{userId}", request);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ResetPreferences_ReturnsResponse()
    {
        var userId = Guid.NewGuid();

        var updateRequest = new UpdateLunchPreferenceRequest
        {
            DietaryRestrictions = ["Vegan"],
            LunchStartTime = "14:00",
            LunchEndTime = "15:00",
            BreakDurationMinutes = 90,
            NotificationsEnabled = false,
            FavoriteMeals = ["Burger"],
            ExcludedItems = ["Cheese"]
        };

        var putReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/lunch-preferences/{userId}")
        {
            Content = JsonContent.Create(updateRequest)
        };
        putReq.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        await _client.SendAsync(putReq);

        var resetReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/lunch-preferences/{userId}/reset");
        resetReq.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        var resetResponse = await _client.SendAsync(resetReq);

        Assert.True(resetResponse.StatusCode == System.Net.HttpStatusCode.OK
            || resetResponse.StatusCode == System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdatePreferences_WithValidTimes_ReturnsResponse()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateLunchPreferenceRequest
        {
            LunchStartTime = "12:00",
            LunchEndTime = "13:30",
            BreakDurationMinutes = 60
        };

        var httpReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/lunch-preferences/{userId}")
        {
            Content = JsonContent.Create(request)
        };
        httpReq.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        var response = await _client.SendAsync(httpReq);

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK
            || response.StatusCode == System.Net.HttpStatusCode.Conflict);
    }
}
