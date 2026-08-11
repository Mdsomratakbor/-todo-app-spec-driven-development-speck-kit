using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Commands.UpdateTodo;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Todos;

public class UpdateTodoCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTodoCommandHandler _handler;

    public UpdateTodoCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateTodoCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidTransitionPendingToInProgress_ShouldUpdate()
    {
        var itemId = Guid.NewGuid();
        var existing = new TodoItem
        {
            Id = itemId,
            Title = "Old Title",
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateTodoCommand
        {
            Id = itemId,
            Title = "Updated Title",
            PriorityId = 2,
            StatusId = 2
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _repositoryMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _mapperMock.Setup(m => m.Map<TodoItemResponse>(It.IsAny<TodoItem>()))
            .Returns((TodoItem item) => new TodoItemResponse
            {
                Id = item.Id,
                Title = item.Title
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.Update(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidTransitionCompletedToPending_ShouldThrow()
    {
        var itemId = Guid.NewGuid();
        var existing = new TodoItem
        {
            Id = itemId,
            Title = "Completed Todo",
            StatusId = 3,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateTodoCommand
        {
            Id = itemId,
            Title = "Updated",
            PriorityId = 2,
            StatusId = 1
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(r => r.Update(It.IsAny<TodoItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        var command = new UpdateTodoCommand
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            PriorityId = 2,
            StatusId = 1
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
