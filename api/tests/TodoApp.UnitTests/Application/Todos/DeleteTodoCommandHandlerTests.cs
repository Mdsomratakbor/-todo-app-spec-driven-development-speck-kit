using Moq;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Commands.DeleteTodo;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Todos;

public class DeleteTodoCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly DeleteTodoCommandHandler _handler;

    public DeleteTodoCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _handler = new DeleteTodoCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSoftDeleteAndCallDelete()
    {
        var itemId = Guid.NewGuid();
        var item = new TodoItem { Id = itemId, Title = "Test", StatusId = 1, CreatedAt = DateTime.UtcNow };

        _repositoryMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        await _handler.Handle(new DeleteTodoCommand { Id = itemId }, CancellationToken.None);

        Assert.NotNull(item.DeletedAt);
        _repositoryMock.Verify(r => r.Delete(item), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        var itemId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new DeleteTodoCommand { Id = itemId }, CancellationToken.None));

        _repositoryMock.Verify(r => r.Delete(It.IsAny<TodoItem>()), Times.Never);
    }
}
