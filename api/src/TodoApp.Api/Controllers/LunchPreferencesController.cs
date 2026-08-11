using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.LunchPreferences.Commands.CreateOrUpdateLunchPreference;
using TodoApp.Application.LunchPreferences.Commands.ResetLunchPreference;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Application.LunchPreferences.Queries.GetLunchPreferenceByUser;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/v1/lunch-preferences")]
[Authorize]
public class LunchPreferencesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LunchPreferencesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<LunchPreferenceResponse>> GetByUser(Guid userId)
    {
        var result = await _mediator.Send(new GetLunchPreferenceByUserQuery { UserId = userId });
        return Ok(result);
    }

    [HttpPut("{userId:guid}")]
    public async Task<ActionResult<LunchPreferenceResponse>> Update(Guid userId, [FromBody] UpdateLunchPreferenceRequest request)
    {
        var command = new CreateOrUpdateLunchPreferenceCommand
        {
            UserId = userId,
            DietaryRestrictions = request.DietaryRestrictions,
            LunchStartTime = request.LunchStartTime,
            LunchEndTime = request.LunchEndTime,
            BreakDurationMinutes = request.BreakDurationMinutes,
            FavoriteMeals = request.FavoriteMeals,
            ExcludedItems = request.ExcludedItems,
            NotificationsEnabled = request.NotificationsEnabled
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{userId:guid}/reset")]
    public async Task<ActionResult<LunchPreferenceResponse>> Reset(Guid userId)
    {
        var result = await _mediator.Send(new ResetLunchPreferenceCommand { UserId = userId });
        return Ok(result);
    }
}
