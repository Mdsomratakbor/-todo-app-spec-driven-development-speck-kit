using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Categories.Commands.UpdateCategory;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Categories;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateCategoryCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateCategoryAndReturnResponse()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Old Name", Color = "#000000" };
        var command = new UpdateCategoryCommand { Id = categoryId, Name = "New Name", Color = "#FF5733" };
        var response = new CategoryResponse { Id = categoryId, Name = "New Name", Color = "#FF5733" };

        _repositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _repositoryMock.Setup(r => r.GetByNameAsync("New Name")).ReturnsAsync((Category?)null);
        _mapperMock.Setup(m => m.Map<CategoryResponse>(category)).Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("New Name", result.Name);
        Assert.Equal("#FF5733", result.Color);
        _repositoryMock.Verify(r => r.Update(category), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        var command = new UpdateCategoryCommand { Id = Guid.NewGuid(), Name = "Test" };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldThrowConflictException()
    {
        var categoryId = Guid.NewGuid();
        var duplicateId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Old Name" };
        var duplicate = new Category { Id = duplicateId, Name = "Existing Name" };
        var command = new UpdateCategoryCommand { Id = categoryId, Name = "Existing Name" };

        _repositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _repositoryMock.Setup(r => r.GetByNameAsync("Existing Name")).ReturnsAsync(duplicate);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSameName_ShouldAllowUpdate()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Same Name", Color = "#000000" };
        var command = new UpdateCategoryCommand { Id = categoryId, Name = "Same Name", Color = "#FFFFFF" };
        var response = new CategoryResponse { Id = categoryId, Name = "Same Name", Color = "#FFFFFF" };

        _repositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _repositoryMock.Setup(r => r.GetByNameAsync("Same Name")).ReturnsAsync(category);
        _mapperMock.Setup(m => m.Map<CategoryResponse>(category)).Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Same Name", result.Name);
        _repositoryMock.Verify(r => r.Update(category), Times.Once);
    }
}
