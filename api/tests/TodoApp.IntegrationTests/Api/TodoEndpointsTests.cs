using System.Net;
using System.Net.Http.Json;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.IntegrationTests.Api;

public class TodoEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TodoEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/todos");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoById_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/v1/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WithoutAuth_Returns401()
    {
        var request = new CreateTodoRequest
        {
            Title = "Test",
            PriorityId = 2
        };

        var response = await _client.PostAsJsonAsync("/api/v1/todos", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WithoutAuth_Returns401()
    {
        var request = new UpdateTodoRequest
        {
            Title = "Test",
            PriorityId = 2,
            StatusId = 1
        };

        var response = await _client.PutAsJsonAsync($"/api/v1/todos/{Guid.NewGuid()}", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WithoutAuth_Returns401()
    {
        var response = await _client.DeleteAsync($"/api/v1/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_WithSearchParam_Returns401_Not400()
    {
        var response = await _client.GetAsync("/api/v1/todos?search=test");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_WithStatusFilter_Returns401_Not400()
    {
        var response = await _client.GetAsync("/api/v1/todos?statusId=1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_WithPriorityFilter_Returns401_Not400()
    {
        var response = await _client.GetAsync("/api/v1/todos?priorityId=2");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_WithCategoryFilter_Returns401_Not400()
    {
        var response = await _client.GetAsync("/api/v1/todos?categoryId=00000000-0000-0000-0000-000000000001");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_WithDateRangeFilter_Returns401_Not400()
    {
        var response = await _client.GetAsync("/api/v1/todos?dueDateFrom=2026-01-01&dueDateTo=2026-12-31");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_WithPagination_Returns401_Not400()
    {
        var response = await _client.GetAsync("/api/v1/todos?page=2&pageSize=10");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_WithAllFilters_Returns401_Not400()
    {
        var response = await _client.GetAsync("/api/v1/todos?search=test&statusId=1&priorityId=2&categoryId=00000000-0000-0000-0000-000000000001&dueDateFrom=2026-01-01&dueDateTo=2026-12-31&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
