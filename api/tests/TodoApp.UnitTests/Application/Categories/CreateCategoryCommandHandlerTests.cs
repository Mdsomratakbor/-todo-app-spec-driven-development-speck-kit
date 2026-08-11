using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Categories.Commands.CreateCategory;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Categories;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCategoryCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateCategoryAndReturnResponse()
    {
        var command = new CreateCategoryCommand
        {
            Name = "Work",
            Color = "#3498DB",
        };

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Work",
            Color = "#3498DB",
            CreatedAt = DateTime.UtcNow,
        };

        var response = new CategoryResponse
        {
            Id = category.Id,
            Name = "Work",
            Color = "#3498DB",
            TodoCount = 0,
        };

        _repositoryMock.Setup(r => r.ExistsByNameAsync(command.Name)).ReturnsAsync(false);

        _mapperMock
            .Setup(m => m.Map<CategoryResponse>(It.IsAny<Category>()))
            .Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(response.Name, result.Name);
        Assert.Equal(response.Color, result.Color);

        _repositoryMock.Verify(r => r.Add(It.Is<Category>(c =>
            c.Name == command.Name &&
            c.Color == command.Color)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenNameAlreadyExists()
    {
        var command = new CreateCategoryCommand
        {
            Name = "Work",
        };

        _repositoryMock.Setup(r => r.ExistsByNameAsync(command.Name)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(r => r.Add(It.IsAny<Category>()), Times.Never);
    }
}
