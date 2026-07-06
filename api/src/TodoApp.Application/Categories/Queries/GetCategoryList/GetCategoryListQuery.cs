using MediatR;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Application.Categories.Queries.GetCategoryList;

public class GetCategoryListQuery : IRequest<List<CategoryResponse>>
{
}
