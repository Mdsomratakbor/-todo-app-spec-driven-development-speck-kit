using MediatR;
using TodoApp.Application.Auth.Dtos;

namespace TodoApp.Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<UserProfileResponse>
{
    public Guid UserId { get; init; }
}
