using System.Net;
using System.Net.Http.Json;

namespace TodoApp.IntegrationTests.Api;

public class CategoryEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoryEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/categories");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithoutAuth_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/categories", new
        {
            name = "Test Category",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_WithoutAuth_Returns401()
    {
        var response = await _client.PutAsJsonAsync($"/api/v1/categories/{Guid.NewGuid()}", new
        {
            name = "Updated Category",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_WithoutAuth_Returns401()
    {
        var response = await _client.DeleteAsync($"/api/v1/categories/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
