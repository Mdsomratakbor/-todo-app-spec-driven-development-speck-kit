using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Domain.Entities;
using Cartographer.Core.Abstractions;

namespace TodoApp.Application.LunchPreferences.Queries.GetLunchPreferenceByUser;

public class GetLunchPreferenceByUserQueryHandler : IRequestHandler<GetLunchPreferenceByUserQuery, LunchPreferenceResponse>
{
    private readonly ILunchPreferenceRepository _repository;
    private readonly IMapper _mapper;

    public GetLunchPreferenceByUserQueryHandler(ILunchPreferenceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<LunchPreferenceResponse> Handle(GetLunchPreferenceByUserQuery request, CancellationToken ct)
    {
        var preference = await _repository.GetByUserIdAsync(request.UserId, ct);

        if (preference is not null)
            return _mapper.Map<LunchPreferenceResponse>(preference);

        // Get-or-create: return default preferences for new users
        var defaultPreference = new LunchPreference
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            DietaryRestrictions = [],
            LunchStartTime = new TimeOnly(12, 0),
            LunchEndTime = new TimeOnly(13, 0),
            BreakDurationMinutes = 60,
            NotificationsEnabled = true,
            FavoriteMeals = [],
            ExcludedItems = [],
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(defaultPreference, ct);
        return _mapper.Map<LunchPreferenceResponse>(created);
    }
}
