using Moq;
using TodoApp.Application.Categories.Commands.DeleteCategory;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _handler = new DeleteCategoryCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReassignTodosAndDeleteCategory()
    {
        var categoryId = Guid.NewGuid();
        var uncategorizedId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = categoryId };

        var category = new Category
        {
            Id = categoryId,
            Name = "Work",
            TodoItems = new List<TodoItem> { new(), new() },
        };

        var uncategorized = new Category
        {
            Id = uncategorizedId,
            Name = "Uncategorized",
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _repositoryMock.Setup(r => r.GetByNameAsync("Uncategorized")).ReturnsAsync(uncategorized);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.ReassignTodosToCategoryAsync(categoryId, uncategorizedId), Times.Once);
        _repositoryMock.Verify(r => r.Delete(category), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenCategoryNotFound()
    {
        var command = new DeleteCategoryCommand { Id = Guid.NewGuid() };

        _repositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenDeletingUncategorized()
    {
        var uncategorizedId = Guid.NewGuid();
        var command = new DeleteCategoryCommand { Id = uncategorizedId };

        var uncategorized = new Category
        {
            Id = uncategorizedId,
            Name = "Uncategorized",
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(uncategorizedId)).ReturnsAsync(uncategorized);
        _repositoryMock.Setup(r => r.GetByNameAsync("Uncategorized")).ReturnsAsync(uncategorized);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
    }
}
