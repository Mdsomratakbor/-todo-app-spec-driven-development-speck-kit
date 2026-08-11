using MediatR;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _repository;

    public DeleteCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.Id);
        if (category is null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var uncategorized = await _repository.GetByNameAsync("Uncategorized");
        if (uncategorized is null)
        {
            throw new InvalidOperationException("Default Uncategorized category not found.");
        }

        if (category.Id == uncategorized.Id)
        {
            throw new InvalidOperationException("Cannot delete the default Uncategorized category.");
        }

        await _repository.ReassignTodosToCategoryAsync(request.Id, uncategorized.Id);
        _repository.Delete(category);
    }
}
