using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Commands.CreateTodo;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Todos;

public class CreateTodoCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTodoCommandHandler _handler;

    public CreateTodoCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateTodoCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTodoAndReturnResponse()
    {
        var command = new CreateTodoCommand
        {
            Title = "Test Todo",
            Description = "Test Description",
            PriorityId = 2
        };

        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            PriorityId = command.PriorityId,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var response = new TodoItemResponse
        {
            Id = todoItem.Id,
            Title = todoItem.Title,
            Description = todoItem.Description
        };

        _mapperMock.Setup(m => m.Map<TodoItem>(command)).Returns(todoItem);
        _mapperMock.Setup(m => m.Map<TodoItemResponse>(It.IsAny<TodoItem>())).Returns(response);

        _repositoryMock.Setup(r => r.GetByIdAsync(todoItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItem);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Title, result.Title);
        _repositoryMock.Verify(r => r.Add(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetStatusIdToPending()
    {
        var command = new CreateTodoCommand
        {
            Title = "Test",
            PriorityId = 1
        };

        TodoItem? capturedItem = null;
        _mapperMock.Setup(m => m.Map<TodoItem>(command))
            .Returns(new TodoItem { Id = Guid.NewGuid(), Title = command.Title });

        _repositoryMock.Setup(r => r.Add(It.IsAny<TodoItem>()))
            .Callback<TodoItem>(item => capturedItem = item);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken ct) => capturedItem);

        _mapperMock.Setup(m => m.Map<TodoItemResponse>(It.IsAny<TodoItem>()))
            .Returns((TodoItem item) => new TodoItemResponse { Id = item.Id, Title = item.Title });

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(capturedItem);
        Assert.Equal(1, capturedItem.StatusId);
    }
}
