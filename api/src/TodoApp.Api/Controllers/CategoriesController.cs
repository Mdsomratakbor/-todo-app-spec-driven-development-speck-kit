using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Categories.Commands.CreateCategory;
using TodoApp.Application.Categories.Commands.DeleteCategory;
using TodoApp.Application.Categories.Commands.UpdateCategory;
using TodoApp.Application.Categories.Dtos;
using TodoApp.Application.Categories.Queries.GetCategoryList;
using TodoApp.Application.Todos.Dtos;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetList()
    {
        var result = await _mediator.Send(new GetCategoryListQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand
        {
            Name = request.Name,
            Color = request.Color,
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetList), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand
        {
            Id = id,
            Name = request.Name,
            Color = request.Color,
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand { Id = id });
        return NoContent();
    }
}
