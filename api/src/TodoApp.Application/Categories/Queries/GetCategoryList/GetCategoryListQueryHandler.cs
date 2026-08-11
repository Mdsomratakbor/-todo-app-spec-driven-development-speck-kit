using Cartographer.Core.Abstractions;
using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Categories.Queries.GetCategoryList;

public class GetCategoryListQueryHandler : IRequestHandler<GetCategoryListQuery, List<CategoryResponse>>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public GetCategoryListQueryHandler(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<CategoryResponse>> Handle(GetCategoryListQuery request, CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => _mapper.Map<CategoryResponse>(c)).ToList();
    }
}
