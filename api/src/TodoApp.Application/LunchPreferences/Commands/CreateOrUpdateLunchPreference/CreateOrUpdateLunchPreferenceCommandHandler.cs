using MediatR;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Domain.Entities;
using Cartographer.Core.Abstractions;

namespace TodoApp.Application.LunchPreferences.Commands.CreateOrUpdateLunchPreference;

public class CreateOrUpdateLunchPreferenceCommandHandler : IRequestHandler<CreateOrUpdateLunchPreferenceCommand, LunchPreferenceResponse>
{
    private readonly ILunchPreferenceRepository _repository;
    private readonly IMapper _mapper;

    public CreateOrUpdateLunchPreferenceCommandHandler(ILunchPreferenceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<LunchPreferenceResponse> Handle(CreateOrUpdateLunchPreferenceCommand request, CancellationToken ct)
    {
        if (request.FavoriteMeals is not null && HasDuplicates(request.FavoriteMeals))
            throw new ConflictException("Favorite meals contain duplicate items.");
        if (request.ExcludedItems is not null && HasDuplicates(request.ExcludedItems))
            throw new ConflictException("Excluded items contain duplicate items.");

        var existing = await _repository.GetByUserIdAsync(request.UserId, ct);

        if (existing is not null)
        {
            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            existing.DietaryRestrictions = request.DietaryRestrictions?.Select(TrimItem).ToList()
                ?? [];
            existing.FavoriteMeals = request.FavoriteMeals?.Select(TrimItem).ToList()
                ?? [];
            existing.ExcludedItems = request.ExcludedItems?.Select(TrimItem).ToList()
                ?? [];
            existing.LunchStartTime = request.LunchStartTime is not null
                ? TimeOnly.Parse(request.LunchStartTime)
                : null;
            existing.LunchEndTime = request.LunchEndTime is not null
                ? TimeOnly.Parse(request.LunchEndTime)
                : null;
            existing.BreakDurationMinutes = request.BreakDurationMinutes;
            existing.NotificationsEnabled = request.NotificationsEnabled ?? true;

            var updated = await _repository.UpdateAsync(existing, ct);
            return _mapper.Map<LunchPreferenceResponse>(updated);
        }

        var preference = _mapper.Map<LunchPreference>(request);
        preference.Id = Guid.NewGuid();
        preference.CreatedAt = DateTime.UtcNow;
        preference.DietaryRestrictions = request.DietaryRestrictions?.Select(TrimItem).ToList() ?? [];
        preference.FavoriteMeals = request.FavoriteMeals?.Select(TrimItem).ToList() ?? [];
        preference.ExcludedItems = request.ExcludedItems?.Select(TrimItem).ToList() ?? [];
        if (request.LunchStartTime is not null)
            preference.LunchStartTime = TimeOnly.Parse(request.LunchStartTime);
        if (request.LunchEndTime is not null)
            preference.LunchEndTime = TimeOnly.Parse(request.LunchEndTime);
        preference.BreakDurationMinutes = request.BreakDurationMinutes;
        preference.NotificationsEnabled = request.NotificationsEnabled ?? true;

        var created = await _repository.CreateAsync(preference, ct);
        return _mapper.Map<LunchPreferenceResponse>(created);
    }

    private static string TrimItem(string item) => item.Trim();

    private static bool HasDuplicates(List<string> items)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in items)
        {
            var trimmed = item.Trim();
            if (!seen.Add(trimmed))
                return true;
        }
        return false;
    }
}
