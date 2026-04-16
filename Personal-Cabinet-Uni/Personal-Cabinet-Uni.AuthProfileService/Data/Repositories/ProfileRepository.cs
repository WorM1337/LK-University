using Microsoft.EntityFrameworkCore;
using Personal_Cabinet_Uni.Data.Contexts;
using Personal_Cabinet_Uni.Models.Entities;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Data.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileDbContext _context;

    public ProfileRepository(ProfileDbContext context)
    {
        _context = context;
    }

    public async Task<Profile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Profiles.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Profile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Profiles
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<Profile?> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Profiles
            .FirstOrDefaultAsync(p => p.RefreshToken == token, cancellationToken);
    }

    public async Task<IEnumerable<Profile>> GetAllAsync(int page, int limit, Role? role = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Profiles.AsQueryable();

        if (role.HasValue)
        {
            query = query.Where(p => p.Role == role.Value);
        }

        return await query
            .OrderBy(p => p.Surname)
            .ThenBy(p => p.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Profile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Profiles
            .OrderBy(p => p.Surname)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await GetByIdAsync(id, cancellationToken);
        if (profile != null)
        {
            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Profile> CreateAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        profile.Id = Guid.NewGuid();
        profile.CreatedAt = DateTime.UtcNow;
        
        await _context.Profiles.AddAsync(profile, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return profile;
    }

    public async Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default)
    {
        profile.UpdatedAt = DateTime.UtcNow;
        
        _context.Profiles.Update(profile);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Profiles.AnyAsync(p => p.Email == email, cancellationToken);
    }
}
