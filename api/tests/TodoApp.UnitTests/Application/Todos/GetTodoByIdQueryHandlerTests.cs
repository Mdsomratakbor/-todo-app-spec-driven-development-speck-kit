using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Application.Todos.Queries.GetTodoById;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Todos;

public class GetTodoByIdQueryHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTodoByIdQueryHandler _handler;

    public GetTodoByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTodoByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedResponse()
    {
        var itemId = Guid.NewGuid();
        var item = new TodoItem { Id = itemId, Title = "Test", StatusId = 1, CreatedAt = DateTime.UtcNow };
        var response = new TodoItemResponse { Id = itemId, Title = "Test" };

        _repositoryMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _mapperMock.Setup(m => m.Map<TodoItemResponse>(item))
            .Returns(response);

        var result = await _handler.Handle(new GetTodoByIdQuery { Id = itemId }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(itemId, result.Id);
        Assert.Equal("Test", result.Title);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        var itemId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetTodoByIdQuery { Id = itemId }, CancellationToken.None));
    }
}
