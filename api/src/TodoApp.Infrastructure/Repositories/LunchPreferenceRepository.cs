using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories;

public class LunchPreferenceRepository : ILunchPreferenceRepository
{
    private readonly AppDbContext _db;

    public LunchPreferenceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<LunchPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.LunchPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);
    }

    public async Task<LunchPreference> CreateAsync(LunchPreference preference, CancellationToken ct = default)
    {
        _db.LunchPreferences.Add(preference);
        await _db.SaveChangesAsync(ct);
        return preference;
    }

    public async Task<LunchPreference> UpdateAsync(LunchPreference preference, CancellationToken ct = default)
    {
        _db.LunchPreferences.Update(preference);
        await _db.SaveChangesAsync(ct);
        return preference;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.LunchPreferences.FindAsync([id], ct);
        if (entity is null) return false;
        _db.LunchPreferences.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
