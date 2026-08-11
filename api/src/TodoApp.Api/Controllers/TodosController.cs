using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Todos.Commands.CreateTodo;
using TodoApp.Application.Todos.Commands.DeleteTodo;
using TodoApp.Application.Todos.Commands.UpdateTodo;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Application.Todos.Queries.GetTodoById;
using TodoApp.Application.Todos.Queries.GetTodoList;

namespace TodoApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/todos")]
public class TodosController : ControllerBase
{
    private readonly IMediator _mediator;

    public TodosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TodoItemResponse>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? statusId = null,
        [FromQuery] int? priorityId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        [FromQuery] string? search = null)
    {
        var query = new GetTodoListQuery
        {
            Page = page,
            PageSize = pageSize,
            StatusId = statusId,
            PriorityId = priorityId,
            CategoryId = categoryId,
            DueDateFrom = dueDateFrom,
            DueDateTo = dueDateTo,
            Search = search
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoItemResponse>> GetById(Guid id)
    {
        var query = new GetTodoByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemResponse>> Create([FromBody] CreateTodoRequest request)
    {
        var command = new CreateTodoCommand
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            PriorityId = request.PriorityId,
            CategoryId = request.CategoryId
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TodoItemResponse>> Update(Guid id, [FromBody] UpdateTodoRequest request)
    {
        var command = new UpdateTodoCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            PriorityId = request.PriorityId,
            CategoryId = request.CategoryId,
            StatusId = request.StatusId
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteTodoCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}
