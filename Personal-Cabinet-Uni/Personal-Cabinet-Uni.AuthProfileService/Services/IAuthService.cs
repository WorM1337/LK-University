using Personal_Cabinet_Uni.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;

namespace Personal_Cabinet_Uni.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}
