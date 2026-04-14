using Personal_Cabinet_Uni.Models.Entities;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Data.Repositories;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Profile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Profile?> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<Profile>> GetAllAsync(int page, int limit, Role? role = null, CancellationToken cancellationToken = default);
    Task<Profile> CreateAsync(Profile profile, CancellationToken cancellationToken = default);
    Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
