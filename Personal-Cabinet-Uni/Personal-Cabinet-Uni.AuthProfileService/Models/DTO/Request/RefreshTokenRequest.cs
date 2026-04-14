namespace Personal_Cabinet_Uni.Models.DTO.Request;

/// <summary>
/// Запрос на обновление токенов
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Refresh токен
    /// </summary>
    public string RefreshToken { get; set; }
}
