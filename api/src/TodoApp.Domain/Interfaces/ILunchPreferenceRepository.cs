using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces;

public interface ILunchPreferenceRepository
{
    Task<LunchPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<LunchPreference> CreateAsync(LunchPreference preference, CancellationToken ct = default);
    Task<LunchPreference> UpdateAsync(LunchPreference preference, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
