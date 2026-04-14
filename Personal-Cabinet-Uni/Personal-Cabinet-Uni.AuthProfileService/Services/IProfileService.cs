using Personal_Cabinet_Uni.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Services;

public interface IProfileService
{
    Task<IEnumerable<ProfileResponse>> GetAllAsync(int page, int limit, Role? role = null, CancellationToken cancellationToken = default);
    Task<ProfileResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ProfileResponse> UpdateAsync(string email, EditProfileRequest request, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<ProfileResponse> CreateManagerAsync(CreateManagerRequest request, CancellationToken cancellationToken = default);
    Task UpdateManagerAsync(string email, EditManagerRequest request, CancellationToken cancellationToken = default);
}
