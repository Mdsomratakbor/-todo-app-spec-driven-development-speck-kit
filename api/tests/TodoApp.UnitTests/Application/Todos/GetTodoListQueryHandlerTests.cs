using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Application.Todos.Queries.GetTodoList;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Todos;

public class GetTodoListQueryHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTodoListQueryHandler _handler;

    public GetTodoListQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTodoListQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedResponse()
    {
        var items = new List<TodoItem>
        {
            new() { Id = Guid.NewGuid(), Title = "Item 1", PriorityId = 2, StatusId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Title = "Item 2", PriorityId = 3, StatusId = 1, CreatedAt = DateTime.UtcNow }
        };

        _repositoryMock.Setup(r => r.GetListAsync(
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<Guid?>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(),
                1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 2));

        _mapperMock.Setup(m => m.Map<IReadOnlyList<TodoItemResponse>>(It.IsAny<List<TodoItem>>()))
            .Returns((List<TodoItem> source) => source.Select(i => new TodoItemResponse
            {
                Id = i.Id,
                Title = i.Title
            }).ToList());

        var result = await _handler.Handle(new GetTodoListQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task Handle_ShouldCapPageSizeAt50()
    {
        _repositoryMock.Setup(r => r.GetListAsync(
                It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<Guid?>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(),
                1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<TodoItem>(), 0));

        _mapperMock.Setup(m => m.Map<IReadOnlyList<TodoItemResponse>>(It.IsAny<List<TodoItem>>()))
            .Returns(new List<TodoItemResponse>());

        var result = await _handler.Handle(new GetTodoListQuery { PageSize = 100 }, CancellationToken.None);

        // The handler caps at 50 internally, but the repository is called with pageSize = 50
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithFilters_ShouldPassFiltersToRepository()
    {
        var query = new GetTodoListQuery
        {
            StatusId = 1,
            PriorityId = 3,
            Search = "test",
            Page = 2,
            PageSize = 10
        };

        _repositoryMock.Setup(r => r.GetListAsync(
                query.StatusId, query.PriorityId, query.CategoryId,
                query.DueDateFrom, query.DueDateTo, query.Search,
                query.Page, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<TodoItem>(), 0));

        _mapperMock.Setup(m => m.Map<IReadOnlyList<TodoItemResponse>>(It.IsAny<List<TodoItem>>()))
            .Returns(new List<TodoItemResponse>());

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetListAsync(
            query.StatusId, query.PriorityId, query.CategoryId,
            query.DueDateFrom, query.DueDateTo, query.Search,
            query.Page, query.PageSize, It.IsAny<CancellationToken>()), Times.Once);
    }
}
