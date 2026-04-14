namespace Personal_Cabinet_Uni.Models.DTO.Response;

/// <summary>
/// Ответ на регистрацию/логин
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Access токен
    /// </summary>
    public string AccessToken { get; set; }
    
    /// <summary>
    /// Refresh токен
    /// </summary>
    public string RefreshToken { get; set; }
}
