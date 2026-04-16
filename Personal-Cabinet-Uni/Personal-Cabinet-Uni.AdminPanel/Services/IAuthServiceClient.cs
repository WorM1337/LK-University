using Personal_Cabinet_Uni.AdminPanel.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;

namespace Personal_Cabinet_Uni.AdminPanel.Services;

public interface IAuthServiceClient
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ProfileResponse?> CreateManagerAsync(CreateManagerRequest request, string adminToken, CancellationToken cancellationToken = default);
    Task<ProfileResponse?> EditManagerAsync(string email, EditManagerRequest request, string adminToken, CancellationToken cancellationToken = default);
    Task<bool> DeleteManagerAsync(string email, string adminToken, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProfileResponse>?> GetAllManagersAsync(string adminToken, CancellationToken cancellationToken = default);
    Task<ProfileResponse?> GetManagerByEmailAsync(string email, string adminToken, CancellationToken cancellationToken = default);
}
