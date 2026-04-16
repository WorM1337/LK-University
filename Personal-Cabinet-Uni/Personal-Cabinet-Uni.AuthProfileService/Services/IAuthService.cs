using Personal_Cabinet_Uni.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    
    // Admin methods for manager management
    Task<ProfileResponse> CreateManagerAsync(CreateManagerRequest request, CancellationToken cancellationToken = default);
    Task<ProfileResponse> EditManagerAsync(string email, EditManagerRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteManagerAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProfileResponse>> GetAllManagersAsync(CancellationToken cancellationToken = default);
    Task<ProfileResponse?> GetManagerByEmailAsync(string email, CancellationToken cancellationToken = default);
}
