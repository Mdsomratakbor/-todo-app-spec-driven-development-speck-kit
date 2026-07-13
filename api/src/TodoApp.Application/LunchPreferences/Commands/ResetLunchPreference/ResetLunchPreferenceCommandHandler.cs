using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Domain.Entities;
using Cartographer.Core.Abstractions;

namespace TodoApp.Application.LunchPreferences.Commands.ResetLunchPreference;

public class ResetLunchPreferenceCommandHandler : IRequestHandler<ResetLunchPreferenceCommand, LunchPreferenceResponse>
{
    private readonly ILunchPreferenceRepository _repository;
    private readonly IMapper _mapper;

    public ResetLunchPreferenceCommandHandler(ILunchPreferenceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<LunchPreferenceResponse> Handle(ResetLunchPreferenceCommand request, CancellationToken ct)
    {
        var existing = await _repository.GetByUserIdAsync(request.UserId, ct);

        LunchPreference preference;
        if (existing is not null)
        {
            existing.DietaryRestrictions = [];
            existing.LunchStartTime = new TimeOnly(12, 0);
            existing.LunchEndTime = new TimeOnly(13, 0);
            existing.BreakDurationMinutes = 60;
            existing.NotificationsEnabled = true;
            existing.FavoriteMeals = [];
            existing.ExcludedItems = [];
            existing.UpdatedAt = DateTime.UtcNow;

            preference = await _repository.UpdateAsync(existing, ct);
        }
        else
        {
            preference = new LunchPreference
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

            preference = await _repository.CreateAsync(preference, ct);
        }

        return _mapper.Map<LunchPreferenceResponse>(preference);
    }
}
