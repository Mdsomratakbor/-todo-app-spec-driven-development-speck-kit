using MediatR;
using TodoApp.Application.LunchPreferences.Dtos;

namespace TodoApp.Application.LunchPreferences.Commands.ResetLunchPreference;

public class ResetLunchPreferenceCommand : IRequest<LunchPreferenceResponse>
{
    public Guid UserId { get; set; }
}
