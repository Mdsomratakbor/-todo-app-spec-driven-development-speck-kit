using Cartographer.Core.Abstractions;
using Moq;
using TodoApp.Application.Categories.Queries.GetCategoryList;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.Categories;

public class GetCategoryListQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCategoryListQueryHandler _handler;

    public GetCategoryListQueryHandlerTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCategoryListQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllCategories()
    {
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Work" },
            new() { Id = Guid.NewGuid(), Name = "Personal" },
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        _mapperMock.Setup(m => m.Map<CategoryResponse>(It.IsAny<Category>()))
            .Returns((Category c) => new CategoryResponse { Id = c.Id, Name = c.Name });

        var result = await _handler.Handle(new GetCategoryListQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Work", result[0].Name);
        Assert.Equal("Personal", result[1].Name);
    }

    [Fact]
    public async Task Handle_WhenNoCategories_ShouldReturnEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

        _mapperMock.Setup(m => m.Map<CategoryResponse>(It.IsAny<Category>()))
            .Returns((Category c) => new CategoryResponse { Id = c.Id, Name = c.Name });

        var result = await _handler.Handle(new GetCategoryListQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
