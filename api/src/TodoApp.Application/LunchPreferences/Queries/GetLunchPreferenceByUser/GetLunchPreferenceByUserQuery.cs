using MediatR;
using TodoApp.Application.LunchPreferences.Dtos;

namespace TodoApp.Application.LunchPreferences.Queries.GetLunchPreferenceByUser;

public class GetLunchPreferenceByUserQuery : IRequest<LunchPreferenceResponse>
{
    public Guid UserId { get; set; }
}
