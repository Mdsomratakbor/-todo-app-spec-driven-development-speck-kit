using Cartographer.Core.Abstractions;
using MediatR;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.Id);
        if (category is null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var duplicate = await _repository.GetByNameAsync(request.Name);
        if (duplicate is not null && duplicate.Id != request.Id)
        {
            throw new ConflictException("A category with this name already exists.");
        }

        category.Name = request.Name.Trim();
        category.Color = request.Color;

        _repository.Update(category);

        return _mapper.Map<CategoryResponse>(category);
    }
}
